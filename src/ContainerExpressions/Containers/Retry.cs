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

            while (!response && retries --> 0)
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

            while (!response && retries --> 0)
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

            while (!response && retries --> 0)
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

        /** T1 **/

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

        /** T2 **/

        /// <summary>Execute a function, and retries (using default values) if the Response is invalid.</summary>
        public static Task<Response<TResult>> ExecuteAsync<T1, T2, TResult>(T1 arg1, T2 arg2, Func<T1, T2, Task<Response<TResult>>> func) => ExecuteAsync(arg1, arg2, func, RetryOptions.Create());

        /// <summary>Execute a function, and retries (using default exponential values) if the Response is invalid.</summary>
        public static Task<Response<TResult>> ExecuteExponentialAsync<T1, T2, TResult>(T1 arg1, T2 arg2, Func<T1, T2, Task<Response<TResult>>> func) => ExecuteAsync(arg1, arg2, func, RetryOptions.CreateExponential());

        /// <summary>Execute a function, and retries (using custom values) if the Response is invalid.</summary>
        public static async Task<Response<TResult>> ExecuteAsync<T1, T2, TResult>(T1 arg1, T2 arg2, Func<T1, T2, Task<Response<TResult>>> func, RetryOptions options)
        {
            var response = await func(arg1, arg2);
            var retries = options.Retries;

            while (!response && retries --> 0)
            {
                await Task.Delay(options.GetMillisecondsDelay(options.Retries - retries));
                response = await func(arg1, arg2);
            }

            return response;
        }

        /** T3 **/

        /// <summary>Execute a function, and retries (using default values) if the Response is invalid.</summary>
        public static Task<Response<TResult>> ExecuteAsync<T1, T2, T3, TResult>(T1 arg1, T2 arg2, T3 arg3, Func<T1, T2, T3, Task<Response<TResult>>> func) => ExecuteAsync(arg1, arg2, arg3, func, RetryOptions.Create());

        /// <summary>Execute a function, and retries (using default exponential values) if the Response is invalid.</summary>
        public static Task<Response<TResult>> ExecuteExponentialAsync<T1, T2, T3, TResult>(T1 arg1, T2 arg2, T3 arg3, Func<T1, T2, T3, Task<Response<TResult>>> func) => ExecuteAsync(arg1, arg2, arg3, func, RetryOptions.CreateExponential());

        /// <summary>Execute a function, and retries (using custom values) if the Response is invalid.</summary>
        public static async Task<Response<TResult>> ExecuteAsync<T1, T2, T3, TResult>(T1 arg1, T2 arg2, T3 arg3, Func<T1, T2, T3, Task<Response<TResult>>> func, RetryOptions options)
        {
            var response = await func(arg1, arg2, arg3);
            var retries = options.Retries;

            while (!response && retries --> 0)
            {
                await Task.Delay(options.GetMillisecondsDelay(options.Retries - retries));
                response = await func(arg1, arg2, arg3);
            }

            return response;
        }

        /** T4 **/

        /// <summary>Execute a function, and retries (using default values) if the Response is invalid.</summary>
        public static Task<Response<TResult>> ExecuteAsync<T1, T2, T3, T4, TResult>(T1 arg1, T2 arg2, T3 arg3, T4 arg4, Func<T1, T2, T3, T4, Task<Response<TResult>>> func) => ExecuteAsync(arg1, arg2, arg3, arg4, func, RetryOptions.Create());

        /// <summary>Execute a function, and retries (using default exponential values) if the Response is invalid.</summary>
        public static Task<Response<TResult>> ExecuteExponentialAsync<T1, T2, T3, T4, TResult>(T1 arg1, T2 arg2, T3 arg3, T4 arg4, Func<T1, T2, T3, T4, Task<Response<TResult>>> func) => ExecuteAsync(arg1, arg2, arg3, arg4, func, RetryOptions.CreateExponential());

        /// <summary>Execute a function, and retries (using custom values) if the Response is invalid.</summary>
        public static async Task<Response<TResult>> ExecuteAsync<T1, T2, T3, T4, TResult>(T1 arg1, T2 arg2, T3 arg3, T4 arg4, Func<T1, T2, T3, T4, Task<Response<TResult>>> func, RetryOptions options)
        {
            var response = await func(arg1, arg2, arg3, arg4);
            var retries = options.Retries;

            while (!response && retries --> 0)
            {
                await Task.Delay(options.GetMillisecondsDelay(options.Retries - retries));
                response = await func(arg1, arg2, arg3, arg4);
            }

            return response;
        }

        /** T5 **/

        /// <summary>Execute a function, and retries (using default values) if the Response is invalid.</summary>
        public static Task<Response<TResult>> ExecuteAsync<T1, T2, T3, T4, T5, TResult>(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, Func<T1, T2, T3, T4, T5, Task<Response<TResult>>> func) => ExecuteAsync(arg1, arg2, arg3, arg4, arg5, func, RetryOptions.Create());

        /// <summary>Execute a function, and retries (using default exponential values) if the Response is invalid.</summary>
        public static Task<Response<TResult>> ExecuteExponentialAsync<T1, T2, T3, T4, T5, TResult>(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, Func<T1, T2, T3, T4, T5, Task<Response<TResult>>> func) => ExecuteAsync(arg1, arg2, arg3, arg4, arg5, func, RetryOptions.CreateExponential());

        /// <summary>Execute a function, and retries (using custom values) if the Response is invalid.</summary>
        public static async Task<Response<TResult>> ExecuteAsync<T1, T2, T3, T4, T5, TResult>(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, Func<T1, T2, T3, T4, T5, Task<Response<TResult>>> func, RetryOptions options)
        {
            var response = await func(arg1, arg2, arg3, arg4, arg5);
            var retries = options.Retries;

            while (!response && retries --> 0)
            {
                await Task.Delay(options.GetMillisecondsDelay(options.Retries - retries));
                response = await func(arg1, arg2, arg3, arg4, arg5);
            }

            return response;
        }

        /** T6 **/

        /// <summary>Execute a function, and retries (using default values) if the Response is invalid.</summary>
        public static Task<Response<TResult>> ExecuteAsync<T1, T2, T3, T4, T5, T6, TResult>(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, Func<T1, T2, T3, T4, T5, T6, Task<Response<TResult>>> func) => ExecuteAsync(arg1, arg2, arg3, arg4, arg5, arg6, func, RetryOptions.Create());

        /// <summary>Execute a function, and retries (using default exponential values) if the Response is invalid.</summary>
        public static Task<Response<TResult>> ExecuteExponentialAsync<T1, T2, T3, T4, T5, T6, TResult>(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, Func<T1, T2, T3, T4, T5, T6, Task<Response<TResult>>> func) => ExecuteAsync(arg1, arg2, arg3, arg4, arg5, arg6, func, RetryOptions.CreateExponential());

        /// <summary>Execute a function, and retries (using custom values) if the Response is invalid.</summary>
        public static async Task<Response<TResult>> ExecuteAsync<T1, T2, T3, T4, T5, T6, TResult>(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, Func<T1, T2, T3, T4, T5, T6, Task<Response<TResult>>> func, RetryOptions options)
        {
            var response = await func(arg1, arg2, arg3, arg4, arg5, arg6);
            var retries = options.Retries;

            while (!response && retries --> 0)
            {
                await Task.Delay(options.GetMillisecondsDelay(options.Retries - retries));
                response = await func(arg1, arg2, arg3, arg4, arg5, arg6);
            }

            return response;
        }

        /** T7 **/

        /// <summary>Execute a function, and retries (using default values) if the Response is invalid.</summary>
        public static Task<Response<TResult>> ExecuteAsync<T1, T2, T3, T4, T5, T6, T7, TResult>(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, Func<T1, T2, T3, T4, T5, T6, T7, Task<Response<TResult>>> func) => ExecuteAsync(arg1, arg2, arg3, arg4, arg5, arg6, arg7, func, RetryOptions.Create());

        /// <summary>Execute a function, and retries (using default exponential values) if the Response is invalid.</summary>
        public static Task<Response<TResult>> ExecuteExponentialAsync<T1, T2, T3, T4, T5, T6, T7, TResult>(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, Func<T1, T2, T3, T4, T5, T6, T7, Task<Response<TResult>>> func) => ExecuteAsync(arg1, arg2, arg3, arg4, arg5, arg6, arg7, func, RetryOptions.CreateExponential());

        /// <summary>Execute a function, and retries (using custom values) if the Response is invalid.</summary>
        public static async Task<Response<TResult>> ExecuteAsync<T1, T2, T3, T4, T5, T6, T7, TResult>(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, Func<T1, T2, T3, T4, T5, T6, T7, Task<Response<TResult>>> func, RetryOptions options)
        {
            var response = await func(arg1, arg2, arg3, arg4, arg5, arg6, arg7);
            var retries = options.Retries;

            while (!response && retries --> 0)
            {
                await Task.Delay(options.GetMillisecondsDelay(options.Retries - retries));
                response = await func(arg1, arg2, arg3, arg4, arg5, arg6, arg7);
            }

            return response;
        }

        /** T8 **/

        /// <summary>Execute a function, and retries (using default values) if the Response is invalid.</summary>
        public static Task<Response<TResult>> ExecuteAsync<T1, T2, T3, T4, T5, T6, T7, T8, TResult>(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, Func<T1, T2, T3, T4, T5, T6, T7, T8, Task<Response<TResult>>> func) => ExecuteAsync(arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, func, RetryOptions.Create());

        /// <summary>Execute a function, and retries (using default exponential values) if the Response is invalid.</summary>
        public static Task<Response<TResult>> ExecuteExponentialAsync<T1, T2, T3, T4, T5, T6, T7, T8, TResult>(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, Func<T1, T2, T3, T4, T5, T6, T7, T8, Task<Response<TResult>>> func) => ExecuteAsync(arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, func, RetryOptions.CreateExponential());

        /// <summary>Execute a function, and retries (using custom values) if the Response is invalid.</summary>
        public static async Task<Response<TResult>> ExecuteAsync<T1, T2, T3, T4, T5, T6, T7, T8, TResult>(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, Func<T1, T2, T3, T4, T5, T6, T7, T8, Task<Response<TResult>>> func, RetryOptions options)
        {
            var response = await func(arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8);
            var retries = options.Retries;

            while (!response && retries --> 0)
            {
                await Task.Delay(options.GetMillisecondsDelay(options.Retries - retries));
                response = await func(arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8);
            }

            return response;
        }

        #endregion

        #region () => ResponseAsync<T>

        /// <summary>Execute a function, and retries (using default values) if the Response is invalid.</summary>
        public static Task<Response<T>> ExecuteAsync<T>(Func<ResponseAsync<T>> func) => ExecuteAsync(func, RetryOptions.Create());

        /// <summary>Execute a function, and retries (using default exponential values) if the Response is invalid.</summary>
        public static Task<Response<T>> ExecuteExponentialAsync<T>(Func<ResponseAsync<T>> func) => ExecuteAsync(func, RetryOptions.CreateExponential());

        /// <summary>Execute a function, and retries (using custom values) if the Response is invalid.</summary>
        public static async Task<Response<T>> ExecuteAsync<T>(Func<ResponseAsync<T>> func, RetryOptions options)
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

        #region T => ResponseAsync<TResult>

        /** T1 **/

        /// <summary>Execute a function, and retries (using default values) if the Response is invalid.</summary>
        public static Task<Response<TResult>> ExecuteAsync<T, TResult>(T arg, Func<T, ResponseAsync<TResult>> func) => ExecuteAsync(arg, func, RetryOptions.Create());

        /// <summary>Execute a function, and retries (using default exponential values) if the Response is invalid.</summary>
        public static Task<Response<TResult>> ExecuteExponentialAsync<T, TResult>(T arg, Func<T, ResponseAsync<TResult>> func) => ExecuteAsync(arg, func, RetryOptions.CreateExponential());

        /// <summary>Execute a function, and retries (using custom values) if the Response is invalid.</summary>
        public static async Task<Response<TResult>> ExecuteAsync<T, TResult>(T arg, Func<T, ResponseAsync<TResult>> func, RetryOptions options)
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

        /** T2 **/

        /// <summary>Execute a function, and retries (using default values) if the Response is invalid.</summary>
        public static Task<Response<TResult>> ExecuteAsync<T1, T2, TResult>(T1 arg1, T2 arg2, Func<T1, T2, ResponseAsync<TResult>> func) => ExecuteAsync(arg1, arg2, func, RetryOptions.Create());

        /// <summary>Execute a function, and retries (using default exponential values) if the Response is invalid.</summary>
        public static Task<Response<TResult>> ExecuteExponentialAsync<T1, T2, TResult>(T1 arg1, T2 arg2, Func<T1, T2, ResponseAsync<TResult>> func) => ExecuteAsync(arg1, arg2, func, RetryOptions.CreateExponential());

        /// <summary>Execute a function, and retries (using custom values) if the Response is invalid.</summary>
        public static async Task<Response<TResult>> ExecuteAsync<T1, T2, TResult>(T1 arg1, T2 arg2, Func<T1, T2, ResponseAsync<TResult>> func, RetryOptions options)
        {
            var response = await func(arg1, arg2);
            var retries = options.Retries;

            while (!response && retries --> 0)
            {
                await Task.Delay(options.GetMillisecondsDelay(options.Retries - retries));
                response = await func(arg1, arg2);
            }

            return response;
        }

        /** T3 **/

        /// <summary>Execute a function, and retries (using default values) if the Response is invalid.</summary>
        public static Task<Response<TResult>> ExecuteAsync<T1, T2, T3, TResult>(T1 arg1, T2 arg2, T3 arg3, Func<T1, T2, T3, ResponseAsync<TResult>> func) => ExecuteAsync(arg1, arg2, arg3, func, RetryOptions.Create());

        /// <summary>Execute a function, and retries (using default exponential values) if the Response is invalid.</summary>
        public static Task<Response<TResult>> ExecuteExponentialAsync<T1, T2, T3, TResult>(T1 arg1, T2 arg2, T3 arg3, Func<T1, T2, T3, ResponseAsync<TResult>> func) => ExecuteAsync(arg1, arg2, arg3, func, RetryOptions.CreateExponential());

        /// <summary>Execute a function, and retries (using custom values) if the Response is invalid.</summary>
        public static async Task<Response<TResult>> ExecuteAsync<T1, T2, T3, TResult>(T1 arg1, T2 arg2, T3 arg3, Func<T1, T2, T3, ResponseAsync<TResult>> func, RetryOptions options)
        {
            var response = await func(arg1, arg2, arg3);
            var retries = options.Retries;

            while (!response && retries --> 0)
            {
                await Task.Delay(options.GetMillisecondsDelay(options.Retries - retries));
                response = await func(arg1, arg2, arg3);
            }

            return response;
        }

        /** T4 **/

        /// <summary>Execute a function, and retries (using default values) if the Response is invalid.</summary>
        public static Task<Response<TResult>> ExecuteAsync<T1, T2, T3, T4, TResult>(T1 arg1, T2 arg2, T3 arg3, T4 arg4, Func<T1, T2, T3, T4, ResponseAsync<TResult>> func) => ExecuteAsync(arg1, arg2, arg3, arg4, func, RetryOptions.Create());

        /// <summary>Execute a function, and retries (using default exponential values) if the Response is invalid.</summary>
        public static Task<Response<TResult>> ExecuteExponentialAsync<T1, T2, T3, T4, TResult>(T1 arg1, T2 arg2, T3 arg3, T4 arg4, Func<T1, T2, T3, T4, ResponseAsync<TResult>> func) => ExecuteAsync(arg1, arg2, arg3, arg4, func, RetryOptions.CreateExponential());

        /// <summary>Execute a function, and retries (using custom values) if the Response is invalid.</summary>
        public static async Task<Response<TResult>> ExecuteAsync<T1, T2, T3, T4, TResult>(T1 arg1, T2 arg2, T3 arg3, T4 arg4, Func<T1, T2, T3, T4, ResponseAsync<TResult>> func, RetryOptions options)
        {
            var response = await func(arg1, arg2, arg3, arg4);
            var retries = options.Retries;

            while (!response && retries --> 0)
            {
                await Task.Delay(options.GetMillisecondsDelay(options.Retries - retries));
                response = await func(arg1, arg2, arg3, arg4);
            }

            return response;
        }

        /** T5 **/

        /// <summary>Execute a function, and retries (using default values) if the Response is invalid.</summary>
        public static Task<Response<TResult>> ExecuteAsync<T1, T2, T3, T4, T5, TResult>(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, Func<T1, T2, T3, T4, T5, ResponseAsync<TResult>> func) => ExecuteAsync(arg1, arg2, arg3, arg4, arg5, func, RetryOptions.Create());

        /// <summary>Execute a function, and retries (using default exponential values) if the Response is invalid.</summary>
        public static Task<Response<TResult>> ExecuteExponentialAsync<T1, T2, T3, T4, T5, TResult>(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, Func<T1, T2, T3, T4, T5, ResponseAsync<TResult>> func) => ExecuteAsync(arg1, arg2, arg3, arg4, arg5, func, RetryOptions.CreateExponential());

        /// <summary>Execute a function, and retries (using custom values) if the Response is invalid.</summary>
        public static async Task<Response<TResult>> ExecuteAsync<T1, T2, T3, T4, T5, TResult>(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, Func<T1, T2, T3, T4, T5, ResponseAsync<TResult>> func, RetryOptions options)
        {
            var response = await func(arg1, arg2, arg3, arg4, arg5);
            var retries = options.Retries;

            while (!response && retries --> 0)
            {
                await Task.Delay(options.GetMillisecondsDelay(options.Retries - retries));
                response = await func(arg1, arg2, arg3, arg4, arg5);
            }

            return response;
        }

        /** T6 **/

        /// <summary>Execute a function, and retries (using default values) if the Response is invalid.</summary>
        public static Task<Response<TResult>> ExecuteAsync<T1, T2, T3, T4, T5, T6, TResult>(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, Func<T1, T2, T3, T4, T5, T6, ResponseAsync<TResult>> func) => ExecuteAsync(arg1, arg2, arg3, arg4, arg5, arg6, func, RetryOptions.Create());

        /// <summary>Execute a function, and retries (using default exponential values) if the Response is invalid.</summary>
        public static Task<Response<TResult>> ExecuteExponentialAsync<T1, T2, T3, T4, T5, T6, TResult>(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, Func<T1, T2, T3, T4, T5, T6, ResponseAsync<TResult>> func) => ExecuteAsync(arg1, arg2, arg3, arg4, arg5, arg6, func, RetryOptions.CreateExponential());

        /// <summary>Execute a function, and retries (using custom values) if the Response is invalid.</summary>
        public static async Task<Response<TResult>> ExecuteAsync<T1, T2, T3, T4, T5, T6, TResult>(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, Func<T1, T2, T3, T4, T5, T6, ResponseAsync<TResult>> func, RetryOptions options)
        {
            var response = await func(arg1, arg2, arg3, arg4, arg5, arg6);
            var retries = options.Retries;

            while (!response && retries --> 0)
            {
                await Task.Delay(options.GetMillisecondsDelay(options.Retries - retries));
                response = await func(arg1, arg2, arg3, arg4, arg5, arg6);
            }

            return response;
        }

        /** T7 **/

        /// <summary>Execute a function, and retries (using default values) if the Response is invalid.</summary>
        public static Task<Response<TResult>> ExecuteAsync<T1, T2, T3, T4, T5, T6, T7, TResult>(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, Func<T1, T2, T3, T4, T5, T6, T7, ResponseAsync<TResult>> func) => ExecuteAsync(arg1, arg2, arg3, arg4, arg5, arg6, arg7, func, RetryOptions.Create());

        /// <summary>Execute a function, and retries (using default exponential values) if the Response is invalid.</summary>
        public static Task<Response<TResult>> ExecuteExponentialAsync<T1, T2, T3, T4, T5, T6, T7, TResult>(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, Func<T1, T2, T3, T4, T5, T6, T7, ResponseAsync<TResult>> func) => ExecuteAsync(arg1, arg2, arg3, arg4, arg5, arg6, arg7, func, RetryOptions.CreateExponential());

        /// <summary>Execute a function, and retries (using custom values) if the Response is invalid.</summary>
        public static async Task<Response<TResult>> ExecuteAsync<T1, T2, T3, T4, T5, T6, T7, TResult>(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, Func<T1, T2, T3, T4, T5, T6, T7, ResponseAsync<TResult>> func, RetryOptions options)
        {
            var response = await func(arg1, arg2, arg3, arg4, arg5, arg6, arg7);
            var retries = options.Retries;

            while (!response && retries --> 0)
            {
                await Task.Delay(options.GetMillisecondsDelay(options.Retries - retries));
                response = await func(arg1, arg2, arg3, arg4, arg5, arg6, arg7);
            }

            return response;
        }

        /** T8 **/

        /// <summary>Execute a function, and retries (using default values) if the Response is invalid.</summary>
        public static Task<Response<TResult>> ExecuteAsync<T1, T2, T3, T4, T5, T6, T7, T8, TResult>(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, Func<T1, T2, T3, T4, T5, T6, T7, T8, ResponseAsync<TResult>> func) => ExecuteAsync(arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, func, RetryOptions.Create());

        /// <summary>Execute a function, and retries (using default exponential values) if the Response is invalid.</summary>
        public static Task<Response<TResult>> ExecuteExponentialAsync<T1, T2, T3, T4, T5, T6, T7, T8, TResult>(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, Func<T1, T2, T3, T4, T5, T6, T7, T8, ResponseAsync<TResult>> func) => ExecuteAsync(arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, func, RetryOptions.CreateExponential());

        /// <summary>Execute a function, and retries (using custom values) if the Response is invalid.</summary>
        public static async Task<Response<TResult>> ExecuteAsync<T1, T2, T3, T4, T5, T6, T7, T8, TResult>(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, Func<T1, T2, T3, T4, T5, T6, T7, T8, ResponseAsync<TResult>> func, RetryOptions options)
        {
            var response = await func(arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8);
            var retries = options.Retries;

            while (!response && retries --> 0)
            {
                await Task.Delay(options.GetMillisecondsDelay(options.Retries - retries));
                response = await func(arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8);
            }

            return response;
        }

        #endregion
    }
}
