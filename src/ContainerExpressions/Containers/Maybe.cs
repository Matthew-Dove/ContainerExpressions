using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ContainerExpressions.Containers
{
    /// <summary>
    /// Maybe TValue contains either an instance of TValue, or an instance of Exception.
    /// <para>When two (or more) Maybe's Bind, their distinct errors are aggregated into a new Maybe.</para>
    /// <para>When both Maybes have some instance of TValue, the provided binding function will be invoked with said values.</para>
    /// </summary>
    public readonly struct Maybe<TValue> : IEquatable<Maybe<TValue>>, IEquatable<TValue>
    {
        internal Exception[] AggregateErrors { get; }
        internal readonly static Exception[] _emptyAggregateErrors = new Exception[0];

        internal readonly bool _hasValue;
        internal readonly TValue _value;
        internal readonly Exception _error;

        /// <summary>Create a Maybe container, with an instance of TValue.</summary>
        /// <param name="value"></param>
        public Maybe(TValue value)
        {
            _value = value;
            _error = default;
            _hasValue = true;
            AggregateErrors = _emptyAggregateErrors;
        }

        /// <summary>Create a Maybe container, with an instance of Exception.</summary>
        public Maybe(Exception error)
        {
            _value = default;
            _error = error;
            _hasValue = false;
            AggregateErrors = _emptyAggregateErrors;
        }

        /// <summary>Create a Maybe container, with an instance of TValue when the Response is valid; otherwise an instance of Exception.</summary>
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

        /// <summary>Create a Maybe container, with an instance of TValue when Either is also TValue;; otherwise an instance of Exception.</summary>
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
            if (aggregateErrors == null) aggregateErrors = _emptyAggregateErrors;
            if (bindAggregateErrors == null) aggregateErrors = _emptyAggregateErrors;

            _value = default;
            _error = bindError;
            _hasValue = false;

            int length = aggregateErrors.Length, bindLength = bindAggregateErrors.Length, totalLength = length + bindLength + 1;
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

        public static bool operator !=(Maybe<TValue> x, Maybe<TValue> y) => !(x == y);
        public static bool operator ==(Maybe<TValue> x, Maybe<TValue> y) => x.Equals(y);

        public static bool operator !=(TValue x, Maybe<TValue> y) => !(x == y);
        public static bool operator ==(TValue x, Maybe<TValue> y) => y.Equals(x);
        public static bool operator !=(Maybe<TValue> x, TValue y) => !(x == y);
        public static bool operator ==(Maybe<TValue> x, TValue y) => x.Equals(y);

        public static bool operator !=(Exception x, Maybe<TValue> y) => !(x == y);
        public static bool operator ==(Exception x, Maybe<TValue> y) => y.Equals(x);
        public static bool operator !=(Maybe<TValue> x, Exception y) => !(x == y);
        public static bool operator ==(Maybe<TValue> x, Exception y) => x.Equals(y);

        public bool Equals(TValue value) => new Maybe<TValue>(value).Equals(this);
        public bool Equals(Exception error) => new Maybe<TValue>(error).Equals(this);

        public override bool Equals(object obj)
        {
            return obj switch
            {
                Maybe<TValue> other => Equals(other),
                TValue value => Equals(value),
                Exception error => Equals(error),
                _ => false
            };
        }

        public bool Equals(Maybe<TValue> other)
        {
            if (_hasValue) return EqualityComparer<TValue>.Default.Equals(_value, other._value);
            return EqualityComparer<Exception>.Default.Equals(_error, other._error);
        }

        public override int GetHashCode()
        {
            if (_hasValue) return _value?.GetHashCode() ?? 0;
            return _error?.GetHashCode() ?? 0;
        }
    }

    /// <summary>
    /// Maybe TValue TError contains either an instance of TValue, or an instance of TError.
    /// <para>When two (or more) Maybe's Bind, their distinct errors are aggregated into a new Maybe.</para>
    /// <para>When both Maybes have some instance of TValue, the provided binding function will be invoked with said values.</para>
    /// </summary>
    public readonly struct Maybe<TValue, TError> : IEquatable<Maybe<TValue, TError>>, IEquatable<TValue>
    {
        internal TError[] AggregateErrors { get; }
        internal readonly static TError[] _emptyAggregateErrors = new TError[0];

        internal readonly bool _hasValue;
        internal readonly TValue _value;
        internal readonly TError _error;

        /// <summary>Create a Maybe container, with an instance of TValue.</summary>
        public Maybe(TValue value)
        {
            _value = value;
            _error = default;
            _hasValue = true;
            AggregateErrors = _emptyAggregateErrors;
        }

        /// <summary>Create a Maybe container, with an instance of TError.</summary>
        public Maybe(TError error)
        {
            _value = default;
            _error = error;
            _hasValue = false;
            AggregateErrors = _emptyAggregateErrors;
        }

        /// <summary>Create a Maybe container, with an instance of TValue when the Response is valid; otherwise an instance of TError.</summary>
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

        /// <summary>Create a Maybe container, with an instance of TValue when Either is also TValue;; otherwise an instance of TError.</summary>
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
            if (aggregateErrors == null) aggregateErrors = _emptyAggregateErrors;
            if (bindAggregateErrors == null) bindAggregateErrors = _emptyAggregateErrors;

            _value = default;
            _error = bindError;
            _hasValue = false;

            int length = aggregateErrors.Length, bindLength = bindAggregateErrors.Length, totalLength = length + bindLength + 1;
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

        public static bool operator !=(Maybe<TValue, TError> x, Maybe<TValue, TError> y) => !(x == y);
        public static bool operator ==(Maybe<TValue, TError> x, Maybe<TValue, TError> y) => x.Equals(y);

        public static bool operator !=(TValue x, Maybe<TValue, TError> y) => !(x == y);
        public static bool operator ==(TValue x, Maybe<TValue, TError> y) => y.Equals(x);
        public static bool operator !=(Maybe<TValue, TError> x, TValue y) => !(x == y);
        public static bool operator ==(Maybe<TValue, TError> x, TValue y) => x.Equals(y);

        public static bool operator !=(TError x, Maybe<TValue, TError> y) => !(x == y);
        public static bool operator ==(TError x, Maybe<TValue, TError> y) => y.Equals(x);
        public static bool operator !=(Maybe<TValue, TError> x, TError y) => !(x == y);
        public static bool operator ==(Maybe<TValue, TError> x, TError y) => x.Equals(y);

        public bool Equals(TValue value) => new Maybe<TValue, TError>(value).Equals(this);
        public bool Equals(TError error) => new Maybe<TValue, TError>(error).Equals(this);

        public override bool Equals(object obj)
        {
            return obj switch
            {
                Maybe<TValue, TError> other => Equals(other),
                TValue value => Equals(value),
                TError error => Equals(error),
                _ => false
            };
        }

        public bool Equals(Maybe<TValue, TError> other)
        {
            if (_hasValue) return EqualityComparer<TValue>.Default.Equals(_value, other._value);
            return EqualityComparer<TError>.Default.Equals(_error, other._error);
        }

        public override int GetHashCode()
        {
            if (_hasValue) return _value?.GetHashCode() ?? 0;
            return _error?.GetHashCode() ?? 0;
        }
    }

    /// <summary>A helper class of static methods used to created Maybe types.</summary>
    public static class Maybe
    {
        #region Maybe<TValue>

        /// <summary>Creates a Maybe with no provided instance, on match a null Exception will be passed to the error function.</summary>
        public static Maybe<TValue> Create<TValue>() => new Maybe<TValue>();

        /// <summary>Creates a Maybe with the provided TValue instance.</summary>
        public static Maybe<TValue> Create<TValue>(TValue value) => new Maybe<TValue>(value);

        /// <summary>Creates a Maybe with the provided Exception instance.</summary>
        public static Maybe<TValue> CreateError<TValue>(Exception error) => new Maybe<TValue>(error);

        /// <summary>Creates a Maybe from a T2 Either.</summary>
        public static Maybe<TValue> Create<TValue>(Either<TValue, Exception> either) => new Maybe<TValue>(either);

        /// <summary>Creates a Maybe from the Response value when valid, otherwise the Exception will be used to create the Maybe.</summary>
        public static Maybe<TValue> Create<TValue>(Response<TValue> response, Exception ex) => new Maybe<TValue>(response, ex);

        /// <summary>Creates a Maybe from the provided Task's result.</summary>
        public static Task<Maybe<TValue>> CreateAsync<TValue>(Task<TValue> value) => value.ToMaybeTaskAsync();

        #endregion

        #region Maybe<TValue, TError>

        /// <summary>Creates a ErrorContainer for the specified TValue, TError can be added later at anytime to create a Maybe.</summary>
        public static ErrorContainer<TValue> Value<TValue>() => new ErrorContainer<TValue>();

        /// <summary>Creates a ValueContainer for the specified TValue, TError can be added later at anytime to create a Maybe.</summary>
        public static ValueContainer<TValue> Value<TValue>(TValue value) => new ValueContainer<TValue>(value);

        /// <summary>Creates a Maybe with no provided instance, on match a default TError will be passed to the error function.</summary>
        public static Maybe<TValue, TError> Create<TValue, TError>() => new Maybe<TValue, TError>();

        /// <summary>Creates a Maybe with the provided TValue instance.</summary>
        public static Maybe<TValue, TError> Create<TValue, TError>(TValue value) => new Maybe<TValue, TError>(value);

        /// <summary>Creates a Maybe with the provided TError instance.</summary>
        public static Maybe<TValue, TError> CreateError<TValue, TError>(TError error) => new Maybe<TValue, TError>(error);

        /// <summary>Creates a Maybe from a T2 Either.</summary>
        public static Maybe<TValue, TError> Create<TValue, TError>(Either<TValue, TError> either) => new Maybe<TValue, TError>(either);

        /// <summary>Creates a Maybe from the Response value when valid, otherwise the TError will be used to create the Maybe.</summary>
        public static Maybe<TValue, TError> Create<TValue, TError>(Response<TValue> response, TError error) => new Maybe<TValue, TError>(response, error);

        /// <summary>Creates a Maybe from the provided Task's result.</summary>
        public static Task<Maybe<TValue, TError>> CreateAsync<TValue, TError>(Task<TValue> value, TError error) => value.ToMaybeTaskAsync(error);

        #endregion
    }

    /// <summary>Helper constructor class for Maybe, when you know the instance will be of type TValue.</summary>
    public readonly struct ValueContainer<TValue>
    {
        private readonly TValue _value;

        public ValueContainer(TValue value)
        {
            _value = value;
        }

        /// <summary>Creates a Maybe with a TValue instance, TError is required only for type matching.</summary>
        public Maybe<TValue, TError> Error<TError>() => new Maybe<TValue, TError>(_value);
    }

    /// <summary>Helper constructor class for Maybe, when you know the instance will be of type TError.</summary>
    public readonly struct ErrorContainer<TValue>
    {
        /// <summary>Creates a Maybe with a TError instance.</summary>
        public Maybe<TValue, TError> Error<TError>(TError error) => new Maybe<TValue, TError>(error);
    }
}