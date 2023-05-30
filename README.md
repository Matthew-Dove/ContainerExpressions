# ContainerExpressions
ContainerExpressions provides generic abstractions to remove boilerplate code needed by programs.  
This package provides various wrappers for any type `T`, that give it some sort of "superpower".  
The containers provide a base, for the expressions to target.  

[PM> Install-Package ContainerExpressions](https://www.nuget.org/packages/ContainerExpressions/)  

## Response`<T>`

An enclosing type around a method's normal return type.  
Used when methods may return an error beyond your control, the container signals if the method ran successfully or not.
If the method completed successfully, you can access the *real* value, otherwise the response is in an error state.

In the example below we show the pattern for using the response container.
The response type will be valid only when a value is set.
```cs
class CustomerService
{
    public Response<Customer> LoadCustomer(int id)
    {
        var response = new Response<Customer>(); // The response starts off in an invalid state.

        try
        {
            string json = File.ReadAllText($"./User/{id}.json");
            Customer customer = JsonConvert.DeserializeObject<Customer>(json);
            response = response.WithValue(customer); // The response is in a valid state.
        }
        catch (Exception ex)
        {
            Log.Error(ex);
        }

        return response;
    }
}
```

The consuming code looks like:
```cs
CustomerService service = new CustomerService();
Response<Customer> customer service.LoadCustomer(1337);
if (customer.IsValid)
{
	// Do something with the customer...
}
```

### Response Extension Methods

Useful utilities for the `Response<T>` type:  

* `T GetValueOrDefault<T>` Returns the default value when the Response is in an invalid state.
* `Response<TResult> Bind<T, TResult>` Invokes a second function with the output of the first one.
* `Response<TResult> Transform<T, TResult>` Changes type `T`, to type `TResult`.
* `Func<T, Response<TResult>> Lift<T, TResult>` Elevate the function’s type from `T`, to `Response<T>`
* `Response<TResult> Pivot<T, TResult>` Execute the first function if the condition is true, otherwise execute the second function.  
* `bool IsTrue<T>` When the Response is in a valid state the func's bool result is returned, otherwise false is returned.  
* `Response<T> Create<T>` Create a response container in a valid state.  
* `Response<T> With<T>` Create a new response container in a valid state, with the same type `T` as the original response.  
* `Task<Response<T>> ToResponseTaskAsync<T>` Convert some `Task<T>` to `Task<Response<T>>`, in a valid state when the task did not fault; and finished executing.  
* `Response<T> Unpack<T>` Converts `Response<Response<T>>` to `Response<T>`, which works much like `Task`'s `Unwrap` extension to flatten `Task<Task<T>>` to `Task<T>`.  

In general you will find various overloads for these extension methods.  
They target `T`, `Response`, and `Response<T>`; with options for both sync, and async types.  

## Response

Similar to Response`<T>`, but used for methods that return void, instead of a *real* type.
```cs
class CustomerService
{
    public Response SaveCustomer(int id, Customer customer)
    {
        var response = new Response(); // The response starts off in an invalid state.

        try
        {
            string json = JsonConvert.SerializeObject(customer);
            File.WriteAllText($"./Users/{id}.json", json);
            response = response.AsValid(); // The response is in a valid state.
        }
        catch (Exception ex)
        {
            Log.Error(ex);
        }

        return response;
    }
}
```

## Later`<T>`

Used in situations where you desire the value to be calulated the first time it's accessed.  

In the example below `IUserService` is injected into other services using [dependency injection](https://en.wikipedia.org/wiki/Dependency_injection), the code runs in the context of a web server.  
So depending on what time the DI framework creates the `UserService`, and what time the user is authenticated (*and therefore sets the thread's `CurrentPrincipal`*), reading the name may cause an error.  
Using the later container we no longer care about the execution order of the authentication, and the dependency injector.
```cs
class UserService : IUserService
{
    public string Name { get { return _username; } }
    private readonly Later<string> _username;
    
    public UserService()
    {
    	_username = Later.Create(() => Thread.CurrentPrincipal.Identity.Name);
    }
}
```
**Note:** there is also `Later.CreateAsync()` for asynchronous values.

## ValueLater`<T>`

Struct option of the `Later<T>` type.  
Use this for lazy loads, where copy semantics make sense.  
There is no `async` version for this value type.  

```cs
var later = new ValueLater<int>(() => 42); // Helper method included: ValueLater.Create(() => 42);.
int number1 = later.Lazy.Value; // Long syntax (function is calculated at this point).
int number2 = later; // Short syntax (same behaviour as above).
```

## Try`<T>`

Wrap an `Action` (a void function), or a `Func<T>` (a function returning a "real" type) in a Try Container to safely execute otherwise problematic code.  
If the code in a function can throw errors, and those aren't handled internally the Try Container can help out.  
This would be used in cases where the function doesn't return a `Response`, or `Response<T>`, and can throw exceptions.  
It can make code clearer as the logic isn't clouded by error handling, however when to use this instead or handling the errors in the function itself is left to the implementer (you).  

By default errors aren't logged, but you can add your own logger that'll be ran each time the Try Container encounters an error.  
If you'd like to log any errors it's suggested you set up a logger at the start of the program, however you're able to change, or remove the error logger at any point in the program.  
Whatever logger is set at the time a Try container is created, is the logger that Container will use. It's suggested your logger is stateless to avoid runtime complications.  
The custom logger is a simple 'Action' that takes an 'Exception' as an argument.  
For example a logger in a console app might look like: `Try.SetExceptionLogger(ex => Console.WriteLine(ex))` - or simply: `Try.SetExceptionLogger(Console.WriteLine)`.  

In the example below a `Widget` is persisted to disk in a fire, and forget fashion.  
Since the result of the save isn't used, the return type is `void`. The function lacks error handling, so it's lifted to a Try Container.
````cs
var result = Try.Run(() => Persist(widget));

private static void Persist(Widget widget)
{
    var contents = JsonConvert.SerializeObject(widget);
    var path = $"{RELATIVE_PATH}/{Path.GetRandomFileName()}.json";
    File.WriteAllText(path, contents);
}
````

Note: there is also Try.RunAsync() for asynchronous functions.

## Either`<T>`

If you have a function that can benefit from returning one type, from a selection of types, then Either is what you're looking for.  
Either can change it's internal type during program execution similar to object, but in a type safe way. 

A case where you might use Either is when you have a function, and you find yourself about to drastically change it's return type for an edge case.  
Let's say you have a SaveCustomer function that takes a Customer, and returns a boolean indicating if the save was successful or not.  
A new requirement comes in, you must display a error message to the client if the customer's email is already in use by another client.  
You can't just rely on the boolean's false, because you don't know if it was false from of a database write error, or a duplicate email.  
So you must change the return type from boolean to CustomerResponse, and CustomerResponse contains two fields, one for the boolean, and another string for the error message.  
Alternatively you could modify the return type to be `Either<bool, string>`, and return a string in the case of a duplicate email.  

In the example below we display different messages based off Either's internal type.  
Here we see types used to indicate state, something that would normally be done with values of a type, not the type itself.  
````cs
// Given the two types:
struct Ok { }
struct Error { }

// We can set Either to contain one of these two types.
Either<Ok, Error> either = new Ok();

if (new Random().Next() % 2 == 0)
{
    either = new Error();
}

string message = either.Match(
    ok => "Operation was successful.", // When Either's type is Ok, this string is returned.
    error => "Internal error - try again later." // When Either's type is Error, this string is returned.
);
````

`Either` is also useful in as an input argument, where you can accept different types; and would normally have to overload the function.  
Below we see `Either` allows you to create only one function that accept either a `string`, or a `Uri`.  

```cs
private User GetUserById(int id); // Some encapsulated function.

// Without Either - we need two functions to accept the type overloads:
public User GetUser(string id) => Get(int.parse(id));
public User GetUser(int id) => GetUserById(id);

// With Either - one function can combine the two overloads:
public User GetUser(Either<string, int> id) => GetUserById(id.Match(int.Parse, x => x));

// Invoking the functions remain the same, as Either has implicit operators. 
User user = GetUser("1337"); // Auto cast to Either<string, int>.
```

## NotNull`<T>`

A containter for a reference type `T`, that ensures the provided value of `T` is not null.  
The constructor is private, therefore callers must cast from `T` to `NotNull<T>` (*an implicit cast is fine, and most appropriate*).  
In essence this is the `ArgumentNullException` parameter check defined in a type.  

A short demo below shows its use:  

```cs
// userId T (in this case string) is wrapped in NotNull, it will not allow a cast from T to NotNull<T> when T is null.
string GetUserName(NotNull<string> userId)
{
    var service = new UserService();
    var username = service.GetUsername(userId); // We can be confident that userId has a value, as passing null to this method results in a runtime exception.
    return username;
}

class UserService
{
    public string GetUsername(string userId) => USER_NAME; // Observe userId is a normal string here, there is no need to explicitly extract the underlying value from NotNull<T>. 
}
```

The method `GetUserName` would be called as if userId was a normal string:  

```cs
var username = GetUserName("87654321");
```

There is a struct version of `NotNull` described as `NN<T>`.  
This was included as creating a reference for such a short lived type seemed wasteful.  
You may select to use `NN<T>` when considering garbage collection, or if you need copy semantics.  
A struct is harder to "lock down", as you cannot stop the caller from invoking the default constructor.  
If you trust the caller enough to not break you on purpose, and pass in `T` letting the implicit cast occur, then using `NN<T>` is preferable in most cases.  

## Maybe`<TValue, TError>`

Maybe contains an optional `TError` type when the caller needs to make different decisions on error, and requires some (*or all*) of the low level details.  
If you do not need the custom `TError` type overload, then the base `Exception` can be used from the type `Maybe<TValue>` (*as opposed to `Maybe<TValue, TError>`*).  
Maybe will contain **one** of *either* the `TValue`, or the `TError` / `Exception` types (*never both value and error*).  

Note that `Maybe` is very similar to that of `Response<T>`, and `Either<T1, T2>`.  
However it deserves it's own container, as it hits a niche that neither  those containers would be good substitutes for.  

`Response<T>` expects the low level code to handle errors, and only surfaces with a false result when an opperation fails (*i.e. containing no error details*).  
This is by desgin, we do not want to encourage "leaky abstractions". The caller may know "creating a new user failed", but not "it failed because of a table lock".  
Table locks can be fixed at the database, or user creation service level, not by the high level caller (*this is not their concern*).  
The caller still has context to generate useful messages for the end client.  

`Either<T1, T2>` can model the value / error types with it's `T2` implementation, but it is not specialized to account for aggregate errors; making it awkward to use at best.  
For example `Either<T1, T2>` would be fine for a single use of some value / error, where the caller unwraps it right away, and handles the value, or error conditions then and there.  
However this is only the very simplest of use cases, more likey the caller wants to invoke many of these in a row thoughout the course of fulfilling some incoming request.  

When `Maybe` has a value, it is propagated to the next `Maybe` for further execution.  
When `Maybe` has an error, and is combining its result with that of another `Maybe`; which also has an error, we need to store *both* errors, so they may be presented to the top level caller together.  

`Either` is not capable of storing the aggregated errors, so the user would be forced to do it manually (*or lose all errors besides the final one, which would not have been the initial cause*).  
We cannot provide a general solution for `Either`, as values do not need to be stored (*they are used, then thrown away*); in addition any `Either` with 3 or more types would not benefit from such a design.  
Lastly `Either` does not have the concept of one of the `<T>`'s being an error. They can be anything at all, so would `T1`, or `T2` be the error type?  This question does not make sense to ask `Either`.  

In summary use...  
`Response` when you do not need to know the details of *how* some operation failed (*just that it did*).  
`Either` when you have a range of values that can be produced from some action (*one of the values could still be an error, but doesn't have to be*).  
`Maybe` when you want low level error details propagated up to the caller, so they can make better decisions with the provided data.  

Below we find a contrived example of how one would use `Maybe`:  

We attempt to parse a integer from a string input.  
Maybe we will get a integer from the operation, or maybe we will get some domain specific error type instead.  

```cs
class ParseService
{
    public enum ParseError { InputNull, InputNotInteger }
    
    private static readonly Maybe<int, ParseError> _maybe = default;
    
    public Maybe<int, ParseError> Integer(string input)
    {
        if (input == null) return _maybe.With(ParseError.InputNull);
        
        if (!int.TryParse(input, out int number)) return _maybe.With(ParseError.InputNotInteger);
        
        return _maybe.With(number);
    }
}
```

The calling code is as follows:  
```cs
var parse = new ParseService();

var number = parse.Integer("1234");
var nan = parse.Integer("hello world");

var result1 = number.Match(value => $"{value} is a integer!", error => $"Error: {error}.");
var result2 = nan.Match(value => $"{value} is a integer!", error => $"Error: {error}.");

// [Output]
// result1: "1234 is a integer!"
// result2: "Error: InputNotInteger."
```

If a custom error type is not required, then ParseService would be implemented as follows:  
```cs
class ParseService
{
    public Maybe<int> Integer(string input)
    {
        var maybe = new Maybe<int>();
        
        try
        {
            var result = int.Parse(input); // Will throw Exception when format is not valid.
            maybe = maybe.With(result);
        }
        catch (Exception ex)
        {
            maybe = maybe.With(ex);
        }
        
        return maybe;
    }
}
```

When you don't define a type for `TError`, then `Exception` is the default.  

## Alias`<T>`

Alias allows you to *"name"* existing types, without changing their underlying behaviour.  
For example, you may find yourself returning a `string` from a method, but it is not clear to the caller what this `string` value is used for.  
`Alias` provides some clarity as to the type's purpose, in the same vein as a property or field name would (*i.e. "string email" is more descriptive than "string" alone*).  

Take the method `Task<Response<string>> GetBearerToken(Authentication model)`.  
This method's name, argument, and return type tells us a lot of information.  
It's clear from the method name that the `string` returned will be a bearer token, the argument will house the username / password + grant type etc.  
`Task` tells us that the method will run over the network asynchronously, and `Response` let's us know the method *can* fail while retrieving the bearer token.  
In this scenario *"naming"* the return type from `string` to something else provides little to no clarification, than what the method already signals to the caller.  

Often when integrating with third party APIs, you need only one value from endpoint A, to then invoke endpoint B.  
A common pattern is to **search** by some client reference, then  gather more **detail** using the search Id.  
Let's say we're getting customer details using a third party API from "**Acme Corporation**".  
Such methods might look like the following (*Task / Response removed for brevity*): 

* `string SearchFor(string email)`
* `Customer DetailFor(string acmeRequestId)`

In this example, the `string` returned from `SearchFor` is the acmeRequestId, but that is somewhat ambiguous.  
It might be obvious when these methods are next to each other on some service / interface, but what if these methods are surrounded by 10 others?  
What if you came to the project a year later, found you need to call the **detail** API, and you must to determine what the acmeRequestId is?  
Since it's a string it really could be anything, it doesn't give you much help in finding what you should be passing in.  
There are other solutions here of course, you could:
* Examine unit tests for examples of the **detail** API usage.
* Check the method's comments.
* Consult the project's documentation.
* Ask a colleague.  

These solutions work, but the answers are found too far away from the code you're trying to write.  
Instead have the types tell the story, be the documentation.  
So we introduce a custom domain model, and change the method signatures to clear up all uncertainty.  

```cs
class SearchModel
{
    public string AcmeRequestId { get; set; }
}
```

* `SearchModel SearchFor(string email)`
* `Customer DetailFor(SearchModel acmeRequest)`

This is pretty good, it's clear if we want to invoke the **detail** API, we must first invoke the **search** API to retrieve a `SearchModel`.  
*But...* look at the verbosity we have introduced for something so simple.  
If `SearchModel` had additional required business values it would be great, as we could justify the POCO.  
There is now more for the developer to understand (*an extra model*), and they may be surpised if they look at the definition, that it only contains a single string.  

Of course you could create an abstraction over the two methods that you expose to the rest of the codebase so you only deal with the "*complexity*" once:  
i.e. `Customer GetCustomerDetails(string email) => DetailFor(SearchFor(email))`.  
That's just an inconvenient truth of the example I've chosen, so let's ignore that for now; and stretch your imagination such that the "low level" search method is commonly used.  
You could also argue adding *another* layer of abstraction here (*i.e. layers of indirection*) needlessly complicates the overall architecture, so you opt to not "combine" these methods.  

I like the `SearchModel` solution, but I didn't want to have a new class, in a different file, that I had to create for one single value.  
I like the "abstract two methods into one method" solution, now "acmeRequest" is hidden from me; but it's harder to see what is happening, as the "real" work is deep down in abstractions.  
While the so called "complexity" of the abstraction is not yet evident, as the codebase grows over time it will be introduced.  
The space between these two methods will widen, as they are split into different services, and the mental leap required to join these two methods adds to your 99 other problems.  
Leaving the return type for the **search** API as a `string` is an option, but then developers have to read the method's implementations, or rely on the comments / documentation staying up to date with the code.  

Enter `Alias<T>`.  
When you want to give a name to a type.  

```cs
class AcmeRequestId : Alias<string> { public AcmeRequestId(string value) : base(value) { } }
```

To define our own `Alias` we start with a class, inhert from `Alias<T>`, and provide a constructor.  

Let's rewrite our initial methods that used raw strings:  

* `AcmeRequestId SearchFor(string email)`
* `Customer DetailFor(AcmeRequestId acmeRequestId)`

`AcmeRequestId` is more or less a normal string - with a name!  
Since this is a one liner (*won't be adding extra properties, or methods to this type*), I'm happy to define this inline with my service code (*instead of creating a new file for it*).  
`Alias` has helped me document the argument for Acme's **detail** API (*with little effort*).  
`AcmeRequestId` will implicitly cast to a `string` (*or any T*), making it easy to use with existing code that expects a string (*or T*) type; no need to manually convert it back to a `string` / `T`.  

Another use case for `Alias<T>` is overloaded methods:  

* `string SearchFor(string email)`
* `string SearchFor(string name)`
* `string SearchFor(string mobile)`

This is not possible, as the runtime can't determine which method you're trying to invoke; you may end up creating different names such as `SearchByMobile(string value)` x 3.  
Let's create an `Alias` for them instead:  

```cs
class Email : Alias<string> { public Email(string value) : base(value) { } }
class Name : Alias<string> { public Name(string value) : base(value) { } }
class Mobile : Alias<string> { public Mobile(string value) : base(value) { } }
```

Now we can have the same method name for all search terms (*even though they are all essentially strings*):  

* `string SearchFor(Email email)`
* `string SearchFor(Name name)`
* `string SearchFor(Mobile mobile)`

Lastly `Alias<T>` can be used for simple value transformations.  
For example, let's say you wanted a string to always be uppercase. Instead of adding validation to ensure it is (*or doing it yourself*), you could create an `Alias<string>`:  

```cs
class UpperCase : Alias<string> { public UpperCase(string value) : base(value?.ToUpper()) { } }
```

As you see we transfom the value in the constructor before passing it down to the base class.  
The type would then be used in place of a string:  

```cs
void Save(UpperCase username)
{
    // username is guaranteed to be upper case (assuming it's not null).
}
```

The base class `Alias<T>`, cannot implement autocasting from `T`, to your custom type.  
This is because the abstract class has no knowledge of the your type, and `implicit operators` do not accept generic parameters in C#.  
If you would like to implement this behaviour (_such that the user does not need to call your constructor_), simply add this one liner to your `Alias` definition.  

```cs
/// <summary>The current element's index.</summary>
public sealed class Index : Alias<int> {
    public Index(int value) : base(value) { }
    public static implicit operator Index(int value) => new (value);
}
```

The above custom type `Index` works great, but it has one big problem.  
It forces you to create a reference class (_`Index`_) for a value struct (_`int`_).  
This might be ok if `T` was a reference type too, but quite wasteful when `T` is a value type.  
To get around this, we also have a struct alias type: `A<T>`.  
Structs are not as versatile as classes, for example they cannot implement inheritance - which is how `Alias<T>` works today (it is abstract, so there is no runtime penality for doing this_).  
There are two work arounds I have for you:  

1) Use a file, or global [using directive](https://learn.microsoft.com/en-us/dotnet/csharp/language-reference/keywords/using-directive) (_a complier feature_).

```cs
global using Index = ContainerExpressions.Containers.A<int>; // Put this in GlobalUsings.cs file.
// OR
using Index = ContainerExpressions.Containers.A<int>; // Put this in the local file - not as useful as the Index name won't work everywhere.
```

That solution is pretty good, you get a descriptive name; and a type overload.  
But you do miss out on a few things:  
\> You cannot add XML comments to the type, to further explain your alias.  
\> You cannot customise the `implicit operator` behaviour, or override any of the `Equals`, `GetHashCode`, or `ToString` methods.  
\> You cannot modify the value in the constructor, before it's sent down to the alias type (_so you could not implement `UpperCase` for example_).  

2) If you need something more than a `using directive`, you'll need to create this wrapper around your `T`.

```cs
/// <summary>The current element's index.</summary>
public readonly struct Index
{
	public A<int> Value { get; }
	public Index(int value) { Value = new(value); }

	public static implicit operator A<int>(Index alias) => alias.Value;
	public static implicit operator Index(A<int> value) => new(value.Value);
	public static implicit operator int(Index alias) => alias.Value.Value;
	public static implicit operator Index(int value) => new(value);

	public override string ToString() => Value.ToString();
}
```

This gets you everything in `Alias<int>`, missing from `A<int>` - but in struct form!  
I cannot make this struct for you unfortunately, you will have to copy + paste it; and replace the `int` with your desired type.  
That said, this should be possible if using [Source Generators](https://learn.microsoft.com/en-us/dotnet/csharp/roslyn-sdk/source-generators-overview), but I haven't played with them yet. 

Be aware that `Index` is not safe to do `Equals` comparison on, or to use in collections such as a `Dictionary`.  
For such cases you would need to expose the underlying `A<T>`, so cast it first, or access it directly though the `Value` property.  
Note: the reference type `Alias<T>` does not have this problem, and is safe to use anywhere you would use `T`.  

You could do away with the the helper methods for a more concise version, you'd just need to manually access the underlying alias type (_`A<int>`_) though the `Value` property:  

```cs
public readonly struct Index
{
	public A<int> Value { get; }
	public Index(int value) { Value = new(value); }
}
```

You might ask - why am I using `Index` in so many examples? When would you wrap a plain `int`?  
Well 1) to show the differences without introducing new concepts; and 2) because I have a perfect example that would benefit from an alias.  
Take the following extension method from the `System.Linq` namespace:

```cs
// Summary:
//   Projects each element of a sequence into a new form by incorporating the element's index.
//
// Selector:
//   A transform function to apply to each source element; the second parameter of the function represents the index of the source element.
//
public static IEnumerable<TResult> Select<TSource, TResult>(this IEnumerable<TSource> source, Func<TSource, int, TResult> selector);

// Code example of using the above extension method:
new string[] { "Jane", "John" }.Select((x, i) => $"Index: {i}, Name: {x}.");
```

The problem here is the selector: `Func<TSource, int, TResult>` - you can infer what `TSource`, and `TResult` are, but what is `int` doing here?  
When creating the lambda function in your IDE, there is no hint on the `int` type, as C# does not provide a way to add XML comments on `Func` type parameters.  
So you either already know it, or you stop what you're doing and go read the full comments, or Google / StackOverflow / ChatGPT it.  
If `int` was named `Index` instead, it would be much more obvious what this parameter is doing here.  
We could put XML comments on the type too, which would show up in the IDE as you are creating the lambda - awesome!  
I had a similar method to write in this project, I started off with an `int`, but changed it to `Index` (_with comments explaining it's usage_):  

```cs
// Before:
public static TError[] LogError<TError>(this TError[] ex, Func<TError, int, string> format) where TError : Exception
{
    for (int i = 0; i < ex.Length; i++) { Trace.Log(format(ex[i], i)); LogException(ex[i]); }
    return ex;
}

// After:
public static TError[] LogError<TError>(this TError[] ex, Func<TError, Index, string> format) where TError : Exception
{
    for (int i = 0; i < ex.Length; i++) { Trace.Log(format(ex[i], i)); LogException(ex[i]); }
    return ex;
}

// Notice how nothing else in the function needed to change after replacing int with Index?
// This is thanks to the implicit casting provided by Alias, the loop's i is converted into an Index automatically.
```

You could make the argument that new C# language features such as a properties' init accessor, or record types make this `Alias` container obsolete.  
They dramatically cut down on the red tape required when creating new types, which is the same goal as `Alias`.  
In the long term I imagine that will be the ultimate fate of this library, every addition here will slowly be eaten away as C# adopts similar concepts into it's specification.  
That is already the fate of the `Match` expression, as C# now has some pretty powerful pattern matching built into the standard kit.  
If you think the language has a better implementation than the types here, you should definitely use them.  
Honesty, if C# ever got some level of Monad support (*not counting Task or LINQ*), that would be the end for this library.  

## Combining `Alias` & `Either`

You can create custom reusable friendly named types by combining `Alias`, and `Either`.  
This can save you from needing to write out the entire `Either` type each time you use it; and you can give the type a descriptive name too.  

```cs
// Accepts either a: string, or an int. Does not do any extra processing on the input.
class StringOrInt : Alias<Either<int, string>> {
    public StringOrInt(Either<int, string> value) : base(value) { }
}

StringOrInt stringOrInt = new StringOrInt("1"); // String value
int number = stringOrInt.Value.Match(x => x, int.Parse); // Output: 1
bool isString = stringOrInt == "1"; // Output: true
bool isInt = stringOrInt == 1; // Output: false

stringOrInt = new StringOrInt(1); // Int value
number = stringOrInt.Value.Match(x => x, int.Parse); // Output: 1
isString = stringOrInt == "1"; // Output: false
isInt = stringOrInt == 1; // Output: true
```

```cs
// Accepts either a: string, short, int, or long. Converts the input to a long.
class ConvertToLong : Alias<long> {
    public ConvertToLong(Either<string, short, int, long> value) : base(value.Match(long.Parse, Convert.ToInt64, Convert.ToInt64, x => x)) { }
}

ConvertToLong convertToLong = new ConvertToLong("1"); // String value
long number = convertToLong; // Output: 1

convertToLong = new ConvertToLong((short)1); // Short value
number = convertToLong; // Output: 1

convertToLong = new ConvertToLong((int)1); // Int value
number = convertToLong; // Output: 1

convertToLong = new ConvertToLong((long)1); // Long value
number = convertToLong; // Output: 1
```

## Compose`<T>`

Used to run dependant functions one after each other, such that the first function's output feeds into the second function's input.  
This continues until the last function, when that type is returned in the container `Response<T>`.
If any of the functions fail, the whole chain will fail and the final container's response will be invalid.

In the example below `DownloadHtml`, and `PersistHtml` are functions with the return type of `Response<T>`, in this case T is string for both functions.  
The function `DownloadHtml` retrieves a webpage from a server, and the function `PersistHtml` saves that html to a file, returning the path to the new file.  
In this case the final function `PersistHtml`, has a return type of `Response<string>`, so the return type of `Expression.Compose`, will also be `Response<string>`.  
If either of these functions fail, the end result of the expression will be an invalid response.
```cs
var filepath = Expression.Compose(DownloadHtml, PersistHtml);
```

Note: there is also Expression.ComposeAsync() for composing asynchronous functions.

## Trace

Logs function inputs, and outputs so you can save them to a trace file.  

Tracing is necessary in any non-trivial program to determine production runtime bugs.  
However these traces typically get in the way of the core code, and force you to break up code into pieces so you can log it's current state.  
The trace container slots in with existing code by taking a type, and returning that same type.  

Before you use Trace you must set a logger, an Action that takes a string (returns void), see below for an example.  

`Trace.SetLogger(Console.WriteLine);`  

See below for an example of logging the output of each function:  

````cs
// Initialize the Trace with a logger.
var logs = new List<string>();
Trace.SetLogger(log => logs.Add(log));

// Create a function to trace the incrementing.
Func<int, string> trace = x => string.Format("The value of the int is {0}.", x);

// Some functions that keep incrementing their input.
Func<Response<int>> identity = () => Response.Create(0);
Func<int, Response<int>> increment = x => Response.Create(x + 1);

var count = Expression.Compose(identity.Log(trace), increment.Log(trace), increment.Log(trace));

// The follow is logged to Trace:
// The value of the int is 0.
// The value of the int is 1.
// The value of the int is 2.
````

## Match`<T>`

Let's say you'd like to do different things based on the state of some input.  
For example summing an array of integers, you have three states, the array is null, the array is empty, the array has more then zero elements.  
You can have a pattern for each state (or a subset of the states), and have different behaviour for each pattern.  

In the example below when the input is null, we return an invalid response, when the input has no elements, we return 0, otherwise we return the sum of the array's elements.

```cs
var input = new int[] { 1, 2, 3 };

var result = Expression.Match(input,
    Pattern.Create<int[], int>(x => x == null, _ => Response.Create<int>()), // When null, return an invalid response.
    Pattern.Create<int[], int>(x => x.Length == 0, _ => Response.Create(0)), // When empty, return 0.
    Pattern.Create<int[], int>(x => x.Length > 0, Sum) // When more than zero elements exist, sum them, and return that result.
);

// Sum all elements in the array.
static Response<int> Sum(int[] numbers)
{
	var count = 0;
	for (int i = 0; i < numbers.Length; i++)
	{
		count += numbers[i];
	}
	return Response.Create(count);
}
```

Note: there is also Expression.MatchAsync() for asynchronous patterns.

## Retry`<T>`

Execute the same function until it's Response is valid, or you run out of retries as defined by the options.  
By default the options are set to 1 retry, and a delay of 100 milliseconds before trying again.  
There is a method overload to pass in your own options for a more customized Retry.  

In the example below, we create a user in a database, and get their Id in return.

```cs
// Using default options, this will try a second time if the first time fails.
var userId = Retry.Execute(() => CreateUser(new UserModel { Name = "John Smith" }));

public Response<T> CreateUser(UserModel user)
{
	var response = new Response<int>(); // Invalid state, indicates a Retry.
	
	try
	{
		using (var connection = new SqlConnection(connectionString))
		using (var command = new SqlCommand("usp_insert_createUser", connection))
		{
			command.CommandType = CommandType.StoredProcedure;
			command.Parameters.Add("@Name" user.Name);
			connection.Open();
			
			var userId = (int)command.ExecuteScalar();
			response = response.WithValue(userId); // State is now valid, no need to run the function again.
		}
	}
	catch (exception ex)
	{
		// Log error here...
	}
	
	return response;
}
```

Note: we also have options for jitter, and `Retry.ExecuteExponential()` to grow the delays exponentially (*instead of linearly*) between failed attempts.  

## Reduce`<T>`

Combine many values of `T` to a single value of `T`.  
Useful for any type that is associative.  
Only valid states of `Response<T>` are combined in the reduce expression.  

In the example below we combine many words into a sentence.  

```cs
Func<string, string, string> combine = (x, y) => string.Concat(x, " ", y);

var words = new Response<string>[] { Response.Create("world") };
var arg1 = "hello";

string sentence = Expression.Reduce(combine, arg1, words);
```

## Funnel`<T>`

Takes many `Response<T>`'s and invokes a function passing each `T` as an argument; if and only if each `Response<T>` is in a valid state.  
If at least one input is in an invalid state, then an invalid `Response<T>` is returned instead of calling the function.  
Useful when you'd like an operation to happen only when several previous operation completed successfully.  

In the example below we only take the power of two numbers if they were calculated correctly.  

```cs
// Returns an Invalid Response<double> when the divisor argument is 0, otherwise the result is stored in a valid Response<double>.
Func<double, double, Response<double>> divide = (dividend, divisor) => divisor == 0 ? new Response<double>() : Response.Create(dividend / divisor);

var e = divide(150D, 55D);
var pi = divide(22D, 7D);

var answer = Expression.Funnel(e, pi, Math.Pow);
```

## Shared Types

Common reusable types for use with containers such as: `Either`, `Maybe`, and `Response`.  
Prebaked types for a "batteries included" feeling, these should be common types you'd make yourself; feel free to use these.  
In general the shared types will be structs, with a generic, and non-generic implementation:  

* `Unit:` A marker type to use when you have no return type, or result. Provides access to a cached `Response<Unit>` for reuse.
* `Ok:` 200 / Created: 201 / Accepted: 202 / NoContent: 204 (Your request was successfully processed).
* `BadRequest:` 400 (Validation error on your request).
* `Unauthorized:` 401 (We do not know who you are).
* `Forbidden:` 403 (We know who you are, but you are not allowed to be here).
* `NotFound:` 404 (Requested server resource was not found).
* `TooManyRequests:` 429 (Rate Limit).
* `Error:` 500 (Internal Server Error).
* `BadGateway:` 502 (Upstream Error).
* `GatewayTimeout:` 504 (Upstream Timeout).

You might have custom types, such as: `InvalidEmail`, `UserNotActive`, `AdminUser`, etc.  
Find a contrived example below:  

```cs
public class UserController : ControllerBase  
{
    public IActionResult Put(User user)
    {
        return Update(user).Match<IActionResult>(
            ok => NoContent(),
            notFound => StatusCode(404),
            badRequest => BadRequest(badRequest.Value), // Using the generic version to provide error messages to the client.
            error => StatusCode(500)
        );
    }

    private static Either<Ok, NotFound, BadRequest<string[]>, Error> Update(User _)
    {
        return new BadRequest<string[]>(new string[] { "Your request is bad." });
    }
}
```

## Shared Functions

Generic, common, little helper functions for lambdas.  
There are only a few implementations (*for now*) - so I'm just going to paste the class here in lieu of documentation:  

```cs
/// <summary>A place to put shared common functions.</summary>
public static class Lambda
{
    /// <summary>Maps the input directly to the output.</summary>
    public static T Identity<T>(T x) => x;

    /// <summary>Pretends to return a T (i.e. to compile), but will really throw the passed exception.</summary>
    public static T Identity<T>(Exception ex) => throw ex;

    /// <summary>Discards the function input, and returns the specified result.</summary>
    public static Func<T, T> Default<T>(T result = default) => _ => result;

    /// <summary>Discards the function input, and returns the specified result (of a different type to the input type).</summary>
    public static Func<T, TResult> Default<T, TResult>(TResult result = default) => _ => result;

    // Wrap method delegates, so the complier can recognise them as Func{TResult} types. 
    public static Func<TResult> ToFunc<TResult>(this Func<TResult> func) => func;

    // Wrap method delegates, so the complier can recognise them as Action types. 
    public static Action ToAction(this Action action) => action;
}
```

Why use these? They are static functions, with descriptive names.  
Of course you can make your anonymous delegates static inline now too i.e. `(static x => x)`.  

When ordering `ints` using `System.Linq`, you must provide a **key** to `OrderBy`.  
This forces you to write: `new int[] { 3, 2, 5, 7, 6 }.OrderBy(x => x);`.  
You can replace `(x => x)`, with the `Identity` function: `new int[] { 3, 2, 5, 7, 6 }.OrderBy(Identity);`.  
This makes the code's intent clear.  
Note: you would normally need to write `OrderBy(Lambda.Identity)` instead.  
I suggest adding `global using static ContainerExpressions.Containers.Lambda;` to your **GlobalUsings.cs** file.  

The `ToFunc<TResult>`, and `ToAction` helpers are used to turn any method into a base that can be targeted with extensions.  
```cs
private static int GetResult(int i, string s) => 1;
private static void GetVoid(int i, string s) => throw new Exception(); // Error is caught (and logged) by the await below.

// Func with 2 args is converted into a func with 0 args.
Response<int> funcResult = await Lambda.ToFunc(() => GetResult(0, "")); // await safely wraps the function in a Response<int>.

// Action with 2 args is converted into a action with 0 args.
Response<Unit> actionResult = await Lambda.ToAction(() => GetVoid(0, "")); // await safely wraps the function in a Response<Unit> (Unit represents void).

// There is an argument builder with compiler implicit type support. The end result is the same as above, so it's more of a preference.
Response<int> argsFuncResult = await Lambda.Args(0, "").ToFunc(GetResult);
Response<Unit> argsActionResult = await Lambda.Args(0, "").ToAction(GetVoid);
```

## Instance

Holds instances for any reference type `TValue`, you can use this instead of `null` when falling back to some default value.  
By not having to create new instances, you are saving on object allocation.  
The instances given to, and read from `Instance` should be considered readonly, and immutable.  
Instances should be created at program start up.  
`Instance` is implemented in classes created at compile time (*i.e. using static + generics*), so there is no backing collection, or explicit locking.  
Along the lines of:  
\> `string.Empty`, and `Array.Empty<T>()`.  
\> We have: `var str = Cache.Get<string>()`, and `var strArray = Cache.Get<string[]>()`.  
You can only have one value per type. However this restriction *can* be worked around using `Alias<T>`.  

```cs
// Example of adding different string references to Instance.
class Jane : Alias<string> { public Jane() : base(nameof(Jane)) { } }
class John : Alias<string> { public John() : base(nameof(John)) { } }

// This works because we are not adding a string type, we are adding two new types: Jane, and John.
Instance.Create(new Jane());
Instance.Create(new John());

John john = Instance.Of<John();
string name = Instance.Of<John>(); // Auto casting provided by Alias<string> works here.
Jane jane = Instance.Of<Jane>();
```

## InstanceAsync

Just like `Instance`, but targets `Task` types.  
* `InstanceAsync.Of<int>():` gets a cached, and completed `Task<int>` with a default `int` value.
* `InstanceAsync.ValueOf<int>():` gets a completed `ValueTask<int>`, with a default `int` value; newly created on each invoke.
* `InstanceAsync.ResponseOf<int>():` gets a cached, and completed `ResponseAsync<int>`, with a default `int` value.

## Combining `Alias` & `Instance`

Below is an example of creating a CVS type, that reuses a `char[]` instance.  
As demonstrated, custom types can be quick to build, and efficient to run.  

```cs
public sealed class Csv : Alias<string[]> {
    private sealed class Comma : Alias<char[]> { public Comma() : base(new char[] { ',' }) { } } // Private type for Instance to avoid collisions on char[].
    static Csv() => Instance.Create(new Comma()); // Static constructor registers the Instance type of Comma.
    public Csv(string csv) : base(csv.Split(Instance.Of<Comma>())) { } // Splits the string into a string[], the char[] argument is reused.
}
```

## ResponseAsync`<T>`

A task-like type that can be used on async functions to catch, and log exceptions thrown in a method.  
You can use `ResponseAsync<T>` anywhere you would use `Task<T>`, and it will return a `Response<T>` - instead of a `T`.  

```cs
private static async ResponseAsync<int> GetValue()
{
    return await Task.FromResult(42);
}

private static async ResponseAsync<int> GetError()
{
    throw new Exception();
    return await Task.FromResult(42);
}

var value = await GetValue(); // Result: 42.
var error = await GetError(); // Result: No value, as an error occurred (but no exception is thrown from this await).
```

**Note:** Exceptions are logged to the `Try.SetExceptionLogger()` listener, so make sure to set this at startup to catch them.

`ResponseAsync<T>` functionality also includes:
* `AsTask():` Convert `ResponseAsync<T>` into a `Task<Response<T>>` (*that won't throw exceptions*).
* `AsValueTask():` Convert `ResponseAsync<T>` into a `ValueTask<Response<T>>` (*that won't throw exceptions*).
* `AsResponse():` Converts a `Task<T>`, or `ValueTask<T>` into a `Task<Response<T>>`, or `ValueTask<Response<T>>` respectively (*with no exceptions*).
* `FromResult<T>(T):` Creates a `ResponseAsync<T>` task in a completed state, with a valid `Response<T>` result.
* `FromError<T>(Exception):` Creates a `ResponseAsync<T>` task in a completed state, with an invalid `Response<T>` result (*errors are logged*).

## Custom Async Method Builders

Override the default async method builder for `Task`, and `Task<T>` / `Task<Response<T>>`.  
This keeps their task-like types, and awaiters intact; only replacing the method builder implementation (*specifically around exception handling*).  
If the method returns a `T`, instead of a `Response<T>`, errors will still be logged, but they will also be thrown if you attempt to get the result.  

Example usage:
```cs
// Returns an invalid response when an exception is thrown in this method (or the task is cancelled), otherwise returns a valid response.
[AsyncMethodBuilder(typeof(ResponseAsyncTaskCompletionSource<>))]
public async Task<Response<string>> Foo() // TaskAwaiter with a custom async method builder.
{
    await Task.Delay(0);
    return Response.Create("Hello, World!");
}
```

## Custom Awaiters

A `Task` can be canceled, or faulted; in these cases you won't get a result from `await`, instead you'll get a runtime exception.  
If you wrap a `Task` in a `Response`, a custom awaiter will kick in:

* **Normal:** `Task<Response<T>>` or `Task<T>`
* **Target:** `Response<Task<Response<T>>>` or `Response<Task<T>>`

```cs
int number = 100;
Response<int> input = Response.Create(number);

Task<Response<int>> task = Task.FromResult(input); // Normal: Task<Response<T>>
Response<Task<Response<int>>> response = Response.Create(task); // Target: Response<Task<Response<T>>>

Response<int> result = await response; // This is a "safe" Response, it will be invalid when the Task is canceled, or faulted - instead of throwing exceptions.

Assert.IsTrue(result);
Assert.AreEqual(number, result);
```

There are other custom awaiters available for `ValueTask`, and `Task`; along with their various **Packed** implementations.  
All these types can be safely converted to `Response` / `Response<T>` using the `await` keyword:  

* `Response<ValueTask>`
* `Response<ValueTask<T>>`
* `Response<Task>`
* `Response<Task<T>>`
* `Response<Task<Response<Response>>>`
* `Response<Task<Response<Task<Response>>>>`
* `Response<Task<Response<Response<T>>>>`
* `Response<Task<Response<Task<Response<T>>>>>`
* `Func<ResponseAsync<T>>`
* `Func<T>`
* `Action`

## ResponseValueTask`<T>`

A helper type for the `Response` custom awaiters.  
`ResponseValueTask<T>` serves as a bridge between `Task<T>` / `ValueTask<T>`, and `Response<T>`.  
The core difference between `ResponseValueTask<T>`, and `Response<T>`; is `ResponseValueTask<T>` is constrained to be a `ValueTask<T>`.  
This allows us to cast easily from some `Task<T>`, to `ResponseValueTask<T>`, and then down to `Response<T>`.  
For example:

```cs
// Function to call.
static async Task<int> Divide(int numerator, int denominator)
{
    var quotient = numerator / denominator;
    await Task.Delay(1);
    return quotient;
}

// Calling code.
ResponseValueTask<int> successResponse = Divide(1, 1); 
Response<int> success = await successResponse; // Response<int> success = Divide(1, 1); is not possible to cast; so we use ResponseValueTask<int>.

ResponseValueTask<int> errorResponse = Divide(1, 0); // This would normally be a runtime exception.
Response<int> error = await errorResponse;

Assert.IsTrue(success);
Assert.AreEqual(1, success);
Assert.IsFalse(error);
```

Writing out the types on seperate lines is not necessary (*i.e.*):

```cs
Response<int> success = await new ResponseValueTask<int>(Divide(1, 1)); // OK.
Response<int> error = await new ResponseValueTask<int>(Divide(1, 0)); // Fail (but no runtime exception).

Assert.IsTrue(success);
Assert.AreEqual(1, success);
Assert.IsFalse(error);
```

It's fine to use `var` too, as it's the awaiter that determines the type:

```cs
// No need to wrap Task<T> types in try catches anymore!
var success = await new ResponseValueTask<int>(Divide(1, 1));
var error = await new ResponseValueTask<int>(Divide(1, 0));
```

## ResponseTask`<T>`

`ResponseTask<T>`, has all the properties of `ResponseValueTask<T>` - except it only works with `Task`, not `ValueTask`, or some pre-calculated value of `T`.  
This is the one you should be using 90% of the time, as `ResponseValueTask<T>` is only better in specific scenarios (*for all the same reasons as .net's `ValueTask`*).  

**Note:** Exceptions on a **Faulted** `Task` will be logged though to `Try.SetExceptionLogger()` subscribers, so make sure to set this at program startup.  

# Credits
* [Icon](https://www.flaticon.com/free-icon/bird_2630452) made by [Vitaly Gorbachev](https://www.flaticon.com/authors/vitaly-gorbachev) from [Flaticon](https://www.flaticon.com/)

# Changelog

## 6.0.0

* Started a changelog (*i.e. pre version 6 you need to check the raw git commits*).  
* `Bind` and `Transform` now take any `T` off some `Response` instead of only a `Response<T>`.  
* You can now `Log` off any `T`, previously it was only for `Response<T>`.  
* Async `Try` container now handles `AggregateException` separately from the standard `Exception` type.  
* The `Retry` container now accepts an exponential backoff algorithm, with optional jitter.  
* Any `T` value can now initiate a `Response` chain using the `Push` extension.  
* Introduced `NotNull<T>` as a way to counter reference types being null.  
* Lots of small method signature changes to flesh out `Bind`, `Transform`, and `Log`; allowing more combinations of `T`, `Response<T>`, and `Task<Response<T>>` in chains.  
* New container `Maybe<TResult, TError>` with operations for `Match`, `Bind`, and `Transform`.  

There is nothing "major" in this release, most of he changes are quality of life around logging: "anything anywhere"; and composing varied functions via `Bind` and `Transform` that were missing.  
The major version was bumped (*MAJOR.MINOR.PATCH*), as we've introduced backwards incompatible changes to some signatures that weren't quite right before.  

## 6.0.1

* Nuget package metadata added to the project file, allowing easy releases via the Visual Studio Pack command.  

## 7.0.0

* Breaking change: renamed extension method `Response.WithValue(Any<T>)` to `Response.With(Any<T>)` to follow the conventions of other containers.  
* New container `Alias<T>` allows you to give names to types, while retaining the behavior of the underlying type.  

## 8.0.0

* Breaking change: some methods have been renamed, as the complier had some issues working out the overloaded types (`T` / `Response` / `Response<T>` / `Task` / `Task<T>` / `Task<Response>` / `Task<Response<T>>`).  
* When a raw `Task`, or `Task<T>` are passed into extension methods; they will now be safely unwrapped, and any errors will be logged out.  

## 8.0.1

* Added readme file to nuget.

## 9.0.0

* Updated additional **Pack** details in the solution file (*debug symbols, xml comments, readme file, etc*).
* New extension method for `Response`, and `Response<T>` types (*both sync, and async*) called `Unpack()` - which flattens response containers.
* When `Try.SetExceptionLogger()` is configured, exceptions logged though `LogError()` - or found in `Response`, and `Maybe` containers, are sent though.
* Added a `LogError` extension method, so you can which forwards exceptions to the logger setup for: `Try.SetExceptionLogger`.
* Expanded the function targets for `Funnel`. Was only `T`, now includes: `Response<T>`, `Task<T>`, and `Task<Response<T>>`.
* Added equals overloads to `Either`, so you can easily compare some `T` value to the `Either` container.
* Added `TryGetT*` to `Either`, allowing access to the types without going though `Match<TResult>`.
* Added a struct version of `Alias<T>` called `A<T>`.
* Added shared types to resue in containers, such as `OK`, `BadRequest`, `Error`, etc.
* Created shared common lambda functions, such as `Lambda.Identity` to reflect the input as the output. i.e. instead of having to write `(x => x)`.
* Added `TryGetValue()`, and `TryGetError()` to `Maybe`.
* Added `Unpack` to `Maybe`.
* Created a new container `Cache:` store readonly default values for reference types (*i.e. instead of using null*).

## 10.0.0

* Renamed `Cache` to `Instance`, as the former name lead to confusion on what the container did.
* Added custom awaiters for `Response` types wrapping `Task` / `ValueTask`.
* Created `ResponseTask` to make casting between `Task`, and `Response` easier (*`ResponseValueTask` is also available for `ValueTask` sources*).
* `ResponseAsync<T>` is a task-like type that can be used on `async` functions to catch, and log exceptions thrown in a method.
* Added async method builders for `Task`, and `ValueTask` to safely create `Response` types.
* Added a value type verion of `Later<T>` - `ValueLater<T>`.
