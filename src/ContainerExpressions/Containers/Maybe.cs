using System;
using System.Linq;
using System.Threading.Tasks;

namespace ContainerExpressions.Containers
{
    public readonly struct Maybe<TValue>
    {
        private readonly bool _hasValue;
        private readonly TValue _value;
        private readonly Exception _exception;

        public Maybe(TValue value)
        {
            _value = value;
            _exception = default;
            _hasValue = true;
        }

        public Maybe(Exception exception)
        {
            _value = default;
            _exception = exception;
            _hasValue = false;
        }
    }

    public readonly struct Maybe<TValue, TError>
    {
        public TError[] AggregateErrors { get; }
        internal readonly static TError[] _emptyAggregateErrors = new TError[0];

        internal readonly bool _hasValue;
        internal readonly TValue _value;
        internal readonly TError _error;

        public Maybe(TValue value)
        {
            _value = value;
            _error = default;
            _hasValue = true;
            AggregateErrors = _emptyAggregateErrors;
        }

        public Maybe(TError error)
        {
            _value = default;
            _error = error;
            _hasValue = false;
            AggregateErrors = _emptyAggregateErrors;
        }

        public Maybe(Response<TValue> value, TError error)
        {
            if (value)
            {
                _value = value;
                _error = default;
                _hasValue = true;
                AggregateErrors = _emptyAggregateErrors;
            }
            else
            {
                _value = default;
                _error = error;
                _hasValue = false;
                AggregateErrors = _emptyAggregateErrors;
            }
        }

        public Maybe(Either<TValue, TError> either)
        {
            var result = either.Match(x => (isValue: true, value: x, error: default(TError)), x => (isValue: false, value: default(TValue), error: x));
            
            if (result.isValue)
            {
                _value = result.value;
                _error = default;
                _hasValue = true;
                AggregateErrors = _emptyAggregateErrors;
            }
            else
            {
                _value = default;
                _error = result.error;
                _hasValue = false;
                AggregateErrors = _emptyAggregateErrors;
            }
        }

        internal Maybe(TError error, TError[] aggregateErrors, TError bindError, TError[] bindAggregateErrors)
        {
            _value = default;
            _error = bindError;
            _hasValue = false;

            int length = (aggregateErrors?.Length ?? 0), bindLength = (bindAggregateErrors?.Length ?? 0), totalLength = length + bindLength + 1;
            var errors = new TError[totalLength];

            int i;
            for (i = 0; i < length; i++)
            {
                errors[i] = aggregateErrors[i];
            }
            errors[i] = error;

            for (i = 0; i < bindLength; i++)
            {
                errors[i + length + 1] = bindAggregateErrors[i];
            }

            AggregateErrors = errors;
        }

        public TMatch Match<TMatch>(Func<TValue, TMatch> getValue, Func<TError, TMatch> getError) => _hasValue ? getValue(_value) : getError(_error);

        public Task<TMatch> MatchAsync<TMatch>(Func<TValue, Task<TMatch>> getValue, Func<TError, TMatch> getError) => _hasValue ? getValue(_value) : Task.FromResult(getError(_error));

        public Maybe<TResult, TError> Transform<TResult>(Func<TValue, TResult> transform) => Match(x => new Maybe<TResult, TError>(transform(x)), x => new Maybe<TResult, TError>(x));

        public Task<Maybe<TResult, TError>> TransformAsync<TResult>(Func<TValue, Task<TResult>> transform) => Match(x => transform(x).ContinueWith(y => new Maybe<TResult, TError>(y.Result)), x => Task.FromResult(new Maybe<TResult, TError>(x)));

        public Either<TValue, TError> ToEither() => Match(x => new Either<TValue, TError>(x), x => new Either<TValue, TError>(x));

        public Response<TValue> ToResponse() => Match(x => new Response<TValue>(x), _ => new Response<TValue>());

        public override string ToString() => Match(x => x?.ToString(), x => x?.ToString()) ?? string.Empty;

        public static implicit operator Response<TValue>(Maybe<TValue, TError> maybe) => maybe.Match(x => new Response<TValue>(x), _ => new Response<TValue>());

        public static implicit operator Maybe<TValue, TError>(Either<TValue, TError> either) => new Maybe<TValue, TError>(either);

        public static implicit operator Either<TValue, TError>(Maybe<TValue, TError> maybe) => maybe.Match(x => new Either<TValue, TError>(x), x => new Either<TValue, TError>(x));

        public static implicit operator Maybe<TValue, TError>(TValue value) => new Maybe<TValue, TError>(value);

        public static implicit operator Maybe<TValue, TError>(TError error) => new Maybe<TValue, TError>(error);
    }

