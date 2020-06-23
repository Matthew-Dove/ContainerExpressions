using System;
using System.Threading.Tasks;

namespace ContainerExpressions.Containers
{
    public static class MaybeTValueExtensions
    {
        #region Miscellaneous

        public static TValue GetValueOrDefault<TValue>(this Maybe<TValue> maybe, TValue @default) => maybe.Match(x => x, _ => @default);

        public static Either<TValue, Exception> ToEither<TValue>(this Maybe<TValue> maybe) => maybe.Match(x => new Either<TValue, Exception>(x), x => new Either<TValue, Exception>(x));

        public static Response<TValue> ToResponse<TValue>(this Maybe<TValue> maybe) => maybe.Match(x => new Response<TValue>(x), _ => new Response<TValue>());

        public static Task<TValue> GetValueOrDefaultAsync<TValue>(this Task<Maybe<TValue>> maybe, TValue @default) => maybe.MatchAsync(x => x, _ => @default);

        public static Task<Either<TValue, Exception>> ToEitherAsync<TValue>(this Task<Maybe<TValue>> maybe) => maybe.MatchAsync(x => new Either<TValue, Exception>(x), x => new Either<TValue, Exception>(x));

        public static Task<Response<TValue>> ToResponseAsync<TValue>(this Task<Maybe<TValue>> maybe) => maybe.MatchAsync(x => new Response<TValue>(x), _ => new Response<TValue>());

        #endregion

        #region With

        public static Maybe<TValue> With<TValue>(this Maybe<TValue> _, Response<TValue> value, Exception error) => new Maybe<TValue>(value, error);

        public static Maybe<TValue> With<TValue>(this Maybe<TValue> _, Either<TValue, Exception> either) => new Maybe<TValue>(either);

        public static Maybe<TValue> With<TValue>(this Maybe<TValue> _, TValue value) => new Maybe<TValue>(value);

        public static Maybe<TValue> With<TValue>(this Maybe<TValue> _, Exception error) => new Maybe<TValue>(error);

        public static Maybe<TValue> With<TValue>(this Task<Maybe<TValue>> _, Response<TValue> value, Exception error) => new Maybe<TValue>(value, error);

        public static Maybe<TValue> With<TValue>(this Task<Maybe<TValue>> _, Either<TValue, Exception> either) => new Maybe<TValue>(either);

        public static Maybe<TValue> With<TValue>(this Task<Maybe<TValue>> _, TValue value) => new Maybe<TValue>(value);

        public static Maybe<TValue> With<TValue>(this Task<Maybe<TValue>> _, Exception error) => new Maybe<TValue>(error);

        #endregion

        #region Match

        /** maybe => sync, getValue => sync  **/

        public static TResult Match<TValue, TResult>(this Maybe<TValue> maybe, Func<TValue, TResult> getValue, Func<Exception, TResult> getError) => maybe._hasValue ? getValue(maybe._value) : getError(maybe._error);

        public static TResult Match<TValue, TResult>(this Maybe<TValue> maybe, Func<TValue, TResult> getValue, Func<Exception, Exception[], TResult> getError) => maybe._hasValue ? getValue(maybe._value) : getError(maybe._error, maybe.AggregateErrors);

        /** maybe => sync, getValue => async  **/

        public static Task<TResult> MatchAsync<TValue, TResult>(this Maybe<TValue> maybe, Func<TValue, Task<TResult>> getValue, Func<Exception, TResult> getError) => maybe._hasValue ? getValue(maybe._value) : Task.FromResult(getError(maybe._error));

        public static Task<TResult> MatchAsync<TValue, TResult>(this Maybe<TValue> maybe, Func<TValue, Task<TResult>> getValue, Func<Exception, Exception[], TResult> getError) => maybe._hasValue ? getValue(maybe._value) : Task.FromResult(getError(maybe._error, maybe.AggregateErrors));

        /** maybe => async, getValue => sync  **/

        public static Task<TResult> MatchAsync<TValue, TResult>(this Task<Maybe<TValue>> maybe, Func<TValue, TResult> getValue, Func<Exception, TResult> getError) => maybe.ContinueWith(x => x.Result._hasValue ? getValue(x.Result._value) : getError(x.Result._error));

        public static Task<TResult> MatchAsync<TValue, TResult>(this Task<Maybe<TValue>> maybe, Func<TValue, TResult> getValue, Func<Exception, Exception[], TResult> getError) => maybe.ContinueWith(x => x.Result._hasValue ? getValue(x.Result._value) : getError(x.Result._error, x.Result.AggregateErrors));

        /** maybe => async, getValue => async  **/

