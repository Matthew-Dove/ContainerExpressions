using System;

namespace ContainerExpressions.Containers
{
    /// <summary>
    /// A wrapper around the real value a method will return.
    /// <para>Using this pattern you can tell if a returned value is the correct one, or if some error happened trying to get the real vlaue.</para>
    /// </summary>
    /// <typeparam name="T">The value to return.</typeparam>
    public struct Response<T>
    {
        /// <summary>True if the value was set correctly, false if some error occurred getting the vlaue.</summary>
        public bool IsValid { get; }

        /// <summary>The value that was calculated, with the guarantee it's in a valid state.</summary>
        public T Value { get { if (!IsValid) throw new InvalidOperationException("Cannot access the value if the container is not valid."); return _value; } }
        private readonly T _value;

        /// <summary>Create a response container in a valid state.</summary>
        /// <param name="value">The response's value.</param>
        public Response(T value)
        {
            _value = value;
            IsValid = true;
        }

        /// <summary>Gets the value, unless the state is invalid, then the default value is returned.</summary>
        /// <param name="defaultValue">The value to return when the container is in an invaild state.</param>
        /// <returns>The value, or the specified default value.</returns>
        public T GetValueOrDefault(T defaultValue) => IsValid ? _value : defaultValue;

        /// <summary>Creates a valid container response.</summary>
        /// <param name="value">The value for the response.</param>
        public Response<T> WithValue(T value) => new Response<T>(value);

        /// <summary>Creates an invalid container response</summary>
        public Response<T> WithNoValue() => new Response<T>();

        /// <summary>When compared to a bool, the IsValid property value will be used.</summary>
        public static implicit operator bool(Response<T> response) => response.IsValid;

        /// <summary>When compared to T, the Value property will be used.</summary>
        public static implicit operator T(Response<T> response) => response.Value;

        /// <summary>Returns the underlying value's string representation.</summary>
        public override string ToString() => _value == null ? string.Empty : _value.ToString();
    }

    /// <summary>A helper class for the Response generic class.</summary>
    public struct Response
    {
        public bool IsValid { get { return _isValid; } }
        private readonly bool _isValid;

        /// <summary>Create a response container in an valid state.</summary>
        public Response(bool isValid)
        {
            _isValid = isValid;
        }

        /// <summary>Create a response container in a valid state.</summary>
        /// <param name="value">The response's value.</param>
        public static Response<T> Create<T>(T value) => new Response<T>(value); // A little trick so the caller doesn't have to specify T.

        /// <summary>Create a response container in an invalid state.</summary>
        public static Response<T> Create<T>() => new Response<T>(); // A little trick so the caller doesn't have to specify T.

        /// <summary>Create a response container in an valid state.</summary>
        public Response AsValid() => new Response(true);

        /// <summary>Create a response container in an invalid state.</summary>
        public Response AsInvalid() => new Response(false);

        /// <summary>When compared to a bool, the IsValid properties value will be used.</summary>
        public static implicit operator bool(Response response) => response.IsValid;

        /// <summary>Gets teh string value for if this response is valid or not.</summary>
        /// <returns>The bool string value for the IsValid Property.</returns>
        public override string ToString() => _isValid.ToString();
    }
}