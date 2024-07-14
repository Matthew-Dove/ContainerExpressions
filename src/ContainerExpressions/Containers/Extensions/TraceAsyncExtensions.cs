using ContainerExpressions.Containers.Internal;
using System;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace ContainerExpressions.Containers
{
    /// <summary>Extensions for the Trace Container.</summary>
    public static class TraceAsyncExtensions
    {
        #region TError

        /// <summary>Logs exceptions from faulted tasks.</summary>
        public static Task LogErrorAsync(
            this Task task,
            [CallerArgumentExpression(nameof(task))] string argument = "",
            [CallerMemberName] string caller = "",
            [CallerFilePath] string path = "",
            [CallerLineNumber] int line = 0
            )
        {
            if (task == null) return default;
            if (!task.IsCompleted) return task.ContinueWith(t => { if (t.IsFaulted) TraceExtensions.LogException(t.Exception, argument, caller, path, line); t.Wait(); });
            if (task.IsFaulted) TraceExtensions.LogException(task.Exception, argument, caller, path, line);
            return task;
        }

        /// <summary>Logs exceptions from faulted tasks.</summary>
        public static Task<T> LogErrorAsync<T>(
            this Task<T> task,
            [CallerArgumentExpression(nameof(task))] string argument = "",
            [CallerMemberName] string caller = "",
            [CallerFilePath] string path = "",
            [CallerLineNumber] int line = 0
            )
        {
            if (task == null) return default;
            if (!task.IsCompleted) return task.ContinueWith(t => { if (t.IsFaulted) TraceExtensions.LogException(t.Exception, argument, caller, path, line); return t.Result; });
            if (task.IsFaulted) TraceExtensions.LogException(task.Exception, argument, caller, path, line);
            return task;
        }

        /// <summary>Logs exceptions from faulted tasks.</summary>
        public static Task<Response> LogErrorAsync(
            this Task<Response> task,
            [CallerArgumentExpression(nameof(task))] string argument = "",
            [CallerMemberName] string caller = "",
            [CallerFilePath] string path = "",
            [CallerLineNumber] int line = 0
            )
        {
            if (task == null) return default;

            if (!task.IsCompleted) return task.ContinueWith(t => {
                if (t.IsFaulted) TraceExtensions.LogException(t.Exception, argument, caller, path, line);
                return t.Status == TaskStatus.RanToCompletion ? t.Result : Response.Error;
            });

            if (task.IsFaulted) TraceExtensions.LogException(task.Exception, argument, caller, path, line);
            return task.Status == TaskStatus.RanToCompletion ? task : Pool.ResponseError;
        }

        /// <summary>Logs exceptions from faulted tasks.</summary>
        public static Task<Response<T>> LogErrorAsync<T>(
            this Task<Response<T>> task,
            [CallerArgumentExpression(nameof(task))] string argument = "",
            [CallerMemberName] string caller = "",
            [CallerFilePath] string path = "",
            [CallerLineNumber] int line = 0
            )
        {
            if (task == null) return default;

            if (!task.IsCompleted) return task.ContinueWith(t => {
                if (t.IsFaulted) TraceExtensions.LogException(t.Exception, argument, caller, path, line);
                return t.Status == TaskStatus.RanToCompletion ? t.Result : Response<T>.Error;
            });

            if (task.IsFaulted) TraceExtensions.LogException(task.Exception, argument, caller, path, line);
            return task.Status == TaskStatus.RanToCompletion ? task : Pool<T>.ResponseError;
        }

        /// <summary>Logs exceptions from faulted tasks.</summary>
        public static Task<Response<Unit>> LogErrorAsync(
            this Task<Response<Unit>> task,
            [CallerArgumentExpression(nameof(task))] string argument = "",
            [CallerMemberName] string caller = "",
            [CallerFilePath] string path = "",
            [CallerLineNumber] int line = 0
            )
        {
            if (task == null) return default;

            if (!task.IsCompleted) return task.ContinueWith(t => {
                if (t.IsFaulted) TraceExtensions.LogException(t.Exception, argument, caller, path, line);
                return t.Status == TaskStatus.RanToCompletion ? t.Result : Unit.ResponseError;
            });

            if (task.IsFaulted) TraceExtensions.LogException(task.Exception, argument, caller, path, line);
            return task.Status == TaskStatus.RanToCompletion ? task : Pool.UnitResponseError;
        }

        #endregion

        #region T

        /// <summary>Logs a trace step.</summary>
        /// <param name="message">The message to trace.</param>
        /// <returns>The initial value.</returns>
        public static Task<T> LogValueAsync<T>(
            this Task<T> task, Format message,
            [CallerArgumentExpression(nameof(task))] string argument = "",
            [CallerMemberName] string caller = "",
            [CallerFilePath] string path = "",
            [CallerLineNumber] int line = 0
            ) =>
            task.ContinueWith(x =>
            {
                if (x.Status == TaskStatus.RanToCompletion) Trace.Log(message);
                else if (x.Status == TaskStatus.Faulted) TraceExtensions.LogException(x.Exception, argument, caller, path, line);
                return x.Result;
            });

        /// <summary>Logs a trace step.</summary>
        /// <param name="message">The message to trace.</param>
        /// <returns>The initial value.</returns>
        public static Task<T> LogValueAsync<T>(
            this Task<T> task,
            Func<T, Format> message,
            [CallerArgumentExpression(nameof(task))] string argument = "",
            [CallerMemberName] string caller = "",
            [CallerFilePath] string path = "",
            [CallerLineNumber] int line = 0
            ) =>
            task.ContinueWith(x =>
            {
                if (x.Status == TaskStatus.RanToCompletion) Trace.Log(message(x.Result));
                else if (x.Status == TaskStatus.Faulted) TraceExtensions.LogException(x.Exception, argument, caller, path, line);
                return x.Result;
            });

        #endregion

        #region Response

        /// <summary>Logs a trace step.</summary>
        /// <param name="response">The Response Container.</param>
        /// <param name="success">The message to trace if the response is in a valid state.</param>
        /// <returns>The same response from the input.</returns>
        public static Task<Response> LogAsync(this Task<Response> response, Format success)
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
        public static Task<Response> LogAsync(this Task<Response> response, Format success, Format fail)
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
        public static Task<Response<T>> LogAsync<T>(this Task<Response<T>> response, Format success)
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
        public static Task<Response<T>> LogAsync<T>(this Task<Response<T>> response, Func<T, Format> success)
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
        public static Task<Response<T>> LogAsync<T>(this Task<Response<T>> response, Func<T, Format> success, Format fail)
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
        public static Task<Response<T>> LogAsync<T>(this Task<Response<T>> response, Format success, Format fail)
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
        public static Func<Task<Response<T>>> LogAsync<T>(this Func<Task<Response<T>>> func, Format success) => () => func().ContinueWith(x => x.Result.Log(success));

        /// <summary>Logs a trace step.</summary>
        /// <param name="func">A function to the Response Container.</param>
        /// <param name="success">The message to trace if the response is in a valid state.</param>
        /// <param name="fail">The message to trace when the response is in an invalid state.</param>
        /// <returns>The same response from the input.</returns>
        public static Func<Task<Response<T>>> LogAsync<T>(this Func<Task<Response<T>>> func, Format success, Format fail) => () => func().ContinueWith(x => x.Result.Log(success, fail));

        /// <summary>Logs a trace step.</summary>
        /// <param name="func">A function to the Response Container.</param>
        /// <param name="success">The message to trace if the response is in a valid state.</param>
        /// <returns>The same response from the input.</returns>
        public static Func<Task<Response<T>>> LogAsync<T>(this Func<Task<Response<T>>> func, Func<T, Format> success) => () => func().ContinueWith(x => x.Result.Log(success));

        /// <summary>Logs a trace step.</summary>
        /// <param name="func">A function to the Response Container.</param>
        /// <param name="success">The message to trace if the response is in a valid state.</param>
        /// <param name="fail">The message to trace when the response is in an invalid state.</param>
        /// <returns>The same response from the input.</returns>
        public static Func<Task<Response<T>>> LogAsync<T>(this Func<Task<Response<T>>> func, Func<T, Format> success, Format fail) => () => func().ContinueWith(x => x.Result.Log(success, fail));

        #endregion

        #region ResponseTCompose WithOutput

        /// <summary>Logs a trace step.</summary>
        /// <param name="func">A function to the Response Container.</param>
        /// <param name="success">The message to trace if the response is in a valid state.</param>
        /// <returns>The same response from the input.</returns>
        public static Func<T, Task<Response<TResult>>> LogAsync<T, TResult>(this Func<T, Task<Response<TResult>>> func, Format success) => x => func(x).ContinueWith(y => y.Result.Log(success));

        /// <summary>Logs a trace step.</summary>
        /// <param name="func">A function to the Response Container.</param>
        /// <param name="success">The message to trace if the response is in a valid state.</param>
        /// <param name="fail">The message to trace when the response is in an invalid state.</param>
        /// <returns>The same response from the input.</returns>
        public static Func<T, Task<Response<TResult>>> LogAsync<T, TResult>(this Func<T, Task<Response<TResult>>> func, Format success, Format fail) => x => func(x).ContinueWith(y => y.Result.Log(success, fail));

        /// <summary>Logs a trace step.</summary>
        /// <param name="func">A function to the Response Container.</param>
        /// <param name="success">The message to trace if the response is in a valid state.</param>
        /// <returns>The same response from the input.</returns>
        public static Func<T, Task<Response<TResult>>> LogAsync<T, TResult>(this Func<T, Task<Response<TResult>>> func, Func<TResult, Format> success) => x => func(x).ContinueWith(y => y.Result.Log(success));

        /// <summary>Logs a trace step.</summary>
        /// <param name="func">A function to the Response Container.</param>
        /// <param name="success">The message to trace if the response is in a valid state.</param>
        /// <param name="fail">The message to trace when the response is in an invalid state.</param>
        /// <returns>The same response from the input.</returns>
        public static Func<T, Task<Response<TResult>>> LogAsync<T, TResult>(this Func<T, Task<Response<TResult>>> func, Func<TResult, Format> success, Format fail) => x => func(x).ContinueWith(y => y.Result.Log(success, fail));

        #endregion

        #region ResponseTCompose WithInputAndOutput

        /// <summary>Logs a trace step.</summary>
        /// <param name="func">A function to the Response Container.</param>
        /// <param name="success">The message to trace if the response is in a valid state.</param>
        /// <returns>The same response from the input.</returns>
        public static Func<T, Task<Response<TResult>>> LogAsync<T, TResult>(this Func<T, Task<Response<TResult>>> func, Func<T, TResult, Format> success) => x =>
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
        public static Func<T, Task<Response<TResult>>> LogAsync<T, TResult>(this Func<T, Task<Response<TResult>>> func, Func<T, TResult, Format> success, Format fail) => x =>
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

        #region Maybe

        /// <summary>Logs a trace step.</summary>
        /// <param name="value">A function that takes some TValue instance, and returns a message for tracing.</param>
        /// <param name="error">A function that takes some TError instance, and returns a message for tracing.</param>
        /// <returns>The same Maybe as the input.</returns>
        public static Task<Maybe<TValue, TError>> LogAsync<TValue, TError>(
            this Task<Maybe<TValue, TError>> maybe,
            Func<TValue, Format> value,
            Func<TError, Format> error,
            [CallerArgumentExpression(nameof(maybe))] string argument = "",
            [CallerMemberName] string caller = "",
            [CallerFilePath] string path = "",
            [CallerLineNumber] int line = 0
            ) =>
            maybe.ContinueWith(x =>
            {
                var message = x.Result.Match(value, error);
                Trace.Log(message);

                if (!x.Result._hasValue)
                {
                    TraceExtensions.LogErrorValue(x.Result._error, argument, caller, path, line);
                    TraceExtensions.LogErrorValue(x.Result.AggregateErrors, argument, caller, path, line);
                }

                return x.Result;
            });

        /// <summary>Logs a trace step.</summary>
        /// <param name="value">A message to trace when Maybe contains some instance of TValue.</param>
        /// <param name="error">A message to trace when Maybe contains some instance of TError.</param>
        /// <returns>The same Maybe as the input.</returns>
        public static Task<Maybe<TValue, TError>> LogAsync<TValue, TError>(
            this Task<Maybe<TValue, TError>> maybe,
            Format value,
            Format error,
            [CallerArgumentExpression(nameof(maybe))] string argument = "",
            [CallerMemberName] string caller = "",
            [CallerFilePath] string path = "",
            [CallerLineNumber] int line = 0
            ) =>
            maybe.ContinueWith(x =>
            {
                var message = x.Result._hasValue ? value : error;
                Trace.Log(message);

                if (!x.Result._hasValue)
                {
                    TraceExtensions.LogErrorValue(x.Result._error, argument, caller, path, line);
                    TraceExtensions.LogErrorValue(x.Result.AggregateErrors, argument, caller, path, line);
                }

                return x.Result;
            });

        /// <summary>Logs a trace step.</summary>
        /// <param name="value">A function that takes some TValue instance, and returns a message for tracing.</param>
        /// <param name="error">A function that takes some Exception instance, and returns a message for tracing.</param>
        /// <returns>The same Maybe as the input.</returns>
        public static Task<Maybe<TValue>> LogAsync<TValue>(
            this Task<Maybe<TValue>> maybe,
            Func<TValue, Format> value,
            Func<Exception, Format> error,
            [CallerArgumentExpression(nameof(maybe))] string argument = "",
            [CallerMemberName] string caller = "",
            [CallerFilePath] string path = "",
            [CallerLineNumber] int line = 0
            ) =>
            maybe.ContinueWith(x =>
            {
                var message = x.Result.Match(value, error);
                Trace.Log(message);

                if (!x.Result._hasValue)
                {
                    TraceExtensions.LogError(x.Result._error, argument, caller, path, line);
                    TraceExtensions.LogError(x.Result.AggregateErrors, argument, caller, path, line);
                }

                return x.Result;
            });

        /// <summary>Logs a trace step.</summary>
        /// <param name="value">A message to trace when Maybe contains some instance of TValue.</param>
        /// <param name="error">A message to trace when Maybe contains some instance of Exception.</param>
        /// <returns>The same Maybe as the input.</returns>
        public static Task<Maybe<TValue>> LogAsync<TValue>(
            this Task<Maybe<TValue>> maybe,
            Format value,
            Format error,
            [CallerArgumentExpression(nameof(maybe))] string argument = "",
            [CallerMemberName] string caller = "",
            [CallerFilePath] string path = "",
            [CallerLineNumber] int line = 0
            ) =>
            maybe.ContinueWith(x =>
            {
                var message = x.Result._hasValue ? value : error;
                Trace.Log(message);

                if (!x.Result._hasValue)
                {
                    TraceExtensions.LogError(x.Result._error, argument, caller, path, line);
                    TraceExtensions.LogError(x.Result.AggregateErrors, argument, caller, path, line);
                }

                return x.Result;
            });

        #endregion
    }
}
