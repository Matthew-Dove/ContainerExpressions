using Repeat = ContainerExpressions.Containers.Retry;
using ContainerExpressions.Expressions.Models;
using System;
using System.Threading.Tasks;

namespace ContainerExpressions.Containers
{
    /// <summary>Utility methods for the Retry Container.</summary>
    public static class RetryExtensions
    {
        #region () => Response

        /// <summary>Execute a function, and retries (using default values) if the Response is invalid.</summary>
        public static Func<Response> Retry(this Func<Response> func) => () => Repeat.Execute(func, RetryOptions.Create());

        /// <summary>Execute a function, and retries (using exponential default values) if the Response is invalid.</summary>
        public static Func<Response> RetryExponential(this Func<Response> func) => () => Repeat.Execute(func, RetryOptions.CreateExponential());

        /// <summary>Execute a function, and retries (using custom values) if the Response is invalid.</summary>
        public static Func<Response> Retry(this Func<Response> func, RetryOptions options) => () => Repeat.Execute(func, options);

        #endregion

        #region T => Response

        /// <summary>Execute a function, and retries (using default values) if the Response is invalid.</summary>
        public static Func<T, Response> Retry<T>(this Func<T, Response> func) => x => Repeat.Execute(x, func, RetryOptions.Create());

        /// <summary>Execute a function, and retries (using default exponential values) if the Response is invalid.</summary>
        public static Func<T, Response> RetryExponential<T>(this Func<T, Response> func) => x => Repeat.Execute(x, func, RetryOptions.CreateExponential());

        /// <summary>Execute a function, and retries (using custom values) if the Response is invalid.</summary>
        public static Func<T, Response> Retry<T>(this Func<T, Response> func, RetryOptions options) => x => Repeat.Execute(x, func, options);

        #endregion

        #region () => Task<Response>

        /// <summary>Execute a function, and retries (using default values) if the Response is invalid.</summary>
        public static Func<Task<Response>> RetryAsync(this Func<Task<Response>> func) => () => Repeat.ExecuteAsync(func, RetryOptions.Create());

        /// <summary>Execute a function, and retries (using default exponential values) if the Response is invalid.</summary>
        public static Func<Task<Response>> RetryExponentialAsync(this Func<Task<Response>> func) => () => Repeat.ExecuteAsync(func, RetryOptions.CreateExponential());

        /// <summary>Execute a function, and retries (using custom values) if the Response is invalid.</summary>
        public static Func<Task<Response>> RetryAsync(this Func<Task<Response>> func, RetryOptions options) => () => Repeat.ExecuteAsync(func, options);

        #endregion

        #region T => Task<Response>

        /// <summary>Execute a function, and retries (using default values) if the Response is invalid.</summary>
        public static Func<T, Task<Response>> RetryAsync<T>(this Func<T, Task<Response>> func) => x => Repeat.ExecuteAsync(x, func, RetryOptions.Create());

        /// <summary>Execute a function, and retries (using default exponential values) if the Response is invalid.</summary>
        public static Func<T, Task<Response>> RetryExponentialAsync<T>(this Func<T, Task<Response>> func) => x => Repeat.ExecuteAsync(x, func, RetryOptions.CreateExponential());

        /// <summary>Execute a function, and retries (using custom values) if the Response is invalid.</summary>
        public static Func<T, Task<Response>> RetryAsync<T>(this Func<T, Task<Response>> func, RetryOptions options) => x => Repeat.ExecuteAsync(x, func, options);

        #endregion

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

        /** T1 **/

        /// <summary>Execute a function, and retries (using default values) if the Response is invalid.</summary>
        public static Func<T, Task<Response<TResult>>> RetryAsync<T, TResult>(this Func<T, Task<Response<TResult>>> func) => x => Repeat.ExecuteAsync(x, func, RetryOptions.Create());

