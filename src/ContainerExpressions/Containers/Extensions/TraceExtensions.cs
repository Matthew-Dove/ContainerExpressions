﻿using ContainerExpressions.Expressions.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

namespace ContainerExpressions.Containers
{
    /// <summary>Extensions for the Trace Container.</summary>
    public static class TraceExtensions
    {
        #region Utilities

        /// <summary>Logs an exception, will check for aggregate exceptions, and log them out individually.</summary>
        internal static void LogException(
            Exception ex,
            Format message,
            string argument = "",
            string caller = "",
            string path = "",
            int line = 0
            )
        {
            if (ex == null) return;
            var logger = ExceptionLogger.Create(message, argument, caller, path, line);
            if (ex is AggregateException ae) { LogAggregatedExceptions(logger, ae, message, argument, caller, path, line); }
            else { logger.Log(ex); LogException(ex.InnerException, message, argument, caller, path, line); }
        }

        /// <summary>Recursively walks through an AggregateException, and logs out all the "real" exceptions.</summary>
        private static void LogAggregatedExceptions(ExceptionLogger logger, AggregateException ae, Format message, string argument, string caller, string path, int line)
        {
            if (ae == null) return;
            foreach (var e in ae.Flatten().InnerExceptions)
            {
                if (e is AggregateException ex) { LogAggregatedExceptions(logger, ex, message, argument, caller, path, line); } // Flattened exceptions can still contain aggregate exceptions themselves.
                else { logger.Log(e); LogException(e.InnerException, message, argument, caller, path, line); }
            }
        }

        #endregion

        #region TError

        /// <summary>Logs a custom error type.</summary>
        /// <param name="error">The custom error type to log (i.e. not an exception).</param>
        /// <param name="message">The message to trace.</param>
        /// <returns>The initial error.</returns>
        public static TError LogErrorValue<TError>(
            this TError error,
            Format message = default,
            [CallerArgumentExpression(nameof(error))] string argument = "",
            [CallerMemberName] string caller = "",
            [CallerFilePath] string path = "",
            [CallerLineNumber] int line = 0
            )
        {
            if (error == null) return default;

            Exception ex;
            if (error is Exception e) ex = e;
            else ex = new GenericErrorException<TError>(error, message);
            LogException(ex, message, argument, caller, path, line);

            return error;
        }

        /// <summary>Logs custom error types.</summary>
        /// <param name="error">The custom error types to log (i.e. not exceptions).</param>
        /// <param name="message">The message to trace (only once).</param>
        /// <returns>The errors originally provided.</returns>
        public static IEnumerable<TError> LogErrorValue<TError>(
            this IEnumerable<TError> error,
            Format message = default,
            [CallerArgumentExpression(nameof(error))] string argument = "",
            [CallerMemberName] string caller = "",
            [CallerFilePath] string path = "",
            [CallerLineNumber] int line = 0
            )
        {
            if (error == null) return default;

            foreach (var err in error)
            {
                if (err == null) continue;
                Exception ex;
                if (error is Exception e) ex = e;
                else ex = new GenericErrorException<TError>(err, message);
                LogException(ex, message, argument, caller, path, line);
            }

            return error;
        }

        /// <summary>Logs custom error types.</summary>
        /// <param name="error">The custom error types to log (i.e. not exceptions).</param>
        /// <param name="message">The message to trace (only once).</param>
        /// <returns>The errors originally provided.</returns>
        public static TError[] LogErrorValue<TError>(
            this TError[] error,
            Format message = default,
            [CallerArgumentExpression(nameof(error))] string argument = "",
            [CallerMemberName] string caller = "",
            [CallerFilePath] string path = "",
            [CallerLineNumber] int line = 0
            )
        {
            if (error == null) return default;

            for (int i = 0; i < error.Length; i++)
            {
                if (error[i] == null) continue;
                Exception ex;
                if (error is Exception e) ex = e;
                else ex = new GenericErrorException<TError>(error[i], message);
                LogException(ex, message, argument, caller, path, line);
            }

            return error;
        }

