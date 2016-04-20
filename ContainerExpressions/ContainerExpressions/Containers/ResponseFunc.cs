using ContainerExpressions.Expressions.Models;
using System;
using System.Threading.Tasks;

namespace ContainerExpressions.Containers
{
    /// <summary>Wraps a function that returns a response T.</summary>
    /// <typeparam name="T">The response of the underlying function.</typeparam>
    public struct ResponseFuncTask<T>
    {
        /// <summary>Gets the result of the underlying function's value, the underlying function is invoked on each call.</summary>
        public Task<Response<T>> Result { get { return _func(); } }

        /// <summary>Gets the underlying function.</summary>
        internal Func<Task<Response<T>>> Func { get { return _func; } }
        private readonly Func<Task<Response<T>>> _func;

        /// <summary>Wraps a function that returns a response T.</summary>
        public ResponseFuncTask(Func<Task<Response<T>>> func)
        {
            _func = func;
        }

        /// <summary>When compared to Response T, the underlying type is returned.</summary>
        public static implicit operator Task<Response<T>>(ResponseFuncTask<T> response) => response.Result;
    }

    /// <summary>Wraps a function that returns a response T.</summary>
    /// <typeparam name="T">The response of the underlying function.</typeparam>
    public struct ResponseFunc<T>
    {
        /// <summary>Gets the result of the underlying function's value, the underlying function is invoked on each call.</summary>
        public Response<T> Result { get { return _func(); } }

        /// <summary>Gets the underlying function.</summary>
        internal Func<Response<T>> Func { get { return _func; } }
        private readonly Func<Response<T>> _func;

        /// <summary>Wraps a function that returns a response T.</summary>
        public ResponseFunc(Func<Response<T>> func)
        {
            _func = func;
        }

        /// <summary>When compared to Response T, the underlying type is returned.</summary>
        public static implicit operator Response<T>(ResponseFunc<T> response) => response.Result;
    }

    /// <summary>Utility methods for the Function Response Container.</summary>
    public static class ResponseFuncExtensions
    {
        /// <summary>Execute a function, and retries (using default values) if the Response is invalid.</summary>
        public static Response<T> Retry<T>(this ResponseFunc<T> response) => Containers.Retry.Execute(response.Func);

        /// <summary>Execute a function, and retries (using custom values) if the Response is invalid.</summary>
        public static Response<T> Retry<T>(this ResponseFunc<T> response, RetryOptions options) => Containers.Retry.Execute(response.Func, options);

        /// <summary>Execute a function, and retries (using default values) if the Response is invalid.</summary>
        public static Task<Response<T>> RetryAsync<T>(this ResponseFuncTask<T> response) => Containers.Retry.ExecuteAsync(response.Func);

        /// <summary>Execute a function, and retries (using custom values) if the Response is invalid.</summary>
        public static Task<Response<T>> RetryAsync<T>(this ResponseFuncTask<T> response, RetryOptions options) => Containers.Retry.ExecuteAsync(response.Func, options);
    }
}
