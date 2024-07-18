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
            Format message = default,
            [CallerArgumentExpression(nameof(task))] string argument = "",
            [CallerMemberName] string caller = "",
            [CallerFilePath] string path = "",
            [CallerLineNumber] int line = 0
            )
        {
            if (task == null) return default;
            if (!task.IsCompleted) return task.ContinueWith(t => { if (t.IsFaulted) TraceExtensions.LogException(t.Exception, message, argument, caller, path, line); });
            if (task.IsFaulted) TraceExtensions.LogException(task.Exception, message, argument, caller, path, line);
            return task;
        }

        /// <summary>Logs exceptions from faulted tasks.</summary>
        public static Task<T> LogErrorAsync<T>(
            this Task<T> task,
            Format message = default,
            [CallerArgumentExpression(nameof(task))] string argument = "",
            [CallerMemberName] string caller = "",
            [CallerFilePath] string path = "",
            [CallerLineNumber] int line = 0
            )
        {
            if (task == null) return default;
            if (!task.IsCompleted) return task.ContinueWith(t => { if (t.IsFaulted) TraceExtensions.LogException(t.Exception, message, argument, caller, path, line); return t.Result; });
            if (task.IsFaulted) TraceExtensions.LogException(task.Exception, message, argument, caller, path, line);
            return task;
        }

        /// <summary>Logs exceptions from faulted tasks.</summary>
        public static Task<Response> LogErrorAsync(
            this Task<Response> task,
            Format message = default,
            [CallerArgumentExpression(nameof(task))] string argument = "",
            [CallerMemberName] string caller = "",
            [CallerFilePath] string path = "",
            [CallerLineNumber] int line = 0
            )
        {
            if (task == null) return default;

            if (!task.IsCompleted) return task.ContinueWith(t => {
                if (t.IsFaulted) TraceExtensions.LogException(t.Exception, message, argument, caller, path, line);
                return t.Status == TaskStatus.RanToCompletion ? t.Result : Response.Error;
            });

            if (task.IsFaulted) TraceExtensions.LogException(task.Exception, message, argument, caller, path, line);
            return task.Status == TaskStatus.RanToCompletion ? task : Pool.ResponseError;
        }

        /// <summary>Logs exceptions from faulted tasks.</summary>
        public static Task<Response<T>> LogErrorAsync<T>(
            this Task<Response<T>> task,
            Format message = default,
            [CallerArgumentExpression(nameof(task))] string argument = "",
            [CallerMemberName] string caller = "",
            [CallerFilePath] string path = "",
            [CallerLineNumber] int line = 0
            )
        {
            if (task == null) return default;

            if (!task.IsCompleted) return task.ContinueWith(t => {
                if (t.IsFaulted) TraceExtensions.LogException(t.Exception, message, argument, caller, path, line);
                return t.Status == TaskStatus.RanToCompletion ? t.Result : Response<T>.Error;
            });

            if (task.IsFaulted) TraceExtensions.LogException(task.Exception, message, argument, caller, path, line);
            return task.Status == TaskStatus.RanToCompletion ? task : Pool<T>.ResponseError;
        }

        /// <summary>Logs exceptions from faulted tasks.</summary>
        public static Task<Response<Unit>> LogErrorAsync(
            this Task<Response<Unit>> task,
            Format message = default,
            [CallerArgumentExpression(nameof(task))] string argument = "",
            [CallerMemberName] string caller = "",
            [CallerFilePath] string path = "",
            [CallerLineNumber] int line = 0
            )
        {
            if (task == null) return default;

            if (!task.IsCompleted) return task.ContinueWith(t => {
                if (t.IsFaulted) TraceExtensions.LogException(t.Exception, message, argument, caller, path, line);
                return t.Status == TaskStatus.RanToCompletion ? t.Result : Unit.ResponseError;
            });

            if (task.IsFaulted) TraceExtensions.LogException(task.Exception, message, argument, caller, path, line);
            return task.Status == TaskStatus.RanToCompletion ? task : Pool.UnitResponseError;
        }

        #endregion

        #region T

        /// <summary>Logs a trace step.</summary>
        /// <param name="success">The message to trace when the task runs to completion.</param>
        /// <returns>The initial value.</returns>
        public static Task<T> LogValueAsync<T>(
            this Task<T> task,
            Format success
            ) =>
            task.ContinueWith(x =>
            {
                if (x.Status == TaskStatus.RanToCompletion)
                {
                    Trace.Log(success);
                }
                else
                {
                    if (x.Status == TaskStatus.Faulted) x.Exception.LogErrorPlain();
                }

                return x.Result;
            });

        /// <summary>Logs a trace step.</summary>
        /// <param name="success">The message to trace when the task runs to completion.</param>
        /// <param name="fail">The message to trace when the task is faulted.</param>
        /// <returns>The initial value.</returns>
        public static Task<T> LogValueAsync<T>(
            this Task<T> task,
            Format success,
            Format fail
            ) =>
            task.ContinueWith(x =>
            {
                if (x.Status == TaskStatus.RanToCompletion)
                {
                    Trace.Log(success);
                }
                else
                {
                    Trace.Log(fail);
                    if (x.Status == TaskStatus.Faulted) x.Exception.LogErrorPlain();
                }

                return x.Result;
            });

        /// <summary>Logs a trace step.</summary>
        /// <param name="success">The message to trace when the task runs to completion.</param>
        /// <returns>The initial value.</returns>
        public static Task<T> LogValueAsync<T>(
            this Task<T> task,
            Func<T, Format> success
            ) =>
            task.ContinueWith(x =>
            {
                if (x.Status == TaskStatus.RanToCompletion)
                {
                    Trace.Log(success(x.Result));
                }
                else
                {
                    if (x.Status == TaskStatus.Faulted) x.Exception.LogErrorPlain();
                }

                return x.Result;
            });

        /// <summary>Logs a trace step.</summary>
        /// <param name="success">The message to trace when the task runs to completion.</param>
        /// <param name="fail">The message to trace when the task is faulted.</param>
        /// <returns>The initial value.</returns>
        public static Task<T> LogValueAsync<T>(
            this Task<T> task,
            Func<T, Format> success,
            Format fail
            ) =>
            task.ContinueWith(x =>
            {
                if (x.Status == TaskStatus.RanToCompletion)
                {
                    Trace.Log(success(x.Result));
                }
                else
                {
                    Trace.Log(fail);
                    if (x.Status == TaskStatus.Faulted) x.Exception.LogErrorPlain();
                }

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
                if (x.Status == TaskStatus.RanToCompletion && x.Result.IsValid)
                {
                    Trace.Log(success);
                }
                else
                {
                    if (x.Status == TaskStatus.Faulted) x.Exception.LogErrorPlain();
                }

                return x.Status == TaskStatus.RanToCompletion ? x.Result : Response.Error;
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
                if (x.Status == TaskStatus.RanToCompletion && x.Result.IsValid)
                {
                    Trace.Log(success);
                }
                else
                {
                    Trace.Log(fail);
                    if (x.Status == TaskStatus.Faulted) x.Exception.LogErrorPlain();
                }

                return x.Status == TaskStatus.RanToCompletion ? x.Result : Response.Error;
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
                if (x.Status == TaskStatus.RanToCompletion && x.Result.IsValid)
                {
                    Trace.Log(success);
                }
                else
                {
                    if (x.Status == TaskStatus.Faulted) x.Exception.LogErrorPlain();
                }

                return x.Status == TaskStatus.RanToCompletion ? x.Result : Response<T>.Error;
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
                if (x.Status == TaskStatus.RanToCompletion && x.Result.IsValid)
                {
                    Trace.Log(success(x.Result.Value));
                }
                else
                {
                    if (x.Status == TaskStatus.Faulted) x.Exception.LogErrorPlain();
                }

                return x.Status == TaskStatus.RanToCompletion ? x.Result : Response<T>.Error;
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
                if (x.Status == TaskStatus.RanToCompletion && x.Result.IsValid)
                {
                    Trace.Log(success(x.Result.Value));
                }
                else
                {
                    Trace.Log(fail);
                    if (x.Status == TaskStatus.Faulted) x.Exception.LogErrorPlain();
                }

                return x.Status == TaskStatus.RanToCompletion ? x.Result : Response<T>.Error;
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
                if (x.Status == TaskStatus.RanToCompletion && x.Result.IsValid)
                {
                    Trace.Log(success);
                }
                else
                {
                    Trace.Log(fail);
                    if (x.Status == TaskStatus.Faulted) x.Exception.LogErrorPlain();
                }

                return x.Status == TaskStatus.RanToCompletion ? x.Result : Response<T>.Error;
            });
        }

        #endregion

        #region ResponseTCompose NoInput

        /// <summary>Logs a trace step.</summary>
        /// <param name="func">A function to the Response Container.</param>
        /// <param name="success">The message to trace if the response is in a valid state.</param>
        /// <returns>The same response from the input.</returns>
        public static Func<Task<Response<T>>> LogAsync<T>(this Func<Task<Response<T>>> func, Format success) => () =>
            func().ContinueWith(x =>
            {
                if (x.Status == TaskStatus.RanToCompletion && x.Result.IsValid)
                {
                    Trace.Log(success);
                }
                else
                {
                    if (x.Status == TaskStatus.Faulted) x.Exception.LogErrorPlain();
                }

                return x.Status == TaskStatus.RanToCompletion ? x.Result : Response<T>.Error;
            });

        /// <summary>Logs a trace step.</summary>
        /// <param name="func">A function to the Response Container.</param>
        /// <param name="success">The message to trace if the response is in a valid state.</param>
        /// <param name="fail">The message to trace when the response is in an invalid state.</param>
        /// <returns>The same response from the input.</returns>
        public static Func<Task<Response<T>>> LogAsync<T>(this Func<Task<Response<T>>> func, Format success, Format fail) => () =>
            func().ContinueWith(x =>
            {
                if (x.Status == TaskStatus.RanToCompletion && x.Result.IsValid)
                {
                    Trace.Log(success);
                }
                else
                {
                    Trace.Log(fail);
                    if (x.Status == TaskStatus.Faulted) x.Exception.LogErrorPlain();
                }

                return x.Status == TaskStatus.RanToCompletion ? x.Result : Response<T>.Error;
            });

        /// <summary>Logs a trace step.</summary>
        /// <param name="func">A function to the Response Container.</param>
        /// <param name="success">The message to trace if the response is in a valid state.</param>
        /// <returns>The same response from the input.</returns>
        public static Func<Task<Response<T>>> LogAsync<T>(this Func<Task<Response<T>>> func, Func<T, Format> success) => () =>
            func().ContinueWith(x =>
            {
                if (x.Status == TaskStatus.RanToCompletion && x.Result.IsValid)
                {
                    Trace.Log(success(x.Result.Value));
                }
                else
                {
                    if (x.Status == TaskStatus.Faulted) x.Exception.LogErrorPlain();
                }

                return x.Status == TaskStatus.RanToCompletion ? x.Result : Response<T>.Error;
            });

        /// <summary>Logs a trace step.</summary>
        /// <param name="func">A function to the Response Container.</param>
        /// <param name="success">The message to trace if the response is in a valid state.</param>
        /// <param name="fail">The message to trace when the response is in an invalid state.</param>
        /// <returns>The same response from the input.</returns>
        public static Func<Task<Response<T>>> LogAsync<T>(this Func<Task<Response<T>>> func, Func<T, Format> success, Format fail) => () =>
            func().ContinueWith(x =>
            {
                if (x.Status == TaskStatus.RanToCompletion && x.Result.IsValid)
                {
                    Trace.Log(success(x.Result.Value));
                }
                else
                {
                    Trace.Log(fail);
                    if (x.Status == TaskStatus.Faulted) x.Exception.LogErrorPlain();
                }

                return x.Status == TaskStatus.RanToCompletion ? x.Result : Response<T>.Error;
            });

        #endregion

        #region ResponseTCompose WithOutput

        /// <summary>Logs a trace step.</summary>
        /// <param name="func">A function to the Response Container.</param>
        /// <param name="success">The message to trace if the response is in a valid state.</param>
        /// <returns>The same response from the input.</returns>
        public static Func<T, Task<Response<TResult>>> LogAsync<T, TResult>(this Func<T, Task<Response<TResult>>> func, Format success) => x =>
            func(x).ContinueWith(y =>
            {
                if (y.Status == TaskStatus.RanToCompletion && y.Result.IsValid)
                {
                    Trace.Log(success);
                }
                else
                {
                    if (y.Status == TaskStatus.Faulted) y.Exception.LogErrorPlain();
                }

                return y.Status == TaskStatus.RanToCompletion ? y.Result : Response<TResult>.Error;
            });

        /// <summary>Logs a trace step.</summary>
        /// <param name="func">A function to the Response Container.</param>
        /// <param name="success">The message to trace if the response is in a valid state.</param>
        /// <param name="fail">The message to trace when the response is in an invalid state.</param>
        /// <returns>The same response from the input.</returns>
        public static Func<T, Task<Response<TResult>>> LogAsync<T, TResult>(this Func<T, Task<Response<TResult>>> func, Format success, Format fail) => x =>
            func(x).ContinueWith(y =>
            {
                if (y.Status == TaskStatus.RanToCompletion && y.Result.IsValid)
                {
                    Trace.Log(success);
                }
                else
                {
                    Trace.Log(fail);
                    if (y.Status == TaskStatus.Faulted) y.Exception.LogErrorPlain();
                }

                return y.Status == TaskStatus.RanToCompletion ? y.Result : Response<TResult>.Error;
            });

        /// <summary>Logs a trace step.</summary>
        /// <param name="func">A function to the Response Container.</param>
        /// <param name="success">The message to trace if the response is in a valid state.</param>
        /// <returns>The same response from the input.</returns>
        public static Func<T, Task<Response<TResult>>> LogAsync<T, TResult>(this Func<T, Task<Response<TResult>>> func, Func<TResult, Format> success) => x =>
            func(x).ContinueWith(y =>
            {
                if (y.Status == TaskStatus.RanToCompletion && y.Result.IsValid)
                {
                    Trace.Log(success(y.Result.Value));
                }
                else
                {
                    if (y.Status == TaskStatus.Faulted) y.Exception.LogErrorPlain();
                }

                return y.Status == TaskStatus.RanToCompletion ? y.Result : Response<TResult>.Error;
            });

        /// <summary>Logs a trace step.</summary>
        /// <param name="func">A function to the Response Container.</param>
        /// <param name="success">The message to trace if the response is in a valid state.</param>
        /// <param name="fail">The message to trace when the response is in an invalid state.</param>
        /// <returns>The same response from the input.</returns>
        public static Func<T, Task<Response<TResult>>> LogAsync<T, TResult>(this Func<T, Task<Response<TResult>>> func, Func<TResult, Format> success, Format fail) => x =>
            func(x).ContinueWith(y =>
            {
                if (y.Status == TaskStatus.RanToCompletion && y.Result.IsValid)
                {
                    Trace.Log(success(y.Result.Value));
                }
                else
                {
                    Trace.Log(fail);
                    if (y.Status == TaskStatus.Faulted) y.Exception.LogErrorPlain();
                }

                return y.Status == TaskStatus.RanToCompletion ? y.Result : Response<TResult>.Error;
            });

        #endregion

        #region ResponseTCompose WithInputAndOutput

        /// <summary>Logs a trace step.</summary>
        /// <param name="func">A function to the Response Container.</param>
        /// <param name="success">The message to trace if the response is in a valid state.</param>
        /// <returns>The same response from the input.</returns>
        public static Func<T, Task<Response<TResult>>> LogAsync<T, TResult>(this Func<T, Task<Response<TResult>>> func, Func<T, TResult, Format> success) => x =>
            func(x).ContinueWith(y =>
            {
                if (y.Status == TaskStatus.RanToCompletion && y.Result)
                {
                    Trace.Log(success(x, y.Result));
                }
                else
                {
                    if (y.Status == TaskStatus.Faulted) y.Exception.LogErrorPlain();
                }

                return y.Status == TaskStatus.RanToCompletion ? y.Result : Response<TResult>.Error;
            });

        /// <summary>Logs a trace step.</summary>
        /// <param name="func">A function to the Response Container.</param>
        /// <param name="success">The message to trace if the response is in a valid state.</param>
        /// <param name="fail">The message to trace when the response is in an invalid state.</param>
        /// <returns>The same response from the input.</returns>
        public static Func<T, Task<Response<TResult>>> LogAsync<T, TResult>(this Func<T, Task<Response<TResult>>> func, Func<T, TResult, Format> success, Format fail) => x =>
            func(x).ContinueWith(y =>
            {
                if (y.Status == TaskStatus.RanToCompletion && y.Result)
                {
                    Trace.Log(success(x, y.Result));
                }
                else
                {
                    Trace.Log(fail);
                    if (y.Status == TaskStatus.Faulted) y.Exception.LogErrorPlain();
                }

                return y.Status == TaskStatus.RanToCompletion ? y.Result : Response<TResult>.Error;
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
                if (x.Status == TaskStatus.Faulted)
                {
                    x.Exception.LogErrorPlain();
                }

                var message = x.Result.Match(value, error);
                Trace.Log(message);

                if (!x.Result._hasValue)
                {
                    TraceExtensions.LogErrorValue(x.Result._error, message, argument, caller, path, line);
                    TraceExtensions.LogErrorValue(x.Result.AggregateErrors, message, argument, caller, path, line);
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
                if (x.Status == TaskStatus.Faulted)
                {
                    Trace.Log(error);
                    x.Exception.LogErrorPlain();
                }

                var message = x.Result._hasValue ? value : error;
                Trace.Log(message);

                if (!x.Result._hasValue)
                {
                    TraceExtensions.LogErrorValue(x.Result._error, message, argument, caller, path, line);
                    TraceExtensions.LogErrorValue(x.Result.AggregateErrors, message, argument, caller, path, line);
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
                if (x.Status == TaskStatus.Faulted)
                {
                    x.Exception.LogErrorPlain();
                }

                var message = x.Result.Match(value, error);
                Trace.Log(message);

                if (!x.Result._hasValue)
                {
                    TraceExtensions.LogError(x.Result._error, message, argument, caller, path, line);
                    TraceExtensions.LogError(x.Result.AggregateErrors, message, argument, caller, path, line);
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
                if (x.Status == TaskStatus.Faulted)
                {
                    Trace.Log(error);
                    x.Exception.LogErrorPlain();
                }

                var message = x.Result._hasValue ? value : error;
                Trace.Log(message);

                if (!x.Result._hasValue)
                {
                    TraceExtensions.LogError(x.Result._error, message, argument, caller, path, line);
                    TraceExtensions.LogError(x.Result.AggregateErrors, message, argument, caller, path, line);
                }

                return x.Result;
            });

        #endregion
    }
}