        /// <summary>Logs a custom error type.</summary>
        /// <param name="error">The custom error type to log (i.e. not an exception).</param>
        /// <param name="message">The message to trace.</param>
        /// <returns>The initial error.</returns>
        public static TError LogErrorValue<TError>(
            this TError error,
            Func<TError, Format> message,
            [CallerArgumentExpression(nameof(error))] string argument = "",
            [CallerMemberName] string caller = "",
            [CallerFilePath] string path = "",
            [CallerLineNumber] int line = 0
            )
        {
            if (error == null) return default;

            var msg = message(error);
            Exception ex;
            if (error is Exception e) ex = e;
            else ex = new GenericErrorException<TError>(error, msg);
            LogException(ex, msg, argument, caller, path, line);

            return error;
        }

        /// <summary>Logs custom error types.</summary>
        /// <param name="error">The custom error types to log (i.e. not exceptions).</param>
        /// <param name="message">The messages to trace, one for each error.</param>
        /// <returns>The errors originally provided.</returns>
        public static IEnumerable<TError> LogErrorValue<TError>(
            this IEnumerable<TError> error,
            Func<TError, Format> message,
            [CallerArgumentExpression(nameof(error))] string argument = "",
            [CallerMemberName] string caller = "",
            [CallerFilePath] string path = "",
            [CallerLineNumber] int line = 0
            )
        {
            if (error == null) return default;

            foreach (var err in error)
            {
                if (err == null) continue;
                var msg = message(err);
                Exception ex;
                if (error is Exception e) ex = e;
                else ex = new GenericErrorException<TError>(err, msg);
                LogException(ex, msg, argument, caller, path, line);
            }

            return error;
        }

        /// <summary>Logs custom error types.</summary>
        /// <param name="error">The custom error types to log (i.e. not exceptions).</param>
        /// <param name="message">The messages to trace, one for each error.</param>
        /// <returns>The errors originally provided.</returns>
        public static IEnumerable<TError> LogErrorValue<TError>(
            this IEnumerable<TError> error,
            Func<TError, Index, Format> message,
            [CallerArgumentExpression(nameof(error))] string argument = "",
            [CallerMemberName] string caller = "",
            [CallerFilePath] string path = "",
            [CallerLineNumber] int line = 0
            )
        {
            if (error == null) return default;

            int i = 0;
            foreach (var err in error)
            {
                if (err == null) continue;
                var msg = message(err, Index.From(i++));
                Exception ex;
                if (error is Exception e) ex = e;
                else ex = new GenericErrorException<TError>(err, msg);
                LogException(ex, msg, argument, caller, path, line);
            }

            return error;
        }

        /// <summary>Logs custom error types.</summary>
        /// <param name="error">The custom error types to log (i.e. not exceptions).</param>
        /// <param name="message">The messages to trace, one for each error.</param>
        /// <returns>The errors originally provided.</returns>
        public static IEnumerable<TError> LogErrorValue<TError>(
            this IEnumerable<TError> error,
            Func<TError, Index, Length, Format> message,
            [CallerArgumentExpression(nameof(error))] string argument = "",
            [CallerMemberName] string caller = "",
            [CallerFilePath] string path = "",
            [CallerLineNumber] int line = 0
            )
        {
            if (error == null) return default;

            int i = 0, count = error is List<TError> lst ? lst.Count : error.Count();
            var length = Length.From(count);
            foreach (var err in error)
            {
                if (err == null) continue;
                var msg = message(err, Index.From(i++), length);
                Exception ex;
                if (error is Exception e) ex = e;
                else ex = new GenericErrorException<TError>(err, msg);
                LogException(ex, msg, argument, caller, path, line);
            }

            return error;
        }

