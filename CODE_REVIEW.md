# Code Review: ContainerExpressions

Here is a review of the `ContainerExpressions` project, focusing on correctness, performance, and API design.

## 1. Correctness Issues

### `ValueLater<T>` Defeats Caching (Struct Mutation)
**Location:** `Later.cs` (`LazyLoader<T>` and `ValueLater<T>`)
**Issue:** `ValueLater<T>` contains a `readonly LazyLoader<T> Lazy;` field. Because `LazyLoader<T>` is a mutable struct (it mutates `_value` and `_func` in its getter), accessing `Lazy.Value` causes the C# compiler to create a **defensive copy** of the struct before accessing the property.
**Impact:** The mutation only happens on the copy. The original `ValueLater.Lazy` remains untouched, meaning `_func` is never set to null and `_value` is never cached. The `_func` will be executed **every single time** `ValueLater.Value` is accessed, completely defeating the purpose of being a lazy type.
**Recommendation:** Change `LazyLoader<T>` to a `class`, or avoid exposing it as a `readonly` struct field when it relies on mutating its own state.

### `SmartEnum<T>` Initialization and Mutation
**Location:** `SmartEnum.cs`
**Issue:** In the static constructor, `SmartEnum` modifies properties (`Name`, `Value`, `Aliases`) of static field instances.
**Impact:** This violates the expectation that static fields (especially ones representing enum values) should be immutable. Furthermore, the condition `if (se.Value <= 0) se.Value = i;` means that negative values are silently overwritten, preventing the definition of enums with negative values.
**Recommendation:** Enforce immutability. Pass `Name`, `Value`, etc., to the constructor of the `SmartEnum` base class instead of mutating properties after instantiation via reflection.

## 2. Performance Bottlenecks

### `ResponseAsyncMethodBuilder<T>` Allocates on Every Await
**Location:** `ResponseAsync.cs` (`ResponseAsyncMethodBuilder<T>` and `ResponseAsyncTaskCompletionSource<T>`)
**Issue:** In the `AwaitOnCompleted` and `AwaitUnsafeOnCompleted` methods, the implementation is:
```csharp
awaiter.OnCompleted(stateMachine.MoveNext);
```
**Impact:** This involves passing a method group (`stateMachine.MoveNext`), which creates a new `Action` delegate allocation. Worse, `stateMachine` is a struct value type (a compiler-generated struct for the async method). Passing its method group boxes the entire state machine struct on the heap, on **every single await call**. This negates all the performance benefits of writing a custom async method builder and causes severe memory traffic.
**Recommendation:** Implement proper state machine boxing (by calling `awaiter.OnCompleted` via a state machine runner object) or simply delegate to the built-in `AsyncTaskMethodBuilder<T>` internally, which is highly optimized.

### Inefficient `ValueTask` Wrappers
**Location:** `Response.cs` (`ResponseAwaiterExtensions`)
**Issue:** When wrapping a `ValueTask` to `Response` (e.g., `GetAwaiter(this Response<ValueTask> response)`), if the `ValueTask` is incomplete, the code calls `response.Value.AsTask().ContinueWith(...)`.
**Impact:** `ValueTask` exists primarily to avoid allocations. Calling `.AsTask()` on an incomplete `ValueTask` forces it to allocate a `Task` on the heap. Using `.ContinueWith()` allocates a delegate, an action, and a continuation task.
**Recommendation:** Create a custom awaiter that registers an `IValueTaskSource` continuation without converting it to a heavy `Task` using `ContinueWith`.

## 3. Public API & Interface Design

### Implicit Cast to `T` on `Response<T>` Throws Exceptions
**Location:** `Response.cs`
**Issue:** `Response<T>` provides an implicit conversion operator to `T`:
```csharp
public static implicit operator T(Response<T> response) => response.Value;
```
If the response is in an invalid state, accessing `.Value` throws an `InvalidOperationException`.
**Impact:** Implicit conversions in C# are expected to be safe and never throw exceptions (per Microsoft's API Design Guidelines). If an implicit conversion can throw, it should be an `explicit` cast instead. This design makes it very easy to accidentally assign a failed `Response<T>` to a `T` and crash the application without a clear syntactic warning.
**Recommendation:** Make this cast `explicit` or rely solely on `response.Value`.

### `Either<T1, T2>` Implicit Conversions with Ambiguous Types
**Location:** `Either.cs`
**Issue:** `Either` has implicit conversions from `T1` and `T2`.
```csharp
public static implicit operator Either<T1, T2>(T1 value) => new Either<T1, T2>(value);
```
**Impact:** If a user creates an `Either<int, int>` or if `T1` inherits from `T2`, the compiler cannot determine which implicit operator to invoke when assigning a value, leading to ambiguous compiler errors.
**Recommendation:** While this can be a limitation of generic Either types, you might consider defining type constraints, creating static factory methods (`Either.FromLeft(...)`, `Either.FromRight(...)`), or documenting this behavior clearly, as C# currently lacks `where T1 : not T2` constraints.

### Use of `[CallerArgumentExpression]` Polyfill vs Modern .NET
**Location:** `Try.cs`
**Issue:** The project redefines `CallerArgumentExpressionAttribute` in the `System.Runtime.CompilerServices` namespace for .NET Core 3.0 and older targets.
**Impact:** While this allows usage of the attribute on older frameworks, redefining a `System.*` type can sometimes cause namespace collisions or conflict warnings if an application consumes the library in a slightly mixed context. It's generally safer to set `internal` rather than `public` for these polyfills, which you did, but it's something to keep a close eye on.

## Summary

* **High Priority Fix:** Fix `ValueLater<T>` by converting the internal structure to reference types or removing `readonly` constraints, as it currently does not cache values.
* **High Priority Fix:** Address the boxing and delegate allocation in `ResponseAsyncMethodBuilder<T>`, which causes massive allocation overhead for async functions.
* **API Break:** Strongly consider changing the implicit cast of `Response<T> -> T` to an explicit cast to avoid silent runtime crashes.