        /// <summary>Execute a function, and retries (using default exponential values) if the Response is invalid.</summary>
        public static Func<T, Task<Response<TResult>>> RetryExponentialAsync<T, TResult>(this Func<T, Task<Response<TResult>>> func) => x => Repeat.ExecuteAsync(x, func, RetryOptions.CreateExponential());

        /// <summary>Execute a function, and retries (using custom values) if the Response is invalid.</summary>
        public static Func<T, Task<Response<TResult>>> RetryAsync<T, TResult>(this Func<T, Task<Response<TResult>>> func, RetryOptions options) => x => Repeat.ExecuteAsync(x, func, options);

        /** T2 **/

        /// <summary>Execute a function, and retries (using default values) if the Response is invalid.</summary>
        public static Func<T1, T2, Task<Response<TResult>>> RetryAsync<T1, T2, TResult>(this Func<T1, T2, Task<Response<TResult>>> func) => (x1, x2) => Repeat.ExecuteAsync(x1, x2, func, RetryOptions.Create());

        /// <summary>Execute a function, and retries (using default exponential values) if the Response is invalid.</summary>
        public static Func<T1, T2, Task<Response<TResult>>> RetryExponentialAsync<T1, T2, TResult>(this Func<T1, T2, Task<Response<TResult>>> func) => (x1, x2) => Repeat.ExecuteAsync(x1, x2, func, RetryOptions.CreateExponential());

        /// <summary>Execute a function, and retries (using custom values) if the Response is invalid.</summary>
        public static Func<T1, T2, Task<Response<TResult>>> RetryAsync<T1, T2, TResult>(this Func<T1, T2, Task<Response<TResult>>> func, RetryOptions options) => (x1, x2) => Repeat.ExecuteAsync(x1, x2, func, options);

        /** T3 **/

        /// <summary>Execute a function, and retries (using default values) if the Response is invalid.</summary>
        public static Func<T1, T2, T3, Task<Response<TResult>>> RetryAsync<T1, T2, T3, TResult>(this Func<T1, T2, T3, Task<Response<TResult>>> func) => (x1, x2, x3) => Repeat.ExecuteAsync(x1, x2, x3, func, RetryOptions.Create());

        /// <summary>Execute a function, and retries (using default exponential values) if the Response is invalid.</summary>
        public static Func<T1, T2, T3, Task<Response<TResult>>> RetryExponentialAsync<T1, T2, T3, TResult>(this Func<T1, T2, T3, Task<Response<TResult>>> func) => (x1, x2, x3) => Repeat.ExecuteAsync(x1, x2, x3, func, RetryOptions.CreateExponential());

        /// <summary>Execute a function, and retries (using custom values) if the Response is invalid.</summary>
        public static Func<T1, T2, T3, Task<Response<TResult>>> RetryAsync<T1, T2, T3, TResult>(this Func<T1, T2, T3, Task<Response<TResult>>> func, RetryOptions options) => (x1, x2, x3) => Repeat.ExecuteAsync(x1, x2, x3, func, options);

        /** T4 **/

        /// <summary>Execute a function, and retries (using default values) if the Response is invalid.</summary>
        public static Func<T1, T2, T3, T4, Task<Response<TResult>>> RetryAsync<T1, T2, T3, T4, TResult>(this Func<T1, T2, T3, T4, Task<Response<TResult>>> func) => (x1, x2, x3, x4) => Repeat.ExecuteAsync(x1, x2, x3, x4, func, RetryOptions.Create());

        /// <summary>Execute a function, and retries (using default exponential values) if the Response is invalid.</summary>
        public static Func<T1, T2, T3, T4, Task<Response<TResult>>> RetryExponentialAsync<T1, T2, T3, T4, TResult>(this Func<T1, T2, T3, T4, Task<Response<TResult>>> func) => (x1, x2, x3, x4) => Repeat.ExecuteAsync(x1, x2, x3, x4, func, RetryOptions.CreateExponential());

