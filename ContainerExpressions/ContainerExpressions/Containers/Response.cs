using System;
using System.Threading.Tasks;

namespace ContainerExpressions.Containers
{
    /// <summary>
    /// A wrapper around the real value a method will return.
    /// <para>Using this pattern you can tell if a returned value is the correct one, or if some error happened trying to get the real value.</para>
    /// </summary>
    /// <typeparam name="T">The value to return.</typeparam>
    public struct Response<T>
    {
        /// <summary>True if the value was set correctly, false if some error occurred getting the value.</summary>
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
        /// <summary>True if the container is in a valid state, otherwise the operation didn't run successfully.</summary>
        public bool IsValid { get { return _isValid; } }
        private readonly bool _isValid;

        /// <summary>Create a response container in an valid state.</summary>
        public Response(bool isValid)
        {
            _isValid = isValid;
        }

        /// <summary>Create a response container in a valid state.</summary>
        /// <param name="value">The response's value.</param>
        public static Response<T> Create<T>(T value) => new Response<T>(value);

        /// <summary>Create a response container in an invalid state.</summary>
        public static Response<T> Create<T>() => new Response<T>();

        /// <summary>Creates a function wrapper that will be invoked each time it's value is accessed.</summary>
        public static ResponseFunc<T> Create<T>(Func<Response<T>> func) => new ResponseFunc<T>(func);

        /// <summary>Creates a function wrapper that will be invoked each time it's value is accessed.</summary>
        public static ResponseFuncTask<T> CreateAsync<T>(Func<Task<Response<T>>> func) => new ResponseFuncTask<T>(func);

        /// <summary>Turn a function that doesn't return a Response, into one that does.</summary>
        public static Func<T, Response<TResult>> Lift<T, TResult>(Func<T, TResult> func) => Create<Func<T, Response<TResult>>>(x => Create(func(x)));

        /// <summary>When compared to a bool, the IsValid properties value will be used.</summary>
        public static implicit operator bool(Response response) => response.IsValid;

        /// <summary>Gets the string value for if this response is valid or not.</summary>
        /// <returns>The bool string value for the IsValid Property.</returns>
        public override string ToString() => _isValid.ToString();
    }

    /// <summary>Utility methods for the Response Container.</summary>
    public static class ResponseExtensions
    {
        /// <summary>Creates a valid container response.</summary>
        public static Response<T> WithValue<T>(this Response<T> response, T value) => new Response<T>(value);

        /// <summary>Creates an invalid container response.</summary>
        public static Response<T> WithNoValue<T>(this Response<T> response) => new Response<T>();

        /// <summary>Create a response container in an valid state.</summary>
        public static Response AsValid(this Response response) => new Response(true);

        /// <summary>Create a response container in an invalid state.</summary>
        public static Response AsInvalid(this Response response) => new Response(false);

        /// <summary>Executes the bind func only if the input Response is valid, otherwise an invalid response is returned.</summary>
        public static Response Bind<T>(this Response<T> response, Func<T, Response> func) => response.IsValid ? func(response.Value) : new Response();

        /// <summary>Executes the bind func only if the input Response is valid, otherwise an invalid response is returned.</summary>
        public static Response Bind<T, TState>(this Response<T> response, TState state, Func<TState, T, Response> func) => response.IsValid ? func(state, response.Value) : new Response();

        /// <summary>Executes the bind func only if the input Response is valid, otherwise an invalid response is returned.</summary>
        public static Response<TResult> Bind<T, TResult>(this Response<T> response, Func<T, Response<TResult>> func) => response.IsValid ? func(response.Value) : Response.Create<TResult>();

        /// <summary>Executes the bind func only if the input Response is valid, otherwise an invalid response is returned.</summary>
        public static Response<TResult> Bind<T, TState, TResult>(this Response<T> response, TState state, Func<TState, T, Response<TResult>> func) => response.IsValid ? func(state, response.Value) : Response.Create<TResult>();

        /// <summary>Gets the value, unless the state is invalid, then the default value is returned.</summary>
        public static T GetValueOrDefault<T>(this Response<T> response, T defaultValue) => response.IsValid ? response.Value : defaultValue;

        /// <summary>Map the underlying Response type, to another type.</summary>
        /// <typeparam name="T">The type of the input response.</typeparam>
        /// <typeparam name="TResult">The type of the output response.</typeparam>
        /// <param name="response">The result of the last ran code.</param>
        /// <param name="func">An error free function that maps one type to another.</param>
        /// <returns>The mapped response, or an invalid response if the input was in an invalid state.</returns>
        public static Response<TResult> Transform<T, TResult>(this Response<T> response, Func<T, TResult> func) => response ? Response.Create(func(response)) : Response.Create<TResult>();
    }
}