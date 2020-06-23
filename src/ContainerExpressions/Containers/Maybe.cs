﻿using System;

namespace ContainerExpressions.Containers
{
    public readonly struct Maybe<TValue>
    {
        internal Exception[] AggregateErrors { get; }
        internal readonly static Exception[] _emptyAggregateErrors = new Exception[0];

        internal readonly bool _hasValue;
        internal readonly TValue _value;
        internal readonly Exception _error;

        public Maybe(TValue value)
        {
            _value = value;
            _error = default;
            _hasValue = true;
            AggregateErrors = _emptyAggregateErrors;
        }

        public Maybe(Exception error)
        {
            _value = default;
            _error = error;
            _hasValue = false;
            AggregateErrors = _emptyAggregateErrors;
        }

        public Maybe(Response<TValue> value, Exception error)
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

        public Maybe(Either<TValue, Exception> either)
        {
            var result = either.Match(x => (isValue: true, value: x, error: default(Exception)), x => (isValue: false, value: default(TValue), error: x));

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

        internal Maybe(Exception error, Exception[] aggregateErrors, Exception bindError, Exception[] bindAggregateErrors)
        {
            _value = default;
            _error = bindError;
            _hasValue = false;

            int length = (aggregateErrors?.Length ?? 0), bindLength = (bindAggregateErrors?.Length ?? 0), totalLength = length + bindLength + 1;
            var errors = new Exception[totalLength];

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

        public override string ToString() => this.Match(x => x?.ToString(), x => x?.ToString()) ?? string.Empty;

        public static implicit operator Response<TValue>(Maybe<TValue> maybe) => maybe.Match(x => new Response<TValue>(x), _ => new Response<TValue>());

        public static implicit operator Maybe<TValue>(Either<TValue, Exception> either) => new Maybe<TValue>(either);

        public static implicit operator Either<TValue, Exception>(Maybe<TValue> maybe) => maybe.Match(x => new Either<TValue, Exception>(x), x => new Either<TValue, Exception>(x));

        public static implicit operator Maybe<TValue>(TValue value) => new Maybe<TValue>(value);

        public static implicit operator Maybe<TValue>(Exception error) => new Maybe<TValue>(error);
    }

    public readonly struct Maybe<TValue, TError>
    {
        internal TError[] AggregateErrors { get; }
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

        public override string ToString() => this.Match(x => x?.ToString(), x => x?.ToString()) ?? string.Empty;

        public static implicit operator Response<TValue>(Maybe<TValue, TError> maybe) => maybe.Match(x => new Response<TValue>(x), _ => new Response<TValue>());

        public static implicit operator Maybe<TValue, TError>(Either<TValue, TError> either) => new Maybe<TValue, TError>(either);

        public static implicit operator Either<TValue, TError>(Maybe<TValue, TError> maybe) => maybe.Match(x => new Either<TValue, TError>(x), x => new Either<TValue, TError>(x));

        public static implicit operator Maybe<TValue, TError>(TValue value) => new Maybe<TValue, TError>(value);

        public static implicit operator Maybe<TValue, TError>(TError error) => new Maybe<TValue, TError>(error);
    }

    public class Maybe
    {
        private Maybe() { }

        #region Maybe<TValue>

        public static Maybe<TValue> Create<TValue>() => new Maybe<TValue>();

        public static Maybe<TValue> Create<TValue>(TValue value) => new Maybe<TValue>(value);

        public static Maybe<TValue> Create<TValue>(Exception error) => new Maybe<TValue>(error);

        public static Maybe<TValue> Create<TValue>(Either<TValue, Exception> either) => new Maybe<TValue>(either);

        public static Maybe<TValue> Create<TValue>(Response<TValue> response, Exception ex) => new Maybe<TValue>(response, ex);

        #endregion

        #region Maybe<TValue, TError>

        public static ErrorContainer<TValue> Value<TValue>() => new ErrorContainer<TValue>();

        public static ValueContainer<TValue> Value<TValue>(TValue value) => new ValueContainer<TValue>(value);

        public static Maybe<TValue, TError> Create<TValue, TError>() => new Maybe<TValue, TError>();

        public static Maybe<TValue, TError> Create<TValue, TError>(TValue value) => new Maybe<TValue, TError>(value);

        public static Maybe<TValue, TError> Create<TValue, TError>(TError error) => new Maybe<TValue, TError>(error);

        public static Maybe<TValue, TError> Create<TValue, TError>(Either<TValue, TError> either) => new Maybe<TValue, TError>(either);

        public static Maybe<TValue, TError> Create<TValue, TError>(Response<TValue> response, TError error) => new Maybe<TValue, TError>(response, error);

        #endregion
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
}