        /// <summary>Execute a function, and retries (using custom values) if the Response is invalid.</summary>
        public static Func<T1, T2, T3, T4, Task<Response<TResult>>> RetryAsync<T1, T2, T3, T4, TResult>(this Func<T1, T2, T3, T4, Task<Response<TResult>>> func, RetryOptions options) => (x1, x2, x3, x4) => Repeat.ExecuteAsync(x1, x2, x3, x4, func, options);

        /** T5 **/

        /// <summary>Execute a function, and retries (using default values) if the Response is invalid.</summary>
        public static Func<T1, T2, T3, T4, T5, Task<Response<TResult>>> RetryAsync<T1, T2, T3, T4, T5, TResult>(this Func<T1, T2, T3, T4, T5, Task<Response<TResult>>> func) => (x1, x2, x3, x4, x5) => Repeat.ExecuteAsync(x1, x2, x3, x4, x5, func, RetryOptions.Create());

        /// <summary>Execute a function, and retries (using default exponential values) if the Response is invalid.</summary>
        public static Func<T1, T2, T3, T4, T5, Task<Response<TResult>>> RetryExponentialAsync<T1, T2, T3, T4, T5, TResult>(this Func<T1, T2, T3, T4, T5, Task<Response<TResult>>> func) => (x1, x2, x3, x4, x5) => Repeat.ExecuteAsync(x1, x2, x3, x4, x5, func, RetryOptions.CreateExponential());

        /// <summary>Execute a function, and retries (using custom values) if the Response is invalid.</summary>
        public static Func<T1, T2, T3, T4, T5, Task<Response<TResult>>> RetryAsync<T1, T2, T3, T4, T5, TResult>(this Func<T1, T2, T3, T4, T5, Task<Response<TResult>>> func, RetryOptions options) => (x1, x2, x3, x4, x5) => Repeat.ExecuteAsync(x1, x2, x3, x4, x5, func, options);

        /** T6 **/

        /// <summary>Execute a function, and retries (using default values) if the Response is invalid.</summary>
        public static Func<T1, T2, T3, T4, T5, T6, Task<Response<TResult>>> RetryAsync<T1, T2, T3, T4, T5, T6, TResult>(this Func<T1, T2, T3, T4, T5, T6, Task<Response<TResult>>> func) => (x1, x2, x3, x4, x5, x6) => Repeat.ExecuteAsync(x1, x2, x3, x4, x5, x6, func, RetryOptions.Create());

        /// <summary>Execute a function, and retries (using default exponential values) if the Response is invalid.</summary>
        public static Func<T1, T2, T3, T4, T5, T6, Task<Response<TResult>>> RetryExponentialAsync<T1, T2, T3, T4, T5, T6, TResult>(this Func<T1, T2, T3, T4, T5, T6, Task<Response<TResult>>> func) => (x1, x2, x3, x4, x5, x6) => Repeat.ExecuteAsync(x1, x2, x3, x4, x5, x6, func, RetryOptions.CreateExponential());

        /// <summary>Execute a function, and retries (using custom values) if the Response is invalid.</summary>
        public static Func<T1, T2, T3, T4, T5, T6, Task<Response<TResult>>> RetryAsync<T1, T2, T3, T4, T5, T6, TResult>(this Func<T1, T2, T3, T4, T5, T6, Task<Response<TResult>>> func, RetryOptions options) => (x1, x2, x3, x4, x5, x6) => Repeat.ExecuteAsync(x1, x2, x3, x4, x5, x6, func, options);

        /** T7 **/

        /// <summary>Execute a function, and retries (using default values) if the Response is invalid.</summary>
        public static Func<T1, T2, T3, T4, T5, T6, T7, Task<Response<TResult>>> RetryAsync<T1, T2, T3, T4, T5, T6, T7, TResult>(this Func<T1, T2, T3, T4, T5, T6, T7, Task<Response<TResult>>> func) => (x1, x2, x3, x4, x5, x6, x7) => Repeat.ExecuteAsync(x1, x2, x3, x4, x5, x6, x7, func, RetryOptions.Create());

