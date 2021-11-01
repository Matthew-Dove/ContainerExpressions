using System;

namespace ContainerExpressions.Containers
{
    /// <summary>Extensions for the Trace Container.</summary>
    public static class TraceExtensions
    {
        #region T

        /// <summary>Logs a trace step.</summary>
        /// <param name="message">The message to trace.</param>
        /// <returns>The initial value.</returns>
        public static T LogValue<T>(this T value, string message)
        {
            Trace.Log(message);
            return value;
        }

        /// <summary>Logs a trace step.</summary>
        /// <param name="format">The message to trace.</param>
        /// <returns>The initial value.</returns>
        public static T LogValue<T>(this T value, Func<T, string> format)
        {
            Trace.Log(format(value));
            return value;
        }

        #endregion

        #region Response

        /// <summary>Logs a trace step.</summary>
        /// <param name="response">The Response Container.</param>
        /// <param name="success">The message to trace if the response is in a valid state.</param>
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
        /// <returns>The same response as the input.</returns>
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
        /// <param name="success">The message to trace if the response is in a valid state.</param>
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
        /// <param name="success">The message to trace if the response is in a valid state.</param>
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
        /// <param name="success">The message to trace if the response is in a valid state.</param>
        /// <returns>The same response from the input.</returns>
        public static Func<Response<T>> Log<T>(this Func<Response<T>> func, string success) => () => func().Log(success);

        /// <summary>Logs a trace step.</summary>
        /// <param name="func">A function to the Response Container.</param>
        /// <param name="success">The message to trace if the response is in a valid state.</param>
        /// <param name="fail">The message to trace when the response is in an invalid state.</param>
        /// <returns>The same response from the input.</returns>
        public static Func<Response<T>> Log<T>(this Func<Response<T>> func, string success, string fail) => () => func().Log(success, fail);

        /// <summary>Logs a trace step.</summary>
        /// <param name="func">A function to the Response Container.</param>
        /// <param name="success">The message to trace if the response is in a valid state.</param>
        /// <returns>The same response from the input.</returns>
        public static Func<Response<T>> Log<T>(this Func<Response<T>> func, Func<T, string> success) => () => func().Log(success);

        /// <summary>Logs a trace step.</summary>
        /// <param name="func">A function to the Response Container.</param>
        /// <param name="success">The message to trace if the response is in a valid state.</param>
        /// <param name="fail">The message to trace when the response is in an invalid state.</param>
        /// <returns>The same response from the input.</returns>
        public static Func<Response<T>> Log<T>(this Func<Response<T>> func, Func<T, string> success, string fail) => () => func().Log(success, fail);

        #endregion

        #region ResponseTCompose WithOutput

        /// <summary>Logs a trace step.</summary>
        /// <param name="func">A function to the Response Container.</param>
        /// <param name="success">The message to trace if the response is in a valid state.</param>
        /// <returns>The same response from the input.</returns>
        public static Func<T, Response<TResult>> Log<T, TResult>(this Func<T, Response<TResult>> func, string success) => x => func(x).Log(success);

        /// <summary>Logs a trace step.</summary>
        /// <param name="func">A function to the Response Container.</param>
        /// <param name="success">The message to trace if the response is in a valid state.</param>
        /// <param name="fail">The message to trace when the response is in an invalid state.</param>
        /// <returns>The same response from the input.</returns>
        public static Func<T, Response<TResult>> Log<T, TResult>(this Func<T, Response<TResult>> func, string success, string fail) => x => func(x).Log(success, fail);

        /// <summary>Logs a trace step.</summary>
        /// <param name="func">A function to the Response Container.</param>
        /// <param name="success">The message to trace f the response is in a valid state.</param>
        /// <returns>The same response from the input.</returns>i
        public static Func<T, Response<TResult>> Log<T, TResult>(this Func<T, Response<TResult>> func, Func<TResult, string> success) => x => func(x).Log(success);

        /// <summary>Logs a trace step.</summary>
        /// <param name="func">A function to the Response Container.</param>
        /// <param name="success">The message to trace if the response is in a valid state.</param>
        /// <param name="fail">The message to trace when the response is in an invalid state.</param>
        /// <returns>The same response from the input.</returns>
        public static Func<T, Response<TResult>> Log<T, TResult>(this Func<T, Response<TResult>> func, Func<TResult, string> success, string fail) => x => func(x).Log(success, fail);

        #endregion

        #region ResponseTCompose WithInputAndOutput

        /// <summary>Logs a trace step.</summary>
        /// <param name="func">A function to the Response Container.</param>
        /// <param name="success">The message to trace if the response is in a valid state.</param>
        /// <returns>The same response from the input.</returns>
        public static Func<T, Response<TResult>> Log<T, TResult>(this Func<T, Response<TResult>> func, Func<T, TResult, string> success) => x => {
            var result = func(x);
            if (result)
            {
                Trace.Log(success(x, result));
            }
            return result;
        };

        /// <summary>Logs a trace step.</summary>
        /// <param name="func">A function to the Response Container.</param>
        /// <param name="success">The message to trace if the response is in a valid state.</param>
        /// <param name="fail">The message to trace when the response is in an invalid state.</param>
        /// <returns>The same response from the input.</returns>
        public static Func<T, Response<TResult>> Log<T, TResult>(this Func<T, Response<TResult>> func, Func<T, TResult, string> success, string fail) => x => {
            var result = func(x);
            if (result)
            {
                Trace.Log(success(x, result));
            }
            else
            {
                Trace.Log(fail);
            }
            return result;
        };

        #endregion

        #region Maybe

        /// <summary>Logs a trace step.</summary>
        /// <param name="value">A function that takes some TValue instance, and returns a message for tracing.</param>
        /// <param name="error">A function that takes some TError instance, and returns a message for tracing.</param>
        /// <returns>The same Maybe as the input.</returns>
        public static Maybe<TValue, TError> Log<TValue, TError>(this Maybe<TValue, TError> maybe, Func<TValue, string> value, Func<TError, string> error)
        {
            var message = maybe.Match(value, error);
            Trace.Log(message);
            return maybe;
        }

        /// <summary>Logs a trace step.</summary>
        /// <param name="value">A message to trace when Maybe contains some instance of TValue.</param>
        /// <param name="error">A message to trace when Maybe contains some instance of TError.</param>
        /// <returns>The same Maybe as the input.</returns>
        public static Maybe<TValue, TError> Log<TValue, TError>(this Maybe<TValue, TError> maybe, string value, string error)
        {
            var message = maybe._hasValue ? value : error;
            Trace.Log(message);
            return maybe;
        }

        /// <summary>Logs a trace step.</summary>
        /// <param name="value">A function that takes some TValue instance, and returns a message for tracing.</param>
        /// <param name="error">A function that takes some Exception instance, and returns a message for tracing.</param>
        /// <returns>The same Maybe as the input.</returns>
        public static Maybe<TValue> Log<TValue>(this Maybe<TValue> maybe, Func<TValue, string> value, Func<Exception, string> error)
        {
            var message = maybe.Match(value, error);
            Trace.Log(message);
            return maybe;
        }

        /// <summary>Logs a trace step.</summary>
        /// <param name="value">A message to trace when Maybe contains some instance of TValue.</param>
        /// <param name="error">A message to trace when Maybe contains some instance of Exception.</param>
        /// <returns>The same Maybe as the input.</returns>
        public static Maybe<TValue> Log<TValue>(this Maybe<TValue> maybe, string value, string error)
        {
            var message = maybe._hasValue ? value : error;
            Trace.Log(message);
            return maybe;
        }

        #endregion
    }
}
