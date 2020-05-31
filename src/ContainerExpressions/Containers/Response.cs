using System;
using System.Threading.Tasks;

namespace ContainerExpressions.Containers
{
    /// <summary>
    /// A wrapper around the real value a method will return.
    /// <para>Using this pattern you can tell if a returned value is the correct one, or if some error happened trying to get the real value.</para>
    /// </summary>
    /// <typeparam name="T">The value to return.</typeparam>
    public readonly struct Response<T> : IEquatable<Response<T>>
    {
        /// <summary>True if the value was set correctly, false if some error occurred getting the value.</summary>
        public bool IsValid { get; }

        /// <summary>The value that was calculated, with the guarantee it's in a valid state, throws InvalidOperationException if Response is in an invalid state.</summary>
        public T Value { get { if (!IsValid) throw new InvalidOperationException("Cannot access the value if the container is not valid."); return _value; } }
        private readonly T _value;

        /// <summary>Create a response container in a valid state.</summary>
        /// <param name="value">The response's value.</param>
        public Response(T value)
        {
            _value = value;
            IsValid = true;
        }

        /// <summary>When compared to a bool, the IsValid property value will be used.</summary>
        public static implicit operator bool(Response<T> response) => response.IsValid;

        /// <summary>When compared to T, the Value property will be used.</summary>
        public static implicit operator T(Response<T> response) => response.Value;

        /// <summary>When compared to Response, the IsValid property will be used to create the response model.</summary>
        public static implicit operator Response(Response<T> response) => new Response(response.IsValid);

        /// <summary>Returns the underlying value's string representation.</summary>
        public override string ToString() => IsValid ? _value?.ToString() ?? string.Empty : string.Empty;

        /// <summary>If both response types are valid, then the values are compared, otherwise the IsValid property is compared.</summary>
        public bool Equals(Response<T> other)
        {
            if (other.IsValid && IsValid)
            {
                if (_value == null) return other._value == null;
                if (other._value == null) return _value == null;
                return _value.Equals(other._value);
            }
            return other.IsValid == IsValid;
        }

        /// <summary>Compares the provided value, to the Response's Value, throws InvalidOperationException if Response is in an invalid state.</summary>
        public bool Equals(T value)
        {
            if (!IsValid) throw new InvalidOperationException("Cannot access the value if the container is not valid.");
            if (_value == null) return value == null;
            return Value.Equals(value);
        }

        /// <summary>Compares obj if it is of type Response<T>, or of type T. Throws InvalidOperationException if Response is in an invalid state, and obj is a T value.</summary>
        public override bool Equals(object obj) => obj is T value && Equals(value) || obj is Response<T> response && Equals(response);

        /// <summary>Returns the Value's hash code (T), or IsValid's hash code (bool) if there is no value.</summary>
        public override int GetHashCode() => IsValid ? Value.GetHashCode() : IsValid.GetHashCode();

        public static bool operator !=(Response<T> x, Response<T> y) => !(x == y);
        public static bool operator ==(Response<T> x, Response<T> y) => x.Equals(y);

        public static bool operator !=(Response<T> x, T y) => !(x == y);
        public static bool operator ==(Response<T> x, T y) => x.Equals(y);

        public static bool operator !=(T x, Response<T> y) => !(x == y);
        public static bool operator ==(T x, Response<T> y) => y.Equals(x);
    }

    /// <summary>A helper class for the Response generic class.</summary>
    public readonly struct Response
    {
        /// <summary>True if the container is in a valid state, otherwise the operation didn't run successfully.</summary>
        public bool IsValid { get; }

        /// <summary>Create a response container in an valid state.</summary>
        public Response(bool isValid)
        {
            IsValid = isValid;
        }

        /// <summary>Create a response container in a valid state.</summary>
        /// <param name="value">The response's value.</param>
        public static Response<T> Create<T>(T value) => new Response<T>(value);

        /// <summary>Turn a function that doesn't return a Response, into one that does.</summary>
        public static Func<Response<T>> Lift<T>(Func<T> func) => () => Create(func());

        /// <summary>Turn a function that doesn't return a Response, into one that does.</summary>
        public static Func<T, Response<TResult>> Lift<T, TResult>(Func<T, TResult> func) => Create<Func<T, Response<TResult>>>(x => Create(func(x)));

        /// <summary>Turn a function that doesn't return a Response, into one that does.</summary>
        public static Func<Task<Response<T>>> LiftAsync<T>(Func<Task<T>> func) => () => func().ContinueWith(x => Create(x.Result));

        /// <summary>Turn an async function that doesn't return a task Response, into one that does.</summary>
        public static Func<T, Task<Response<TResult>>> LiftAsync<T, TResult>(Func<T, Task<TResult>> func) => Create<Func<T, Task<Response<TResult>>>>(async x => Create(await func(x)));

        /// <summary>When compared to a bool, the IsValid properties value will be used.</summary>
        public static implicit operator bool(Response response) => response.IsValid;

        /// <summary>Gets the string value for if this response is valid or not.</summary>
        /// <returns>The bool string value for the IsValid Property.</returns>
        public override string ToString() => IsValid.ToString();
    }
}