        /// <summary>Execute a function, and retries (using default exponential values) if the Response is invalid.</summary>
        public static Func<T1, T2, T3, T4, T5, T6, T7, Task<Response<TResult>>> RetryExponentialAsync<T1, T2, T3, T4, T5, T6, T7, TResult>(this Func<T1, T2, T3, T4, T5, T6, T7, Task<Response<TResult>>> func) => (x1, x2, x3, x4, x5, x6, x7) => Repeat.ExecuteAsync(x1, x2, x3, x4, x5, x6, x7, func, RetryOptions.CreateExponential());

        /// <summary>Execute a function, and retries (using custom values) if the Response is invalid.</summary>
        public static Func<T1, T2, T3, T4, T5, T6, T7, Task<Response<TResult>>> RetryAsync<T1, T2, T3, T4, T5, T6, T7, TResult>(this Func<T1, T2, T3, T4, T5, T6, T7, Task<Response<TResult>>> func, RetryOptions options) => (x1, x2, x3, x4, x5, x6, x7) => Repeat.ExecuteAsync(x1, x2, x3, x4, x5, x6, x7, func, options);

        /** T8 **/

        /// <summary>Execute a function, and retries (using default values) if the Response is invalid.</summary>
        public static Func<T1, T2, T3, T4, T5, T6, T7, T8, Task<Response<TResult>>> RetryAsync<T1, T2, T3, T4, T5, T6, T7, T8, TResult>(this Func<T1, T2, T3, T4, T5, T6, T7, T8, Task<Response<TResult>>> func) => (x1, x2, x3, x4, x5, x6, x7, x8) => Repeat.ExecuteAsync(x1, x2, x3, x4, x5, x6, x7, x8, func, RetryOptions.Create());

        /// <summary>Execute a function, and retries (using default exponential values) if the Response is invalid.</summary>
        public static Func<T1, T2, T3, T4, T5, T6, T7, T8, Task<Response<TResult>>> RetryExponentialAsync<T1, T2, T3, T4, T5, T6, T7, T8, TResult>(this Func<T1, T2, T3, T4, T5, T6, T7, T8, Task<Response<TResult>>> func) => (x1, x2, x3, x4, x5, x6, x7, x8) => Repeat.ExecuteAsync(x1, x2, x3, x4, x5, x6, x7, x8, func, RetryOptions.CreateExponential());

        /// <summary>Execute a function, and retries (using custom values) if the Response is invalid.</summary>
        public static Func<T1, T2, T3, T4, T5, T6, T7, T8, Task<Response<TResult>>> RetryAsync<T1, T2, T3, T4, T5, T6, T7, T8, TResult>(this Func<T1, T2, T3, T4, T5, T6, T7, T8, Task<Response<TResult>>> func, RetryOptions options) => (x1, x2, x3, x4, x5, x6, x7, x8) => Repeat.ExecuteAsync(x1, x2, x3, x4, x5, x6, x7, x8, func, options);

        #endregion

        #region () => ResponseAsync<T>

        /// <summary>Execute a function, and retries (using default values) if the Response is invalid.</summary>
        public static Func<ResponseAsync<T>> RetryAsync<T>(this Func<ResponseAsync<T>> func) => () => Repeat.ExecuteAsync(func, RetryOptions.Create());

        /// <summary>Execute a function, and retries (using default exponential values) if the Response is invalid.</summary>
        public static Func<ResponseAsync<T>> RetryExponentialAsync<T>(this Func<ResponseAsync<T>> func) => () => Repeat.ExecuteAsync(func, RetryOptions.CreateExponential());

