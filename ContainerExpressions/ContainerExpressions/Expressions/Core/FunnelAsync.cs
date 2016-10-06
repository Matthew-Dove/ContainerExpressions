using ContainerExpressions.Containers;
using System;
using System.Threading.Tasks;

namespace ContainerExpressions.Expressions.Core
{
    internal static class FunnelAsync
    {
        public static Task<Response<TResult>> ConvergeAsync<T1, T2, TResult>(Task<Response<T1>> response1, Task<Response<T2>> response2, Func<T1, T2, TResult> func) => Task.WhenAll(response1, response2).ContinueWith(x => response1.Result && response2.Result ? Response.Create(func(response1.Result, response2.Result)) : new Response<TResult>());

        public static Task<Response<TResult>> ConvergeAsync<T1, T2, T3, TResult>(Task<Response<T1>> response1, Task<Response<T2>> response2, Task<Response<T3>> response3, Func<T1, T2, T3, TResult> func) => Task.WhenAll(response1, response2, response3).ContinueWith(x => response1.Result && response2.Result && response3.Result ? Response.Create(func(response1.Result, response2.Result, response3.Result)) : new Response<TResult>());

        public static Task<Response<TResult>> ConvergeAsync<T1, T2, T3, T4, TResult>(Task<Response<T1>> response1, Task<Response<T2>> response2, Task<Response<T3>> response3, Task<Response<T4>> response4, Func<T1, T2, T3, T4, TResult> func) => Task.WhenAll(response1, response2, response3, response4).ContinueWith(x => response1.Result && response2.Result && response3.Result && response4.Result ? Response.Create(func(response1.Result, response2.Result, response3.Result, response4.Result)) : new Response<TResult>());

        public static Task<Response<TResult>> ConvergeAsync<T1, T2, T3, T4, T5, TResult>(Task<Response<T1>> response1, Task<Response<T2>> response2, Task<Response<T3>> response3, Task<Response<T4>> response4, Task<Response<T5>> response5, Func<T1, T2, T3, T4, T5, TResult> func) => Task.WhenAll(response1, response2, response3, response4, response5).ContinueWith(x => response1.Result && response2.Result && response3.Result && response4.Result && response5.Result ? Response.Create(func(response1.Result, response2.Result, response3.Result, response4.Result, response5.Result)) : new Response<TResult>());

        public static Task<Response<TResult>> ConvergeAsync<T1, T2, T3, T4, T5, T6, TResult>(Task<Response<T1>> response1, Task<Response<T2>> response2, Task<Response<T3>> response3, Task<Response<T4>> response4, Task<Response<T5>> response5, Task<Response<T6>> response6, Func<T1, T2, T3, T4, T5, T6, TResult> func) => Task.WhenAll(response1, response2, response3, response4, response5, response6).ContinueWith(x => response1.Result && response2.Result && response3.Result && response4.Result && response5.Result && response6.Result ? Response.Create(func(response1.Result, response2.Result, response3.Result, response4.Result, response5.Result, response6.Result)) : new Response<TResult>());

        public static Task<Response<TResult>> ConvergeAsync<T1, T2, T3, T4, T5, T6, T7, TResult>(Task<Response<T1>> response1, Task<Response<T2>> response2, Task<Response<T3>> response3, Task<Response<T4>> response4, Task<Response<T5>> response5, Task<Response<T6>> response6, Task<Response<T7>> response7, Func<T1, T2, T3, T4, T5, T6, T7, TResult> func) => Task.WhenAll(response1, response2, response3, response4, response5, response6, response7).ContinueWith(x => response1.Result && response2.Result && response3.Result && response4.Result && response5.Result && response6.Result && response7.Result ? Response.Create(func(response1.Result, response2.Result, response3.Result, response4.Result, response5.Result, response6.Result, response7.Result)) : new Response<TResult>());

        public static Task<Response<TResult>> ConvergeAsync<T1, T2, T3, T4, T5, T6, T7, T8, TResult>(Task<Response<T1>> response1, Task<Response<T2>> response2, Task<Response<T3>> response3, Task<Response<T4>> response4, Task<Response<T5>> response5, Task<Response<T6>> response6, Task<Response<T7>> response7, Task<Response<T8>> response8, Func<T1, T2, T3, T4, T5, T6, T7, T8, TResult> func) => Task.WhenAll(response1, response2, response3, response4, response5, response6, response7, response8).ContinueWith(x => response1.Result && response2.Result && response3.Result && response4.Result && response5.Result && response6.Result && response7.Result && response8.Result ? Response.Create(func(response1.Result, response2.Result, response3.Result, response4.Result, response5.Result, response6.Result, response7.Result, response8.Result)) : new Response<TResult>());
    }
}
