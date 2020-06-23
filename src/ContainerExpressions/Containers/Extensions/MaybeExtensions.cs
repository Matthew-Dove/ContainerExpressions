using System;
using System.Threading.Tasks;

namespace ContainerExpressions.Containers
{
    public static class MaybeExtensions
    {
        #region Miscellaneous

        public static TValue GetValueOrDefault<TValue, TError>(this Maybe<TValue, TError> maybe, TValue @default) => maybe.Match(x => x, _ => @default);

        public static Either<TValue, TError> ToEither<TValue, TError>(this Maybe<TValue, TError> maybe) => maybe.Match(x => new Either<TValue, TError>(x), x => new Either<TValue, TError>(x));

        public static Response<TValue> ToResponse<TValue, TError>(this Maybe<TValue, TError> maybe) => maybe.Match(x => new Response<TValue>(x), _ => new Response<TValue>());

        public static Task<TValue> GetValueOrDefaultAsync<TValue, TError>(this Task<Maybe<TValue, TError>> maybe, TValue @default) => maybe.MatchAsync(x => x, _ => @default);

        public static Task<Either<TValue, TError>> ToEitherAsync<TValue, TError>(this Task<Maybe<TValue, TError>> maybe) => maybe.MatchAsync(x => new Either<TValue, TError>(x), x => new Either<TValue, TError>(x));

        public static Task<Response<TValue>> ToResponseAsync<TValue, TError>(this Task<Maybe<TValue, TError>> maybe) => maybe.MatchAsync(x => new Response<TValue>(x), _ => new Response<TValue>());

        #endregion

        #region With

        public static Maybe<TValue, TError> With<TValue, TError>(this Maybe<TValue, TError> _, Response<TValue> value, TError error) => new Maybe<TValue, TError>(value, error);

        public static Maybe<TValue, TError> With<TValue, TError>(this Maybe<TValue, TError> _, Either<TValue, TError> either) => new Maybe<TValue, TError>(either);

        public static Maybe<TValue, TError> With<TValue, TError>(this Maybe<TValue, TError> _, TValue value) => new Maybe<TValue, TError>(value);

        public static Maybe<TValue, TError> With<TValue, TError>(this Maybe<TValue, TError> _, TError error) => new Maybe<TValue, TError>(error);

        public static Maybe<TValue, TError> With<TValue, TError>(this Task<Maybe<TValue, TError>> _, Response<TValue> value, TError error) => new Maybe<TValue, TError>(value, error);

        public static Maybe<TValue, TError> With<TValue, TError>(this Task<Maybe<TValue, TError>> _, Either<TValue, TError> either) => new Maybe<TValue, TError>(either);

        public static Maybe<TValue, TError> With<TValue, TError>(this Task<Maybe<TValue, TError>> _, TValue value) => new Maybe<TValue, TError>(value);

        public static Maybe<TValue, TError> With<TValue, TError>(this Task<Maybe<TValue, TError>> _, TError error) => new Maybe<TValue, TError>(error);

        #endregion

        #region Match

        /** maybe => sync, getValue => sync  **/

        public static TResult Match<TValue, TError, TResult>(this Maybe<TValue, TError> maybe, Func<TValue, TResult> getValue, Func<TError, TResult> getError) => maybe._hasValue ? getValue(maybe._value) : getError(maybe._error);

        public static TResult Match<TValue, TError, TResult>(this Maybe<TValue, TError> maybe, Func<TValue, TResult> getValue, Func<TError, TError[], TResult> getError) => maybe._hasValue ? getValue(maybe._value) : getError(maybe._error, maybe.AggregateErrors);

        /** maybe => sync, getValue => async  **/

        public static Task<TResult> MatchAsync<TValue, TError, TResult>(this Maybe<TValue, TError> maybe, Func<TValue, Task<TResult>> getValue, Func<TError, TResult> getError) => maybe._hasValue ? getValue(maybe._value) : Task.FromResult(getError(maybe._error));
        