        /// <summary>Execute a function, and retries (using custom values) if the Response is invalid.</summary>
        public static Func<ResponseAsync<T>> RetryAsync<T>(this Func<ResponseAsync<T>> func, RetryOptions options) => () => Repeat.ExecuteAsync(func, options);

        #endregion

        #region T => ResponseAsync<TResult>

        /** T1 **/

        /// <summary>Execute a function, and retries (using default values) if the Response is invalid.</summary>
        public static Func<T, ResponseAsync<TResult>> RetryAsync<T, TResult>(this Func<T, ResponseAsync<TResult>> func) => x => Repeat.ExecuteAsync(x, func, RetryOptions.Create());

        /// <summary>Execute a function, and retries (using default exponential values) if the Response is invalid.</summary>
        public static Func<T, ResponseAsync<TResult>> RetryExponentialAsync<T, TResult>(this Func<T, ResponseAsync<TResult>> func) => x => Repeat.ExecuteAsync(x, func, RetryOptions.CreateExponential());

        /// <summary>Execute a function, and retries (using custom values) if the Response is invalid.</summary>
        public static Func<T, ResponseAsync<TResult>> RetryAsync<T, TResult>(this Func<T, ResponseAsync<TResult>> func, RetryOptions options) => x => Repeat.ExecuteAsync(x, func, options);

        /** T2 **/

        /// <summary>Execute a function, and retries (using default values) if the Response is invalid.</summary>
        public static Func<T1, T2, ResponseAsync<TResult>> RetryAsync<T1, T2, TResult>(this Func<T1, T2, ResponseAsync<TResult>> func) => (x1, x2) => Repeat.ExecuteAsync(x1, x2, func, RetryOptions.Create());

        /// <summary>Execute a function, and retries (using default exponential values) if the Response is invalid.</summary>
        public static Func<T1, T2, ResponseAsync<TResult>> RetryExponentialAsync<T1, T2, TResult>(this Func<T1, T2, ResponseAsync<TResult>> func) => (x1, x2) => Repeat.ExecuteAsync(x1, x2,func, RetryOptions.CreateExponential());

        /// <summary>Execute a function, and retries (using custom values) if the Response is invalid.</summary>
        public static Func<T1, T2, ResponseAsync<TResult>> RetryAsync<T1, T2, TResult>(this Func<T1, T2, ResponseAsync<TResult>> func, RetryOptions options) => (x1, x2) => Repeat.ExecuteAsync(x1, x2, func, options);

        /** T3 **/

        /// <summary>Execute a function, and retries (using default values) if the Response is invalid.</summary>
        public static Func<T1, T2, T3, ResponseAsync<TResult>> RetryAsync<T1, T2, T3, TResult>(this Func<T1, T2, T3, ResponseAsync<TResult>> func) => (x1, x2, x3) => Repeat.ExecuteAsync(x1, x2, x3, func, RetryOptions.Create());

        /// <summary>Execute a function, and retries (using default exponential values) if the Response is invalid.</summary>
        public static Func<T1, T2, T3, ResponseAsync<TResult>> RetryExponentialAsync<T1, T2, T3, TResult>(this Func<T1, T2, T3, ResponseAsync<TResult>> func) => (x1, x2, x3) => Repeat.ExecuteAsync(x1, x2, x3, func, RetryOptions.CreateExponential());

        /// <summary>Execute a function, and retries (using custom values) if the Response is invalid.</summary>
        public static Func<T1, T2, T3, ResponseAsync<TResult>> RetryAsync<T1, T2, T3, TResult>(this Func<T1, T2, T3, ResponseAsync<TResult>> func, RetryOptions options) => (x1, x2, x3) => Repeat.ExecuteAsync(x1, x2, x3, func, options);

        /** T4 **/

        /// <summary>Execute a function, and retries (using default values) if the Response is invalid.</summary>
        public static Func<T1, T2, T3, T4, ResponseAsync<TResult>> RetryAsync<T1, T2, T3, T4, TResult>(this Func<T1, T2, T3, T4, ResponseAsync<TResult>> func) => (x1, x2, x3, x4) => Repeat.ExecuteAsync(x1, x2, x3, x4, func, RetryOptions.Create());