        public static Task<TResult> MatchAsync<TValue, TResult>(this Task<Maybe<TValue>> maybe, Func<TValue, Task<TResult>> getValue, Func<Exception, TResult> getError) => maybe.ContinueWith(x => x.Result._hasValue ? getValue(x.Result._value) : Task.FromResult(getError(x.Result._error))).Unwrap();

        public static Task<TResult> MatchAsync<TValue, TResult>(this Task<Maybe<TValue>> maybe, Func<TValue, Task<TResult>> getValue, Func<Exception, Exception[], TResult> getError) => maybe.ContinueWith(x => x.Result._hasValue ? getValue(x.Result._value) : Task.FromResult(getError(x.Result._error, x.Result.AggregateErrors))).Unwrap();

        #endregion

        #region Transform

        public static Maybe<TResult> Transform<TValue, TResult>(this Maybe<TValue> maybe, Func<TValue, TResult> transform) => maybe.Match(x => new Maybe<TResult>(transform(x)), x => new Maybe<TResult>(x));

        public static Task<Maybe<TResult>> TransformAsync<TValue, TResult>(this Maybe<TValue> maybe, Func<TValue, Task<TResult>> transform) => maybe.Match(x => transform(x).ContinueWith(y => new Maybe<TResult>(y.Result)), x => Task.FromResult(new Maybe<TResult>(x)));

        public static Task<Maybe<TResult>> TransformAsync<TValue, TResult>(this Task<Maybe<TValue>> maybe, Func<TValue, TResult> transform) => maybe.ContinueWith(x => x.Result.Match(y => new Maybe<TResult>(transform(y)), y => new Maybe<TResult>(y)));

        public static Task<Maybe<TResult>> TransformAsync<TValue, TResult>(this Task<Maybe<TValue>> maybe, Func<TValue, Task<TResult>> transform) => maybe.ContinueWith(x => x.Result.Match(y => transform(y).ContinueWith(z => new Maybe<TResult>(z.Result)), y => Task.FromResult(new Maybe<TResult>(y)))).Unwrap();

        #endregion

        #region Bind

        /** Maybe Bind implementation. **/

        public static Maybe<TResult> Bind<TValue, TBindValue, TResult>(this Maybe<TValue> value, Maybe<TBindValue> maybe, Func<TValue, TBindValue, Maybe<TResult>> bind)
        {
            if (value._hasValue && maybe._hasValue) return bind(value._value, maybe._value);
            if (!value._hasValue && !maybe._hasValue) return new Maybe<TResult>(value._error, value.AggregateErrors, maybe._error, maybe.AggregateErrors);
            if (!maybe._hasValue) return new Maybe<TResult>(maybe._error);
            return new Maybe<TResult>(value._error);
        }

        public static Maybe<TResult> Bind<TValue, TBindValue, TResult>(this Maybe<TValue> value, Maybe<TBindValue> maybe, Func<TValue, TBindValue, TResult> bind)
        {
            if (value._hasValue && maybe._hasValue) return new Maybe<TResult>(bind(value._value, maybe._value));
            if (!value._hasValue && !maybe._hasValue) return new Maybe<TResult>(value._error, value.AggregateErrors, maybe._error, maybe.AggregateErrors);
            if (!maybe._hasValue) return new Maybe<TResult>(maybe._error);
            return new Maybe<TResult>(value._error);
        }

        public static Task<Maybe<TResult>> BindAsync<TValue, TBindValue, TResult>(this Maybe<TValue> value, Maybe<TBindValue> maybe, Func<TValue, TBindValue, Task<Maybe<TResult>>> bind)
        {
            if (value._hasValue && maybe._hasValue) return bind(value._value, maybe._value);
            if (!value._hasValue && !maybe._hasValue) return Task.FromResult(new Maybe<TResult>(value._error, value.AggregateErrors, maybe._error, maybe.AggregateErrors));
            if (!maybe._hasValue) return Task.FromResult(new Maybe<TResult>(maybe._error));
            return Task.FromResult(new Maybe<TResult>(value._error));
        }

        public static Task<Maybe<TResult>> BindAsync<TValue, TBindValue, TResult>(this Maybe<TValue> value, Maybe<TBindValue> maybe, Func<TValue, TBindValue, Task<TResult>> bind)
        {
            if (value._hasValue && maybe._hasValue) return bind(value._value, maybe._value).ContinueWith(x => new Maybe<TResult>(x.Result));
            if (!value._hasValue && !maybe._hasValue) return Task.FromResult(new Maybe<TResult>(value._error, value.AggregateErrors, maybe._error, maybe.AggregateErrors));
            if (!maybe._hasValue) return Task.FromResult(new Maybe<TResult>(maybe._error));
            return Task.FromResult(new Maybe<TResult>(value._error));
        }

