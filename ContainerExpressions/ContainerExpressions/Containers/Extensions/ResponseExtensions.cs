using System;
using System.Threading.Tasks;

namespace ContainerExpressions.Containers
{
    /// <summary>Utility methods for the Response Container.</summary>
    public static class ResponseExtensions
    {
        /// <summary>Creates a valid container response.</summary>
        public static Response<T> WithValue<T>(this Response<T> response, T value) => new Response<T>(value);

        /// <summary>Create a response container in an valid state.</summary>
        public static Response AsValid(this Response response) => new Response(true);

        /// <summary>Executes the bind func only if the input Response is valid, otherwise an invalid response is returned.</summary>
        public static Response<TResult> Bind<T, TResult>(this Response<T> response, Func<T, Response<TResult>> func) => response.IsValid ? func(response.Value) : new Response<TResult>();

        /// <summary>Executes the bind func only if the input Response is valid, otherwise an invalid response is returned.</summary>
        public static Task<Response<TResult>> BindAsync<T, TResult>(this Response<T> response, Func<T, Task<Response<TResult>>> func) => response.IsValid ? func(response.Value) : Task.FromResult(new Response<TResult>());

        /// <summary>Executes the bind func only if the input Response is valid, otherwise an invalid response is returned.</summary>
        public static Task<Response<TResult>> BindAsync<T, TResult>(this Task<Response<T>> response, Func<T, Task<Response<TResult>>> func) => response.ContinueWith(x => x.Result ? func(x.Result) : Task.FromResult(new Response<TResult>())).Unwrap();

        /// <summary>Gets the value, unless the state is invalid, then the default value is returned.</summary>
        public static T GetValueOrDefault<T>(this Response<T> response, T defaultValue) => response.IsValid ? response.Value : defaultValue;

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
        /// <param name="response">The result of the last ran code.</param>
        /// <param name="func">An error free function that maps one type to another.</param>
        /// <returns>The mapped response, or an invalid response if the input was in an invalid state.</returns>
        public static Task<Response<TResult>> TransformAsync<T, TResult>(this Task<Response<T>> response, Func<T, TResult> func) => response.ContinueWith(x => x.Result ? Response.Create(func(x.Result)) : new Response<TResult>());

        /// <summary>
        /// Executes one of the functions when the input response is valid, otherwise an invalid response is returned.
        /// <para>When the condition is true the first function is executed, otherwise the second function is executed.</para>
        /// </summary>
        public static Response<TResult> Pivot<T, TResult>(this Response<T> response, bool condition, Func<T, Response<TResult>> func1, Func<T, Response<TResult>> func2) =>
            response.IsValid ? (condition ? func1(response) : func2(response)) : new Response<TResult>();

        /// <summary>
        /// Executes one of the functions when the input response is valid, otherwise an invalid response is returned.
        /// <para>When the condition is true the first function is executed, otherwise the second function is executed.</para>
        /// </summary>
        public static Task<Response<TResult>> PivotAsync<T, TResult>(this Response<T> response, bool condition, Func<T, Task<Response<TResult>>> func1, Func<T, Task<Response<TResult>>> func2) =>
            response.IsValid ? (condition ? func1(response) : func2(response)) : Task.FromResult(new Response<TResult>());

        /// <summary>
        /// Executes one of the functions when the input response is valid, otherwise an invalid response is returned.
        /// <para>When the condition is true the first function is executed, otherwise the second function is executed.</para>
        /// </summary>
        public static Task<Response<TResult>> PivotAsync<T, TResult>(this Task<Response<T>> response, bool condition, Func<T, Task<Response<TResult>>> func1, Func<T, Task<Response<TResult>>> func2) =>
          response.ContinueWith(x =>  x.Result.IsValid ? (condition ? func1(x.Result) : func2(x.Result)) : Task.FromResult(new Response<TResult>())).Unwrap();

        /// <summary>When the Response is in a valid state the Func's result is returned, otherwise false is returned.</summary>
        public static bool IsTrue<T>(this Response<T> response, Func<T, bool> condition) => response.IsValid ? condition(response.Value) : false;
    }
}