        /// <summary>Logs custom error types.</summary>
        /// <param name="error">The custom error types to log (i.e. not exceptions).</param>
        /// <param name="message">The messages to trace, one for each error.</param>
        /// <returns>The errors originally provided.</returns>
        public static TError[] LogErrorValue<TError>(
            this TError[] error,
            Func<TError, Format> message,
            [CallerArgumentExpression(nameof(error))] string argument = "",
            [CallerMemberName] string caller = "",
            [CallerFilePath] string path = "",
            [CallerLineNumber] int line = 0
            )
        {
            if (error == null) return default;

            for (int i = 0; i < error.Length; i++)
            {
                if (error[i] == null) continue;
                var msg = message(error[i]);
                Exception ex;
                if (error is Exception e) ex = e;
                else ex = new GenericErrorException<TError>(error[i], msg);
                LogException(ex, msg, argument, caller, path, line);
            }

            return error;
        }

        /// <summary>Logs custom error types.</summary>
        /// <param name="error">The custom error types to log (i.e. not exceptions).</param>
        /// <param name="message">The messages to trace, one for each error.</param>
        /// <returns>The errors originally provided.</returns>
        public static TError[] LogErrorValue<TError>(
            this TError[] error,
            Func<TError, Index, Format> message,
            [CallerArgumentExpression(nameof(error))] string argument = "",
            [CallerMemberName] string caller = "",
            [CallerFilePath] string path = "",
            [CallerLineNumber] int line = 0
            )
        {
            if (error == null) return default;

            for (int i = 0; i < error.Length; i++)
            {
                if (error[i] == null) continue;
                var msg = message(error[i], Index.From(i));
                Exception ex;
                if (error is Exception e) ex = e;
                else ex = new GenericErrorException<TError>(error[i], msg);
                LogException(ex, msg, argument, caller, path, line);
            }

            return error;
        }

        /// <summary>Logs custom error types.</summary>
        /// <param name="error">The custom error types to log (i.e. not exceptions).</param>
        /// <param name="message">The messages to trace, one for each error.</param>
        /// <returns>The errors originally provided.</returns>
        public static TError[] LogErrorValue<TError>(
            this TError[] error,
            Func<TError, Index, Length, Format> message,
            [CallerArgumentExpression(nameof(error))] string argument = "",
            [CallerMemberName] string caller = "",
            [CallerFilePath] string path = "",
            [CallerLineNumber] int line = 0
            )
        {
            if (error == null) return default;

            var length = Length.From(error.Length);
            for (int i = 0; i < error.Length; i++)
            {
                if (error[i] == null) continue;
                var msg = message(error[i], Index.From(i), length);
                Exception ex;
                if (error is Exception e) ex = e;
                else ex = new GenericErrorException<TError>(error[i], msg);
                LogException(ex, msg, argument, caller, path, line);
            }

            return error;
        }

        // Log without caturing any caller attributes, so the logs are not confusing (i.e. exposing internal methods / expressions).
        internal static TError LogErrorPlain<TError>(this TError ex, Format message = default) where TError : Exception
        {
            if (ex == null) return default;
            LogException(ex, message, string.Empty, string.Empty, string.Empty, 0);
            return ex;
        }

        /// <summary>Logs an exception.</summary>
        /// <param name="ex">The exception to log.</param>
        /// <returns>The initial exception.</returns>
        public static AggregateException LogError(
            this AggregateException ex,
            Format message = default,
            [CallerArgumentExpression(nameof(ex))] string argument = "",
            [CallerMemberName] string caller = "",
            [CallerFilePath] string path = "",
            [CallerLineNumber] int line = 0
            )
        {
            if (ex == null) return default;
            LogException(ex, message, argument, caller, path, line);
            return ex;
        }

        // Log without caturing any caller attributes, so the logs are not confusing (i.e. exposing internal methods / expressions).
        internal static AggregateException LogErrorPlain(this AggregateException ex, Format message = default)
        {
            if (ex == null) return default;
            LogException(ex, message, string.Empty, string.Empty, string.Empty, 0);
            return ex;
        }