        public static Task<TResult> MatchAsync<TValue, TError, TResult>(this Maybe<TValue, TError> maybe, Func<TValue, Task<TResult>> getValue, Func<TError, TError[], TResult> getError) => maybe._hasValue ? getValue(maybe._value) : Task.FromResult(getError(maybe._error, maybe.AggregateErrors));

        /** maybe => async, getValue => sync  **/

        public static Task<TResult> MatchAsync<TValue, TError, TResult>(this Task<Maybe<TValue, TError>> maybe, Func<TValue, TResult> getValue, Func<TError, TResult> getError) => maybe.ContinueWith(x => x.Result._hasValue ? getValue(x.Result._value) : getError(x.Result._error));

        public static Task<TResult> MatchAsync<TValue, TError, TResult>(this Task<Maybe<TValue, TError>> maybe, Func<TValue, TResult> getValue, Func<TError, TError[], TResult> getError) => maybe.ContinueWith(x => x.Result._hasValue ? getValue(x.Result._value) : getError(x.Result._error, x.Result.AggregateErrors));

        /** maybe => async, getValue => async  **/

        public static Task<TResult> MatchAsync<TValue, TError, TResult>(this Task<Maybe<TValue, TError>> maybe, Func<TValue, Task<TResult>> getValue, Func<TError, TResult> getError) => maybe.ContinueWith(x => x.Result._hasValue ? getValue(x.Result._value) : Task.FromResult(getError(x.Result._error))).Unwrap();

        public static Task<TResult> MatchAsync<TValue, TError, TResult>(this Task<Maybe<TValue, TError>> maybe, Func<TValue, Task<TResult>> getValue, Func<TError, TError[], TResult> getError) => maybe.ContinueWith(x => x.Result._hasValue ? getValue(x.Result._value) : Task.FromResult(getError(x.Result._error, x.Result.AggregateErrors))).Unwrap();

        #endregion

        #region Transform

        public static Maybe<TResult, TError> Transform<TValue, TError, TResult>(this Maybe<TValue, TError> maybe, Func<TValue, TResult> transform) => maybe.Match(x => new Maybe<TResult, TError>(transform(x)), x => new Maybe<TResult, TError>(x));

        public static Task<Maybe<TResult, TError>> TransformAsync<TValue, TError, TResult>(this Maybe<TValue, TError> maybe, Func<TValue, Task<TResult>> transform) => maybe.Match(x => transform(x).ContinueWith(y => new Maybe<TResult, TError>(y.Result)), x => Task.FromResult(new Maybe<TResult, TError>(x)));

        public static Task<Maybe<TResult, TError>> TransformAsync<TValue, TError, TResult>(this Task<Maybe<TValue, TError>> maybe, Func<TValue, TResult> transform) => maybe.ContinueWith(x => x.Result.Match(y => new Maybe<TResult, TError>(transform(y)), y => new Maybe<TResult, TError>(y)));

        public static Task<Maybe<TResult, TError>> TransformAsync<TValue, TError, TResult>(this Task<Maybe<TValue, TError>> maybe, Func<TValue, Task<TResult>> transform) => maybe.ContinueWith(x => x.Result.Match(y => transform(y).ContinueWith(z => new Maybe<TResult, TError>(z.Result)), y => Task.FromResult(new Maybe<TResult, TError>(y)))).Unwrap();

        #endregion

        #region Bind

        /** Maybe Bind implementation. **/

        public static Maybe<TResult, TError> Bind<TValue, TError, TBindValue, TResult>(this Maybe<TValue, TError> value, Maybe<TBindValue, TError> maybe, Func<TValue, TBindValue, Maybe<TResult, TError>> bind)
        {
            if (value._hasValue && maybe._hasValue) return bind(value._value, maybe._value);
            if (!value._hasValue && !maybe._hasValue) return new Maybe<TResult, TError>(value._error, value.AggregateErrors, maybe._error, maybe.AggregateErrors);
            if (!maybe._hasValue) return new Maybe<TResult, TError>(maybe._error);
            return new Maybe<TResult, TError>(value._error);
        }