        /** Maybe Bind first arg is Task. **/

        public static Task<Maybe<TResult>> BindAsync<TValue, TBindValue, TResult>(this Task<Maybe<TValue>> value, Maybe<TBindValue> maybe, Func<TValue, TBindValue, Maybe<TResult>> bind)
        {
            return value.ContinueWith(x => Bind(x.Result, maybe, bind));
        }

        public static Task<Maybe<TResult>> BindAsync<TValue, TBindValue, TResult>(this Task<Maybe<TValue>> value, Maybe<TBindValue> maybe, Func<TValue, TBindValue, TResult> bind)
        {
            return value.ContinueWith(x => Bind(x.Result, maybe, bind));
        }

        public static Task<Maybe<TResult>> BindAsync<TValue, TBindValue, TResult>(this Task<Maybe<TValue>> value, Maybe<TBindValue> maybe, Func<TValue, TBindValue, Task<Maybe<TResult>>> bind)
        {
            return value.ContinueWith(x => BindAsync(x.Result, maybe, bind)).Unwrap();
        }

        public static Task<Maybe<TResult>> BindAsync<TValue, TBindValue, TResult>(this Task<Maybe<TValue>> value, Maybe<TBindValue> maybe, Func<TValue, TBindValue, Task<TResult>> bind)
        {
            return value.ContinueWith(x => BindAsync(x.Result, maybe, bind)).Unwrap();
        }

        /** Maybe Bind second arg is Task. **/

        public static Task<Maybe<TResult>> BindAsync<TValue, TBindValue, TResult>(this Maybe<TValue> value, Task<Maybe<TBindValue>> maybe, Func<TValue, TBindValue, Maybe<TResult>> bind)
        {
            return maybe.ContinueWith(x => Bind(value, x.Result, bind));
        }

        public static Task<Maybe<TResult>> BindAsync<TValue, TBindValue, TResult>(this Maybe<TValue> value, Task<Maybe<TBindValue>> maybe, Func<TValue, TBindValue, TResult> bind)
        {
            return maybe.ContinueWith(x => Bind(value, x.Result, bind));
        }

        public static Task<Maybe<TResult>> BindAsync<TValue, TBindValue, TResult>(this Maybe<TValue> value, Task<Maybe<TBindValue>> maybe, Func<TValue, TBindValue, Task<Maybe<TResult>>> bind)
        {
            return maybe.ContinueWith(x => BindAsync(value, x.Result, bind)).Unwrap();
        }

        public static Task<Maybe<TResult>> BindAsync<TValue, TBindValue, TResult>(this Maybe<TValue> value, Task<Maybe<TBindValue>> maybe, Func<TValue, TBindValue, Task<TResult>> bind)
        {
            return maybe.ContinueWith(x => BindAsync(value, x.Result, bind)).Unwrap();
        }

        /** Maybe Bind first, and second args are Tasks. **/

        public static Task<Maybe<TResult>> BindAsync<TValue, TBindValue, TResult>(this Task<Maybe<TValue>> value, Task<Maybe<TBindValue>> maybe, Func<TValue, TBindValue, Maybe<TResult>> bind)
        {
            return Task.WhenAll(value, maybe).ContinueWith(_ => Bind(value.Result, maybe.Result, bind));
        }

        public static Task<Maybe<TResult>> BindAsync<TValue, TBindValue, TResult>(this Task<Maybe<TValue>> value, Task<Maybe<TBindValue>> maybe, Func<TValue, TBindValue, TResult> bind)
        {
            return Task.WhenAll(value, maybe).ContinueWith(_ => Bind(value.Result, maybe.Result, bind));
        }

        public static Task<Maybe<TResult>> BindAsync<TValue, TBindValue, TResult>(this Task<Maybe<TValue>> value, Task<Maybe<TBindValue>> maybe, Func<TValue, TBindValue, Task<Maybe<TResult>>> bind)
        {
            return Task.WhenAll(value, maybe).ContinueWith(_ => BindAsync(value.Result, maybe.Result, bind)).Unwrap();
        }

        public static Task<Maybe<TResult>> BindAsync<TValue, TBindValue, TResult>(this Task<Maybe<TValue>> value, Task<Maybe<TBindValue>> maybe, Func<TValue, TBindValue, Task<TResult>> bind)
        {
            return Task.WhenAll(value, maybe).ContinueWith(_ => BindAsync(value.Result, maybe.Result, bind)).Unwrap();
        }

        #endregion
    }
}