        /// <summary>Logs an exception.</summary>
        /// <param name="ex">The exception to log.</param>
        /// <param name="message">The message to trace.</param>
        /// <returns>The initial exception.</returns>
        public static TError LogError<TError>(
            this TError ex,
            Format message = default,
            [CallerArgumentExpression(nameof(ex))] string argument = "",
            [CallerMemberName] string caller = "",
            [CallerFilePath] string path = "",
            [CallerLineNumber] int line = 0
            ) where TError : Exception
        {
            if (ex == null) return default;
            LogException(ex, message, argument, caller, path, line);
            return ex;
        }

        /// <summary>Logs many exceptions.</summary>
        /// <param name="ex">The exceptions to log.</param>
        /// <param name="message">The message to trace (only once).</param>
        /// <returns>The exceptions originally provided.</returns>
        public static IEnumerable<TError> LogError<TError>(
            this IEnumerable<TError> ex,
            Format message = default,
            [CallerArgumentExpression(nameof(ex))] string argument = "",
            [CallerMemberName] string caller = "",
            [CallerFilePath] string path = "",
            [CallerLineNumber] int line = 0
            ) where TError : Exception
        {
            if (ex == null) return default;
            foreach (var e in ex) LogException(e, message, argument, caller, path, line);
            return ex;
        }

        /// <summary>Logs many exceptions.</summary>
        /// <param name="ex">The exceptions to log.</param>
        /// <param name="message">The message to trace (only once).</param>
        /// <returns>The exceptions originally provided.</returns>
        public static TError[] LogError<TError>(
            this TError[] ex,
            Format message = default,
            [CallerArgumentExpression(nameof(ex))] string argument = "",
            [CallerMemberName] string caller = "",
            [CallerFilePath] string path = "",
            [CallerLineNumber] int line = 0
            ) where TError : Exception
        {
            if (ex == null) return default;
            for (int i = 0; i < ex.Length; i++) LogException(ex[i], message, argument, caller, path, line);
            return ex;
        }

        /// <summary>Logs an exception.</summary>
        /// <param name="ex">The exception to log.</param>
        /// <param name="message">The message to trace.</param>
        /// <returns>The initial exception.</returns>
        public static TError LogError<TError>(
            this TError ex,
            Func<TError, Format> message,
            [CallerArgumentExpression(nameof(ex))] string argument = "",
            [CallerMemberName] string caller = "",
            [CallerFilePath] string path = "",
            [CallerLineNumber] int line = 0
            ) where TError : Exception
        {
            if (ex == null) return default;
            var msg = message(ex);
            LogException(ex, msg, argument, caller, path, line);
            return ex;
        }

        /// <summary>Logs many exceptions.</summary>
        /// <param name="ex">The exceptions to log.</param>
        /// <param name="message">The messages to trace, one for each error.</param>
        /// <returns>The exceptions originally provided.</returns>
        public static IEnumerable<TError> LogError<TError>(
            this IEnumerable<TError> ex,
            Func<TError, Format> message,
            [CallerArgumentExpression(nameof(ex))] string argument = "",
            [CallerMemberName] string caller = "",
            [CallerFilePath] string path = "",
            [CallerLineNumber] int line = 0
            ) where TError : Exception
        {
            if (ex == null) return default;
            foreach (var e in ex) { var msg = message(e); LogException(e, msg, argument, caller, path, line); }
            return ex;
        }

        /// <summary>Logs many exceptions.</summary>
        /// <param name="ex">The exceptions to log.</param>
        /// <param name="message">The messages to trace, one for each error.</param>
        /// <returns>The exceptions originally provided.</returns>
        public static IEnumerable<TError> LogError<TError>(
            this IEnumerable<TError> ex,
            Func<TError, Index, Format> message,
            [CallerArgumentExpression(nameof(ex))] string argument = "",
            [CallerMemberName] string caller = "",
            [CallerFilePath] string path = "",
            [CallerLineNumber] int line = 0
            ) where TError : Exception
        {
            if (ex == null) return default;
            int i = 0;
            foreach (var e in ex) { var msg = message(e, Index.From(i++)); LogException(e, msg, argument, caller, path, line); }
            return ex;
        }