        public static Maybe<TResult, TError> Bind<TValue, TError, TBindValue, TResult>(this Maybe<TValue, TError> value, Maybe<TBindValue, TError> maybe, Func<TValue, TBindValue, TResult> bind)
        {
            if (value._hasValue && maybe._hasValue) return new Maybe<TResult, TError>(bind(value._value, maybe._value));
            if (!value._hasValue && !maybe._hasValue) return new Maybe<TResult, TError>(value._error, value.AggregateErrors, maybe._error, maybe.AggregateErrors);
            if (!maybe._hasValue) return new Maybe<TResult, TError>(maybe._error);
            return new Maybe<TResult, TError>(value._error);
        }

        public static Task<Maybe<TResult, TError>> BindAsync<TValue, TError, TBindValue, TResult>(this Maybe<TValue, TError> value, Maybe<TBindValue, TError> maybe, Func<TValue, TBindValue, Task<Maybe<TResult, TError>>> bind)
        {
            if (value._hasValue && maybe._hasValue) return bind(value._value, maybe._value);
            if (!value._hasValue && !maybe._hasValue) return Task.FromResult(new Maybe<TResult, TError>(value._error, value.AggregateErrors, maybe._error, maybe.AggregateErrors));
            if (!maybe._hasValue) return Task.FromResult(new Maybe<TResult, TError>(maybe._error));
            return Task.FromResult(new Maybe<TResult, TError>(value._error));
        }

        public static Task<Maybe<TResult, TError>> BindAsync<TValue, TError, TBindValue, TResult>(this Maybe<TValue, TError> value, Maybe<TBindValue, TError> maybe, Func<TValue, TBindValue, Task<TResult>> bind)
        {
            if (value._hasValue && maybe._hasValue) return bind(value._value, maybe._value).ContinueWith(x => new Maybe<TResult, TError>(x.Result));
            if (!value._hasValue && !maybe._hasValue) return Task.FromResult(new Maybe<TResult, TError>(value._error, value.AggregateErrors, maybe._error, maybe.AggregateErrors));
            if (!maybe._hasValue) return Task.FromResult(new Maybe<TResult, TError>(maybe._error));
            return Task.FromResult(new Maybe<TResult, TError>(value._error));
        }

        /** Maybe Bind implementation with Error conversion. **/

        public static Maybe<TResult, TBindError> BindAggregate<TValue, TError, TBindValue, TBindError, TResult>(this Maybe<TValue, TError> value, Maybe<TBindValue, TBindError> maybe, Func<TError, TBindError> convert, Func<TValue, TBindValue, Maybe<TResult, TBindError>> bind)
        {
            return value.Match(x => new Maybe<TValue, TBindError>(x), x => new Maybe<TValue, TBindError>(convert(x))).Bind(maybe, bind);
        }

        public static Maybe<TResult, TBindError> BindAggregate<TValue, TError, TBindValue, TBindError, TResult>(this Maybe<TValue, TError> value, Maybe<TBindValue, TBindError> maybe, Func<TError, TBindError> convert, Func<TValue, TBindValue, TResult> bind)
        {
            return value.Match(x => new Maybe<TValue, TBindError>(x), x => new Maybe<TValue, TBindError>(convert(x))).Bind(maybe, bind);
        }

        public static Maybe<TResult, (TError, TBindError)> BindAggregate<TValue, TError, TBindValue, TBindError, TResult>(this Maybe<TValue, TError> value, Maybe<TBindValue, TBindError> maybe, Func<TValue, TBindValue, Maybe<TResult, (TError, TBindError)>> bind)
        {
            var valueMatch = value.Match(x => new Maybe<TValue, (TError, TBindError)>(x), x => new Maybe<TValue, (TError, TBindError)>((x, default)));
            var maybeMatch = maybe.Match(x => new Maybe<TBindValue, (TError, TBindError)>(x), x => new Maybe<TBindValue, (TError, TBindError)>((default, x)));
            return valueMatch.Bind(maybeMatch, bind);
        }

