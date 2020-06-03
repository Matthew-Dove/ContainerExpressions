using System;
using System.Threading.Tasks;

namespace ContainerExpressions.Containers
{
    /// <summary>Extensions for the Trace Container.</summary>
    public static class TraceAsyncExtensions
    {
        #region T

        /// <summary>Logs a trace step.</summary>
        /// <param name="message">The message to trace.</param>
        /// <returns>The initial value.</returns>
        public static Task<T> LogAsync<T>(this Task<T> value, string message) => value.ContinueWith(x => { Trace.Log(message); return x.Result; });

        /// <summary>Logs a trace step.</summary>
        /// <param name="format">The message to trace.</param>
        /// <returns>The initial value.</returns>
        public static Task<T> LogAsync<T>(this Task<T> value, Func<T, string> format) => value.ContinueWith(x => { Trace.Log(format(x.Result)); return x.Result; });

        #endregion

        #region Response

        /// <summary>Logs a trace step.</summary>
        /// <param name="response">The Response Container.</param>
        /// <param name="success">The message to trace if the response is in a valid state.</param>
        /// <returns>The same response from the input.</returns>
        public static Task<Response> LogAsync(this Task<Response> response, string success)
        {
            return response.ContinueWith(x =>
            {
                if (x.Result.IsValid)
                {
                    Trace.Log(success);
                }

                return x.Result;
            });
        }

        /// <summary>Logs a trace step.</summary>
        /// <param name="response">The Response Container.</param>
        /// <param name="success">The message to trace when the response is in a valid state.</param>
        /// <param name="fail">The message to trace when the response is in an invalid state.</param>
        /// <returns>The same response from the input.</returns>
        public static Task<Response> LogAsync(this Task<Response> response, string success, string fail)
        {
            return response.ContinueWith(x =>
            {
                if (x.Result.IsValid)
                {
                    Trace.Log(success);
                }
                else
                {
                    Trace.Log(fail);
                }

                return x.Result;
            });
        }

        #endregion

        #region ResponseT

        /// <summary>Logs a trace step.</summary>
        /// <param name="response">The Response Container.</param>
        /// <param name="success">The message to trace if the response is in a valid state.</param>
        /// <returns>The same response from the input.</returns>
        public static Task<Response<T>> LogAsync<T>(this Task<Response<T>> response, string success)
        {
            return response.ContinueWith(x =>
            {
                if (x.Result.IsValid)
                {
                    Trace.Log(success);
                }

                return x.Result;
            });
        }

        /// <summary>Logs a trace step.</summary>
        /// <param name="response">The Response Container.</param>
        /// <param name="success">The message to trace if the response is in a valid state.</param>
        /// <returns>The same response from the input.</returns>
        public static Task<Response<T>> LogAsync<T>(this Task<Response<T>> response, Func<T, string> success)
        {
            return response.ContinueWith(x =>
            {
                if (x.Result.IsValid)
                {
                    Trace.Log(success(x.Result.Value));
                }

                return x.Result;
            });
        }

        /// <summary>Logs a trace step.</summary>
        /// <param name="response">The Response Container.</param>
        /// <param name="success">The message to trace when the response is in a valid state.</param>
        /// <param name="fail">The message to trace when the response is in an invalid state.</param>
        /// <returns>The same response from the input.</returns>
        public static Task<Response<T>> LogAsync<T>(this Task<Response<T>> response, Func<T, string> success, string fail)
        {
            return response.ContinueWith(x =>
            {
                if (x.Result.IsValid)
                {
                    Trace.Log(success(x.Result.Value));
                }
                else
                {
                    Trace.Log(fail);
                }

                return x.Result;
            });
        }

        /// <summary>Logs a trace step.</summary>
        /// <param name="response">The Response Container.</param>
        /// <param name="success">The message to trace when the response is in a valid state.</param>
        /// <param name="fail">The message to trace when the response is in an invalid state.</param>
        /// <returns>The same response from the input.</returns>
        public static Task<Response<T>> LogAsync<T>(this Task<Response<T>> response, string success, string fail)
        {
            return response.ContinueWith(x =>
            {
                if (x.Result.IsValid)
                {
                    Trace.Log(success);
                }
                else
                {
                    Trace.Log(fail);
                }

                return x.Result;
            });
        }

        #endregion

        #region ResponseTCompose NoInput

        /// <summary>Logs a trace step.</summary>
        /// <param name="func">A function to the Response Container.</param>
        /// <param name="success">The message to trace if the response is in a valid state.</param>
        /// <returns>The same response from the input.</returns>
        public static Func<Task<Response<T>>> LogAsync<T>(this Func<Task<Response<T>>> func, string success) => () => func().ContinueWith(x => x.Result.Log(success));