        /// <summary>Execute a function, and retries (using default exponential values) if the Response is invalid.</summary>
        public static Func<T1, T2, T3, T4, ResponseAsync<TResult>> RetryExponentialAsync<T1, T2, T3, T4, TResult>(this Func<T1, T2, T3, T4, ResponseAsync<TResult>> func) => (x1, x2, x3, x4) => Repeat.ExecuteAsync(x1, x2, x3, x4, func, RetryOptions.CreateExponential());

        /// <summary>Execute a function, and retries (using custom values) if the Response is invalid.</summary>
        public static Func<T1, T2, T3, T4, ResponseAsync<TResult>> RetryAsync<T1, T2, T3, T4, TResult>(this Func<T1, T2, T3, T4, ResponseAsync<TResult>> func, RetryOptions options) => (x1, x2, x3, x4) => Repeat.ExecuteAsync(x1, x2, x3, x4, func, options);

        /** T5 **/

        /// <summary>Execute a function, and retries (using default values) if the Response is invalid.</summary>
        public static Func<T1, T2, T3, T4, T5, ResponseAsync<TResult>> RetryAsync<T1, T2, T3, T4, T5, TResult>(this Func<T1, T2, T3, T4, T5, ResponseAsync<TResult>> func) => (x1, x2, x3, x4, x5) => Repeat.ExecuteAsync(x1, x2, x3, x4, x5, func, RetryOptions.Create());

        /// <summary>Execute a function, and retries (using default exponential values) if the Response is invalid.</summary>
        public static Func<T1, T2, T3, T4, T5, ResponseAsync<TResult>> RetryExponentialAsync<T1, T2, T3, T4, T5, TResult>(this Func<T1, T2, T3, T4, T5, ResponseAsync<TResult>> func) => (x1, x2, x3, x4, x5) => Repeat.ExecuteAsync(x1, x2, x3, x4, x5, func, RetryOptions.CreateExponential());

        /// <summary>Execute a function, and retries (using custom values) if the Response is invalid.</summary>
        public static Func<T1, T2, T3, T4, T5, ResponseAsync<TResult>> RetryAsync<T1, T2, T3, T4, T5, TResult>(this Func<T1, T2, T3, T4, T5, ResponseAsync<TResult>> func, RetryOptions options) => (x1, x2, x3, x4, x5) => Repeat.ExecuteAsync(x1, x2, x3, x4, x5, func, options);

        /** T6 **/

        /// <summary>Execute a function, and retries (using default values) if the Response is invalid.</summary>
        public static Func<T1, T2, T3, T4, T5, T6, ResponseAsync<TResult>> RetryAsync<T1, T2, T3, T4, T5, T6, TResult>(this Func<T1, T2, T3, T4, T5, T6, ResponseAsync<TResult>> func) => (x1, x2, x3, x4, x5, x6) => Repeat.ExecuteAsync(x1, x2, x3, x4, x5, x6, func, RetryOptions.Create());

        /// <summary>Execute a function, and retries (using default exponential values) if the Response is invalid.</summary>
        public static Func<T1, T2, T3, T4, T5, T6, ResponseAsync<TResult>> RetryExponentialAsync<T1, T2, T3, T4, T5, T6, TResult>(this Func<T1, T2, T3, T4, T5, T6, ResponseAsync<TResult>> func) => (x1, x2, x3, x4, x5, x6) => Repeat.ExecuteAsync(x1, x2, x3, x4, x5, x6, func, RetryOptions.CreateExponential());