        public static Maybe<TResult, (TError, TBindError)> BindAggregate<TValue, TError, TBindValue, TBindError, TResult>(this Maybe<TValue, TError> value, Maybe<TBindValue, TBindError> maybe, Func<TValue, TBindValue, TResult> bind)
        {
            var valueMatch = value.Match(x => new Maybe<TValue, (TError, TBindError)>(x), x => new Maybe<TValue, (TError, TBindError)>((x, default)));
            var maybeMatch = maybe.Match(x => new Maybe<TBindValue, (TError, TBindError)>(x), x => new Maybe<TBindValue, (TError, TBindError)>((default, x)));
            if (valueMatch._hasValue && maybeMatch._hasValue) return new Maybe<TResult, (TError, TBindError)>(bind(valueMatch._value, maybeMatch._value));
            if (!valueMatch._hasValue && !maybeMatch._hasValue) return new Maybe<TResult, (TError, TBindError)>(valueMatch._error, valueMatch.AggregateErrors, maybeMatch._error, maybeMatch.AggregateErrors);
            if (!maybeMatch._hasValue) return new Maybe<TResult, (TError, TBindError)>(maybeMatch._error);
            return new Maybe<TResult, (TError, TBindError)>(valueMatch._error);
        }

        public static Task<Maybe<TResult, TBindError>> BindAggregateAsync<TValue, TError, TBindValue, TBindError, TResult>(this Maybe<TValue, TError> value, Maybe<TBindValue, TBindError> maybe, Func<TError, TBindError> convert, Func<TValue, TBindValue, Task<Maybe<TResult, TBindError>>> bind)
        {
            return value.Match(x => new Maybe<TValue, TBindError>(x), x => new Maybe<TValue, TBindError>(convert(x))).BindAsync(maybe, bind);
        }

        public static Task<Maybe<TResult, TBindError>> BindAggregateAsync<TValue, TError, TBindValue, TBindError, TResult>(this Maybe<TValue, TError> value, Maybe<TBindValue, TBindError> maybe, Func<TError, TBindError> convert, Func<TValue, TBindValue, Task<TResult>> bind)
        {
            return value.Match(x => new Maybe<TValue, TBindError>(x), x => new Maybe<TValue, TBindError>(convert(x))).BindAsync(maybe, bind);
        }

        public static Task<Maybe<TResult, (TError, TBindError)>> BindAggregateAsync<TValue, TError, TBindValue, TBindError, TResult>(this Maybe<TValue, TError> value, Maybe<TBindValue, TBindError> maybe, Func<TValue, TBindValue, Task<Maybe<TResult, (TError, TBindError)>>> bind)
        {
            var valueMatch = value.Match(x => new Maybe<TValue, (TError, TBindError)>(x), x => new Maybe<TValue, (TError, TBindError)>((x, default)));
            var maybeMatch = maybe.Match(x => new Maybe<TBindValue, (TError, TBindError)>(x), x => new Maybe<TBindValue, (TError, TBindError)>((default, x)));
            return valueMatch.BindAsync(maybeMatch, bind);
        }

        public static Task<Maybe<TResult, (TError, TBindError)>> BindAggregateAsync<TValue, TError, TBindValue, TBindError, TResult>(this Maybe<TValue, TError> value, Maybe<TBindValue, TBindError> maybe, Func<TValue, TBindValue, Task<TResult>> bind)
        {
            var valueMatch = value.Match(x => new Maybe<TValue, (TError, TBindError)>(x), x => new Maybe<TValue, (TError, TBindError)>((x, default)));
            var maybeMatch = maybe.Match(x => new Maybe<TBindValue, (TError, TBindError)>(x), x => new Maybe<TBindValue, (TError, TBindError)>((default, x)));
            if (valueMatch._hasValue && maybeMatch._hasValue) return bind(valueMatch._value, maybeMatch._value).ContinueWith(x => new Maybe<TResult, (TError, TBindError)>(x.Result));
            if (!valueMatch._hasValue && !maybeMatch._hasValue) return Task.FromResult(new Maybe<TResult, (TError, TBindError)>(valueMatch._error, valueMatch.AggregateErrors, maybeMatch._error, maybeMatch.AggregateErrors));
            if (!maybeMatch._hasValue) return Task.FromResult(new Maybe<TResult, (TError, TBindError)>(maybeMatch._error));
            return Task.FromResult(new Maybe<TResult, (TError, TBindError)>(valueMatch._error));
        }