        /// <summary>Logs a trace step.</summary>
        /// <param name="func">A function to the Response Container.</param>
        /// <param name="success">The message to trace if the response is in a valid state.</param>
        /// <param name="fail">The message to trace when the response is in an invalid state.</param>
        /// <returns>The same response from the input.</returns>
        public static Func<Task<Response<T>>> LogAsync<T>(this Func<Task<Response<T>>> func, string success, string fail) => () => func().ContinueWith(x => x.Result.Log(success, fail));

        /// <summary>Logs a trace step.</summary>
        /// <param name="func">A function to the Response Container.</param>
        /// <param name="success">The message to trace if the response is in a valid state.</param>
        /// <returns>The same response from the input.</returns>
        public static Func<Task<Response<T>>> LogAsync<T>(this Func<Task<Response<T>>> func, Func<T, string> success) => () => func().ContinueWith(x => x.Result.Log(success));

        /// <summary>Logs a trace step.</summary>
        /// <param name="func">A function to the Response Container.</param>
        /// <param name="success">The message to trace if the response is in a valid state.</param>
        /// <param name="fail">The message to trace when the response is in an invalid state.</param>
        /// <returns>The same response from the input.</returns>
        public static Func<Task<Response<T>>> LogAsync<T>(this Func<Task<Response<T>>> func, Func<T, string> success, string fail) => () => func().ContinueWith(x => x.Result.Log(success, fail));

        #endregion

        #region ResponseTCompose WithOutput

        /// <summary>Logs a trace step.</summary>
        /// <param name="func">A function to the Response Container.</param>
        /// <param name="success">The message to trace if the response is in a valid state.</param>
        /// <returns>The same response from the input.</returns>
        public static Func<T, Task<Response<TResult>>> LogAsync<T, TResult>(this Func<T, Task<Response<TResult>>> func, string success) => x => func(x).ContinueWith(y => y.Result.Log(success));

        /// <summary>Logs a trace step.</summary>
        /// <param name="func">A function to the Response Container.</param>
        /// <param name="success">The message to trace if the response is in a valid state.</param>
        /// <param name="fail">The message to trace when the response is in an invalid state.</param>
        /// <returns>The same response from the input.</returns>
        public static Func<T, Task<Response<TResult>>> LogAsync<T, TResult>(this Func<T, Task<Response<TResult>>> func, string success, string fail) => x => func(x).ContinueWith(y => y.Result.Log(success, fail));

        /// <summary>Logs a trace step.</summary>
        /// <param name="func">A function to the Response Container.</param>
        /// <param name="success">The message to trace if the response is in a valid state.</param>
        /// <returns>The same response from the input.</returns>
        public static Func<T, Task<Response<TResult>>> LogAsync<T, TResult>(this Func<T, Task<Response<TResult>>> func, Func<TResult, string> success) => x => func(x).ContinueWith(y => y.Result.Log(success));

        /// <summary>Logs a trace step.</summary>
        /// <param name="func">A function to the Response Container.</param>
        /// <param name="success">The message to trace if the response is in a valid state.</param>
        /// <param name="fail">The message to trace when the response is in an invalid state.</param>
        /// <returns>The same response from the input.</returns>
        public static Func<T, Task<Response<TResult>>> LogAsync<T, TResult>(this Func<T, Task<Response<TResult>>> func, Func<TResult, string> success, string fail) => x => func(x).ContinueWith(y => y.Result.Log(success, fail));

        #endregion

        #region ResponseTCompose WithInputAndOutput

        /// <summary>Logs a trace step.</summary>
        /// <param name="func">A function to the Response Container.</param>
        /// <param name="success">The message to trace if the response is in a valid state.</param>
        /// <returns>The same response from the input.</returns>
        public static Func<T, Task<Response<TResult>>> LogAsync<T, TResult>(this Func<T, Task<Response<TResult>>> func, Func<T, TResult, string> success) => x =>
            func(x).ContinueWith(y =>
            {
                if (y.Result)
                {
                    Trace.Log(success(x, y.Result));
                }

                return y.Result;
            });

        /// <summary>Logs a trace step.</summary>
        /// <param name="func">A function to the Response Container.</param>
        /// <param name="success">The message to trace if the response is in a valid state.</param>
        /// <param name="fail">The message to trace when the response is in an invalid state.</param>
        /// <returns>The same response from the input.</returns>
        public static Func<T, Task<Response<TResult>>> LogAsync<T, TResult>(this Func<T, Task<Response<TResult>>> func, Func<T, TResult, string> success, string fail) => x =>
            func(x).ContinueWith(y =>
            {
                if (y.Result)
                {
                    Trace.Log(success(x, y.Result));
                }
                else
                {
                    Trace.Log(fail);
                }

                return y.Result;
            });

        #endregion
    }
}
