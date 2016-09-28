using System;

namespace ContainerExpressions.Containers
{
    /// <summary>Filter down many responses into a single valid function call.</summary>
    public static class FunnelExtensions
    {
        /// <summary>Takes multiple Response types, and passes the results to the final function only if all Response types are in a valid state.</summary>
        public static Response<TResult> Funnel<T1, T2, TResult>(this Response<T1> response1, Response<T2> response2, Func<T1, T2, Response<TResult>> func) =>
            response1 && response2 ? func(response1, response2) : new Response<TResult>();

        /// <summary>Takes multiple Response types, and passes the results to the final function only if all Response types are in a valid state.</summary>
        public static Response<TResult> Funnel<T1, T2, T3, TResult>(this Response<T1> response1, Response<T2> response2, Response<T3> response3, Func<T1, T2, T3, Response<TResult>> func) =>
            response1 && response2 && response3 ? func(response1, response2, response3) : new Response<TResult>();

        /// <summary>Takes multiple Response types, and passes the results to the final function only if all Response types are in a valid state.</summary>
        public static Response<TResult> Funnel<T1, T2, T3, T4, TResult>(this Response<T1> response1, Response<T2> response2, Response<T3> response3, Response<T4> response4, Func<T1, T2, T3, T4, Response<TResult>> func) =>
            response1 && response2 && response3 && response4 ? func(response1, response2, response3, response4) : new Response<TResult>();

        /// <summary>Takes multiple Response types, and passes the results to the final function only if all Response types are in a valid state.</summary>
        public static Response<TResult> Funnel<T1, T2, T3, T4, T5, TResult>(this Response<T1> response1, Response<T2> response2, Response<T3> response3, Response<T4> response4, Response<T5> response5, Func<T1, T2, T3, T4, T5, Response<TResult>> func) =>
            response1 && response2 && response3 && response4 && response5 ? func(response1, response2, response3, response4, response5) : new Response<TResult>();

        /// <summary>Takes multiple Response types, and passes the results to the final function only if all Response types are in a valid state.</summary>
        public static Response<TResult> Funnel<T1, T2, T3, T4, T5, T6, TResult>(this Response<T1> response1, Response<T2> response2, Response<T3> response3, Response<T4> response4, Response<T5> response5, Response<T6> response6, Func<T1, T2, T3, T4, T5, T6, Response<TResult>> func) =>
            response1 && response2 && response3 && response4 && response5 && response6 ? func(response1, response2, response3, response4, response5, response6) : new Response<TResult>();

        /// <summary>Takes multiple Response types, and passes the results to the final function only if all Response types are in a valid state.</summary>
        public static Response<TResult> Funnel<T1, T2, T3, T4, T5, T6, T7, TResult>(this Response<T1> response1, Response<T2> response2, Response<T3> response3, Response<T4> response4, Response<T5> response5, Response<T6> response6, Response<T7> response7, Func<T1, T2, T3, T4, T5, T6, T7, Response<TResult>> func) =>
            response1 && response2 && response3 && response4 && response5 && response6 && response7 ? func(response1, response2, response3, response4, response5, response6, response7) : new Response<TResult>();

        /// <summary>Takes multiple Response types, and passes the results to the final function only if all Response types are in a valid state.</summary>
        public static Response<TResult> Funnel<T1, T2, T3, T4, T5, T6, T7, T8, TResult>(this Response<T1> response1, Response<T2> response2, Response<T3> response3, Response<T4> response4, Response<T5> response5, Response<T6> response6, Response<T7> response7, Response<T8> response8, Func<T1, T2, T3, T4, T5, T6, T7, T8, Response<TResult>> func) =>
            response1 && response2 && response3 && response4 && response5 && response6 && response7 && response8 ? func(response1, response2, response3, response4, response5, response6, response7, response8) : new Response<TResult>();
    }
}
