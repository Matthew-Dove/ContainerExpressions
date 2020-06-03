using Repeat = ContainerExpressions.Containers.Retry;
using ContainerExpressions.Expressions.Models;
using System;
using System.Threading.Tasks;

namespace ContainerExpressions.Containers
{
    /// <summary>Utility methods for the Retry Container.</summary>
    public static class RetryExtensions
    {
        #region () => Response<T>

        /// <summary>Execute a function, and retries (using default values) if the Response is invalid.</summary>
        public static Func<Response<T>> Retry<T>(this Func<Response<T>> func) => () => Repeat.Execute(func, RetryOptions.Create());

        /// <summary>Execute a function, and retries (using exponential default values) if the Response is invalid.</summary>
        public static Func<Response<T>> RetryExponential<T>(this Func<Response<T>> func) => () => Repeat.Execute(func, RetryOptions.CreateExponential());

        /// <summary>Execute a function, and retries (using custom values) if the Response is invalid.</summary>
        public static Func<Response<T>> Retry<T>(this Func<Response<T>> func, RetryOptions options) => () => Repeat.Execute(func, options);

        #endregion

        #region T => Response<TResult>

        /// <summary>Execute a function, and retries (using default values) if the Response is invalid.</summary>
        public static Func<T, Response<TResult>> Retry<T, TResult>(this Func<T, Response<TResult>> func) => x => Repeat.Execute(x, func, RetryOptions.Create());

        /// <summary>Execute a function, and retries (using default exponential values) if the Response is invalid.</summary>
        public static Func<T, Response<TResult>> RetryExponential<T, TResult>(this Func<T, Response<TResult>> func) => x => Repeat.Execute(x, func, RetryOptions.CreateExponential());

        /// <summary>Execute a function, and retries (using custom values) if the Response is invalid.</summary>
        public static Func<T, Response<TResult>> Retry<T, TResult>(this Func<T, Response<TResult>> func, RetryOptions options) => x => Repeat.Execute(x, func, options);

        #endregion

        #region () => Task<Response<T>>

        /// <summary>Execute a function, and retries (using default values) if the Response is invalid.</summary>
        public static Func<Task<Response<T>>> RetryAsync<T>(this Func<Task<Response<T>>> func) => () => Repeat.ExecuteAsync(func, RetryOptions.Create());

        /// <summary>Execute a function, and retries (using default exponential values) if the Response is invalid.</summary>
        public static Func<Task<Response<T>>> RetryExponentialAsync<T>(this Func<Task<Response<T>>> func) => () => Repeat.ExecuteAsync(func, RetryOptions.CreateExponential());

        /// <summary>Execute a function, and retries (using custom values) if the Response is invalid.</summary>
        public static Func<Task<Response<T>>> RetryAsync<T>(this Func<Task<Response<T>>> func, RetryOptions options) => () => Repeat.ExecuteAsync(func, options);

        #endregion

        #region T => Task<Response<TResult>>

        /// <summary>Execute a function, and retries (using default values) if the Response is invalid.</summary>
        public static Func<T, Task<Response<TResult>>> RetryAsync<T, TResult>(this Func<T, Task<Response<TResult>>> func) => x => Repeat.ExecuteAsync(x, func, RetryOptions.Create());

        /// <summary>Execute a function, and retries (using default exponential values) if the Response is invalid.</summary>
        public static Func<T, Task<Response<TResult>>> RetryExponentialAsync<T, TResult>(this Func<T, Task<Response<TResult>>> func) => x => Repeat.ExecuteAsync(x, func, RetryOptions.CreateExponential());

        /// <summary>Execute a function, and retries (using custom values) if the Response is invalid.</summary>
        public static Func<T, Task<Response<TResult>>> RetryAsync<T, TResult>(this Func<T, Task<Response<TResult>>> func, RetryOptions options) => x => Repeat.ExecuteAsync(x, func, options);

        #endregion
    }
}