    public readonly struct ValueContainer<TValue>
    {
        private readonly TValue _value;

        public ValueContainer(TValue value)
        {
            _value = value;
        }

        public Maybe<TValue, TError> Error<TError>() => new Maybe<TValue, TError>(_value);
    }

    public readonly struct ErrorContainer<TValue>
    {
        public Maybe<TValue, TError> Error<TError>(TError error) => new Maybe<TValue, TError>(error);
    }

    public static class Maybe
    {
        public static Maybe<TValue, TError> Create<TValue, TError>() => new Maybe<TValue, TError>();

        public static ErrorContainer<TValue> Value<TValue>() => new ErrorContainer<TValue>();

        public static ValueContainer<TValue> Value<TValue>(TValue value) => new ValueContainer<TValue>(value);
    }

    public static class MaybeExtensions
    {
        public static TValue GetValueOrDefault<TValue, TError>(this Maybe<TValue, TError> maybe, TValue @default) => maybe.Match(x => x, _ => @default);

        #region With

        public static Maybe<TValue, TError> With<TValue, TError>(this Maybe<TValue, TError> _, Response<TValue> value, TError error) => new Maybe<TValue, TError>(value, error);

        public static Maybe<TValue, TError> With<TValue, TError>(this Maybe<TValue, TError> _, Either<TValue, TError> either) => new Maybe<TValue, TError>(either);

        public static Maybe<TValue, TError> With<TValue, TError>(this Maybe<TValue, TError> _, TValue value) => new Maybe<TValue, TError>(value);

        public static Maybe<TValue, TError> With<TValue, TError>(this Maybe<TValue, TError> _, TError error) => new Maybe<TValue, TError>(error);

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

        public static Task<Maybe<TResult, TError>> BindAsync<TValue, TError, TBindValue, TResult>(this Maybe<TValue, TError> value, Maybe<TBindValue, TError> maybe, Func<TValue, TBindValue, Task<Maybe<TResult, TError>>> bind)
        {
            if (value._hasValue && maybe._hasValue) return bind(value._value, maybe._value);
            if (!value._hasValue && !maybe._hasValue) return Task.FromResult(new Maybe<TResult, TError>(value._error, value.AggregateErrors, maybe._error, maybe.AggregateErrors));
            if (!maybe._hasValue) return Task.FromResult(new Maybe<TResult, TError>(maybe._error));
            return Task.FromResult(new Maybe<TResult, TError>(value._error));
        }

        /** Maybe Bind implementation with Error conversion. **/

        public static Maybe<TResult, TBindError> Bind<TValue, TError, TBindValue, TBindError, TResult>(this Maybe<TValue, TError> value, Maybe<TBindValue, TBindError> maybe, Func<TError, TBindError> convert, Func<TValue, TBindValue, Maybe<TResult, TBindError>> bind)
        {
            return value.Match(x => new Maybe<TValue, TBindError>(x), x => new Maybe<TValue, TBindError>(convert(x))).Bind(maybe, bind);
        }

        public static Maybe<TResult, (TError, TBindError)> Bind<TValue, TError, TBindValue, TBindError, TResult>(this Maybe<TValue, TError> value, Maybe<TBindValue, TBindError> maybe, Func<TValue, TBindValue, Maybe<TResult, (TError, TBindError)>> bind)
        {
            var valueMatch = value.Match(x => new Maybe<TValue, (TError, TBindError)>(x), x => new Maybe<TValue, (TError, TBindError)>((x, default)));
            var maybeMatch = maybe.Match(x => new Maybe<TBindValue, (TError, TBindError)>(x), x => new Maybe<TBindValue, (TError, TBindError)>((default, x)));
            return valueMatch.Bind(maybeMatch, bind);
        }

        public static Task<Maybe<TResult, TBindError>> BindAsync<TValue, TError, TBindValue, TBindError, TResult>(this Maybe<TValue, TError> value, Maybe<TBindValue, TBindError> maybe, Func<TError, TBindError> convert, Func<TValue, TBindValue, Task<Maybe<TResult, TBindError>>> bind)
        {
            return value.Match(x => new Maybe<TValue, TBindError>(x), x => new Maybe<TValue, TBindError>(convert(x))).BindAsync(maybe, bind);
        }