        /** Maybe Bind first arg is Task. **/

        public static Task<Maybe<TResult, TError>> BindAsync<TValue, TError, TBindValue, TResult>(this Task<Maybe<TValue, TError>> value, Maybe<TBindValue, TError> maybe, Func<TValue, TBindValue, Maybe<TResult, TError>> bind)
        {
            return value.ContinueWith(x => Bind(x.Result, maybe, bind));
        }

        public static Task<Maybe<TResult, TError>> BindAsync<TValue, TError, TBindValue, TResult>(this Task<Maybe<TValue, TError>> value, Maybe<TBindValue, TError> maybe, Func<TValue, TBindValue, TResult> bind)
        {
            return value.ContinueWith(x => Bind(x.Result, maybe, bind));
        }

        public static Task<Maybe<TResult, TError>> BindAsync<TValue, TError, TBindValue, TResult>(this Task<Maybe<TValue, TError>> value, Maybe<TBindValue, TError> maybe, Func<TValue, TBindValue, Task<Maybe<TResult, TError>>> bind)
        {
            return value.ContinueWith(x => BindAsync(x.Result, maybe, bind)).Unwrap();
        }

        public static Task<Maybe<TResult, TError>> BindAsync<TValue, TError, TBindValue, TResult>(this Task<Maybe<TValue, TError>> value, Maybe<TBindValue, TError> maybe, Func<TValue, TBindValue, Task<TResult>> bind)
        {
            return value.ContinueWith(x => BindAsync(x.Result, maybe, bind)).Unwrap();
        }

        /** Maybe Bind first arg is Task with Error conversion. **/

        public static Task<Maybe<TResult, TBindError>> BindAggregateAsync<TValue, TError, TBindValue, TBindError, TResult>(this Task<Maybe<TValue, TError>> value, Maybe<TBindValue, TBindError> maybe, Func<TError, TBindError> convert, Func<TValue, TBindValue, Maybe<TResult, TBindError>> bind)
        {
            return value.ContinueWith(x => BindAggregate(x.Result, maybe, convert, bind));
        }

        public static Task<Maybe<TResult, TBindError>> BindAggregateAsync<TValue, TError, TBindValue, TBindError, TResult>(this Task<Maybe<TValue, TError>> value, Maybe<TBindValue, TBindError> maybe, Func<TError, TBindError> convert, Func<TValue, TBindValue, TResult> bind)
        {
            return value.ContinueWith(x => BindAggregate(x.Result, maybe, convert, bind));
        }

        public static Task<Maybe<TResult, (TError, TBindError)>> BindAggregateAsync<TValue, TError, TBindValue, TBindError, TResult>(this Task<Maybe<TValue, TError>> value, Maybe<TBindValue, TBindError> maybe, Func<TValue, TBindValue, Maybe<TResult, (TError, TBindError)>> bind)
        {
            return value.ContinueWith(x => BindAggregate(x.Result, maybe, bind));
        }

        public static Task<Maybe<TResult, (TError, TBindError)>> BindAggregateAsync<TValue, TError, TBindValue, TBindError, TResult>(this Task<Maybe<TValue, TError>> value, Maybe<TBindValue, TBindError> maybe, Func<TValue, TBindValue, TResult> bind)
        {
            return value.ContinueWith(x => BindAggregate(x.Result, maybe, bind));
        }

        public static Task<Maybe<TResult, TBindError>> BindAggregateAsync<TValue, TError, TBindValue, TBindError, TResult>(this Task<Maybe<TValue, TError>> value, Maybe<TBindValue, TBindError> maybe, Func<TError, TBindError> convert, Func<TValue, TBindValue, Task<Maybe<TResult, TBindError>>> bind)
        {
            return value.ContinueWith(x => BindAggregateAsync(x.Result, maybe, convert, bind)).Unwrap();
        }

