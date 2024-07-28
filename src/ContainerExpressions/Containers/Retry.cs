using ContainerExpressions.Expressions.Models;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace ContainerExpressions.Containers
{
    /// <summary>Keep retrying a function until it succeeds, or the number of allowed retries is exceeded.</summary>
    public static class Retry
    {
        #region () => Response

        /// <summary>Execute a function, and retries (using default values) if the Response is invalid.</summary>
        public static Response Execute(Func<Response> func) => Execute(func, RetryOptions.Create());

        /// <summary>Execute a function, and retries (using default exponential values) if the Response is invalid.</summary>
        public static Response ExecuteExponential(Func<Response> func) => Execute(func, RetryOptions.CreateExponential());

        /// <summary>Execute a function, and retries (using custom values) if the Response is invalid.</summary>
        public static Response Execute(Func<Response> func, RetryOptions options)
        {
            var response = func();
            var retries = options.Retries;

            while (!response && retries --> 0)
            {
                Thread.Sleep(options.GetMillisecondsDelay(options.Retries - retries));
                response = func();
            }

            return response; 
        }

        #endregion

        #region T => Response

        /// <summary>Execute a function, and retries (using default values) if the Response is invalid.</summary>
        public static Response Execute<T>(T arg, Func<T, Response> func) => Execute(arg, func, RetryOptions.Create());

        /// <summary>Execute a function, and retries (using default exponential values) if the Response is invalid.</summary>
        public static Response ExecuteExponential<T>(T arg, Func<T, Response> func) => Execute(arg, func, RetryOptions.CreateExponential());

        /// <summary>Execute a function, and retries (using custom values) if the Response is invalid.</summary>
        public static Response Execute<T>(T arg, Func<T, Response> func, RetryOptions options)
        {
            var response = func(arg);
            var retries = options.Retries;

            while (!response && retries-- > 0)
            {
                Thread.Sleep(options.GetMillisecondsDelay(options.Retries - retries));
                response = func(arg);
            }

            return response;
        }

        #endregion

        #region () => Task<Response>

        /// <summary>Execute a function, and retries (using default values) if the Response is invalid.</summary>
        public static Task<Response> ExecuteAsync(Func<Task<Response>> func) => ExecuteAsync(func, RetryOptions.Create());

        /// <summary>Execute a function, and retries (using default exponential values) if the Response is invalid.</summary>
        public static Task<Response> ExecuteExponentialAsync(Func<Task<Response>> func) => ExecuteAsync(func, RetryOptions.CreateExponential());

        /// <summary>Execute a function, and retries (using custom values) if the Response is invalid.</summary>
        public static async Task<Response> ExecuteAsync(Func<Task<Response>> func, RetryOptions options)
        {
            var response = await func();
            var retries = options.Retries;

            while (!response && retries-- > 0)
            {
                await Task.Delay(options.GetMillisecondsDelay(options.Retries - retries));
                response = await func();
            }

            return response;
        }

        #endregion

        #region T => Task<Response>

        /// <summary>Execute a function, and retries (using default values) if the Response is invalid.</summary>
        public static Task<Response> ExecuteAsync<T>(T arg, Func<T, Task<Response>> func) => ExecuteAsync(arg, func, RetryOptions.Create());

        /// <summary>Execute a function, and retries (using default exponential values) if the Response is invalid.</summary>
        public static Task<Response> ExecuteExponentialAsync<T>(T arg, Func<T, Task<Response>> func) => ExecuteAsync(arg, func, RetryOptions.CreateExponential());

        /// <summary>Execute a function, and retries (using custom values) if the Response is invalid.</summary>
        public static async Task<Response> ExecuteAsync<T>(T arg, Func<T, Task<Response>> func, RetryOptions options)
        {
            var response = await func(arg);
            var retries = options.Retries;

            while (!response && retries-- > 0)
            {
                await Task.Delay(options.GetMillisecondsDelay(options.Retries - retries));
                response = await func(arg);
            }

            return response;
        }

        #endregion

        #region () => Response<T>

        /// <summary>Execute a function, and retries (using default values) if the Response is invalid.</summary>
        public static Response<T> Execute<T>(Func<Response<T>> func) => Execute(func, RetryOptions.Create());

        /// <summary>Execute a function, and retries (using default exponential values) if the Response is invalid.</summary>
        public static Response<T> ExecuteExponential<T>(Func<Response<T>> func) => Execute(func, RetryOptions.CreateExponential());

        /// <summary>Execute a function, and retries (using custom values) if the Response is invalid.</summary>
        public static Response<T> Execute<T>(Func<Response<T>> func, RetryOptions options)
        {
            var response = func();
            var retries = options.Retries;

            while (!response && retries --> 0)
            {
                Thread.Sleep(options.GetMillisecondsDelay(options.Retries - retries));
                response = func();
            }

            return response;
        }

        #endregion

        #region T => Response<TResult>

        /// <summary>Execute a function, and retries (using default values) if the Response is invalid.</summary>
        public static Response<TResult> Execute<T, TResult>(T arg, Func<T, Response<TResult>> func) => Execute(arg, func, RetryOptions.Create());

        /// <summary>Execute a function, and retries (using default exponential values) if the Response is invalid.</summary>
        public static Response<TResult> ExecuteExponential<T, TResult>(T arg, Func<T, Response<TResult>> func) => Execute(arg, func, RetryOptions.CreateExponential());

        /// <summary>Execute a function, and retries (using custom values) if the Response is invalid.</summary>
        public static Response<TResult> Execute<T, TResult>(T arg, Func<T, Response<TResult>> func, RetryOptions options)
        {
            var response = func(arg);
            var retries = options.Retries;

            while (!response && retries --> 0)
            {
                Thread.Sleep(options.GetMillisecondsDelay(options.Retries - retries));
                response = func(arg);
            }

            return response;
        }

        #endregion

        #region () => Task<Response<T>>

        /// <summary>Execute a function, and retries (using default values) if the Response is invalid.</summary>
        public static Task<Response<T>> ExecuteAsync<T>(Func<Task<Response<T>>> func) => ExecuteAsync(func, RetryOptions.Create());

        /// <summary>Execute a function, and retries (using default exponential values) if the Response is invalid.</summary>
        public static Task<Response<T>> ExecuteExponentialAsync<T>(Func<Task<Response<T>>> func) => ExecuteAsync(func, RetryOptions.CreateExponential());

        /// <summary>Execute a function, and retries (using custom values) if the Response is invalid.</summary>
        public static async Task<Response<T>> ExecuteAsync<T>(Func<Task<Response<T>>> func, RetryOptions options)
        {
            var response = await func();
            var retries = options.Retries;

            while (!response && retries --> 0)
            {
                await Task.Delay(options.GetMillisecondsDelay(options.Retries - retries));
                response = await func();
            }

            return response;
        }

        #endregion

        #region T => Task<Response<TResult>>

        /// <summary>Execute a function, and retries (using default values) if the Response is invalid.</summary>
        public static Task<Response<TResult>> ExecuteAsync<T, TResult>(T arg, Func<T, Task<Response<TResult>>> func) => ExecuteAsync(arg, func, RetryOptions.Create());

        /// <summary>Execute a function, and retries (using default exponential values) if the Response is invalid.</summary>
        public static Task<Response<TResult>> ExecuteExponentialAsync<T, TResult>(T arg, Func<T, Task<Response<TResult>>> func) => ExecuteAsync(arg, func, RetryOptions.CreateExponential());

        /// <summary>Execute a function, and retries (using custom values) if the Response is invalid.</summary>
        public static async Task<Response<TResult>> ExecuteAsync<T, TResult>(T arg, Func<T, Task<Response<TResult>>> func, RetryOptions options)
        {
            var response = await func(arg);
            var retries = options.Retries;

            while (!response && retries --> 0)
            {
                await Task.Delay(options.GetMillisecondsDelay(options.Retries - retries));
                response = await func(arg);
            }

            return response;
        }

        #endregion

        #region () => ResponseAsync<T>

        /// <summary>Execute a function, and retries (using default values) if the Response is invalid.</summary>
        public static ResponseAsync<T> ExecuteAsync<T>(Func<ResponseAsync<T>> func) => ExecuteAsync(func, RetryOptions.Create());

        /// <summary>Execute a function, and retries (using default exponential values) if the Response is invalid.</summary>
        public static ResponseAsync<T> ExecuteExponentialAsync<T>(Func<ResponseAsync<T>> func) => ExecuteAsync(func, RetryOptions.CreateExponential());

        /// <summary>Execute a function, and retries (using custom values) if the Response is invalid.</summary>
        public static async ResponseAsync<T> ExecuteAsync<T>(Func<ResponseAsync<T>> func, RetryOptions options)
        {
            var response = await func();
            var retries = options.Retries;

            while (!response && retries --> 0)
            {
                await Task.Delay(options.GetMillisecondsDelay(options.Retries - retries));
                response = await func();
            }

            return response; // This is going to throw an "InvalidOperationException", as it casts to "ResponseAsync<T>"; if we did not get a valid "Response<T>" out of func.
        }

        #endregion

        #region T => ResponseAsync<TResult>

        /// <summary>Execute a function, and retries (using default values) if the Response is invalid.</summary>
        public static ResponseAsync<TResult> ExecuteAsync<T, TResult>(T arg, Func<T, ResponseAsync<TResult>> func) => ExecuteAsync(arg, func, RetryOptions.Create());

        /// <summary>Execute a function, and retries (using default exponential values) if the Response is invalid.</summary>
        public static ResponseAsync<TResult> ExecuteExponentialAsync<T, TResult>(T arg, Func<T, ResponseAsync<TResult>> func) => ExecuteAsync(arg, func, RetryOptions.CreateExponential());

        /// <summary>Execute a function, and retries (using custom values) if the Response is invalid.</summary>
        public static async ResponseAsync<TResult> ExecuteAsync<T, TResult>(T arg, Func<T, ResponseAsync<TResult>> func, RetryOptions options)
        {
            var response = await func(arg);
            var retries = options.Retries;

            while (!response && retries --> 0)
            {
                await Task.Delay(options.GetMillisecondsDelay(options.Retries - retries));
                response = await func(arg);
            }

            return response; // This is going to throw an "InvalidOperationException", as it casts to "ResponseAsync<T>"; if we did not get a valid "Response<T>" out of func.
        }

        #endregion
    }
}