        public static Task<Maybe<TResult, (TError, TBindError)>> BindAsync<TValue, TError, TBindValue, TBindError, TResult>(this Maybe<TValue, TError> value, Maybe<TBindValue, TBindError> maybe, Func<TValue, TBindValue, Task<Maybe<TResult, (TError, TBindError)>>> bind)
        {
            var valueMatch = value.Match(x => new Maybe<TValue, (TError, TBindError)>(x), x => new Maybe<TValue, (TError, TBindError)>((x, default)));
            var maybeMatch = maybe.Match(x => new Maybe<TBindValue, (TError, TBindError)>(x), x => new Maybe<TBindValue, (TError, TBindError)>((default, x)));
            return valueMatch.BindAsync(maybeMatch, bind);
        }

        /** Maybe Bind first arg is Task. **/

        public static Task<Maybe<TResult, TError>> BindAsync<TValue, TError, TBindValue, TResult>(this Task<Maybe<TValue, TError>> value, Maybe<TBindValue, TError> maybe, Func<TValue, TBindValue, Maybe<TResult, TError>> bind)
        {
            return value.ContinueWith(x => Bind(x.Result, maybe, bind));
        }

        public static Task<Maybe<TResult, TError>> BindAsync<TValue, TError, TBindValue, TResult>(this Task<Maybe<TValue, TError>> value, Maybe<TBindValue, TError> maybe, Func<TValue, TBindValue, Task<Maybe<TResult, TError>>> bind)
        {
            return value.ContinueWith(x => BindAsync(x.Result, maybe, bind)).Unwrap();
        }

        /** Maybe Bind first arg is Task with Error conversion. **/

        public static Task<Maybe<TResult, TBindError>> BindAsync<TValue, TError, TBindValue, TBindError, TResult>(this Task<Maybe<TValue, TError>> value, Maybe<TBindValue, TBindError> maybe, Func<TError, TBindError> convert, Func<TValue, TBindValue, Maybe<TResult, TBindError>> bind)
        {
            return value.ContinueWith(x => Bind(x.Result, maybe, convert, bind));
        }

        public static Task<Maybe<TResult, (TError, TBindError)>> BindAsync<TValue, TError, TBindValue, TBindError, TResult>(this Task<Maybe<TValue, TError>> value, Maybe<TBindValue, TBindError> maybe, Func<TValue, TBindValue, Maybe<TResult, (TError, TBindError)>> bind)
        {
            return value.ContinueWith(x => Bind(x.Result, maybe, bind));
        }

        public static Task<Maybe<TResult, TBindError>> BindAsync<TValue, TError, TBindValue, TBindError, TResult>(this Task<Maybe<TValue, TError>> value, Maybe<TBindValue, TBindError> maybe, Func<TError, TBindError> convert, Func<TValue, TBindValue, Task<Maybe<TResult, TBindError>>> bind)
        {
            return value.ContinueWith(x => BindAsync(x.Result, maybe, convert, bind)).Unwrap();
        }

        public static Task<Maybe<TResult, (TError, TBindError)>> BindAsync<TValue, TError, TBindValue, TBindError, TResult>(this Task<Maybe<TValue, TError>> value, Maybe<TBindValue, TBindError> maybe, Func<TValue, TBindValue, Task<Maybe<TResult, (TError, TBindError)>>> bind)
        {
            return value.ContinueWith(x => BindAsync(x.Result, maybe, bind)).Unwrap();
        }

        /** Maybe Bind second arg is Task. **/

        public static Task<Maybe<TResult, TError>> BindAsync<TValue, TError, TBindValue, TResult>(this Maybe<TValue, TError> value, Task<Maybe<TBindValue, TError>> maybe, Func<TValue, TBindValue, Maybe<TResult, TError>> bind)
        {
            return maybe.ContinueWith(x => Bind(value, x.Result, bind));
        }

        public static Task<Maybe<TResult, TError>> BindAsync<TValue, TError, TBindValue, TResult>(this Maybe<TValue, TError> value, Task<Maybe<TBindValue, TError>> maybe, Func<TValue, TBindValue, Task<Maybe<TResult, TError>>> bind)
        {
            return maybe.ContinueWith(x => BindAsync(value, x.Result, bind)).Unwrap();
        }

        /** Maybe Bind second arg is Task with Error conversion. **/

        public static Task<Maybe<TResult, TBindError>> BindAsync<TValue, TError, TBindValue, TBindError, TResult>(this Maybe<TValue, TError> value, Task<Maybe<TBindValue, TBindError>> maybe, Func<TError, TBindError> convert, Func<TValue, TBindValue, Maybe<TResult, TBindError>> bind)
        {
            return maybe.ContinueWith(x => Bind(value, x.Result, convert, bind));
        }

