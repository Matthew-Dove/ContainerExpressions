using System;
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
        private readonly bool _hasValue;
        private readonly TValue _value;
        private readonly TError _error;

        public Maybe(TValue value)
        {
            _value = value;
            _error = default;
            _hasValue = true;
        }

        public Maybe(TError error)
        {
            _value = default;
            _error = error;
            _hasValue = false;
        }

        public Maybe(Response<TValue> value, TError error)
        {
            if (value)
            {
                _value = value;
                _error = default;
                _hasValue = true;
            }
            else
            {
                _value = default;
                _error = error;
                _hasValue = false;
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
            }
            else
            {
                _value = default;
                _error = result.error;
                _hasValue = false;
            }
        }

        private Maybe(TError error, TError bindError)
        {
            _value = default;
            _error = bindError;
            _hasValue = false;
        }

        public TMatch Match<TMatch>(Func<TValue, TMatch> getValue, Func<TError, TMatch> getError) => _hasValue ? getValue(_value) : getError(_error);

        public Task<TMatch> MatchAsync<TMatch>(Func<TValue, Task<TMatch>> getValue, Func<TError, TMatch> getError) => _hasValue ? getValue(_value) : Task.FromResult(getError(_error));

        public Maybe<TResult, TError> Bind<TBind, TResult>(Maybe<TBind, TError> maybe, Func<TValue, TBind, Maybe<TResult, TError>> bind)
        {
            if (_hasValue && maybe._hasValue) return bind(_value, maybe._value);
            if (!_hasValue && !maybe._hasValue) return new Maybe<TResult, TError>(_error, maybe._error);
            if (!maybe._hasValue) return new Maybe<TResult, TError>(maybe._error);
            return new Maybe<TResult, TError>(_error);
        }

        public Task<Maybe<TResult, TError>> BindAsync<TResult>(Func<TValue, Task<Maybe<TResult, TError>>> bind) => _hasValue ? bind(_value) : Task.FromResult(new Maybe<TResult, TError>(_error));

        public Maybe<TResult, TError> Transform<TResult>(Func<TValue, TResult> transform) => Match(x => new Maybe<TResult, TError>(transform(x)), x => new Maybe<TResult, TError>(x));

        public Task<Maybe<TResult, TError>> TransformAsync<TResult>(Func<TValue, Task<TResult>> transform) => Match(x => transform(x).ContinueWith(y => new Maybe<TResult, TError>(y.Result)), x => Task.FromResult(new Maybe<TResult, TError>(x)));

        public Either<TValue, TError> ToEither() => Match(x => new Either<TValue, TError>(x), x => new Either<TValue, TError>(x));

        public Response<TValue> ToResponse() => Match(x => new Response<TValue>(x), _ => new Response<TValue>());

        public TValue GetValueOrDefault(TValue @default) => Match(x => x, _ => @default);

        public Maybe<TValue, TError> With(Response<TValue> value, TError error) => new Maybe<TValue, TError>(value, error);

        public Maybe<TValue, TError> With(Either<TValue, TError> either) => new Maybe<TValue, TError>(either);

        public Maybe<TValue, TError> With(TValue value) => new Maybe<TValue, TError>(value);

        public Maybe<TValue, TError> With(TError error) => new Maybe<TValue, TError>(error);

        public override string ToString() => Match(x => x?.ToString(), x => x?.ToString()) ?? string.Empty;

        public static implicit operator Response<TValue>(Maybe<TValue, TError> maybe) => maybe.Match(x => new Response<TValue>(x), _ => new Response<TValue>());

        public static implicit operator Maybe<TValue, TError>(Either<TValue, TError> either) => new Maybe<TValue, TError>(either);

        public static implicit operator Either<TValue, TError>(Maybe<TValue, TError> maybe) => maybe.Match(x => new Either<TValue, TError>(x), x => new Either<TValue, TError>(x));

        public static implicit operator Maybe<TValue, TError>(TValue value) => new Maybe<TValue, TError>(value);

        public static implicit operator Maybe<TValue, TError>(TError error) => new Maybe<TValue, TError>(error);
    }
}