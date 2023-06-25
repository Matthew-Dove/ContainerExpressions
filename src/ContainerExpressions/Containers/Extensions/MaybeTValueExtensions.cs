using System;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace ContainerExpressions.Containers
{
    /// <summary>Utility methods for the Maybe TValue Container.</summary>
    public static class MaybeTValueExtensions
    {
        #region Miscellaneous

        private static class Cache<TValue>
        {
            public static Exception[] Value(TValue _) => Array.Empty<Exception>();
            public static Exception[] AggregateErrors(Exception _, Exception[] aggregateErrors) => aggregateErrors;
            public static Exception[] AllErrors(Exception error, Exception[] aggregateErrors)
            {
                Array.Resize(ref aggregateErrors, aggregateErrors.Length + 1);
                aggregateErrors[aggregateErrors.Length - 1] = error;
                return aggregateErrors;
            }
        }

        /// <summary>Returns the value of Maybe when the instance is TValue, otherwise @default is returned.</summary>
        /// <param name="default">The value to use when Maybe contains an instance of Exception.</param>
        public static TValue GetValueOrDefault<TValue>(this Maybe<TValue> maybe, TValue @default) => maybe.Match(x => x, _ => @default);

        /// <summary>Converts Maybe into a T2 Either.</summary>
        public static Either<TValue, Exception> ToEither<TValue>(this Maybe<TValue> maybe) => maybe.Match(x => new Either<TValue, Exception>(x), x => new Either<TValue, Exception>(x));

        /// <summary>Converts Maybe into a Response, when the instance is TValue a valid Response is returned; otherwise an invalid one is selected.</summary>
        public static Response<TValue> ToResponse<TValue>(this Maybe<TValue> maybe) => maybe.Match(x => new Response<TValue>(x), _ => new Response<TValue>());

        /// <summary>When Maybe has a value (i.e. not an error), the Func's result is returned, otherwise false is returned.</summary>
        /// <param name="condition">A function that accepts the Maybe's value, and returns a boolean.</param>
        public static bool IsTrue<TValue>(this Maybe<TValue> maybe, Func<TValue, bool> condition) => maybe._hasValue ? condition(maybe._value) : false;

        /// <summary>Returns the value of Maybe when the instance is TValue, otherwise @default is returned.</summary>
        /// <param name="default">The value to use when Maybe contains an instance of Exception.</param>
        public static Task<TValue> GetValueOrDefaultAsync<TValue>(this Task<Maybe<TValue>> maybe, TValue @default) => maybe.MatchAsync(x => x, _ => @default);

        /// <summary>Converts Maybe into a T2 Either.</summary>
        public static Task<Either<TValue, Exception>> ToEitherAsync<TValue>(this Task<Maybe<TValue>> maybe) => maybe.MatchAsync(x => new Either<TValue, Exception>(x), x => new Either<TValue, Exception>(x));

        /// <summary>Converts Maybe into a Response, when the instance is TValue a valid Response is returned; otherwise an invalid one is selected.</summary>
        public static Task<Response<TValue>> ToResponseAsync<TValue>(this Task<Maybe<TValue>> maybe) => maybe.MatchAsync(x => new Response<TValue>(x), _ => new Response<TValue>());

        private static readonly Exception _taskExCache = new Exception("Error waiting for task to successfully complete.");

        /// <summary>Creates a Maybe, that wraps a Task. When the task is successful, the value is set, otherwise the error is set.</summary>
        public static Task<Maybe<Unit>> ToMaybeTaskAsync(
            this Task value,
            [CallerArgumentExpression(nameof(value))] string argument = "",
            [CallerMemberName] string caller = "",
            [CallerFilePath] string path = "",
            [CallerLineNumber] int line = 0
            )
        {
            return value.ContinueWith(t =>
            {
                var ex = _taskExCache;
                if (t.Status == TaskStatus.Faulted)
                {
                    ex = t.Exception;
                    t.Exception.LogError(argument, caller, path, line);
                }
                return t.Status == TaskStatus.RanToCompletion ? Unit.MaybeValue : Unit.MaybeError(ex);
            });
        }

        /// <summary>Creates a Maybe, that wraps a Task. When the task is successful, the value is set, otherwise the error is set.</summary>
        public static Task<Maybe<TValue>> ToMaybeTaskAsync<TValue>(
            this Task<TValue> value,
            [CallerArgumentExpression(nameof(value))] string argument = "",
            [CallerMemberName] string caller = "",
            [CallerFilePath] string path = "",
            [CallerLineNumber] int line = 0
            )
        {
            return value.ContinueWith(t =>
            {
                var ex = _taskExCache;
                if (t.Status == TaskStatus.Faulted)
                {
                    ex = t.Exception;
                    t.Exception.LogError(argument, caller, path, line);
                }
                return t.Status == TaskStatus.RanToCompletion ? new Maybe<TValue>(t.Result) : new Maybe<TValue>(ex);
            });
        }

        /// <summary>Returns true when Maybe had a value set.</summary>
        public static bool TryGetValue<TValue>(this Maybe<TValue> maybe, out TValue value) { if (maybe._hasValue) { value = maybe._value; return true; }; value = default; return false; }

        /// <summary>Returns true when Maybe had an error set.</summary>
        public static bool TryGetError<TValue>(this Maybe<TValue> maybe, out Exception error) { if (!maybe._hasValue) { error = maybe._error; return true; }; error = default; return false; }

        /// <summary>Returns true when Maybe has aggregate errors, with are are bundled together.</summary>
        public static bool TryGetAggregateErrors<TValue>(this Maybe<TValue> maybe, out Exception[] errors)
        {
            errors = maybe.Match(Cache<TValue>.Value, Cache<TValue>.AggregateErrors);
            return errors.Length > 0; // We don't check maybe's "_hasValue", as you can have a single error, without aggregate errors.
        }

        /// <summary>Returns true when Maybe had an error set, aggregate errors are bundled together; along with the top level error.</summary>
        public static bool TryGetAllErrors<TValue>(this Maybe<TValue> maybe, out Exception[] errors)
        {
            errors = maybe.Match(Cache<TValue>.Value, Cache<TValue>.AllErrors);
            return !maybe._hasValue;
        }

        #endregion

        #region With

        /// <summary>Creates a new Maybe from the existing types, with the specified values.</summary>
        public static Maybe<TValue> With<TValue>(this Maybe<TValue> _, Response<TValue> value, Exception error) => new Maybe<TValue>(value, error);

        /// <summary>Creates a new Maybe from the existing types, with the specified values.</summary>
        public static Maybe<TValue> With<TValue>(this Maybe<TValue> _, Either<TValue, Exception> either) => new Maybe<TValue>(either);

        /// <summary>Creates a new Maybe from the existing types, with the specified values.</summary>
        public static Maybe<TValue> With<TValue>(this Maybe<TValue> _, TValue value) => new Maybe<TValue>(value);

        /// <summary>Creates a new Maybe from the existing types, with the specified values.</summary>
        public static Maybe<TValue> With<TValue>(this Maybe<TValue> _, Exception error) => new Maybe<TValue>(error);

        /// <summary>Creates a new Maybe from the existing types, with the specified values.</summary>
        public static Maybe<TValue> With<TValue>(this Task<Maybe<TValue>> _, Response<TValue> value, Exception error) => new Maybe<TValue>(value, error);

        /// <summary>Creates a new Maybe from the existing types, with the specified values.</summary>
        public static Maybe<TValue> With<TValue>(this Task<Maybe<TValue>> _, Either<TValue, Exception> either) => new Maybe<TValue>(either);

        /// <summary>Creates a new Maybe from the existing types, with the specified values.</summary>
        public static Maybe<TValue> With<TValue>(this Task<Maybe<TValue>> _, TValue value) => new Maybe<TValue>(value);

        /// <summary>Creates a new Maybe from the existing types, with the specified values.</summary>
        public static Maybe<TValue> With<TValue>(this Task<Maybe<TValue>> _, Exception error) => new Maybe<TValue>(error);

        #endregion

        #region Match

        /** maybe => sync, getValue => sync  **/

        /// <summary>Converts the value or error into a TResult, only one function is invoked depending on Maybe's underlying instance type.</summary>
        /// <param name="getValue">When Maybe contains a value, this function is called passing the instance in as a parameter.</param>
        /// <param name="getError">When Maybe contains a error, this function is called passing the instance in as a parameter.</param>
        public static TResult Match<TValue, TResult>(this Maybe<TValue> maybe, Func<TValue, TResult> getValue, Func<Exception, TResult> getError) => maybe._hasValue ? getValue(maybe._value) : getError(maybe._error);

        /// <summary>Converts the value or error into a TResult, only one function is invoked depending on Maybe's underlying instance type.</summary>
        /// <param name="getValue">When Maybe contains a value, this function is called passing the instance in as a parameter.</param>
        /// <param name="getError">When Maybe contains a error, this function is called passing the instance in as a parameter.</param>
        public static TResult Match<TValue, TResult>(this Maybe<TValue> maybe, Func<TValue, TResult> getValue, Func<Exception, Exception[], TResult> getError) => maybe._hasValue ? getValue(maybe._value) : getError(maybe._error, maybe.AggregateErrors);

        /** maybe => sync, getValue => async  **/

        /// <summary>Converts the value or error into a TResult, only one function is invoked depending on Maybe's underlying instance type.</summary>
        /// <param name="getValue">When Maybe contains a value, this function is called passing the instance in as a parameter.</param>
        /// <param name="getError">When Maybe contains a error, this function is called passing the instance in as a parameter.</param>
        public static Task<TResult> MatchAsync<TValue, TResult>(this Maybe<TValue> maybe, Func<TValue, Task<TResult>> getValue, Func<Exception, TResult> getError) => maybe._hasValue ? getValue(maybe._value) : Task.FromResult(getError(maybe._error));

        /// <summary>Converts the value or error into a TResult, only one function is invoked depending on Maybe's underlying instance type.</summary>
        /// <param name="getValue">When Maybe contains a value, this function is called passing the instance in as a parameter.</param>
        /// <param name="getError">When Maybe contains a error, this function is called passing the instance in as a parameter.</param>
        public static Task<TResult> MatchAsync<TValue, TResult>(this Maybe<TValue> maybe, Func<TValue, Task<TResult>> getValue, Func<Exception, Exception[], TResult> getError) => maybe._hasValue ? getValue(maybe._value) : Task.FromResult(getError(maybe._error, maybe.AggregateErrors));

        /** maybe => async, getValue => sync  **/

        /// <summary>Converts the value or error into a TResult, only one function is invoked depending on Maybe's underlying instance type.</summary>
        /// <param name="getValue">When Maybe contains a value, this function is called passing the instance in as a parameter.</param>
        /// <param name="getError">When Maybe contains a error, this function is called passing the instance in as a parameter.</param>
        public static Task<TResult> MatchAsync<TValue, TResult>(this Task<Maybe<TValue>> maybe, Func<TValue, TResult> getValue, Func<Exception, TResult> getError) => maybe.ContinueWith(x => x.Result._hasValue ? getValue(x.Result._value) : getError(x.Result._error));

        /// <summary>Converts the value or error into a TResult, only one function is invoked depending on Maybe's underlying instance type.</summary>
        /// <param name="getValue">When Maybe contains a value, this function is called passing the instance in as a parameter.</param>
        /// <param name="getError">When Maybe contains a error, this function is called passing the instance in as a parameter.</param>
        public static Task<TResult> MatchAsync<TValue, TResult>(this Task<Maybe<TValue>> maybe, Func<TValue, TResult> getValue, Func<Exception, Exception[], TResult> getError) => maybe.ContinueWith(x => x.Result._hasValue ? getValue(x.Result._value) : getError(x.Result._error, x.Result.AggregateErrors));

        /** maybe => async, getValue => async  **/

        /// <summary>Converts the value or error into a TResult, only one function is invoked depending on Maybe's underlying instance type.</summary>
        /// <param name="getValue">When Maybe contains a value, this function is called passing the instance in as a parameter.</param>
        /// <param name="getError">When Maybe contains a error, this function is called passing the instance in as a parameter.</param>
        public static Task<TResult> MatchAsync<TValue, TResult>(this Task<Maybe<TValue>> maybe, Func<TValue, Task<TResult>> getValue, Func<Exception, TResult> getError) => maybe.ContinueWith(x => x.Result._hasValue ? getValue(x.Result._value) : Task.FromResult(getError(x.Result._error))).Unwrap();

        /// <summary>Converts the value or error into a TResult, only one function is invoked depending on Maybe's underlying instance type.</summary>
        /// <param name="getValue">When Maybe contains a value, this function is called passing the instance in as a parameter.</param>
        /// <param name="getError">When Maybe contains a error, this function is called passing the instance in as a parameter.</param>
        public static Task<TResult> MatchAsync<TValue, TResult>(this Task<Maybe<TValue>> maybe, Func<TValue, Task<TResult>> getValue, Func<Exception, Exception[], TResult> getError) => maybe.ContinueWith(x => x.Result._hasValue ? getValue(x.Result._value) : Task.FromResult(getError(x.Result._error, x.Result.AggregateErrors))).Unwrap();

        #endregion

        #region Transform

        /// <summary>
        /// Converts the value of Maybe from one type to another.
        /// <para>If Maybe contains an error, then the error is propagated to the new maybe instead.</para>
        /// </summary>
        /// <param name="transform">A function to change the value's type.</param>
        public static Maybe<TResult> Transform<TValue, TResult>(this Maybe<TValue> maybe, Func<TValue, TResult> transform) => maybe.Match(x => new Maybe<TResult>(transform(x)), x => new Maybe<TResult>(x));

        /// <summary>
        /// Converts the value of Maybe from one type to another.
        /// <para>If Maybe contains an error, then the error is propagated to the new maybe instead.</para>
        /// </summary>
        /// <param name="transform">A function to change the value's type.</param>
        public static Task<Maybe<TResult>> TransformAsync<TValue, TResult>(this Maybe<TValue> maybe, Func<TValue, Task<TResult>> transform) => maybe.Match(x => transform(x).ContinueWith(y => new Maybe<TResult>(y.Result)), x => Task.FromResult(new Maybe<TResult>(x)));

        /// <summary>
        /// Converts the value of Maybe from one type to another.
        /// <para>If Maybe contains an error, then the error is propagated to the new maybe instead.</para>
        /// </summary>
        /// <param name="transform">A function to change the value's type.</param>
        public static Task<Maybe<TResult>> TransformAsync<TValue, TResult>(this Task<Maybe<TValue>> maybe, Func<TValue, TResult> transform) => maybe.ContinueWith(x => x.Result.Match(y => new Maybe<TResult>(transform(y)), y => new Maybe<TResult>(y)));

        /// <summary>
        /// Converts the value of Maybe from one type to another.
        /// <para>If Maybe contains an error, then the error is propagated to the new maybe instead.</para>
        /// </summary>
        /// <param name="transform">A function to change the value's type.</param>
        public static Task<Maybe<TResult>> TransformAsync<TValue, TResult>(this Task<Maybe<TValue>> maybe, Func<TValue, Task<TResult>> transform) => maybe.ContinueWith(x => x.Result.Match(y => transform(y).ContinueWith(z => new Maybe<TResult>(z.Result)), y => Task.FromResult(new Maybe<TResult>(y)))).Unwrap();

        #endregion

        #region Bind

        /** Maybe Bind implementation. **/

        /// <summary>
        /// When both Maybes contain values, they are passed to the Bind function; the value TResult is then used to create a new Maybe.
        /// <para>Alternatively the existing error(s) are propagated to the new Maybe without invoking Bind.</para>
        /// </summary>
        /// <param name="maybe">The second Maybe to Bind with.</param>
        /// <param name="bind">A function that takes values from both Maybes, and produces a TResult.</param>
        public static Maybe<TResult> Bind<TValue, TBindValue, TResult>(this Maybe<TValue> value, Maybe<TBindValue> maybe, Func<TValue, TBindValue, Maybe<TResult>> bind)
        {
            if (value._hasValue && maybe._hasValue) return bind(value._value, maybe._value);
            if (!value._hasValue && !maybe._hasValue) return new Maybe<TResult>(value._error, value.AggregateErrors, maybe._error, maybe.AggregateErrors);
            if (!maybe._hasValue) return new Maybe<TResult>(maybe._error);
            return new Maybe<TResult>(value._error);
        }

        /// <summary>
        /// When both Maybes contain values, they are passed to the Bind function; the value TResult is then used to create a new Maybe.
        /// <para>Alternatively the existing error(s) are propagated to the new Maybe without invoking Bind.</para>
        /// </summary>
        /// <param name="maybe">The second Maybe to Bind with.</param>
        /// <param name="bind">A function that takes values from both Maybes, and produces a TResult.</param>
        public static Maybe<TResult> Bind<TValue, TBindValue, TResult>(this Maybe<TValue> value, Maybe<TBindValue> maybe, Func<TValue, TBindValue, TResult> bind)
        {
            if (value._hasValue && maybe._hasValue) return new Maybe<TResult>(bind(value._value, maybe._value));
            if (!value._hasValue && !maybe._hasValue) return new Maybe<TResult>(value._error, value.AggregateErrors, maybe._error, maybe.AggregateErrors);
            if (!maybe._hasValue) return new Maybe<TResult>(maybe._error);
            return new Maybe<TResult>(value._error);
        }

        /// <summary>
        /// When both Maybes contain values, they are passed to the Bind function; the value TResult is then used to create a new Maybe.
        /// <para>Alternatively the existing error(s) are propagated to the new Maybe without invoking Bind.</para>
        /// </summary>
        /// <param name="maybe">The second Maybe to Bind with.</param>
        /// <param name="bind">A function that takes values from both Maybes, and produces a TResult.</param>
        public static Task<Maybe<TResult>> BindAsync<TValue, TBindValue, TResult>(this Maybe<TValue> value, Maybe<TBindValue> maybe, Func<TValue, TBindValue, Task<Maybe<TResult>>> bind)
        {
            if (value._hasValue && maybe._hasValue) return bind(value._value, maybe._value);
            if (!value._hasValue && !maybe._hasValue) return Task.FromResult(new Maybe<TResult>(value._error, value.AggregateErrors, maybe._error, maybe.AggregateErrors));
            if (!maybe._hasValue) return Task.FromResult(new Maybe<TResult>(maybe._error));
            return Task.FromResult(new Maybe<TResult>(value._error));
        }

        /// <summary>
        /// When both Maybes contain values, they are passed to the Bind function; the value TResult is then used to create a new Maybe.
        /// <para>Alternatively the existing error(s) are propagated to the new Maybe without invoking Bind.</para>
        /// </summary>
        /// <param name="maybe">The second Maybe to Bind with.</param>
        /// <param name="bind">A function that takes values from both Maybes, and produces a TResult.</param>
        public static Task<Maybe<TResult>> BindAsync<TValue, TBindValue, TResult>(this Maybe<TValue> value, Maybe<TBindValue> maybe, Func<TValue, TBindValue, Task<TResult>> bind)
        {
            if (value._hasValue && maybe._hasValue) return bind(value._value, maybe._value).ContinueWith(x => new Maybe<TResult>(x.Result));
            if (!value._hasValue && !maybe._hasValue) return Task.FromResult(new Maybe<TResult>(value._error, value.AggregateErrors, maybe._error, maybe.AggregateErrors));
            if (!maybe._hasValue) return Task.FromResult(new Maybe<TResult>(maybe._error));
            return Task.FromResult(new Maybe<TResult>(value._error));
        }

        /** Maybe Bind first arg is Task. **/

        /// <summary>
        /// When both Maybes contain values, they are passed to the Bind function; the value TResult is then used to create a new Maybe.
        /// <para>Alternatively the existing error(s) are propagated to the new Maybe without invoking Bind.</para>
        /// </summary>
        /// <param name="maybe">The second Maybe to Bind with.</param>
        /// <param name="bind">A function that takes values from both Maybes, and produces a TResult.</param>
        public static Task<Maybe<TResult>> BindAsync<TValue, TBindValue, TResult>(this Task<Maybe<TValue>> value, Maybe<TBindValue> maybe, Func<TValue, TBindValue, Maybe<TResult>> bind)
        {
            return value.ContinueWith(x => Bind(x.Result, maybe, bind));
        }

        /// <summary>
        /// When both Maybes contain values, they are passed to the Bind function; the value TResult is then used to create a new Maybe.
        /// <para>Alternatively the existing error(s) are propagated to the new Maybe without invoking Bind.</para>
        /// </summary>
        /// <param name="maybe">The second Maybe to Bind with.</param>
        /// <param name="bind">A function that takes values from both Maybes, and produces a TResult.</param>
        public static Task<Maybe<TResult>> BindAsync<TValue, TBindValue, TResult>(this Task<Maybe<TValue>> value, Maybe<TBindValue> maybe, Func<TValue, TBindValue, TResult> bind)
        {
            return value.ContinueWith(x => Bind(x.Result, maybe, bind));
        }

        /// <summary>
        /// When both Maybes contain values, they are passed to the Bind function; the value TResult is then used to create a new Maybe.
        /// <para>Alternatively the existing error(s) are propagated to the new Maybe without invoking Bind.</para>
        /// </summary>
        /// <param name="maybe">The second Maybe to Bind with.</param>
        /// <param name="bind">A function that takes values from both Maybes, and produces a TResult.</param>
        public static Task<Maybe<TResult>> BindAsync<TValue, TBindValue, TResult>(this Task<Maybe<TValue>> value, Maybe<TBindValue> maybe, Func<TValue, TBindValue, Task<Maybe<TResult>>> bind)
        {
            return value.ContinueWith(x => BindAsync(x.Result, maybe, bind)).Unwrap();
        }

        /// <summary>
        /// When both Maybes contain values, they are passed to the Bind function; the value TResult is then used to create a new Maybe.
        /// <para>Alternatively the existing error(s) are propagated to the new Maybe without invoking Bind.</para>
        /// </summary>
        /// <param name="maybe">The second Maybe to Bind with.</param>
        /// <param name="bind">A function that takes values from both Maybes, and produces a TResult.</param>
        public static Task<Maybe<TResult>> BindAsync<TValue, TBindValue, TResult>(this Task<Maybe<TValue>> value, Maybe<TBindValue> maybe, Func<TValue, TBindValue, Task<TResult>> bind)
        {
            return value.ContinueWith(x => BindAsync(x.Result, maybe, bind)).Unwrap();
        }

        /** Maybe Bind second arg is Task. **/

        /// <summary>
        /// When both Maybes contain values, they are passed to the Bind function; the value TResult is then used to create a new Maybe.
        /// <para>Alternatively the existing error(s) are propagated to the new Maybe without invoking Bind.</para>
        /// </summary>
        /// <param name="maybe">The second Maybe to Bind with.</param>
        /// <param name="bind">A function that takes values from both Maybes, and produces a TResult.</param>
        public static Task<Maybe<TResult>> BindAsync<TValue, TBindValue, TResult>(this Maybe<TValue> value, Task<Maybe<TBindValue>> maybe, Func<TValue, TBindValue, Maybe<TResult>> bind)
        {
            return maybe.ContinueWith(x => Bind(value, x.Result, bind));
        }

        /// <summary>
        /// When both Maybes contain values, they are passed to the Bind function; the value TResult is then used to create a new Maybe.
        /// <para>Alternatively the existing error(s) are propagated to the new Maybe without invoking Bind.</para>
        /// </summary>
        /// <param name="maybe">The second Maybe to Bind with.</param>
        /// <param name="bind">A function that takes values from both Maybes, and produces a TResult.</param>
        public static Task<Maybe<TResult>> BindAsync<TValue, TBindValue, TResult>(this Maybe<TValue> value, Task<Maybe<TBindValue>> maybe, Func<TValue, TBindValue, TResult> bind)
        {
            return maybe.ContinueWith(x => Bind(value, x.Result, bind));
        }

        /// <summary>
        /// When both Maybes contain values, they are passed to the Bind function; the value TResult is then used to create a new Maybe.
        /// <para>Alternatively the existing error(s) are propagated to the new Maybe without invoking Bind.</para>
        /// </summary>
        /// <param name="maybe">The second Maybe to Bind with.</param>
        /// <param name="bind">A function that takes values from both Maybes, and produces a TResult.</param>
        public static Task<Maybe<TResult>> BindAsync<TValue, TBindValue, TResult>(this Maybe<TValue> value, Task<Maybe<TBindValue>> maybe, Func<TValue, TBindValue, Task<Maybe<TResult>>> bind)
        {
            return maybe.ContinueWith(x => BindAsync(value, x.Result, bind)).Unwrap();
        }

        /// <summary>
        /// When both Maybes contain values, they are passed to the Bind function; the value TResult is then used to create a new Maybe.
        /// <para>Alternatively the existing error(s) are propagated to the new Maybe without invoking Bind.</para>
        /// </summary>
        /// <param name="maybe">The second Maybe to Bind with.</param>
        /// <param name="bind">A function that takes values from both Maybes, and produces a TResult.</param>
        public static Task<Maybe<TResult>> BindAsync<TValue, TBindValue, TResult>(this Maybe<TValue> value, Task<Maybe<TBindValue>> maybe, Func<TValue, TBindValue, Task<TResult>> bind)
        {
            return maybe.ContinueWith(x => BindAsync(value, x.Result, bind)).Unwrap();
        }

        /** Maybe Bind first, and second args are Tasks. **/

        /// <summary>
        /// When both Maybes contain values, they are passed to the Bind function; the value TResult is then used to create a new Maybe.
        /// <para>Alternatively the existing error(s) are propagated to the new Maybe without invoking Bind.</para>
        /// </summary>
        /// <param name="maybe">The second Maybe to Bind with.</param>
        /// <param name="bind">A function that takes values from both Maybes, and produces a TResult.</param>
        public static Task<Maybe<TResult>> BindAsync<TValue, TBindValue, TResult>(this Task<Maybe<TValue>> value, Task<Maybe<TBindValue>> maybe, Func<TValue, TBindValue, Maybe<TResult>> bind)
        {
            return Task.WhenAll(value, maybe).ContinueWith(_ => Bind(value.Result, maybe.Result, bind));
        }

        /// <summary>
        /// When both Maybes contain values, they are passed to the Bind function; the value TResult is then used to create a new Maybe.
        /// <para>Alternatively the existing error(s) are propagated to the new Maybe without invoking Bind.</para>
        /// </summary>
        /// <param name="maybe">The second Maybe to Bind with.</param>
        /// <param name="bind">A function that takes values from both Maybes, and produces a TResult.</param>
        public static Task<Maybe<TResult>> BindAsync<TValue, TBindValue, TResult>(this Task<Maybe<TValue>> value, Task<Maybe<TBindValue>> maybe, Func<TValue, TBindValue, TResult> bind)
        {
            return Task.WhenAll(value, maybe).ContinueWith(_ => Bind(value.Result, maybe.Result, bind));
        }

        /// <summary>
        /// When both Maybes contain values, they are passed to the Bind function; the value TResult is then used to create a new Maybe.
        /// <para>Alternatively the existing error(s) are propagated to the new Maybe without invoking Bind.</para>
        /// </summary>
        /// <param name="maybe">The second Maybe to Bind with.</param>
        /// <param name="bind">A function that takes values from both Maybes, and produces a TResult.</param>
        public static Task<Maybe<TResult>> BindAsync<TValue, TBindValue, TResult>(this Task<Maybe<TValue>> value, Task<Maybe<TBindValue>> maybe, Func<TValue, TBindValue, Task<Maybe<TResult>>> bind)
        {
            return Task.WhenAll(value, maybe).ContinueWith(_ => BindAsync(value.Result, maybe.Result, bind)).Unwrap();
        }

        /// <summary>
        /// When both Maybes contain values, they are passed to the Bind function; the value TResult is then used to create a new Maybe.
        /// <para>Alternatively the existing error(s) are propagated to the new Maybe without invoking Bind.</para>
        /// </summary>
        /// <param name="maybe">The second Maybe to Bind with.</param>
        /// <param name="bind">A function that takes values from both Maybes, and produces a TResult.</param>
        public static Task<Maybe<TResult>> BindAsync<TValue, TBindValue, TResult>(this Task<Maybe<TValue>> value, Task<Maybe<TBindValue>> maybe, Func<TValue, TBindValue, Task<TResult>> bind)
        {
            return Task.WhenAll(value, maybe).ContinueWith(_ => BindAsync(value.Result, maybe.Result, bind)).Unwrap();
        }

        #endregion

        #region Unpack

        /// <summary>Flattens nested Maybe types into a single one.</summary>
        public static Maybe<TValue> Unpack<TValue>(this Maybe<Maybe<TValue>> maybe)
        {
            if (!maybe._hasValue) return new Maybe<TValue>(maybe._error);
            if (maybe._value._hasValue) return new Maybe<TValue>(maybe._value._value);
            return new Maybe<TValue>(maybe._value._error);
        }

        /// <summary>Flattens nested Maybe types into a single one.</summary>
        public static Task<Maybe<TValue>> UnpackAsync<TValue>(
            this Task<Maybe<Maybe<TValue>>> maybe,
            [CallerArgumentExpression(nameof(maybe))] string argument = "",
            [CallerMemberName] string caller = "",
            [CallerFilePath] string path = "",
            [CallerLineNumber] int line = 0
            )
        {
            return maybe.ContinueWith(t =>
            {
                var ex = _taskExCache;
                if (t.Status == TaskStatus.RanToCompletion) return t.Result.Unpack();
                if (t.Status == TaskStatus.Faulted) { ex = t.Exception; t.Exception.LogError(argument, caller, path, line); }
                return new Maybe<TValue>(ex);
            });
        }

        /// <summary>Flattens nested Maybe types into a single one.</summary>
        public static Task<Maybe<TValue>> UnpackAsync<TValue>(
            this Task<Maybe<Task<Maybe<TValue>>>> maybe,
            [CallerArgumentExpression(nameof(maybe))] string argument = "",
            [CallerMemberName] string caller = "",
            [CallerFilePath] string path = "",
            [CallerLineNumber] int line = 0
            )
        {
            return maybe.ContinueWith(t =>
            {
                var ex = _taskExCache;
                if (t.Status == TaskStatus.RanToCompletion)
                {
                    if (t.Result._hasValue) return t.Result._value;
                    return Task.FromResult(Maybe.CreateError<TValue>(t.Result._error));
                }
                if (t.Status == TaskStatus.Faulted) { ex = t.Exception; t.Exception.LogError(argument, caller, path, line); }
                return Task.FromResult(Maybe.CreateError<TValue>(ex));
            }).ContinueWith(t =>
            {
                var ex = _taskExCache;
                if (t.Status == TaskStatus.Faulted) { ex = t.Exception; t.Exception.LogError(argument, caller, path, line); }
                if (t.Status == TaskStatus.RanToCompletion)
                {
                    if (t.Result.Status == TaskStatus.Faulted) { ex = t.Exception; t.Exception.LogError(argument, caller, path, line); }
                    if (t.Result.Status == TaskStatus.RanToCompletion) return t.Result.Result;
                }
                return new Maybe<TValue>(ex);
            });
        }

        #endregion
    }
}
