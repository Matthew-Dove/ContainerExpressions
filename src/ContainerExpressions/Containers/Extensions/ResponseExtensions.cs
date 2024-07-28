using System;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace ContainerExpressions.Containers
{
    /// <summary>Utility methods for the Response Container.</summary>
    public static class ResponseExtensions
    {
        #region Utilities

        /// <summary>
        /// Takes a task's result, and puts it in a response container.
        /// <para>Aggregate errors are logged (if any), the task is expected to be completed (i.e. from a continuation).</para>
        /// </summary>
        /// <returns>A valid response, when the task runs to completion; otherwise an invalid response.</returns>
        private static Response TaskToResponse(Task value)
        {
            if (value.Status == TaskStatus.Faulted)
            {
                value.Exception.LogErrorPlain();
            }
            return new Response(value.Status == TaskStatus.RanToCompletion);
        }

        /// <summary>
        /// Extracts the value produced by a task, and puts it in a response container.
        /// <para>Aggregate errors are logged (if any), the task is expected to be completed (i.e. from a continuation).</para>
        /// </summary>
        /// <returns>A valid response, when the task produces a value; otherwise an invalid response.</returns>
        private static Response<T> TaskToResponse<T>(Task<T> value)
        {
            if (value.Status == TaskStatus.Faulted)
            {
                value.Exception.LogErrorPlain();
            }
            return value.Status == TaskStatus.RanToCompletion ? new Response<T>(value.Result) : new Response<T>();
        }

        #endregion

        #region Miscellaneous

        /// <summary>Creates a valid container response.</summary>
        public static Response<T> With<T>(this Response<T> _, T value) => new Response<T>(value);

        /// <summary>Create a response container in a valid state.</summary>
        /// <param name="value">The response's value.</param>
        public static Response<T> ToResponse<T>(this T value) => new Response<T>(value);

        /// <summary>Creates a task, that wraps a response container in a valid state.</summary>
        /// <param name="value">The response's value.</param>
        public static Task<Response<T>> ToResponseAsync<T>(this T value) => Task.FromResult(new Response<T>(value));

        /// <summary>Creates a task, that wraps a response container in a valid state.</summary>
        /// <param name="value">The response's value.</param>
        public static Task<Response> ToResponseTaskAsync<T>(this Task value) => value.ContinueWith(TaskToResponse);

        /// <summary>Creates a task, that wraps a response container in a valid state.</summary>
        /// <param name="value">The response's value.</param>
        public static Task<Response<T>> ToResponseTaskAsync<T>(this Task<T> value) => value.ContinueWith(TaskToResponse);

        /// <summary>Create a response container in an valid state.</summary>
        public static Response AsValid(this Response _) => new Response(true);

        /// <summary>Create a response container in a valid state.</summary>
        private static Response<T> Create<T>(T value) => new Response<T>(value);

        /// <summary>Gets the value, unless the state is invalid, then the default value is returned.</summary>
        public static T GetValueOrDefault<T>(this Response<T> response, T defaultValue) => response ? response : defaultValue;

        /// <summary>Gets the value, unless the state is invalid, or the task failed, then the default value is returned.</summary>
        public static Task<T> GetValueOrDefaultAsync<T>(
            this Task<Response<T>> response,
            T defaultValue,
            [CallerArgumentExpression(nameof(response))] string argument = "",
            [CallerMemberName] string caller = "",
            [CallerFilePath] string path = "",
            [CallerLineNumber] int line = 0
            ) => response.ContinueWith(t =>
        {
            if (t.Status == TaskStatus.Faulted) t.Exception.LogError(Format.Default, argument, caller, path, line);
            if (t.Status == TaskStatus.RanToCompletion && t.Result.IsValid) return t.Result.Value;
            return defaultValue;
        });

        /// <summary>When the Response is in a valid state the Func's result is returned, otherwise false is returned.</summary>
        public static bool IsTrue<T>(this Response<T> response, Func<T, bool> condition) => response ? condition(response) : false;

        /**
         * Overloaded types to target for containers.
         * Note: Actions, and Funcs can take N additional input parameters.
         * Note: All actions map to Func{Response}, as they don't return a type; and async void methods don't return a Task either.
         * 
         * [Void]
         * Response
         * Task{Response}
         * Func{Response}
         * Func{Task{Response}}
         * 
         * [Value]
         * T
         * Task{T}
         * Func{T}
         * Func{Task{T}}
         * 
         * [Response and Value]
         * Response{T}
         * Task{Response{T}}
         * Func{Response{T}}
         * Func{Task{Response{T}}}
        **/

        /// <summary>Converts the target into a Response Unit, based on the original response's IsValid state.</summary>
        public static Response<Unit> ToUnit(this Response response) => response.IsValid ? Unit.ResponseSuccess : Unit.ResponseError;

        /// <summary>Turn a function that returns a Response, into one that returns a Response Unit.</summary>
        public static Func<Response<Unit>> ToUnit(this Func<Response> func) => () => func().ToUnit();

        #endregion

        #region Lift

        /// <summary>Turn a Response into a function that wraps the initial Response.</summary>
        public static Func<Response<T>> Lift<T>(this Response<T> value) => () => value;

        /// <summary>Turn a Response into a function that wraps the initial Response.</summary>
        public static Func<Task<Response<T>>> LiftAsync<T>(this Response<T> value) => () => Task.FromResult(value);

        /// <summary>Turn a value into a function that wraps the initial value in a Response.</summary>
        public static Func<Response<T>> Lift<T>(this T value) => () => Create(value);

        /// <summary>Turn a value into a function that wraps the initial value in a Response.</summary>
        public static Func<Task<Response<T>>> LiftAsync<T>(this T value) => () => Task.FromResult(Create(value));

        /// <summary>Turn a function that doesn't return a Response, into one that does.</summary>
        public static Func<Response> Lift(this Action func) { return () => { func(); return new Response(true); }; }

        /// <summary>Turn a function that doesn't return a Response, into one that does.</summary>
        public static Func<T1, Response> Lift<T1>(this Action<T1> func) { return (t1) => { func(t1); return new Response(true); }; }

        /// <summary>Turn a function that doesn't return a Response, into one that does.</summary>
        public static Func<T1, T2, Response> Lift<T1, T2>(this Action<T1, T2> func) { return (t1, t2) => { func(t1, t2); return new Response(true); }; }

        /// <summary>Turn a function that doesn't return a Response, into one that does.</summary>
        public static Func<T1, T2, T3, Response> Lift<T1, T2, T3>(this Action<T1, T2, T3> func) { return (t1, t2, t3) => { func(t1, t2, t3); return new Response(true); }; }

        /// <summary>Turn a function that doesn't return a Response, into one that does.</summary>
        public static Func<T1, T2, T3, T4, Response> Lift<T1, T2, T3, T4>(this Action<T1, T2, T3, T4> func) { return (t1, t2, t3, t4) => { func(t1, t2, t3, t4); return new Response(true); }; }

        /// <summary>Turn a function that doesn't return a Response, into one that does.</summary>
        public static Func<T1, T2, T3, T4, T5, Response> Lift<T1, T2, T3, T4, T5>(this Action<T1, T2, T3, T4, T5> func) { return (t1, t2, t3, t4, t5) => { func(t1, t2, t3, t4, t5); return new Response(true); }; }

        /// <summary>Turn a function that doesn't return a Response, into one that does.</summary>
        public static Func<T1, T2, T3, T4, T5, T6, Response> Lift<T1, T2, T3, T4, T5, T6>(this Action<T1, T2, T3, T4, T5, T6> func) { return (t1, t2, t3, t4, t5, t6) => { func(t1, t2, t3, t4, t5, t6); return new Response(true); }; }

        /// <summary>Turn a function that doesn't return a Response, into one that does.</summary>
        public static Func<T1, T2, T3, T4, T5, T6, T7, Response> Lift<T1, T2, T3, T4, T5, T6, T7>(this Action<T1, T2, T3, T4, T5, T6, T7> func) { return (t1, t2, t3, t4, t5, t6, t7) => { func(t1, t2, t3, t4, t5, t6, t7); return new Response(true); }; }

        /// <summary>Turn a function that doesn't return a Response, into one that does.</summary>
        public static Func<T1, T2, T3, T4, T5, T6, T7, T8, Response> Lift<T1, T2, T3, T4, T5, T6, T7, T8>(this Action<T1, T2, T3, T4, T5, T6, T7, T8> func) { return (t1, t2, t3, t4, t5, t6, t7, t8) => { func(t1, t2, t3, t4, t5, t6, t7, t8); return new Response(true); }; }

        /// <summary>Turn a function that doesn't return a Response, into one that does.</summary>
        public static Func<Response<T>> Lift<T>(this Func<T> func) => () => Create(func());

        /// <summary>Turn a function that doesn't return a Response, into one that does.</summary>
        public static Func<T1, Response<TResult>> Lift<T1, TResult>(this Func<T1, TResult> func) => new Func<T1, Response<TResult>>((t1) => Create(func(t1)));

        /// <summary>Turn a function that doesn't return a Response, into one that does.</summary>
        public static Func<T1, T2, Response<TResult>> Lift<T1, T2, TResult>(this Func<T1, T2, TResult> func) => new Func<T1, T2, Response<TResult>>((t1, t2) => Create(func(t1, t2)));

        /// <summary>Turn a function that doesn't return a Response, into one that does.</summary>
        public static Func<T1, T2, T3, Response<TResult>> Lift<T1, T2, T3, TResult>(this Func<T1, T2, T3, TResult> func) => new Func<T1, T2, T3, Response<TResult>>((t1, t2, t3) => Create(func(t1, t2, t3)));

        /// <summary>Turn a function that doesn't return a Response, into one that does.</summary>
        public static Func<T1, T2, T3, T4, Response<TResult>> Lift<T1, T2, T3, T4, TResult>(this Func<T1, T2, T3, T4, TResult> func) => new Func<T1, T2, T3, T4, Response<TResult>>((t1, t2, t3, t4) => Create(func(t1, t2, t3, t4)));

        /// <summary>Turn a function that doesn't return a Response, into one that does.</summary>
        public static Func<T1, T2, T3, T4, T5, Response<TResult>> Lift<T1, T2, T3, T4, T5, TResult>(this Func<T1, T2, T3, T4, T5, TResult> func) => new Func<T1, T2, T3, T4, T5, Response<TResult>>((t1, t2, t3, t4, t5) => Create(func(t1, t2, t3, t4, t5)));

        /// <summary>Turn a function that doesn't return a Response, into one that does.</summary>
        public static Func<T1, T2, T3, T4, T5, T6, Response<TResult>> Lift<T1, T2, T3, T4, T5, T6, TResult>(this Func<T1, T2, T3, T4, T5, T6, TResult> func) => new Func<T1, T2, T3, T4, T5, T6, Response<TResult>>((t1, t2, t3, t4, t5, t6) => Create(func(t1, t2, t3, t4, t5, t6)));

        /// <summary>Turn a function that doesn't return a Response, into one that does.</summary>
        public static Func<T1, T2, T3, T4, T5, T6, T7, Response<TResult>> Lift<T1, T2, T3, T4, T5, T6, T7, TResult>(this Func<T1, T2, T3, T4, T5, T6, T7, TResult> func) => new Func<T1, T2, T3, T4, T5, T6, T7, Response<TResult>>((t1, t2, t3, t4, t5, t6, t7) => Create(func(t1, t2, t3, t4, t5, t6, t7)));

        /// <summary>Turn a function that doesn't return a Response, into one that does.</summary>
        public static Func<T1, T2, T3, T4, T5, T6, T7, T8, Response<TResult>> Lift<T1, T2, T3, T4, T5, T6, T7, T8, TResult>(this Func<T1, T2, T3, T4, T5, T6, T7, T8, TResult> func) => new Func<T1, T2, T3, T4, T5, T6, T7, T8, Response<TResult>>((t1, t2, t3, t4, t5, t6, t7, t8) => Create(func(t1, t2, t3, t4, t5, t6, t7, t8)));

        /// <summary>Turn an async function that doesn't return a Response, into one that does.</summary>
        public static Func<Task<Response<T>>> LiftAsync<T>(this Func<Task<T>> func) => () => func().ContinueWith(TaskToResponse);

        /// <summary>Turn an async function that doesn't return a task Response, into one that does.</summary>
        public static Func<T1, Task<Response<TResult>>> LiftAsync<T1, TResult>(this Func<T1, Task<TResult>> func) => new Func<T1, Task<Response<TResult>>>((t1) => func(t1).ContinueWith(TaskToResponse));

        /// <summary>Turn an async function that doesn't return a task Response, into one that does.</summary>
        public static Func<T1, T2, Task<Response<TResult>>> LiftAsync<T1, T2, TResult>(this Func<T1, T2, Task<TResult>> func) => new Func<T1, T2, Task<Response<TResult>>>((t1, t2) => func(t1, t2).ContinueWith(TaskToResponse));

        /// <summary>Turn an async function that doesn't return a task Response, into one that does.</summary>
        public static Func<T1, T2, T3, Task<Response<TResult>>> LiftAsync<T1, T2, T3, TResult>(this Func<T1, T2, T3, Task<TResult>> func) => new Func<T1, T2, T3, Task<Response<TResult>>>((t1, t2, t3) => func(t1, t2, t3).ContinueWith(TaskToResponse));

        /// <summary>Turn an async function that doesn't return a task Response, into one that does.</summary>
        public static Func<T1, T2, T3, T4, Task<Response<TResult>>> LiftAsync<T1, T2, T3, T4, TResult>(this Func<T1, T2, T3, T4, Task<TResult>> func) => new Func<T1, T2, T3, T4, Task<Response<TResult>>>((t1, t2, t3, t4) => func(t1, t2, t3, t4).ContinueWith(TaskToResponse));

        /// <summary>Turn an async function that doesn't return a task Response, into one that does.</summary>
        public static Func<T1, T2, T3, T4, T5, Task<Response<TResult>>> LiftAsync<T1, T2, T3, T4, T5, TResult>(this Func<T1, T2, T3, T4, T5, Task<TResult>> func) => new Func<T1, T2, T3, T4, T5, Task<Response<TResult>>>((t1, t2, t3, t4, t5) => func(t1, t2, t3, t4, t5).ContinueWith(TaskToResponse));

        /// <summary>Turn an async function that doesn't return a task Response, into one that does.</summary>
        public static Func<T1, T2, T3, T4, T5, T6, Task<Response<TResult>>> LiftAsync<T1, T2, T3, T4, T5, T6, TResult>(this Func<T1, T2, T3, T4, T5, T6, Task<TResult>> func) => new Func<T1, T2, T3, T4, T5, T6, Task<Response<TResult>>>((t1, t2, t3, t4, t5, t6) => func(t1, t2, t3, t4, t5, t6).ContinueWith(TaskToResponse));

        /// <summary>Turn an async function that doesn't return a task Response, into one that does.</summary>
        public static Func<T1, T2, T3, T4, T5, T6, T7, Task<Response<TResult>>> LiftAsync<T1, T2, T3, T4, T5, T6, T7, TResult>(this Func<T1, T2, T3, T4, T5, T6, T7, Task<TResult>> func) => new Func<T1, T2, T3, T4, T5, T6, T7, Task<Response<TResult>>>((t1, t2, t3, t4, t5, t6, t7) => func(t1, t2, t3, t4, t5, t6, t7).ContinueWith(TaskToResponse));

        /// <summary>Turn an async function that doesn't return a task Response, into one that does.</summary>
        public static Func<T1, T2, T3, T4, T5, T6, T7, T8, Task<Response<TResult>>> LiftAsync<T1, T2, T3, T4, T5, T6, T7, T8, TResult>(this Func<T1, T2, T3, T4, T5, T6, T7, T8, Task<TResult>> func) => new Func<T1, T2, T3, T4, T5, T6, T7, T8, Task<Response<TResult>>>((t1, t2, t3, t4, t5, t6, t7, t8) => func(t1, t2, t3, t4, t5, t6, t7, t8).ContinueWith(TaskToResponse));

        #endregion

        #region Bind

        /**
         * For bind we have the following input => output scenarios.
         * Where input is the extension method type target, and output is the the return type from the func.
         * 
         * T        => Response{TResult}
         * Task{T}  => Response{TResult}
         * T        => Task{Response{TResult}}
         * Task{T}  => Task{Response{TResult}}
         * 
         * While above is the main use case, we also want to cover void scenarios where Response{TResult} is replaced with Response.
         * i.e. T => Task{Response{TResult}} becomes T => Task{Response}.
        **/

        /** T => Response{TResult} **/

        /// <summary>Executes the bind func, passing in T as an argument.</summary>
        public static Response<TResult> BindValue<T, TResult>(this T value, Func<T, Response<TResult>> func) => func(value);

        /// <summary>Executes the bind func, passing in T as an argument.</summary>
        public static Task<Response<TResult>> BindValueAsync<T, TResult>(this Task<T> value, Func<T, Response<TResult>> func) => value.ContinueWith(TaskToResponse).ContinueWith(x => x.Result ? func(x.Result) : new Response<TResult>());

        /// <summary>Executes the bind func, passing in T as an argument.</summary>
        public static Task<Response<TResult>> BindValueAsync<T, TResult>(this T value, Func<T, Task<Response<TResult>>> func) => func(value);

        /// <summary>Executes the bind func, passing in T as an argument.</summary>
        public static Task<Response<TResult>> BindValueAsync<T, TResult>(this Task<T> value, Func<T, Task<Response<TResult>>> func) => value.ContinueWith(TaskToResponse).ContinueWith(x => x.Result ? func(x.Result) : Task.FromResult(new Response<TResult>())).Unwrap();

        /** T => Response **/

        /// <summary>Executes the bind func, passing in T as an argument.</summary>
        public static Response BindValue<T>(this T value, Func<T, Response> func) => func(value);

        /// <summary>Executes the bind func, passing in T as an argument.</summary>
        public static Task<Response> BindValueAsync<T>(this Task<T> value, Func<T, Response> func) => value.ContinueWith(TaskToResponse).ContinueWith(x => x.Result ? func(x.Result) : new Response());

        /// <summary>Executes the bind func, passing in T as an argument.</summary>
        public static Task<Response> BindValueAsync<T>(this T value, Func<T, Task<Response>> func) => func(value);

        /// <summary>Executes the bind func, passing in T as an argument.</summary>
        public static Task<Response> BindValueAsync<T>(this Task<T> value, Func<T, Task<Response>> func) => value.ContinueWith(TaskToResponse).ContinueWith(x => x.Result ? func(x.Result) : Task.FromResult(new Response())).Unwrap();

        /** Response => Response{TResult} **/

        /// <summary>Executes the bind func only if the input Response is valid, otherwise an invalid response is returned.</summary>
        public static Response<T> Bind<T>(this Response response, Func<Response<T>> func) => response ? func() : new Response<T>();

        /// <summary>Executes the bind func only if the input Response is valid, otherwise an invalid response is returned.</summary>
        public static Task<Response<T>> BindAsync<T>(this Task<Response> response, Func<Response<T>> func) => response.ContinueWith(x => x.Result ? func() : new Response<T>());

        /// <summary>Executes the bind func only if the input Response is valid, otherwise an invalid response is returned.</summary>
        public static Task<Response<T>> BindAsync<T>(this Response response, Func<Task<Response<T>>> func) => response ? func() : Task.FromResult(new Response<T>());

        /// <summary>Executes the bind func only if the input Response is valid, otherwise an invalid response is returned.</summary>
        public static Task<Response<T>> BindAsync<T>(this Task<Response> response, Func<Task<Response<T>>> func) => response.ContinueWith(x => x.Result ? func() : Task.FromResult(new Response<T>())).Unwrap();

        /** Response => Response **/

        /// <summary>Executes the bind func only if the input Response is valid, otherwise an invalid response is returned.</summary>
        public static Response Bind(this Response response, Func<Response> func) => response ? func() : new Response();

        /// <summary>Executes the bind func only if the input Response is valid, otherwise an invalid response is returned.</summary>
        public static Task<Response> BindAsync(this Task<Response> response, Func<Response> func) => response.ContinueWith(x => x.Result ? func() : new Response());

        /// <summary>Executes the bind func only if the input Response is valid, otherwise an invalid response is returned.</summary>
        public static Task<Response> BindAsync(this Response response, Func<Task<Response>> func) => response ? func() : Task.FromResult(new Response());

        /// <summary>Executes the bind func only if the input Response is valid, otherwise an invalid response is returned.</summary>
        public static Task<Response> BindAsync(this Task<Response> response, Func<Task<Response>> func) => response.ContinueWith(x => x.Result ? func() : Task.FromResult(new Response())).Unwrap();

        /** Response{T} => Response{Result} **/

        /// <summary>Executes the bind func only if the input Response is valid, otherwise an invalid response is returned.</summary>
        public static Response<TResult> Bind<T, TResult>(this Response<T> response, Func<T, Response<TResult>> func) => response ? func(response) : new Response<TResult>();

        /// <summary>Executes the bind func only if the input Response is valid, otherwise an invalid response is returned.</summary>
        public static Task<Response<TResult>> BindAsync<T, TResult>(this Task<Response<T>> response, Func<T, Response<TResult>> func) => response.ContinueWith(x => x.Result ? func(x.Result) : new Response<TResult>());

        /// <summary>Executes the bind func only if the input Response is valid, otherwise an invalid response is returned.</summary>
        public static Task<Response<TResult>> BindAsync<T, TResult>(this Response<T> response, Func<T, Task<Response<TResult>>> func) => response ? func(response) : Task.FromResult(new Response<TResult>());

        /// <summary>Executes the bind func only if the input Response is valid, otherwise an invalid response is returned.</summary>
        public static Task<Response<TResult>> BindAsync<T, TResult>(this Task<Response<T>> response, Func<T, Task<Response<TResult>>> func) => response.ContinueWith(x => x.Result ? func(x.Result) : Task.FromResult(new Response<TResult>())).Unwrap();

        /** Response{T} => Response **/

        /// <summary>Executes the bind func only if the input Response is valid, otherwise an invalid response is returned.</summary>
        public static Response Bind<T>(this Response<T> response, Func<T, Response> func) => response ? func(response) : new Response();

        /// <summary>Executes the bind func only if the input Response is valid, otherwise an invalid response is returned.</summary>
        public static Task<Response> BindAsync<T>(this Task<Response<T>> response, Func<T, Response> func) => response.ContinueWith(x => x.Result ? func(x.Result) : new Response());

        /// <summary>Executes the bind func only if the input Response is valid, otherwise an invalid response is returned.</summary>
        public static Task<Response> BindAsync<T>(this Response<T> response, Func<T, Task<Response>> func) => response ? func(response) : Task.FromResult(new Response());

        /// <summary>Executes the bind func only if the input Response is valid, otherwise an invalid response is returned.</summary>
        public static Task<Response> BindAsync<T>(this Task<Response<T>> response, Func<T, Task<Response>> func) => response.ContinueWith(x => x.Result ? func(x.Result) : Task.FromResult(new Response())).Unwrap();

        #endregion

        #region Transform

        /// <summary>Map the response to a calculated value.</summary>
        /// <typeparam name="T">The type of the pre-calculated value.</typeparam>
        /// <param name="response">The result of the last ran code.</param>
        /// <param name="value">The value to assign to response.</param>
        /// <returns>The calculated value, or an invalid response if the input was in an invalid state.</returns>
        public static Response<T> Transform<T>(this Response response, T value) => response ? Response.Create(value) : new Response<T>();

        /// <summary>Map the underlying Response type, to another type.</summary>
        /// <typeparam name="T">The type of the input response.</typeparam>
        /// <typeparam name="TResult">The type of the output response.</typeparam>
        /// <param name="response">The result of the last ran code.</param>
        /// <param name="func">An error free function that maps one type to another.</param>
        /// <returns>The mapped response, or an invalid response if the input was in an invalid state.</returns>
        public static Response<TResult> Transform<T, TResult>(this Response<T> response, Func<T, TResult> func) => response ? Response.Create(func(response)) : new Response<TResult>();

        /// <summary>Map the underlying Response type, to another type.</summary>
        /// <typeparam name="T">The type of the input response.</typeparam>
        /// <typeparam name="TResult">The type of the output response.</typeparam>
        /// <param name="term">The result of the last ran code.</param>
        /// <param name="func">An error free function that maps one type to another.</param>
        /// <returns>The mapped response, or an invalid response if the input was in an invalid state.</returns>
        public static Func<Response<TResult>> Transform<T, TResult>(this Func<Response<T>> term, Func<T, TResult> func) => () => {
            var result = term();
            if (result) return Response.Create(func(result));
            return new Response<TResult>();
        };

        /// <summary>Map the underlying Response type, to another type.</summary>
        /// <typeparam name="T1">The type of the input response.</typeparam>
        /// <typeparam name="T2">The type of the output response.</typeparam>
        /// <typeparam name="TResult">The output type of the transform function.</typeparam>
        /// <param name="term">The result of the last ran code.</param>
        /// <param name="func">An error free function that maps one type to another.</param>
        /// <returns>The mapped response, or an invalid response if the input was in an invalid state.</returns>
        public static Func<T1, Response<TResult>> Transform<T1, T2, TResult>(this Func<T1, Response<T2>> term, Func<T2, TResult> func) => x => term(x).Transform(func);

        /// <summary>Map the response to a calculated value.</summary>
        /// <typeparam name="T">The type of the pre-calculated value.</typeparam>
        /// <param name="response">The result of the last ran code.</param>
        /// <param name="value">The value to assign to response.</param>
        /// <returns>The calculated value, or an invalid response if the input was in an invalid state.</returns>
        public static Task<Response<T>> TransformAsync<T>(this Task<Response> response, T value) => response.ContinueWith(x => x.Result ? Response.Create(value) : new Response<T>());

        /// <summary>Map the underlying Response type, to another type.</summary>
        /// <typeparam name="T">The type of the input response.</typeparam>
        /// <typeparam name="TResult">The type of the output response.</typeparam>
        /// <param name="response">The result of the last ran code.</param>
        /// <param name="func">An error free function that maps one type to another.</param>
        /// <returns>The mapped response, or an invalid response if the input was in an invalid state.</returns>
        public static Task<Response<TResult>> TransformAsync<T, TResult>(this Task<Response<T>> response, Func<T, TResult> func) => response.ContinueWith(x => x.Result ? Response.Create(func(x.Result)) : new Response<TResult>());

        /// <summary>Map the underlying Response type, to another type.</summary>
        /// <typeparam name="T1">The type of the input response.</typeparam>
        /// <typeparam name="T2">The type of the output response.</typeparam>
        /// <typeparam name="TResult">The output type of the transform function.</typeparam>
        /// <param name="term">The result of the last ran code.</param>
        /// <param name="func">An error free function that maps one type to another.</param>
        /// <returns>The mapped response, or an invalid response if the input was in an invalid state.</returns>
        public static Func<T1, Task<Response<TResult>>> TransformAsync<T1, T2, TResult>(this Func<T1, Task<Response<T2>>> term, Func<T2, TResult> func) => x => term(x).TransformAsync(func);

        /// <summary>Map the underlying Response type, to another type.</summary>
        /// <typeparam name="T">The input type of the transform function.</typeparam>
        /// <typeparam name="TResult">The output type of the transform function.</typeparam>
        /// <param name="term">The result of the last ran code</param>
        /// <param name="func">An error free function that maps one type to another.</param>
        /// <returns>The mapped response, or an invalid response if the input was in an invalid state.</returns>
        public static Func<Task<Response<TResult>>> TransformAsync<T, TResult>(this Func<Task<Response<T>>> term, Func<T, TResult> func) => () => term().ContinueWith(x => x.Result ? Response.Create(func(x.Result)) : new Response<TResult>());

        #endregion

        #region Pivot

        /// <summary>
        /// Executes one of the functions when the input response is valid, otherwise an invalid response is returned.
        /// <para>When the condition is true the first function is executed, otherwise the second function is executed.</para>
        /// </summary>
        public static Response<T> Pivot<T>(this Response response, bool condition, Func<Response<T>> func1, Func<Response<T>> func2) => response ? (condition ? func1() : func2()) : new Response<T>();

        /// <summary>
        /// Executes one of the functions when the input response is valid, otherwise an invalid response is returned.
        /// <para>When the condition is true the first function is executed, otherwise the second function is executed.</para>
        /// </summary>
        public static Task<Response<T>> PivotAsync<T>(this Response response, bool condition, Func<Task<Response<T>>> func1, Func<Task<Response<T>>> func2) => response ? (condition ? func1() : func2()) : Task.FromResult(new Response<T>());

        /// <summary>
        /// Executes one of the functions when the input response is valid, otherwise an invalid response is returned.
        /// <para>When the condition is true the first function is executed, otherwise the second function is executed.</para>
        /// </summary>
        public static Task<Response<T>> PivotAsync<T>(this Task<Response> response, bool condition, Func<Task<Response<T>>> func1, Func<Task<Response<T>>> func2) => response.ContinueWith(x => x.Result ? (condition ? func1() : func2()) : Task.FromResult(new Response<T>())).Unwrap();

        /// <summary>
        /// Executes one of the functions when the input response is valid, otherwise an invalid response is returned.
        /// <para>When the condition is true the first function is executed, otherwise the second function is executed.</para>
        /// </summary>
        public static Task<Response<T>> PivotAsync<T>(this Task<Response> response, bool condition, Func<Response<T>> func1, Func<Response<T>> func2) => response.ContinueWith(x => x.Result ? (condition ? func1() : func2()) : new Response<T>());

        /// <summary>
        /// Executes one of the functions when the input response is valid, otherwise an invalid response is returned.
        /// <para>When the condition is true the first function is executed, otherwise the second function is executed.</para>
        /// </summary>
        public static Response<TResult> Pivot<T, TResult>(this Response<T> response, bool condition, Func<T, Response<TResult>> func1, Func<T, Response<TResult>> func2) => response ? (condition ? func1(response) : func2(response)) : new Response<TResult>();

        /// <summary>
        /// Executes one of the functions when the input response is valid, otherwise an invalid response is returned.
        /// <para>When the condition is true the first function is executed, otherwise the second function is executed.</para>
        /// </summary>
        public static Task<Response<TResult>> PivotAsync<T, TResult>(this Response<T> response, bool condition, Func<T, Task<Response<TResult>>> func1, Func<T, Task<Response<TResult>>> func2) => response ? (condition ? func1(response) : func2(response)) : Task.FromResult(new Response<TResult>());

        /// <summary>
        /// Executes one of the functions when the input response is valid, otherwise an invalid response is returned.
        /// <para>When the condition is true the first function is executed, otherwise the second function is executed.</para>
        /// </summary>
        public static Task<Response<TResult>> PivotAsync<T, TResult>(this Task<Response<T>> response, bool condition, Func<T, Task<Response<TResult>>> func1, Func<T, Task<Response<TResult>>> func2) => response.ContinueWith(x =>  x.Result ? (condition ? func1(x.Result) : func2(x.Result)) : Task.FromResult(new Response<TResult>())).Unwrap();

        /// <summary>
        /// Executes one of the functions when the input response is valid, otherwise an invalid response is returned.
        /// <para>When the condition is true the first function is executed, otherwise the second function is executed.</para>
        /// </summary>
        public static Task<Response<TResult>> PivotAsync<T, TResult>(this Task<Response<T>> response, bool condition, Func<T, Response<TResult>> func1, Func<T, Response<TResult>> func2) => response.ContinueWith(x => x.Result ? (condition ? func1(x.Result) : func2(x.Result)) : new Response<TResult>());

        #endregion

        #region Unpack

        /// <summary>Flattens nested Response types into a single one.</summary>
        public static Response Unpack(this Response<Response> response)
        {
            if (response.IsValid) return response.Value;
            return new Response();
        }

        /// <summary>Flattens nested Response types into a single one.</summary>
        public static Task<Response> UnpackAsync(
            this Task<Response<Response>> response,
            [CallerArgumentExpression(nameof(response))] string argument = "",
            [CallerMemberName] string caller = "",
            [CallerFilePath] string path = "",
            [CallerLineNumber] int line = 0
            )
        {
            return response.ContinueWith(t =>
            {
                if (t.Status == TaskStatus.RanToCompletion && t.Result.IsValid) return t.Result.Value;
                if (t.Status == TaskStatus.Faulted) t.Exception.LogError(Format.Default, argument, caller, path, line);
                return new Response();
            });
        }

        /// <summary>Flattens nested Response types into a single one.</summary>
        public static Task<Response> UnpackAsync(
            this Task<Response<Task<Response>>> response,
            [CallerArgumentExpression(nameof(response))] string argument = "",
            [CallerMemberName] string caller = "",
            [CallerFilePath] string path = "",
            [CallerLineNumber] int line = 0
            )
        {
            return response.ContinueWith(t =>
            {
                if (t.Status == TaskStatus.RanToCompletion && t.Result.IsValid) return t.Result.Value;
                if (t.Status == TaskStatus.Faulted) t.Exception.LogError(Format.Default, argument, caller, path, line);
                return Task.FromResult(new Response());
            }).ContinueWith(t =>
            {
                if (t.Status == TaskStatus.Faulted) t.Exception.LogError(Format.Default, argument, caller, path, line);
                if (t.Status == TaskStatus.RanToCompletion)
                {
                    if (t.Result.Status == TaskStatus.RanToCompletion) return t.Result.Result;
                    if (t.Result.Status == TaskStatus.Faulted) t.Exception.LogError(Format.Default, argument, caller, path, line);
                }
                return new Response();
            });
        }

        /// <summary>Flattens nested Response types into a single one.</summary>
        public static Response<T> Unpack<T>(this Response<Response<T>> response)
        {
            if (response.IsValid) return response.Value;
            return new Response<T>();
        }

        /// <summary>Flattens nested Response types into a single one.</summary>
        public static Task<Response<T>> UnpackAsync<T>(
            this Task<Response<Response<T>>> response,
            [CallerArgumentExpression(nameof(response))] string argument = "",
            [CallerMemberName] string caller = "",
            [CallerFilePath] string path = "",
            [CallerLineNumber] int line = 0
            )
        {
            return response.ContinueWith(t =>
            {
                if (t.Status == TaskStatus.RanToCompletion && t.Result.IsValid) return t.Result.Value;
                if (t.Status == TaskStatus.Faulted) t.Exception.LogError(Format.Default, argument, caller, path, line);
                return new Response<T>();
            });
        }

        /// <summary>Flattens nested Response types into a single one.</summary>
        public static Task<Response<T>> UnpackAsync<T>(
            this Task<Response<Task<Response<T>>>> response,
            [CallerArgumentExpression(nameof(response))] string argument = "",
            [CallerMemberName] string caller = "",
            [CallerFilePath] string path = "",
            [CallerLineNumber] int line = 0
            )
        {
            return response.ContinueWith(t =>
            {
                if (t.Status == TaskStatus.RanToCompletion && t.Result.IsValid) return t.Result.Value;
                if (t.Status == TaskStatus.Faulted) t.Exception.LogError(Format.Default, argument, caller, path, line);
                return Task.FromResult(new Response<T>());
            }).ContinueWith(t =>
            {
                if (t.Status == TaskStatus.Faulted) t.Exception.LogError(Format.Default, argument, caller, path, line);
                if (t.Status == TaskStatus.RanToCompletion)
                {
                    if (t.Result.Status == TaskStatus.RanToCompletion && t.Result.Result.IsValid) return t.Result.Result;
                    if (t.Result.Status == TaskStatus.Faulted) t.Exception.LogError(Format.Default, argument, caller, path, line);
                }
                return new Response<T>();
            });
        }

        #endregion
    }
}