        /// <summary>Execute a function, and retries (using custom values) if the Response is invalid.</summary>
        public static Func<T1, T2, T3, T4, T5, T6, ResponseAsync<TResult>> RetryAsync<T1, T2, T3, T4, T5, T6, TResult>(this Func<T1, T2, T3, T4, T5, T6, ResponseAsync<TResult>> func, RetryOptions options) => (x1, x2, x3, x4, x5, x6) => Repeat.ExecuteAsync(x1, x2, x3, x4, x5, x6, func, options);

        /** T7 **/

        /// <summary>Execute a function, and retries (using default values) if the Response is invalid.</summary>
        public static Func<T1, T2, T3, T4, T5, T6, T7, ResponseAsync<TResult>> RetryAsync<T1, T2, T3, T4, T5, T6, T7, TResult>(this Func<T1, T2, T3, T4, T5, T6, T7, ResponseAsync<TResult>> func) => (x1, x2, x3, x4, x5, x6, x7) => Repeat.ExecuteAsync(x1, x2, x3, x4, x5, x6, x7, func, RetryOptions.Create());

        /// <summary>Execute a function, and retries (using default exponential values) if the Response is invalid.</summary>
        public static Func<T1, T2, T3, T4, T5, T6, T7, ResponseAsync<TResult>> RetryExponentialAsync<T1, T2, T3, T4, T5, T6, T7, TResult>(this Func<T1, T2, T3, T4, T5, T6, T7, ResponseAsync<TResult>> func) => (x1, x2, x3, x4, x5, x6, x7) => Repeat.ExecuteAsync(x1, x2, x3, x4, x5, x6, x7, func, RetryOptions.CreateExponential());

        /// <summary>Execute a function, and retries (using custom values) if the Response is invalid.</summary>
        public static Func<T1, T2, T3, T4, T5, T6, T7, ResponseAsync<TResult>> RetryAsync<T1, T2, T3, T4, T5, T6, T7, TResult>(this Func<T1, T2, T3, T4, T5, T6, T7, ResponseAsync<TResult>> func, RetryOptions options) => (x1, x2, x3, x4, x5, x6, x7) => Repeat.ExecuteAsync(x1, x2, x3, x4, x5, x6, x7, func, options);

        /** T8 **/

        /// <summary>Execute a function, and retries (using default values) if the Response is invalid.</summary>
        public static Func<T1, T2, T3, T4, T5, T6, T7, T8, ResponseAsync<TResult>> RetryAsync<T1, T2, T3, T4, T5, T6, T7, T8, TResult>(this Func<T1, T2, T3, T4, T5, T6, T7, T8, ResponseAsync<TResult>> func) => (x1, x2, x3, x4, x5, x6, x7, x8) => Repeat.ExecuteAsync(x1, x2, x3, x4, x5, x6, x7, x8, func, RetryOptions.Create());

        /// <summary>Execute a function, and retries (using default exponential values) if the Response is invalid.</summary>
        public static Func<T1, T2, T3, T4, T5, T6, T7, T8, ResponseAsync<TResult>> RetryExponentialAsync<T1, T2, T3, T4, T5, T6, T7, T8, TResult>(this Func<T1, T2, T3, T4, T5, T6, T7, T8, ResponseAsync<TResult>> func) => (x1, x2, x3, x4, x5, x6, x7, x8) => Repeat.ExecuteAsync(x1, x2, x3, x4, x5, x6, x7, x8, func, RetryOptions.CreateExponential());

        /// <summary>Execute a function, and retries (using custom values) if the Response is invalid.</summary>
        public static Func<T1, T2, T3, T4, T5, T6, T7, T8, ResponseAsync<TResult>> RetryAsync<T1, T2, T3, T4, T5, T6, T7, T8, TResult>(this Func<T1, T2, T3, T4, T5, T6, T7, T8, ResponseAsync<TResult>> func, RetryOptions options) => (x1, x2, x3, x4, x5, x6, x7, x8) => Repeat.ExecuteAsync(x1, x2, x3, x4, x5, x6, x7, x8, func, options);

        #endregion
    }
}