        /// <summary>Logs many exceptions.</summary>
        /// <param name="ex">The exceptions to log.</param>
        /// <param name="message">The messages to trace, one for each error.</param>
        /// <returns>The exceptions originally provided.</returns>
        public static IEnumerable<TError> LogError<TError>(
            this IEnumerable<TError> ex,
            Func<TError, Index, Length, Format> message,
            [CallerArgumentExpression(nameof(ex))] string argument = "",
            [CallerMemberName] string caller = "",
            [CallerFilePath] string path = "",
            [CallerLineNumber] int line = 0
            ) where TError : Exception
        {
            if (ex == null) return default;
            int i = 0, count = ex is List<TError> lst ? lst.Count : ex.Count();
            var length = Length.From(count);
            foreach (var e in ex) { var msg = message(e, Index.From(i++), length); LogException(e, msg, argument, caller, path, line); }
            return ex;
        }

        /// <summary>Logs many exceptions.</summary>
        /// <param name="ex">The exceptions to log.</param>
        /// <param name="message">The messages to trace, one for each error.</param>
        /// <returns>The exceptions originally provided.</returns>
        public static TError[] LogError<TError>(
            this TError[] ex,
            Func<TError, Format> message,
            [CallerArgumentExpression(nameof(ex))] string argument = "",
            [CallerMemberName] string caller = "",
            [CallerFilePath] string path = "",
            [CallerLineNumber] int line = 0
            ) where TError : Exception
        {
            if (ex == null) return default;
            for (int i = 0; i < ex.Length; i++) { var msg = message(ex[i]); LogException(ex[i], msg, argument, caller, path, line); }
            return ex;
        }

        /// <summary>Logs many exceptions.</summary>
        /// <typeparam name="TError"></typeparam>
        /// <param name="ex">The exceptions to log.</param>
        /// <param name="message">The messages to trace, one for each error</param>
        /// <returns>The exceptions originally provided.</returns>
        public static TError[] LogError<TError>(
            this TError[] ex,
            Func<TError, Index, Format> message,
            [CallerArgumentExpression(nameof(ex))] string argument = "",
            [CallerMemberName] string caller = "",
            [CallerFilePath] string path = "",
            [CallerLineNumber] int line = 0
            ) where TError : Exception
        {
            if (ex == null) return default;
            for (int i = 0; i < ex.Length; i++) { var msg = message(ex[i], Index.From(i)); LogException(ex[i], msg, argument, caller, path, line); }
            return ex;
        }

        /// <summary>Logs many exceptions.</summary>
        /// <param name="ex">The exceptions to log.</param>
        /// <param name="message">The messages to trace, one for each error</param>
        /// <returns>The exceptions originally provided.</returns>
        public static TError[] LogError<TError>(
            this TError[] ex,
            Func<TError, Index, Length, Format> message,
            [CallerArgumentExpression(nameof(ex))] string argument = "",
            [CallerMemberName] string caller = "",
            [CallerFilePath] string path = "",
            [CallerLineNumber] int line = 0
            ) where TError : Exception
        {
            if (ex == null) return default;
            var length = Length.From(ex.Length);
            for (int i = 0; i < ex.Length; i++) { var msg = message(ex[i], Index.From(i), length); LogException(ex[i], msg, argument, caller, path, line); }
            return ex;
        }

        #endregion

        #region T

        /// <summary>Logs a trace step.</summary>
        /// <param name="message">The message to trace.</param>
        /// <returns>The initial value.</returns>
        public static T LogValue<T>(this T value, Format message)
        {
            Trace.Log(message);
            return value;
        }