        public static Task<Maybe<TResult, TBindError>> BindAggregateAsync<TValue, TError, TBindValue, TBindError, TResult>(this Task<Maybe<TValue, TError>> value, Maybe<TBindValue, TBindError> maybe, Func<TError, TBindError> convert, Func<TValue, TBindValue, Task<TResult>> bind)
        {
            return value.ContinueWith(x => BindAggregateAsync(x.Result, maybe, convert, bind)).Unwrap();
        }

        public static Task<Maybe<TResult, (TError, TBindError)>> BindAggregateAsync<TValue, TError, TBindValue, TBindError, TResult>(this Task<Maybe<TValue, TError>> value, Maybe<TBindValue, TBindError> maybe, Func<TValue, TBindValue, Task<Maybe<TResult, (TError, TBindError)>>> bind)
        {
            return value.ContinueWith(x => BindAggregateAsync(x.Result, maybe, bind)).Unwrap();
        }

        public static Task<Maybe<TResult, (TError, TBindError)>> BindAggregateAsync<TValue, TError, TBindValue, TBindError, TResult>(this Task<Maybe<TValue, TError>> value, Maybe<TBindValue, TBindError> maybe, Func<TValue, TBindValue, Task<TResult>> bind)
        {
            return value.ContinueWith(x => BindAggregateAsync(x.Result, maybe, bind)).Unwrap();
        }

        /** Maybe Bind second arg is Task. **/

        public static Task<Maybe<TResult, TError>> BindAsync<TValue, TError, TBindValue, TResult>(this Maybe<TValue, TError> value, Task<Maybe<TBindValue, TError>> maybe, Func<TValue, TBindValue, Maybe<TResult, TError>> bind)
        {
            return maybe.ContinueWith(x => Bind(value, x.Result, bind));
        }

        public static Task<Maybe<TResult, TError>> BindAsync<TValue, TError, TBindValue, TResult>(this Maybe<TValue, TError> value, Task<Maybe<TBindValue, TError>> maybe, Func<TValue, TBindValue, TResult> bind)
        {
            return maybe.ContinueWith(x => Bind(value, x.Result, bind));
        }

        public static Task<Maybe<TResult, TError>> BindAsync<TValue, TError, TBindValue, TResult>(this Maybe<TValue, TError> value, Task<Maybe<TBindValue, TError>> maybe, Func<TValue, TBindValue, Task<Maybe<TResult, TError>>> bind)
        {
            return maybe.ContinueWith(x => BindAsync(value, x.Result, bind)).Unwrap();
        }

        public static Task<Maybe<TResult, TError>> BindAsync<TValue, TError, TBindValue, TResult>(this Maybe<TValue, TError> value, Task<Maybe<TBindValue, TError>> maybe, Func<TValue, TBindValue, Task<TResult>> bind)
        {
            return maybe.ContinueWith(x => BindAsync(value, x.Result, bind)).Unwrap();
        }

        /** Maybe Bind second arg is Task with Error conversion. **/

        public static Task<Maybe<TResult, TBindError>> BindAggregateAsync<TValue, TError, TBindValue, TBindError, TResult>(this Maybe<TValue, TError> value, Task<Maybe<TBindValue, TBindError>> maybe, Func<TError, TBindError> convert, Func<TValue, TBindValue, Maybe<TResult, TBindError>> bind)
        {
            return maybe.ContinueWith(x => BindAggregate(value, x.Result, convert, bind));
        }

        public static Task<Maybe<TResult, TBindError>> BindAggregateAsync<TValue, TError, TBindValue, TBindError, TResult>(this Maybe<TValue, TError> value, Task<Maybe<TBindValue, TBindError>> maybe, Func<TError, TBindError> convert, Func<TValue, TBindValue, TResult> bind)
        {
            return maybe.ContinueWith(x => BindAggregate(value, x.Result, convert, bind));
        }

        public static Task<Maybe<TResult, (TError, TBindError)>> BindAggregateAsync<TValue, TError, TBindValue, TBindError, TResult>(this Maybe<TValue, TError> value, Task<Maybe<TBindValue, TBindError>> maybe, Func<TValue, TBindValue, Maybe<TResult, (TError, TBindError)>> bind)
        {
            return maybe.ContinueWith(x => BindAggregate(value, x.Result, bind));
        }

