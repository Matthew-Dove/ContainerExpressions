using ContainerExpressions.Expressions.Models;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace ContainerExpressions.Containers
{
    /// <summary>Keep retrying a function until is succeeds, or the number of allowed retries is exceeded.</summary>
    public static class Retry
    {
        /// <summary>Execute a function, and retries (using default values) if the Response is invalid.</summary>
        public static Response Execute(Func<Response> func) => Execute(func, RetryOptions.Create());

        /// <summary>Execute a function, and retries (using custom values) if the Response is invalid.</summary>
        public static Response Execute(Func<Response> func, RetryOptions options)
        {
            var response = func();
            var retries = options.Retries;

            while (!response.IsValid && retries --> 0)
            {
                Thread.Sleep(options.MillisecondsDelay);
                response = func();
            }

            return response; 
        }

        /// <summary>Execute a function, and retries (using default values) if the Response is invalid.</summary>
        public static Response<T> Execute<T>(Func<Response<T>> func) => Execute(func, RetryOptions.Create());

        /// <summary>Execute a function, and retries (using custom values) if the Response is invalid.</summary>
        public static Response<T> Execute<T>(Func<Response<T>> func, RetryOptions options)
        {
            var response = func();
            var retries = options.Retries;

            while (!response.IsValid && retries --> 0)
            {
                Thread.Sleep(options.MillisecondsDelay);
                response = func();
            }

            return response;
        }

        /// <summary>Execute a function, and retries (using default values) if the Response is invalid.</summary>
        public static Response<TResult> Execute<T, TResult>(T arg, Func<T, Response<TResult>> func) => Execute(arg, func, RetryOptions.Create());

        /// <summary>Execute a function, and retries (using custom values) if the Response is invalid.</summary>
        public static Response<TResult> Execute<T, TResult>(T arg, Func<T, Response<TResult>> func, RetryOptions options)
        {
            var response = func(arg);
            var retries = options.Retries;

            while (!response.IsValid && retries-- > 0)
            {
                Thread.Sleep(options.MillisecondsDelay);
                response = func(arg);
            }

            return response;
        }

        /// <summary>Execute a function, and retries (using default values) if the Response is invalid.</summary>
        public static Task<Response> ExecuteAsync(Func<Task<Response>> func) => ExecuteAsync(func, RetryOptions.Create());

        /// <summary>Execute a function, and retries (using custom values) if the Response is invalid.</summary>
        public static async Task<Response> ExecuteAsync(Func<Task<Response>> func, RetryOptions options)
        {
            var response = await func();
            var retries = options.Retries;

            while (!response.IsValid && retries --> 0)
            {
                await Task.Delay(options.MillisecondsDelay);
                response = await func();
            }

            return response;
        }

        /// <summary>Execute a function, and retries (using default values) if the Response is invalid.</summary>
        public static Task<Response<T>> ExecuteAsync<T>(Func<Task<Response<T>>> func) => ExecuteAsync(func, RetryOptions.Create());

        /// <summary>Execute a function, and retries (using custom values) if the Response is invalid.</summary>
        public static async Task<Response<T>> ExecuteAsync<T>(Func<Task<Response<T>>> func, RetryOptions options)
        {
            var response = await func();
            var retries = options.Retries;

            while (!response.IsValid && retries --> 0)
            {
                await Task.Delay(options.MillisecondsDelay);
                response = await func();
            }

            return response;
        }
    }
}