        /// <summary>Logs a trace step.</summary>
        /// <param name="format">The message to trace.</param>
        /// <returns>The initial value.</returns>
        public static T LogValue<T>(this T value, Func<T, Format> format)
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
        public static Response Log(this Response response, Format success)
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
        public static Response Log(this Response response, Format success, Format fail)
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
        public static Response<T> Log<T>(this Response<T> response, Format success)
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
        public static Response<T> Log<T>(this Response<T> response, Func<T, Format> success)
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
        public static Response<T> Log<T>(this Response<T> response, Func<T, Format> success, Format fail)
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
        public static Response<T> Log<T>(this Response<T> response, Format success, Format fail)
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
        public static Func<Response<T>> Log<T>(this Func<Response<T>> func, Format success) => () => func().Log(success);

        /// <summary>Logs a trace step.</summary>
        /// <param name="func">A function to the Response Container.</param>
        /// <param name="success">The message to trace if the response is in a valid state.</param>
        /// <param name="fail">The message to trace when the response is in an invalid state.</param>
        /// <returns>The same response from the input.</returns>
        public static Func<Response<T>> Log<T>(this Func<Response<T>> func, Format success, Format fail) => () => func().Log(success, fail);

        /// <summary>Logs a trace step.</summary>
        /// <param name="func">A function to the Response Container.</param>
        /// <param name="success">The message to trace if the response is in a valid state.</param>
        /// <returns>The same response from the input.</returns>
        public static Func<Response<T>> Log<T>(this Func<Response<T>> func, Func<T, Format> success) => () => func().Log(success);

        /// <summary>Logs a trace step.</summary>
        /// <param name="func">A function to the Response Container.</param>
        /// <param name="success">The message to trace if the response is in a valid state.</param>
        /// <param name="fail">The message to trace when the response is in an invalid state.</param>
        /// <returns>The same response from the input.</returns>
        public static Func<Response<T>> Log<T>(this Func<Response<T>> func, Func<T, Format> success, Format fail) => () => func().Log(success, fail);

        #endregion

        #region ResponseTCompose WithOutput

        /// <summary>Logs a trace step.</summary>
        /// <param name="func">A function to the Response Container.</param>
        /// <param name="success">The message to trace if the response is in a valid state.</param>
        /// <returns>The same response from the input.</returns>
        public static Func<T, Response<TResult>> Log<T, TResult>(this Func<T, Response<TResult>> func, Format success) => x => func(x).Log(success);

        /// <summary>Logs a trace step.</summary>
        /// <param name="func">A function to the Response Container.</param>
        /// <param name="success">The message to trace if the response is in a valid state.</param>
        /// <param name="fail">The message to trace when the response is in an invalid state.</param>
        /// <returns>The same response from the input.</returns>
        public static Func<T, Response<TResult>> Log<T, TResult>(this Func<T, Response<TResult>> func, Format success, Format fail) => x => func(x).Log(success, fail);

        /// <summary>Logs a trace step.</summary>
        /// <param name="func">A function to the Response Container.</param>
        /// <param name="success">The message to trace f the response is in a valid state.</param>
        /// <returns>The same response from the input.</returns>i
        public static Func<T, Response<TResult>> Log<T, TResult>(this Func<T, Response<TResult>> func, Func<TResult, Format> success) => x => func(x).Log(success);

        /// <summary>Logs a trace step.</summary>
        /// <param name="func">A function to the Response Container.</param>
        /// <param name="success">The message to trace if the response is in a valid state.</param>
        /// <param name="fail">The message to trace when the response is in an invalid state.</param>
        /// <returns>The same response from the input.</returns>
        public static Func<T, Response<TResult>> Log<T, TResult>(this Func<T, Response<TResult>> func, Func<TResult, Format> success, Format fail) => x => func(x).Log(success, fail);

        #endregion

        #region ResponseTCompose WithInputAndOutput