        public static Task<Maybe<TResult, (TError, TBindError)>> BindAsync<TValue, TError, TBindValue, TBindError, TResult>(this Maybe<TValue, TError> value, Task<Maybe<TBindValue, TBindError>> maybe, Func<TValue, TBindValue, Maybe<TResult, (TError, TBindError)>> bind)
        {
            return maybe.ContinueWith(x => Bind(value, x.Result, bind));
        }

        public static Task<Maybe<TResult, TBindError>> BindAsync<TValue, TError, TBindValue, TBindError, TResult>(this Maybe<TValue, TError> value, Task<Maybe<TBindValue, TBindError>> maybe, Func<TError, TBindError> convert, Func<TValue, TBindValue, Task<Maybe<TResult, TBindError>>> bind)
        {
            return maybe.ContinueWith(x => BindAsync(value, x.Result, convert, bind)).Unwrap();
        }

        public static Task<Maybe<TResult, (TError, TBindError)>> BindAsync<TValue, TError, TBindValue, TBindError, TResult>(this Maybe<TValue, TError> value, Task<Maybe<TBindValue, TBindError>> maybe, Func<TValue, TBindValue, Task<Maybe<TResult, (TError, TBindError)>>> bind)
        {
            return maybe.ContinueWith(x => BindAsync(value, x.Result, bind)).Unwrap();
        }

        /** Maybe Bind first, and second args are Tasks. **/

        public static Task<Maybe<TResult, TError>> BindAsync<TValue, TError, TBindValue, TResult>(this Task<Maybe<TValue, TError>> value, Task<Maybe<TBindValue, TError>> maybe, Func<TValue, TBindValue, Maybe<TResult, TError>> bind)
        {
            return Task.WhenAll(value, maybe).ContinueWith(_ => Bind(value.Result, maybe.Result, bind));
        }

        public static Task<Maybe<TResult, TError>> BindAsync<TValue, TError, TBindValue, TResult>(this Task<Maybe<TValue, TError>> value, Task<Maybe<TBindValue, TError>> maybe, Func<TValue, TBindValue, Task<Maybe<TResult, TError>>> bind)
        {
            return Task.WhenAll(value, maybe).ContinueWith(_ => BindAsync(value.Result, maybe.Result, bind)).Unwrap();
        }

        /** Maybe Bind first, and second args are Tasks with Error conversion. **/

        public static Task<Maybe<TResult, TBindError>> BindAsync<TValue, TError, TBindValue, TBindError, TResult>(this Task<Maybe<TValue, TError>> value, Task<Maybe<TBindValue, TBindError>> maybe, Func<TError, TBindError> convert, Func<TValue, TBindValue, Maybe<TResult, TBindError>> bind)
        {
            return Task.WhenAll(value, maybe).ContinueWith(_ => Bind(value.Result, maybe.Result, convert, bind));
        }

        public static Task<Maybe<TResult, (TError, TBindError)>> BindAsync<TValue, TError, TBindValue, TBindError, TResult>(this Task<Maybe<TValue, TError>> value, Task<Maybe<TBindValue, TBindError>> maybe, Func<TValue, TBindValue, Maybe<TResult, (TError, TBindError)>> bind)
        {
            return Task.WhenAll(value, maybe).ContinueWith(_ => Bind(value.Result, maybe.Result, bind));
        }

        public static Task<Maybe<TResult, TBindError>> BindAsync<TValue, TError, TBindValue, TBindError, TResult>(this Task<Maybe<TValue, TError>> value, Task<Maybe<TBindValue, TBindError>> maybe, Func<TError, TBindError> convert, Func<TValue, TBindValue, Task<Maybe<TResult, TBindError>>> bind)
        {
            return Task.WhenAll(value, maybe).ContinueWith(_ => BindAsync(value.Result, maybe.Result, convert, bind)).Unwrap();
        }

        public static Task<Maybe<TResult, (TError, TBindError)>> BindAsync<TValue, TError, TBindValue, TBindError, TResult>(this Task<Maybe<TValue, TError>> value, Task<Maybe<TBindValue, TBindError>> maybe, Func<TValue, TBindValue, Task<Maybe<TResult, (TError, TBindError)>>> bind)
        {
            return Task.WhenAll(value, maybe).ContinueWith(_ => BindAsync(value.Result, maybe.Result, bind)).Unwrap();
        }

        #endregion
    }
}