        public static Task<Maybe<TResult, (TError, TBindError)>> BindAggregateAsync<TValue, TError, TBindValue, TBindError, TResult>(this Maybe<TValue, TError> value, Task<Maybe<TBindValue, TBindError>> maybe, Func<TValue, TBindValue, TResult> bind)
        {
            return maybe.ContinueWith(x => BindAggregate(value, x.Result, bind));
        }

        public static Task<Maybe<TResult, TBindError>> BindAggregateAsync<TValue, TError, TBindValue, TBindError, TResult>(this Maybe<TValue, TError> value, Task<Maybe<TBindValue, TBindError>> maybe, Func<TError, TBindError> convert, Func<TValue, TBindValue, Task<Maybe<TResult, TBindError>>> bind)
        {
            return maybe.ContinueWith(x => BindAggregateAsync(value, x.Result, convert, bind)).Unwrap();
        }

        public static Task<Maybe<TResult, TBindError>> BindAggregateAsync<TValue, TError, TBindValue, TBindError, TResult>(this Maybe<TValue, TError> value, Task<Maybe<TBindValue, TBindError>> maybe, Func<TError, TBindError> convert, Func<TValue, TBindValue, Task<TResult>> bind)
        {
            return maybe.ContinueWith(x => BindAggregateAsync(value, x.Result, convert, bind)).Unwrap();
        }

        public static Task<Maybe<TResult, (TError, TBindError)>> BindAggregateAsync<TValue, TError, TBindValue, TBindError, TResult>(this Maybe<TValue, TError> value, Task<Maybe<TBindValue, TBindError>> maybe, Func<TValue, TBindValue, Task<Maybe<TResult, (TError, TBindError)>>> bind)
        {
            return maybe.ContinueWith(x => BindAggregateAsync(value, x.Result, bind)).Unwrap();
        }

        public static Task<Maybe<TResult, (TError, TBindError)>> BindAggregateAsync<TValue, TError, TBindValue, TBindError, TResult>(this Maybe<TValue, TError> value, Task<Maybe<TBindValue, TBindError>> maybe, Func<TValue, TBindValue, Task<TResult>> bind)
        {
            return maybe.ContinueWith(x => BindAggregateAsync(value, x.Result, bind)).Unwrap();
        }

        /** Maybe Bind first, and second args are Tasks. **/

        public static Task<Maybe<TResult, TError>> BindAsync<TValue, TError, TBindValue, TResult>(this Task<Maybe<TValue, TError>> value, Task<Maybe<TBindValue, TError>> maybe, Func<TValue, TBindValue, Maybe<TResult, TError>> bind)
        {
            return Task.WhenAll(value, maybe).ContinueWith(_ => Bind(value.Result, maybe.Result, bind));
        }

        public static Task<Maybe<TResult, TError>> BindAsync<TValue, TError, TBindValue, TResult>(this Task<Maybe<TValue, TError>> value, Task<Maybe<TBindValue, TError>> maybe, Func<TValue, TBindValue, TResult> bind)
        {
            return Task.WhenAll(value, maybe).ContinueWith(_ => Bind(value.Result, maybe.Result, bind));
        }

        public static Task<Maybe<TResult, TError>> BindAsync<TValue, TError, TBindValue, TResult>(this Task<Maybe<TValue, TError>> value, Task<Maybe<TBindValue, TError>> maybe, Func<TValue, TBindValue, Task<Maybe<TResult, TError>>> bind)
        {
            return Task.WhenAll(value, maybe).ContinueWith(_ => BindAsync(value.Result, maybe.Result, bind)).Unwrap();
        }

        public static Task<Maybe<TResult, TError>> BindAsync<TValue, TError, TBindValue, TResult>(this Task<Maybe<TValue, TError>> value, Task<Maybe<TBindValue, TError>> maybe, Func<TValue, TBindValue, Task<TResult>> bind)
        {
            return Task.WhenAll(value, maybe).ContinueWith(_ => BindAsync(value.Result, maybe.Result, bind)).Unwrap();
        }

