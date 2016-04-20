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

    /// <summary>Wraps a function that returns a response.</summary>
    public struct ResponseFuncTask
    {
        /// <summary>Gets the result of the underlying function's value, the underlying function is invoked on each call.</summary>
        public Task<Response> Result { get { return _func(); } }

        /// <summary>Gets the underlying function.</summary>
        internal Func<Task<Response>> Func { get { return _func; } }
        private readonly Func<Task<Response>> _func;

        /// <summary>Wraps a function that returns a response.</summary>
        public ResponseFuncTask(Func<Task<Response>> func)
        {
            _func = func;
        }

        /// <summary>When compared to Response, the underlying result is returned.</summary>
        public static implicit operator Task<Response>(ResponseFuncTask response) => response.Result;
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

    /// <summary>Wraps a function that returns a response.</summary>
    public struct ResponseFunc
    {
        /// <summary>Gets the result of the underlying function's value, the underlying function is invoked on each call.</summary>
        public Response Result { get { return _func(); } }

        /// <summary>Gets the underlying function.</summary>
        internal Func<Response> Func { get { return _func; } }
        private readonly Func<Response> _func;

        /// <summary>Wraps a function that returns a response.</summary>
        public ResponseFunc(Func<Response> func)
        {
            _func = func;
        }

        /// <summary>When compared to Response, the underlying result is returned.</summary>
        public static implicit operator Response(ResponseFunc response) => response.Result;
    }

    /// <summary>Utility methods for the Function Response Container.</summary>
    public static class ResponseFuncExtensions
    {
        #region Retry

        /// <summary>Execute a function, and retries (using default values) if the Response is invalid.</summary>
        public static Response Retry(this ResponseFunc response) => Containers.Retry.Execute(response.Func);

        /// <summary>Execute a function, and retries (using custom values) if the Response is invalid.</summary>
        public static Response Retry(this ResponseFunc response, RetryOptions options) => Containers.Retry.Execute(response.Func, options);

        /// <summary>Execute a function, and retries (using default values) if the Response is invalid.</summary>
        public static Response<T> Retry<T>(this ResponseFunc<T> response) => Containers.Retry.Execute(response.Func);

        /// <summary>Execute a function, and retries (using custom values) if the Response is invalid.</summary>
        public static Response<T> Retry<T>(this ResponseFunc<T> response, RetryOptions options) => Containers.Retry.Execute(response.Func, options);

        /// <summary>Execute a function, and retries (using default values) if the Response is invalid.</summary>
        public static Task<Response> RetryAsync(this ResponseFuncTask response) => Containers.Retry.ExecuteAsync(response.Func);

        /// <summary>Execute a function, and retries (using custom values) if the Response is invalid.</summary>
        public static Task<Response> RetryAsync(this ResponseFuncTask response, RetryOptions options) => Containers.Retry.ExecuteAsync(response.Func, options);

        /// <summary>Execute a function, and retries (using default values) if the Response is invalid.</summary>
        public static Task<Response<T>> RetryAsync<T>(this ResponseFuncTask<T> response) => Containers.Retry.ExecuteAsync(response.Func);

        /// <summary>Execute a function, and retries (using custom values) if the Response is invalid.</summary>
        public static Task<Response<T>> RetryAsync<T>(this ResponseFuncTask<T> response, RetryOptions options) => Containers.Retry.ExecuteAsync(response.Func, options);

        #endregion
    }
}