        /// <summary>Logs a trace step.</summary>
        /// <param name="func">A function to the Response Container.</param>
        /// <param name="success">The message to trace if the response is in a valid state.</param>
        /// <returns>The same response from the input.</returns>
        public static Func<T, Response<TResult>> Log<T, TResult>(this Func<T, Response<TResult>> func, Func<T, TResult, Format> success) => x => {
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
        public static Func<T, Response<TResult>> Log<T, TResult>(this Func<T, Response<TResult>> func, Func<T, TResult, Format> success, Format fail) => x => {
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
        public static Maybe<TValue, TError> Log<TValue, TError>(
            this Maybe<TValue, TError> maybe,
            Func<TValue, Format> value,
            Func<TError, Format> error,
            [CallerArgumentExpression(nameof(maybe))] string argument = "",
            [CallerMemberName] string caller = "",
            [CallerFilePath] string path = "",
            [CallerLineNumber] int line = 0
            )
        {
            var message = maybe.Match(value, error);
            if (maybe._hasValue) Trace.Log(message);

            if (!maybe._hasValue)
            {
                LogErrorValue(maybe._error, message, argument, caller, path, line);
                LogErrorValue(maybe.AggregateErrors, message, argument, caller, path, line);
            }

            return maybe;
        }

        /// <summary>Logs a trace step.</summary>
        /// <param name="value">A message to trace when Maybe contains some instance of TValue.</param>
        /// <param name="error">A message to trace when Maybe contains some instance of TError.</param>
        /// <returns>The same Maybe as the input.</returns>
        public static Maybe<TValue, TError> Log<TValue, TError>(
            this Maybe<TValue, TError> maybe,
            Format value,
            Format error,
            [CallerArgumentExpression(nameof(maybe))] string argument = "",
            [CallerMemberName] string caller = "",
            [CallerFilePath] string path = "",
            [CallerLineNumber] int line = 0
            )
        {
            var message = maybe._hasValue ? value : error;
            if (maybe._hasValue) Trace.Log(message);

            if (!maybe._hasValue)
            {
                LogErrorValue(maybe._error, message, argument, caller, path, line);
                LogErrorValue(maybe.AggregateErrors, message, argument, caller, path, line);
            }

            return maybe;
        }

        /// <summary>Logs a trace step.</summary>
        /// <param name="value">A function that takes some TValue instance, and returns a message for tracing.</param>
        /// <param name="error">A function that takes some Exception instance, and returns a message for tracing.</param>
        /// <returns>The same Maybe as the input.</returns>
        public static Maybe<TValue> Log<TValue>(
            this Maybe<TValue> maybe,
            Func<TValue, Format> value,
            Func<Exception, Format> error,
            [CallerArgumentExpression(nameof(maybe))] string argument = "",
            [CallerMemberName] string caller = "",
            [CallerFilePath] string path = "",
            [CallerLineNumber] int line = 0
            )
        {
            var message = maybe.Match(value, error);
            if (maybe._hasValue) Trace.Log(message);

            if (!maybe._hasValue)
            {
                LogError(maybe._error, message, argument, caller, path, line);
                LogError(maybe.AggregateErrors, message, argument, caller, path, line);
            }

            return maybe;
        }

        /// <summary>Logs a trace step.</summary>
        /// <param name="value">A message to trace when Maybe contains some instance of TValue.</param>
        /// <param name="error">A message to trace when Maybe contains some instance of Exception.</param>
        /// <returns>The same Maybe as the input.</returns>
        public static Maybe<TValue> Log<TValue>(
            this Maybe<TValue> maybe,
            Format value,
            Format error,
            [CallerArgumentExpression(nameof(maybe))] string argument = "",
            [CallerMemberName] string caller = "",
            [CallerFilePath] string path = "",
            [CallerLineNumber] int line = 0
            )
        {
            var message = maybe._hasValue ? value : error;
            if (maybe._hasValue) Trace.Log(message);

            if (!maybe._hasValue)
            {
                LogError(maybe._error, message, argument, caller, path, line);
                LogError(maybe.AggregateErrors, message, argument, caller, path, line);
            }

            return maybe;
        }

        #endregion
    }
}