        /** Maybe Bind first, and second args are Tasks with Error conversion. **/

        public static Task<Maybe<TResult, TBindError>> BindAggregateAsync<TValue, TError, TBindValue, TBindError, TResult>(this Task<Maybe<TValue, TError>> value, Task<Maybe<TBindValue, TBindError>> maybe, Func<TError, TBindError> convert, Func<TValue, TBindValue, Maybe<TResult, TBindError>> bind)
        {
            return Task.WhenAll(value, maybe).ContinueWith(_ => BindAggregate(value.Result, maybe.Result, convert, bind));
        }

        public static Task<Maybe<TResult, TBindError>> BindAggregateAsync<TValue, TError, TBindValue, TBindError, TResult>(this Task<Maybe<TValue, TError>> value, Task<Maybe<TBindValue, TBindError>> maybe, Func<TError, TBindError> convert, Func<TValue, TBindValue, TResult> bind)
        {
            return Task.WhenAll(value, maybe).ContinueWith(_ => BindAggregate(value.Result, maybe.Result, convert, bind));
        }

        public static Task<Maybe<TResult, (TError, TBindError)>> BindAggregateAsync<TValue, TError, TBindValue, TBindError, TResult>(this Task<Maybe<TValue, TError>> value, Task<Maybe<TBindValue, TBindError>> maybe, Func<TValue, TBindValue, Maybe<TResult, (TError, TBindError)>> bind)
        {
            return Task.WhenAll(value, maybe).ContinueWith(_ => BindAggregate(value.Result, maybe.Result, bind));
        }

        public static Task<Maybe<TResult, (TError, TBindError)>> BindAggregateAsync<TValue, TError, TBindValue, TBindError, TResult>(this Task<Maybe<TValue, TError>> value, Task<Maybe<TBindValue, TBindError>> maybe, Func<TValue, TBindValue, TResult> bind)
        {
            return Task.WhenAll(value, maybe).ContinueWith(_ => BindAggregate(value.Result, maybe.Result, bind));
        }

        public static Task<Maybe<TResult, TBindError>> BindAggregateAsync<TValue, TError, TBindValue, TBindError, TResult>(this Task<Maybe<TValue, TError>> value, Task<Maybe<TBindValue, TBindError>> maybe, Func<TError, TBindError> convert, Func<TValue, TBindValue, Task<Maybe<TResult, TBindError>>> bind)
        {
            return Task.WhenAll(value, maybe).ContinueWith(_ => BindAggregateAsync(value.Result, maybe.Result, convert, bind)).Unwrap();
        }

        public static Task<Maybe<TResult, TBindError>> BindAggregateAsync<TValue, TError, TBindValue, TBindError, TResult>(this Task<Maybe<TValue, TError>> value, Task<Maybe<TBindValue, TBindError>> maybe, Func<TError, TBindError> convert, Func<TValue, TBindValue, Task<TResult>> bind)
        {
            return Task.WhenAll(value, maybe).ContinueWith(_ => BindAggregateAsync(value.Result, maybe.Result, convert, bind)).Unwrap();
        }

        public static Task<Maybe<TResult, (TError, TBindError)>> BindAggregateAsync<TValue, TError, TBindValue, TBindError, TResult>(this Task<Maybe<TValue, TError>> value, Task<Maybe<TBindValue, TBindError>> maybe, Func<TValue, TBindValue, Task<Maybe<TResult, (TError, TBindError)>>> bind)
        {
            return Task.WhenAll(value, maybe).ContinueWith(_ => BindAggregateAsync(value.Result, maybe.Result, bind)).Unwrap();
        }

        public static Task<Maybe<TResult, (TError, TBindError)>> BindAggregateAsync<TValue, TError, TBindValue, TBindError, TResult>(this Task<Maybe<TValue, TError>> value, Task<Maybe<TBindValue, TBindError>> maybe, Func<TValue, TBindValue, Task<TResult>> bind)
        {
            return Task.WhenAll(value, maybe).ContinueWith(_ => BindAggregateAsync(value.Result, maybe.Result, bind)).Unwrap();
        }

        #endregion
    }
}
