﻿using System;
using System.Threading.Tasks;

namespace ContainerExpressions.Containers
{
    /// <summary>Utility methods for the Response Container.</summary>
    public static class ResponseExtensions
    {
        /// <summary>Creates a valid container response.</summary>
        public static Response<T> With<T>(this Response<T> _, T value) => new Response<T>(value);

        /// <summary>Create a response container in a valid state.</summary>
        /// <param name="value">The response's value.</param>
        public static Response<T> ToResponse<T>(this T value) => new Response<T>(value);

        /// <summary>Create a response container in an valid state.</summary>
        public static Response AsValid(this Response _) => new Response(true);

        /// <summary>Create a response container in a valid state.</summary>
        private static Response<T> Create<T>(T value) => new Response<T>(value);

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
        public static Func<T, Response> Lift<T>(this Action<T> func) { return x => { func(x); return new Response(true); }; }

        /// <summary>Turn a function that doesn't return a Response, into one that does.</summary>
        public static Func<Response<T>> Lift<T>(this Func<T> func) => () => Create(func());

        /// <summary>Turn a function that doesn't return a Response, into one that does.</summary>
        public static Func<T, Response<TResult>> Lift<T, TResult>(this Func<T, TResult> func) => Create<Func<T, Response<TResult>>>(x => Create(func(x)));

        /// <summary>Turn an async function that doesn't return a Response, into one that does.</summary>
        public static Func<Task<Response<T>>> LiftAsync<T>(this Func<Task<T>> func) => () => func().ContinueWith(x => Create(x.Result));

        /// <summary>Turn an async function that doesn't return a task Response, into one that does.</summary>
        public static Func<T, Task<Response<TResult>>> LiftAsync<T, TResult>(this Func<T, Task<TResult>> func) => Create<Func<T, Task<Response<TResult>>>>(async x => Create(await func(x)));

        /**
         * For bind we have the following input => output scenarios.
         * Where input is the extension method type target, and output is the the return type from the func.
         * 
         * T        => Response<TResult>
         * Task<T>  => Response<TResult>
         * T        => Task<Response<TResult>>
         * Task<T>  => Task<Response<TResult>>
         * 
         * While above is the main use case, we also want to cover void scenarios where Response<TResult> is replaced with Response.
         * i.e. T => Task<Response<TResult>> becomes T => Task<Response>.
        **/

        /** T => Response<TResult> **/

        /// <summary>Executes the bind func, passing in T as an argument.</summary>
        public static Response<TResult> BindValue<T, TResult>(this T value, Func<T, Response<TResult>> func) => func(value);

        /// <summary>Executes the bind func, passing in T as an argument.</summary>
        public static Task<Response<TResult>> BindValueAsync<T, TResult>(this Task<T> value, Func<T, Response<TResult>> func) => value.ContinueWith(x => func(x.Result));

        /// <summary>Executes the bind func, passing in T as an argument.</summary>
        public static Task<Response<TResult>> BindValueAsync<T, TResult>(this T value, Func<T, Task<Response<TResult>>> func) => func(value);

        /// <summary>Executes the bind func, passing in T as an argument.</summary>
        public static Task<Response<TResult>> BindValueAsync<T, TResult>(this Task<T> value, Func<T, Task<Response<TResult>>> func) => value.ContinueWith(x => func(x.Result)).Unwrap();

        /** T => Response **/

        /// <summary>Executes the bind func, passing in T as an argument.</summary>
        public static Response BindValue<T>(this T value, Func<T, Response> func) => func(value);

        /// <summary>Executes the bind func, passing in T as an argument.</summary>
        public static Task<Response> BindValueAsync<T>(this Task<T> value, Func<T, Response> func) => value.ContinueWith(x => func(x.Result));

        /// <summary>Executes the bind func, passing in T as an argument.</summary>
        public static Task<Response> BindValueAsync<T>(this T value, Func<T, Task<Response>> func) => func(value);

        /// <summary>Executes the bind func, passing in T as an argument.</summary>
        public static Task<Response> BindValueAsync<T>(this Task<T> value, Func<T, Task<Response>> func) => value.ContinueWith(x => func(x.Result)).Unwrap();

        /** Response => Response<TResult> **/

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

        /** Response<T> => Response<Result> **/

        /// <summary>Executes the bind func only if the input Response is valid, otherwise an invalid response is returned.</summary>
        public static Response<TResult> Bind<T, TResult>(this Response<T> response, Func<T, Response<TResult>> func) => response ? func(response) : new Response<TResult>();

        /// <summary>Executes the bind func only if the input Response is valid, otherwise an invalid response is returned.</summary>
        public static Task<Response<TResult>> BindAsync<T, TResult>(this Task<Response<T>> response, Func<T, Response<TResult>> func) => response.ContinueWith(x => x.Result ? func(x.Result) : new Response<TResult>());

        /// <summary>Executes the bind func only if the input Response is valid, otherwise an invalid response is returned.</summary>
        public static Task<Response<TResult>> BindAsync<T, TResult>(this Response<T> response, Func<T, Task<Response<TResult>>> func) => response ? func(response) : Task.FromResult(new Response<TResult>());

        /// <summary>Executes the bind func only if the input Response is valid, otherwise an invalid response is returned.</summary>
        public static Task<Response<TResult>> BindAsync<T, TResult>(this Task<Response<T>> response, Func<T, Task<Response<TResult>>> func) => response.ContinueWith(x => x.Result ? func(x.Result) : Task.FromResult(new Response<TResult>())).Unwrap();

        /** Response<T> => Response **/

        /// <summary>Executes the bind func only if the input Response is valid, otherwise an invalid response is returned.</summary>
        public static Response Bind<T>(this Response<T> response, Func<T, Response> func) => response ? func(response) : new Response();

        /// <summary>Executes the bind func only if the input Response is valid, otherwise an invalid response is returned.</summary>
        public static Task<Response> BindAsync<T>(this Task<Response<T>> response, Func<T, Response> func) => response.ContinueWith(x => x.Result ? func(x.Result) : new Response());

        /// <summary>Executes the bind func only if the input Response is valid, otherwise an invalid response is returned.</summary>
        public static Task<Response> BindAsync<T>(this Response<T> response, Func<T, Task<Response>> func) => response ? func(response) : Task.FromResult(new Response());

        /// <summary>Executes the bind func only if the input Response is valid, otherwise an invalid response is returned.</summary>
        public static Task<Response> BindAsync<T>(this Task<Response<T>> response, Func<T, Task<Response>> func) => response.ContinueWith(x => x.Result ? func(x.Result) : Task.FromResult(new Response())).Unwrap();

        /// <summary>Gets the value, unless the state is invalid, then the default value is returned.</summary>
        public static T GetValueOrDefault<T>(this Response<T> response, T defaultValue) => response ? response : defaultValue;

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

        /// <summary>When the Response is in a valid state the Func's result is returned, otherwise false is returned.</summary>
        public static bool IsTrue<T>(this Response<T> response, Func<T, bool> condition) => response ? condition(response) : false;
    }
}
