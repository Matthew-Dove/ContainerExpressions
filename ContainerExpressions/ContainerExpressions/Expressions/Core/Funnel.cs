using ContainerExpressions.Containers;
using System;

namespace ContainerExpressions.Expressions.Core
{
    internal static class Funnel
    {
        public static Response<TResult> Converge<T1, T2, TResult>(Response<T1> response1, Response<T2> response2, Func<T1, T2, TResult> func) => response1 && response2 ? Response.Create(func(response1, response2)) : new Response<TResult>();

        public static Response<TResult> Converge<T1, T2, T3, TResult>(this Response<T1> response1, Response<T2> response2, Response<T3> response3, Func<T1, T2, T3, TResult> func) => response1 && response2 && response3 ? Response.Create(func(response1, response2, response3)) : new Response<TResult>();

        public static Response<TResult> Converge<T1, T2, T3, T4, TResult>(this Response<T1> response1, Response<T2> response2, Response<T3> response3, Response<T4> response4, Func<T1, T2, T3, T4, TResult> func) => response1 && response2 && response3 && response4 ? Response.Create(func(response1, response2, response3, response4)) : new Response<TResult>();

        public static Response<TResult> Converge<T1, T2, T3, T4, T5, TResult>(this Response<T1> response1, Response<T2> response2, Response<T3> response3, Response<T4> response4, Response<T5> response5, Func<T1, T2, T3, T4, T5, TResult> func) => response1 && response2 && response3 && response4 && response5 ? Response.Create(func(response1, response2, response3, response4, response5)) : new Response<TResult>();

        public static Response<TResult> Converge<T1, T2, T3, T4, T5, T6, TResult>(this Response<T1> response1, Response<T2> response2, Response<T3> response3, Response<T4> response4, Response<T5> response5, Response<T6> response6, Func<T1, T2, T3, T4, T5, T6, TResult> func) => response1 && response2 && response3 && response4 && response5 && response6 ? Response.Create(func(response1, response2, response3, response4, response5, response6)) : new Response<TResult>();

        public static Response<TResult> Converge<T1, T2, T3, T4, T5, T6, T7, TResult>(this Response<T1> response1, Response<T2> response2, Response<T3> response3, Response<T4> response4, Response<T5> response5, Response<T6> response6, Response<T7> response7, Func<T1, T2, T3, T4, T5, T6, T7, TResult> func) => response1 && response2 && response3 && response4 && response5 && response6 && response7 ? Response.Create(func(response1, response2, response3, response4, response5, response6, response7)) : new Response<TResult>();

        public static Response<TResult> Converge<T1, T2, T3, T4, T5, T6, T7, T8, TResult>(this Response<T1> response1, Response<T2> response2, Response<T3> response3, Response<T4> response4, Response<T5> response5, Response<T6> response6, Response<T7> response7, Response<T8> response8, Func<T1, T2, T3, T4, T5, T6, T7, T8, TResult> func) => response1 && response2 && response3 && response4 && response5 && response6 && response7 && response8 ? Response.Create(func(response1, response2, response3, response4, response5, response6, response7, response8)) : new Response<TResult>();
    }
}
