﻿using System;

namespace ContainerExpressions.Containers
{
    /// <summary>Extension for the Trace Container.</summary>
    public static class TraceExtensions
    {
        #region Response

        /// <summary>Logs a trace step.</summary>
        /// <param name="response">The Response Container.</param>
        /// <param name="success">The message to trace of the response is in a valid state.</param>
        /// <returns>The same response from the input.</returns>
        public static Response Log(this Response response, string success)
        {
            if (response.IsValid)
            {
                Trace.Log(success);
            }

            return response;
        }

        /// <summary>Logs a trace step.</summary>
        /// <param name="response">The Response Container.</param>
        /// <param name="success">The message to trace when the response is in a valid state.</param>
        /// <param name="fail">The message to trace when the response is in an invalid state.</param>
        /// <returns>The same response from the input.</returns>
        public static Response Log(this Response response, string success, string fail)
        {
            if (response.IsValid)
            {
                Trace.Log(success);
            }
            else
            {
                Trace.Log(fail);
            }

            return response;
        }

        #endregion

        #region ResponseT

        /// <summary>Logs a trace step.</summary>
        /// <param name="response">The Response Container.</param>
        /// <param name="success">The message to trace of the response is in a valid state.</param>
        /// <returns>The same response from the input.</returns>
        public static Response<T> Log<T>(this Response<T> response, string success)
        {
            if (response.IsValid)
            {
                Trace.Log(success);
            }

            return response;
        }

        /// <summary>Logs a trace step.</summary>
        /// <param name="response">The Response Container.</param>
        /// <param name="success">The message to trace of the response is in a valid state.</param>
        /// <returns>The same response from the input.</returns>
        public static Response<T> Log<T>(this Response<T> response, Func<T, string> success)
        {
            if (response.IsValid)
            {
                Trace.Log(success(response.Value));
            }

            return response;
        }

        /// <summary>Logs a trace step.</summary>
        /// <param name="response">The Response Container.</param>
        /// <param name="success">The message to trace when the response is in a valid state.</param>
        /// <param name="fail">The message to trace when the response is in an invalid state.</param>
        /// <returns>The same response from the input.</returns>
        public static Response<T> Log<T>(this Response<T> response, Func<T, string> success, string fail)
        {
            if (response.IsValid)
            {
                Trace.Log(success(response.Value));
            }
            else
            {
                Trace.Log(fail);
            }

            return response;
        }

        /// <summary>Logs a trace step.</summary>
        /// <param name="response">The Response Container.</param>
        /// <param name="success">The message to trace when the response is in a valid state.</param>
        /// <param name="fail">The message to trace when the response is in an invalid state.</param>
        /// <returns>The same response from the input.</returns>
        public static Response<T> Log<T>(this Response<T> response, string success, string fail)
        {
            if (response.IsValid)
            {
                Trace.Log(success);
            }
            else
            {
                Trace.Log(fail);
            }

            return response;
        }

        #endregion

        #region ResponseTCompose NoInput

        /// <summary>Logs a trace step.</summary>
        /// <param name="func">A function to the Response Container.</param>
        /// <param name="success">The message to trace of the response is in a valid state.</param>
        /// <returns>The same response from the input.</returns>
        public static Func<Response<T>> Log<T>(this Func<Response<T>> func, string success) => () => func().Log(success);

        /// <summary>Logs a trace step.</summary>
        /// <param name="func">A function to the Response Container.</param>
        /// <param name="success">The message to trace of the response is in a valid state.</param>
        /// <param name="fail">The message to trace when the response is in an invalid state.</param>
        /// <returns>The same response from the input.</returns>
        public static Func<Response<T>> Log<T>(this Func<Response<T>> func, string success, string fail) => () => func().Log(success, fail);

        /// <summary>Logs a trace step.</summary>
        /// <param name="func">A function to the Response Container.</param>
        /// <param name="success">The message to trace of the response is in a valid state.</param>
        /// <returns>The same response from the input.</returns>
        public static Func<Response<T>> Log<T>(this Func<Response<T>> func, Func<T, string> success) => () => func().Log(success);

        /// <summary>Logs a trace step.</summary>
        /// <param name="func">A function to the Response Container.</param>
        /// <param name="success">The message to trace of the response is in a valid state.</param>
        /// <param name="fail">The message to trace when the response is in an invalid state.</param>
        /// <returns>The same response from the input.</returns>
        public static Func<Response<T>> Log<T>(this Func<Response<T>> func, Func<T, string> success, string fail) => () => func().Log(success, fail);

        #endregion

        #region ResponseTCompose WithInput

        /// <summary>Logs a trace step.</summary>
        /// <param name="func">A function to the Response Container.</param>
        /// <param name="success">The message to trace of the response is in a valid state.</param>
        /// <returns>The same response from the input.</returns>
        public static Func<T, Response<TResult>> Log<T, TResult>(this Func<T, Response<TResult>> func, string success) => x => func(x).Log(success);

        /// <summary>Logs a trace step.</summary>
        /// <param name="func">A function to the Response Container.</param>
        /// <param name="success">The message to trace of the response is in a valid state.</param>
        /// <param name="fail">The message to trace when the response is in an invalid state.</param>
        /// <returns>The same response from the input.</returns>
        public static Func<T, Response<TResult>> Log<T, TResult>(this Func<T, Response<TResult>> func, string success, string fail) => x => func(x).Log(success, fail);

        /// <summary>Logs a trace step.</summary>
        /// <param name="func">A function to the Response Container.</param>
        /// <param name="success">The message to trace of the response is in a valid state.</param>
        /// <returns>The same response from the input.</returns>
        public static Func<T, Response<TResult>> Log<T, TResult>(this Func<T, Response<TResult>> func, Func<TResult, string> success) => x => func(x).Log(success);

        /// <summary>Logs a trace step.</summary>
        /// <param name="func">A function to the Response Container.</param>
        /// <param name="success">The message to trace of the response is in a valid state.</param>
        /// <param name="fail">The message to trace when the response is in an invalid state.</param>
        /// <returns>The same response from the input.</returns>
        public static Func<T, Response<TResult>> Log<T, TResult>(this Func<T, Response<TResult>> func, Func<TResult, string> success, string fail) => x => func(x).Log(success, fail);

        #endregion

    }
}
