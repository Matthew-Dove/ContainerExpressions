﻿using ContainerExpressions.Containers;
using System;
using System.Threading.Tasks;

namespace ContainerExpressions.Expressions
{
    /// <summary>Entry class for using Expressions.</summary>
    public static class Expression
    {
        #region Reduce

        /// <summary>Reduce many values of T, into a single value of T.</summary>
        public static T Reduce<T>(Func<T, T, T> combine, T arg1, params Response<T>[] values) => Core.Reduce.Fold(combine, arg1, values);

        /// <summary>Reduce many values of T, into a single value of T.</summary>
        public static Task<T> ReduceAsync<T>(Func<T, T, T> combine, T arg1, params Task<Response<T>>[] values) => Core.Reduce.FoldAsync(combine, arg1, values);

        #endregion

        #region Funnel

        #region Synchronous

        // Callback: Func<T1, T2, TResult>

        /// <summary>Takes multiple Response types, and passes the results to the final function only if all Response types are in a valid state.</summary>
        public static Response<TResult> Funnel<T1, T2, TResult>(Response<T1> response1, Response<T2> response2, Func<T1, T2, TResult> func) => Core.Funnel.Converge(response1, response2, func);

        /// <summary>Takes multiple Response types, and passes the results to the final function only if all Response types are in a valid state.</summary>
        public static Response<TResult> Funnel<T1, T2, T3, TResult>(Response<T1> response1, Response<T2> response2, Response<T3> response3, Func<T1, T2, T3, TResult> func) => Core.Funnel.Converge(response1, response2, response3, func);

        /// <summary>Takes multiple Response types, and passes the results to the final function only if all Response types are in a valid state.</summary>
        public static Response<TResult> Funnel<T1, T2, T3, T4, TResult>(Response<T1> response1, Response<T2> response2, Response<T3> response3, Response<T4> response4, Func<T1, T2, T3, T4, TResult> func) => Core.Funnel.Converge(response1, response2, response3, response4, func);

        /// <summary>Takes multiple Response types, and passes the results to the final function only if all Response types are in a valid state.</summary>
        public static Response<TResult> Funnel<T1, T2, T3, T4, T5, TResult>(Response<T1> response1, Response<T2> response2, Response<T3> response3, Response<T4> response4, Response<T5> response5, Func<T1, T2, T3, T4, T5, TResult> func) => Core.Funnel.Converge(response1, response2, response3, response4, response5, func);

        /// <summary>Takes multiple Response types, and passes the results to the final function only if all Response types are in a valid state.</summary>
        public static Response<TResult> Funnel<T1, T2, T3, T4, T5, T6, TResult>(Response<T1> response1, Response<T2> response2, Response<T3> response3, Response<T4> response4, Response<T5> response5, Response<T6> response6, Func<T1, T2, T3, T4, T5, T6, TResult> func) => Core.Funnel.Converge(response1, response2, response3, response4, response5, response6, func);

        /// <summary>Takes multiple Response types, and passes the results to the final function only if all Response types are in a valid state.</summary>
        public static Response<TResult> Funnel<T1, T2, T3, T4, T5, T6, T7, TResult>(Response<T1> response1, Response<T2> response2, Response<T3> response3, Response<T4> response4, Response<T5> response5, Response<T6> response6, Response<T7> response7, Func<T1, T2, T3, T4, T5, T6, T7, TResult> func) => Core.Funnel.Converge(response1, response2, response3, response4, response5, response6, response7, func);

        /// <summary>Takes multiple Response types, and passes the results to the final function only if all Response types are in a valid state.</summary>
        public static Response<TResult> Funnel<T1, T2, T3, T4, T5, T6, T7, T8, TResult>(Response<T1> response1, Response<T2> response2, Response<T3> response3, Response<T4> response4, Response<T5> response5, Response<T6> response6, Response<T7> response7, Response<T8> response8, Func<T1, T2, T3, T4, T5, T6, T7, T8, TResult> func) => Core.Funnel.Converge(response1, response2, response3, response4, response5, response6, response7, response8, func);

        // Callback: Func<T1, T2, Task<TResult>>

        /// <summary>Takes multiple Response types, and passes the results to the final function only if all Response types are in a valid state.</summary>
        public static Task<Response<TResult>> FunnelAsync<T1, T2, TResult>(Response<T1> response1, Response<T2> response2, Func<T1, T2, Task<TResult>> func) => Core.Funnel.ConvergeAsync(response1, response2, func);

        /// <summary>Takes multiple Response types, and passes the results to the final function only if all Response types are in a valid state.</summary>
        public static Task<Response<TResult>> FunnelAsync<T1, T2, T3, TResult>(Response<T1> response1, Response<T2> response2, Response<T3> response3, Func<T1, T2, T3, Task<TResult>> func) => Core.Funnel.ConvergeAsync(response1, response2, response3, func);

        /// <summary>Takes multiple Response types, and passes the results to the final function only if all Response types are in a valid state.</summary>
        public static Task<Response<TResult>> FunnelAsync<T1, T2, T3, T4, TResult>(Response<T1> response1, Response<T2> response2, Response<T3> response3, Response<T4> response4, Func<T1, T2, T3, T4, Task<TResult>> func) => Core.Funnel.ConvergeAsync(response1, response2, response3, response4, func);

        /// <summary>Takes multiple Response types, and passes the results to the final function only if all Response types are in a valid state.</summary>
        public static Task<Response<TResult>> FunnelAsync<T1, T2, T3, T4, T5, TResult>(Response<T1> response1, Response<T2> response2, Response<T3> response3, Response<T4> response4, Response<T5> response5, Func<T1, T2, T3, T4, T5, Task<TResult>> func) => Core.Funnel.ConvergeAsync(response1, response2, response3, response4, response5, func);

        /// <summary>Takes multiple Response types, and passes the results to the final function only if all Response types are in a valid state.</summary>
        public static Task<Response<TResult>> FunnelAsync<T1, T2, T3, T4, T5, T6, TResult>(Response<T1> response1, Response<T2> response2, Response<T3> response3, Response<T4> response4, Response<T5> response5, Response<T6> response6, Func<T1, T2, T3, T4, T5, T6, Task<TResult>> func) => Core.Funnel.ConvergeAsync(response1, response2, response3, response4, response5, response6, func);

        /// <summary>Takes multiple Response types, and passes the results to the final function only if all Response types are in a valid state.</summary>
        public static Task<Response<TResult>> FunnelAsync<T1, T2, T3, T4, T5, T6, T7, TResult>(Response<T1> response1, Response<T2> response2, Response<T3> response3, Response<T4> response4, Response<T5> response5, Response<T6> response6, Response<T7> response7, Func<T1, T2, T3, T4, T5, T6, T7, Task<TResult>> func) => Core.Funnel.ConvergeAsync(response1, response2, response3, response4, response5, response6, response7, func);

        /// <summary>Takes multiple Response types, and passes the results to the final function only if all Response types are in a valid state.</summary>
        public static Task<Response<TResult>> FunnelAsync<T1, T2, T3, T4, T5, T6, T7, T8, TResult>(Response<T1> response1, Response<T2> response2, Response<T3> response3, Response<T4> response4, Response<T5> response5, Response<T6> response6, Response<T7> response7, Response<T8> response8, Func<T1, T2, T3, T4, T5, T6, T7, T8, Task<TResult>> func) => Core.Funnel.ConvergeAsync(response1, response2, response3, response4, response5, response6, response7, response8, func);

        // Callback: Func<T1, T2, Response<TResult>>

        /// <summary>Takes multiple Response types, and passes the results to the final function only if all Response types are in a valid state.</summary>
        public static Response<TResult> Funnel<T1, T2, TResult>(Response<T1> response1, Response<T2> response2, Func<T1, T2, Response<TResult>> func) => Core.Funnel.Converge(response1, response2, func);

        /// <summary>Takes multiple Response types, and passes the results to the final function only if all Response types are in a valid state.</summary>
        public static Response<TResult> Funnel<T1, T2, T3, TResult>(Response<T1> response1, Response<T2> response2, Response<T3> response3, Func<T1, T2, T3, Response<TResult>> func) => Core.Funnel.Converge(response1, response2, response3, func);

        /// <summary>Takes multiple Response types, and passes the results to the final function only if all Response types are in a valid state.</summary>
        public static Response<TResult> Funnel<T1, T2, T3, T4, TResult>(Response<T1> response1, Response<T2> response2, Response<T3> response3, Response<T4> response4, Func<T1, T2, T3, T4, Response<TResult>> func) => Core.Funnel.Converge(response1, response2, response3, response4, func);

        /// <summary>Takes multiple Response types, and passes the results to the final function only if all Response types are in a valid state.</summary>
        public static Response<TResult> Funnel<T1, T2, T3, T4, T5, TResult>(Response<T1> response1, Response<T2> response2, Response<T3> response3, Response<T4> response4, Response<T5> response5, Func<T1, T2, T3, T4, T5, Response<TResult>> func) => Core.Funnel.Converge(response1, response2, response3, response4, response5, func);

        /// <summary>Takes multiple Response types, and passes the results to the final function only if all Response types are in a valid state.</summary>
        public static Response<TResult> Funnel<T1, T2, T3, T4, T5, T6, TResult>(Response<T1> response1, Response<T2> response2, Response<T3> response3, Response<T4> response4, Response<T5> response5, Response<T6> response6, Func<T1, T2, T3, T4, T5, T6, Response<TResult>> func) => Core.Funnel.Converge(response1, response2, response3, response4, response5, response6, func);

        /// <summary>Takes multiple Response types, and passes the results to the final function only if all Response types are in a valid state.</summary>
        public static Response<TResult> Funnel<T1, T2, T3, T4, T5, T6, T7, TResult>(Response<T1> response1, Response<T2> response2, Response<T3> response3, Response<T4> response4, Response<T5> response5, Response<T6> response6, Response<T7> response7, Func<T1, T2, T3, T4, T5, T6, T7, Response<TResult>> func) => Core.Funnel.Converge(response1, response2, response3, response4, response5, response6, response7, func);

        /// <summary>Takes multiple Response types, and passes the results to the final function only if all Response types are in a valid state.</summary>
        public static Response<TResult> Funnel<T1, T2, T3, T4, T5, T6, T7, T8, TResult>(Response<T1> response1, Response<T2> response2, Response<T3> response3, Response<T4> response4, Response<T5> response5, Response<T6> response6, Response<T7> response7, Response<T8> response8, Func<T1, T2, T3, T4, T5, T6, T7, T8, Response<TResult>> func) => Core.Funnel.Converge(response1, response2, response3, response4, response5, response6, response7, response8, func);

        // Callback: Func<T1, T2, Task<Response<TResult>>>

        /// <summary>Takes multiple Response types, and passes the results to the final function only if all Response types are in a valid state.</summary>
        public static Task<Response<TResult>> FunnelAsync<T1, T2, TResult>(Response<T1> response1, Response<T2> response2, Func<T1, T2, Task<Response<TResult>>> func) => Core.Funnel.ConvergeAsync(response1, response2, func);

        /// <summary>Takes multiple Response types, and passes the results to the final function only if all Response types are in a valid state.</summary>
        public static Task<Response<TResult>> FunnelAsync<T1, T2, T3, TResult>(Response<T1> response1, Response<T2> response2, Response<T3> response3, Func<T1, T2, T3, Task<Response<TResult>>> func) => Core.Funnel.ConvergeAsync(response1, response2, response3, func);

        /// <summary>Takes multiple Response types, and passes the results to the final function only if all Response types are in a valid state.</summary>
        public static Task<Response<TResult>> FunnelAsync<T1, T2, T3, T4, TResult>(Response<T1> response1, Response<T2> response2, Response<T3> response3, Response<T4> response4, Func<T1, T2, T3, T4, Task<Response<TResult>>> func) => Core.Funnel.ConvergeAsync(response1, response2, response3, response4, func);

        /// <summary>Takes multiple Response types, and passes the results to the final function only if all Response types are in a valid state.</summary>
        public static Task<Response<TResult>> FunnelAsync<T1, T2, T3, T4, T5, TResult>(Response<T1> response1, Response<T2> response2, Response<T3> response3, Response<T4> response4, Response<T5> response5, Func<T1, T2, T3, T4, T5, Task<Response<TResult>>> func) => Core.Funnel.ConvergeAsync(response1, response2, response3, response4, response5, func);

        /// <summary>Takes multiple Response types, and passes the results to the final function only if all Response types are in a valid state.</summary>
        public static Task<Response<TResult>> FunnelAsync<T1, T2, T3, T4, T5, T6, TResult>(Response<T1> response1, Response<T2> response2, Response<T3> response3, Response<T4> response4, Response<T5> response5, Response<T6> response6, Func<T1, T2, T3, T4, T5, T6, Task<Response<TResult>>> func) => Core.Funnel.ConvergeAsync(response1, response2, response3, response4, response5, response6, func);

        /// <summary>Takes multiple Response types, and passes the results to the final function only if all Response types are in a valid state.</summary>
        public static Task<Response<TResult>> FunnelAsync<T1, T2, T3, T4, T5, T6, T7, TResult>(Response<T1> response1, Response<T2> response2, Response<T3> response3, Response<T4> response4, Response<T5> response5, Response<T6> response6, Response<T7> response7, Func<T1, T2, T3, T4, T5, T6, T7, Task<Response<TResult>>> func) => Core.Funnel.ConvergeAsync(response1, response2, response3, response4, response5, response6, response7, func);

        /// <summary>Takes multiple Response types, and passes the results to the final function only if all Response types are in a valid state.</summary>
        public static Task<Response<TResult>> FunnelAsync<T1, T2, T3, T4, T5, T6, T7, T8, TResult>(Response<T1> response1, Response<T2> response2, Response<T3> response3, Response<T4> response4, Response<T5> response5, Response<T6> response6, Response<T7> response7, Response<T8> response8, Func<T1, T2, T3, T4, T5, T6, T7, T8, Task<Response<TResult>>> func) => Core.Funnel.ConvergeAsync(response1, response2, response3, response4, response5, response6, response7, response8, func);

        #endregion

        #region Asynchronous

        // Callback: Func<T1, T2, TResult>

        /// <summary>Takes multiple Response types, and passes the results to the final function only if all Response types are in a valid state, all tasks are ran at the same time, not one after another.</summary>
        public static Task<Response<TResult>> FunnelAsync<T1, T2, TResult>(Task<Response<T1>> response1, Task<Response<T2>> response2, Func<T1, T2, TResult> func) => Core.FunnelAsync.ConvergeAsync(response1, response2, func);

        /// <summary>Takes multiple Response types, and passes the results to the final function only if all Response types are in a valid state, all tasks are ran at the same time, not one after another.</summary>
        public static Task<Response<TResult>> FunnelAsync<T1, T2, T3, TResult>(Task<Response<T1>> response1, Task<Response<T2>> response2, Task<Response<T3>> response3, Func<T1, T2, T3, TResult> func) => Core.FunnelAsync.ConvergeAsync(response1, response2, response3, func);

        /// <summary>Takes multiple Response types, and passes the results to the final function only if all Response types are in a valid state, all tasks are ran at the same time, not one after another.</summary>
        public static Task<Response<TResult>> FunnelAsync<T1, T2, T3, T4, TResult>(Task<Response<T1>> response1, Task<Response<T2>> response2, Task<Response<T3>> response3, Task<Response<T4>> response4, Func<T1, T2, T3, T4, TResult> func) => Core.FunnelAsync.ConvergeAsync(response1, response2, response3, response4, func);

        /// <summary>Takes multiple Response types, and passes the results to the final function only if all Response types are in a valid state, all tasks are ran at the same time, not one after another.</summary>
        public static Task<Response<TResult>> FunnelAsync<T1, T2, T3, T4, T5, TResult>(Task<Response<T1>> response1, Task<Response<T2>> response2, Task<Response<T3>> response3, Task<Response<T4>> response4, Task<Response<T5>> response5, Func<T1, T2, T3, T4, T5, TResult> func) => Core.FunnelAsync.ConvergeAsync(response1, response2, response3, response4, response5, func);

        /// <summary>Takes multiple Response types, and passes the results to the final function only if all Response types are in a valid state, all tasks are ran at the same time, not one after another.</summary>
        public static Task<Response<TResult>> FunnelAsync<T1, T2, T3, T4, T5, T6, TResult>(Task<Response<T1>> response1, Task<Response<T2>> response2, Task<Response<T3>> response3, Task<Response<T4>> response4, Task<Response<T5>> response5, Task<Response<T6>> response6, Func<T1, T2, T3, T4, T5, T6, TResult> func) => Core.FunnelAsync.ConvergeAsync(response1, response2, response3, response4, response5, response6, func);

        /// <summary>Takes multiple Response types, and passes the results to the final function only if all Response types are in a valid state, all tasks are ran at the same time, not one after another.</summary>
        public static Task<Response<TResult>> FunnelAsync<T1, T2, T3, T4, T5, T6, T7, TResult>(Task<Response<T1>> response1, Task<Response<T2>> response2, Task<Response<T3>> response3, Task<Response<T4>> response4, Task<Response<T5>> response5, Task<Response<T6>> response6, Task<Response<T7>> response7, Func<T1, T2, T3, T4, T5, T6, T7, TResult> func) => Core.FunnelAsync.ConvergeAsync(response1, response2, response3, response4, response5, response6, response7, func);

        /// <summary>Takes multiple Response types, and passes the results to the final function only if all Response types are in a valid state, all tasks are ran at the same time, not one after another.</summary>
        public static Task<Response<TResult>> FunnelAsync<T1, T2, T3, T4, T5, T6, T7, T8, TResult>(Task<Response<T1>> response1, Task<Response<T2>> response2, Task<Response<T3>> response3, Task<Response<T4>> response4, Task<Response<T5>> response5, Task<Response<T6>> response6, Task<Response<T7>> response7, Task<Response<T8>> response8, Func<T1, T2, T3, T4, T5, T6, T7, T8, TResult> func) => Core.FunnelAsync.ConvergeAsync(response1, response2, response3, response4, response5, response6, response7, response8, func);

        // Callback: Func<T1, T2, Task<TResult>>

        /// <summary>Takes multiple Response types, and passes the results to the final function only if all Response types are in a valid state, all tasks are ran at the same time, not one after another.</summary>
        public static Task<Response<TResult>> FunnelAsync<T1, T2, TResult>(Task<Response<T1>> response1, Task<Response<T2>> response2, Func<T1, T2, Task<TResult>> func) => Core.FunnelAsync.ConvergeAsync(response1, response2, func);

        /// <summary>Takes multiple Response types, and passes the results to the final function only if all Response types are in a valid state, all tasks are ran at the same time, not one after another.</summary>
        public static Task<Response<TResult>> FunnelAsync<T1, T2, T3, TResult>(Task<Response<T1>> response1, Task<Response<T2>> response2, Task<Response<T3>> response3, Func<T1, T2, T3, Task<TResult>> func) => Core.FunnelAsync.ConvergeAsync(response1, response2, response3, func);

        /// <summary>Takes multiple Response types, and passes the results to the final function only if all Response types are in a valid state, all tasks are ran at the same time, not one after another.</summary>
        public static Task<Response<TResult>> FunnelAsync<T1, T2, T3, T4, TResult>(Task<Response<T1>> response1, Task<Response<T2>> response2, Task<Response<T3>> response3, Task<Response<T4>> response4, Func<T1, T2, T3, T4, Task<TResult>> func) => Core.FunnelAsync.ConvergeAsync(response1, response2, response3, response4, func);

        /// <summary>Takes multiple Response types, and passes the results to the final function only if all Response types are in a valid state, all tasks are ran at the same time, not one after another.</summary>
        public static Task<Response<TResult>> FunnelAsync<T1, T2, T3, T4, T5, TResult>(Task<Response<T1>> response1, Task<Response<T2>> response2, Task<Response<T3>> response3, Task<Response<T4>> response4, Task<Response<T5>> response5, Func<T1, T2, T3, T4, T5, Task<TResult>> func) => Core.FunnelAsync.ConvergeAsync(response1, response2, response3, response4, response5, func);

        /// <summary>Takes multiple Response types, and passes the results to the final function only if all Response types are in a valid state, all tasks are ran at the same time, not one after another.</summary>
        public static Task<Response<TResult>> FunnelAsync<T1, T2, T3, T4, T5, T6, TResult>(Task<Response<T1>> response1, Task<Response<T2>> response2, Task<Response<T3>> response3, Task<Response<T4>> response4, Task<Response<T5>> response5, Task<Response<T6>> response6, Func<T1, T2, T3, T4, T5, T6, Task<TResult>> func) => Core.FunnelAsync.ConvergeAsync(response1, response2, response3, response4, response5, response6, func);

        /// <summary>Takes multiple Response types, and passes the results to the final function only if all Response types are in a valid state, all tasks are ran at the same time, not one after another.</summary>
        public static Task<Response<TResult>> FunnelAsync<T1, T2, T3, T4, T5, T6, T7, TResult>(Task<Response<T1>> response1, Task<Response<T2>> response2, Task<Response<T3>> response3, Task<Response<T4>> response4, Task<Response<T5>> response5, Task<Response<T6>> response6, Task<Response<T7>> response7, Func<T1, T2, T3, T4, T5, T6, T7, Task<TResult>> func) => Core.FunnelAsync.ConvergeAsync(response1, response2, response3, response4, response5, response6, response7, func);

        /// <summary>Takes multiple Response types, and passes the results to the final function only if all Response types are in a valid state, all tasks are ran at the same time, not one after another.</summary>
        public static Task<Response<TResult>> FunnelAsync<T1, T2, T3, T4, T5, T6, T7, T8, TResult>(Task<Response<T1>> response1, Task<Response<T2>> response2, Task<Response<T3>> response3, Task<Response<T4>> response4, Task<Response<T5>> response5, Task<Response<T6>> response6, Task<Response<T7>> response7, Task<Response<T8>> response8, Func<T1, T2, T3, T4, T5, T6, T7, T8, Task<TResult>> func) => Core.FunnelAsync.ConvergeAsync(response1, response2, response3, response4, response5, response6, response7, response8, func);

        // Callback: Func<T1, T2, Response<TResult>>

        /// <summary>Takes multiple Response types, and passes the results to the final function only if all Response types are in a valid state, all tasks are ran at the same time, not one after another.</summary>
        public static Task<Response<TResult>> FunnelAsync<T1, T2, TResult>(Task<Response<T1>> response1, Task<Response<T2>> response2, Func<T1, T2, Response<TResult>> func) => Core.FunnelAsync.ConvergeAsync(response1, response2, func);

        /// <summary>Takes multiple Response types, and passes the results to the final function only if all Response types are in a valid state, all tasks are ran at the same time, not one after another.</summary>
        public static Task<Response<TResult>> FunnelAsync<T1, T2, T3, TResult>(Task<Response<T1>> response1, Task<Response<T2>> response2, Task<Response<T3>> response3, Func<T1, T2, T3, Response<TResult>> func) => Core.FunnelAsync.ConvergeAsync(response1, response2, response3, func);

        /// <summary>Takes multiple Response types, and passes the results to the final function only if all Response types are in a valid state, all tasks are ran at the same time, not one after another.</summary>
        public static Task<Response<TResult>> FunnelAsync<T1, T2, T3, T4, TResult>(Task<Response<T1>> response1, Task<Response<T2>> response2, Task<Response<T3>> response3, Task<Response<T4>> response4, Func<T1, T2, T3, T4, Response<TResult>> func) => Core.FunnelAsync.ConvergeAsync(response1, response2, response3, response4, func);

        /// <summary>Takes multiple Response types, and passes the results to the final function only if all Response types are in a valid state, all tasks are ran at the same time, not one after another.</summary>
        public static Task<Response<TResult>> FunnelAsync<T1, T2, T3, T4, T5, TResult>(Task<Response<T1>> response1, Task<Response<T2>> response2, Task<Response<T3>> response3, Task<Response<T4>> response4, Task<Response<T5>> response5, Func<T1, T2, T3, T4, T5, Response<TResult>> func) => Core.FunnelAsync.ConvergeAsync(response1, response2, response3, response4, response5, func);

        /// <summary>Takes multiple Response types, and passes the results to the final function only if all Response types are in a valid state, all tasks are ran at the same time, not one after another.</summary>
        public static Task<Response<TResult>> FunnelAsync<T1, T2, T3, T4, T5, T6, TResult>(Task<Response<T1>> response1, Task<Response<T2>> response2, Task<Response<T3>> response3, Task<Response<T4>> response4, Task<Response<T5>> response5, Task<Response<T6>> response6, Func<T1, T2, T3, T4, T5, T6, Response<TResult>> func) => Core.FunnelAsync.ConvergeAsync(response1, response2, response3, response4, response5, response6, func);

        /// <summary>Takes multiple Response types, and passes the results to the final function only if all Response types are in a valid state, all tasks are ran at the same time, not one after another.</summary>
        public static Task<Response<TResult>> FunnelAsync<T1, T2, T3, T4, T5, T6, T7, TResult>(Task<Response<T1>> response1, Task<Response<T2>> response2, Task<Response<T3>> response3, Task<Response<T4>> response4, Task<Response<T5>> response5, Task<Response<T6>> response6, Task<Response<T7>> response7, Func<T1, T2, T3, T4, T5, T6, T7, Response<TResult>> func) => Core.FunnelAsync.ConvergeAsync(response1, response2, response3, response4, response5, response6, response7, func);

        /// <summary>Takes multiple Response types, and passes the results to the final function only if all Response types are in a valid state, all tasks are ran at the same time, not one after another.</summary>
        public static Task<Response<TResult>> FunnelAsync<T1, T2, T3, T4, T5, T6, T7, T8, TResult>(Task<Response<T1>> response1, Task<Response<T2>> response2, Task<Response<T3>> response3, Task<Response<T4>> response4, Task<Response<T5>> response5, Task<Response<T6>> response6, Task<Response<T7>> response7, Task<Response<T8>> response8, Func<T1, T2, T3, T4, T5, T6, T7, T8, Response<TResult>> func) => Core.FunnelAsync.ConvergeAsync(response1, response2, response3, response4, response5, response6, response7, response8, func);

        // Callback: Func<T1, T2, Task<Response<TResult>>>

        /// <summary>Takes multiple Response types, and passes the results to the final function only if all Response types are in a valid state, all tasks are ran at the same time, not one after another.</summary>
        public static Task<Response<TResult>> FunnelAsync<T1, T2, TResult>(Task<Response<T1>> response1, Task<Response<T2>> response2, Func<T1, T2, Task<Response<TResult>>> func) => Core.FunnelAsync.ConvergeAsync(response1, response2, func);

        /// <summary>Takes multiple Response types, and passes the results to the final function only if all Response types are in a valid state, all tasks are ran at the same time, not one after another.</summary>
        public static Task<Response<TResult>> FunnelAsync<T1, T2, T3, TResult>(Task<Response<T1>> response1, Task<Response<T2>> response2, Task<Response<T3>> response3, Func<T1, T2, T3, Task<Response<TResult>>> func) => Core.FunnelAsync.ConvergeAsync(response1, response2, response3, func);

        /// <summary>Takes multiple Response types, and passes the results to the final function only if all Response types are in a valid state, all tasks are ran at the same time, not one after another.</summary>
        public static Task<Response<TResult>> FunnelAsync<T1, T2, T3, T4, TResult>(Task<Response<T1>> response1, Task<Response<T2>> response2, Task<Response<T3>> response3, Task<Response<T4>> response4, Func<T1, T2, T3, T4, Task<Response<TResult>>> func) => Core.FunnelAsync.ConvergeAsync(response1, response2, response3, response4, func);

        /// <summary>Takes multiple Response types, and passes the results to the final function only if all Response types are in a valid state, all tasks are ran at the same time, not one after another.</summary>
        public static Task<Response<TResult>> FunnelAsync<T1, T2, T3, T4, T5, TResult>(Task<Response<T1>> response1, Task<Response<T2>> response2, Task<Response<T3>> response3, Task<Response<T4>> response4, Task<Response<T5>> response5, Func<T1, T2, T3, T4, T5, Task<Response<TResult>>> func) => Core.FunnelAsync.ConvergeAsync(response1, response2, response3, response4, response5, func);

        /// <summary>Takes multiple Response types, and passes the results to the final function only if all Response types are in a valid state, all tasks are ran at the same time, not one after another.</summary>
        public static Task<Response<TResult>> FunnelAsync<T1, T2, T3, T4, T5, T6, TResult>(Task<Response<T1>> response1, Task<Response<T2>> response2, Task<Response<T3>> response3, Task<Response<T4>> response4, Task<Response<T5>> response5, Task<Response<T6>> response6, Func<T1, T2, T3, T4, T5, T6, Task<Response<TResult>>> func) => Core.FunnelAsync.ConvergeAsync(response1, response2, response3, response4, response5, response6, func);

        /// <summary>Takes multiple Response types, and passes the results to the final function only if all Response types are in a valid state, all tasks are ran at the same time, not one after another.</summary>
        public static Task<Response<TResult>> FunnelAsync<T1, T2, T3, T4, T5, T6, T7, TResult>(Task<Response<T1>> response1, Task<Response<T2>> response2, Task<Response<T3>> response3, Task<Response<T4>> response4, Task<Response<T5>> response5, Task<Response<T6>> response6, Task<Response<T7>> response7, Func<T1, T2, T3, T4, T5, T6, T7, Task<Response<TResult>>> func) => Core.FunnelAsync.ConvergeAsync(response1, response2, response3, response4, response5, response6, response7, func);

        /// <summary>Takes multiple Response types, and passes the results to the final function only if all Response types are in a valid state, all tasks are ran at the same time, not one after another.</summary>
        public static Task<Response<TResult>> FunnelAsync<T1, T2, T3, T4, T5, T6, T7, T8, TResult>(Task<Response<T1>> response1, Task<Response<T2>> response2, Task<Response<T3>> response3, Task<Response<T4>> response4, Task<Response<T5>> response5, Task<Response<T6>> response6, Task<Response<T7>> response7, Task<Response<T8>> response8, Func<T1, T2, T3, T4, T5, T6, T7, T8, Task<Response<TResult>>> func) => Core.FunnelAsync.ConvergeAsync(response1, response2, response3, response4, response5, response6, response7, response8, func);

        #endregion

        #endregion

        #region Compose

        #region Synchronous

        #region ResponseT

        /// <summary>Compose functions by giving the output of the first function, to the input of the second function.</summary>
        public static Response<TResult> Compose<T1, TResult>(Func<Response<T1>> func1, Func<T1, Response<TResult>> funcResult) => Core.Compose.Evaluate(func1, funcResult);
        /// <summary>Compose functions by giving the output of the first function, to the input of the second function.</summary>
        public static Response<TResult> Compose<T1, T2, TResult>(Func<Response<T1>> func1, Func<T1, Response<T2>> func2, Func<T2, Response<TResult>> funcResult) => Core.Compose.Evaluate(func1, func2, funcResult);
        /// <summary>Compose functions by giving the output of the first function, to the input of the second function.</summary>
        public static Response<TResult> Compose<T1, T2, T3, TResult>(Func<Response<T1>> func1, Func<T1, Response<T2>> func2, Func<T2, Response<T3>> func3, Func<T3, Response<TResult>> funcResult) => Core.Compose.Evaluate(func1, func2, func3, funcResult);
        /// <summary>Compose functions by giving the output of the first function, to the input of the second function.</summary>
        public static Response<TResult> Compose<T1, T2, T3, T4, TResult>(Func<Response<T1>> func1, Func<T1, Response<T2>> func2, Func<T2, Response<T3>> func3, Func<T3, Response<T4>> func4, Func<T4, Response<TResult>> funcResult) => Core.Compose.Evaluate(func1, func2, func3, func4, funcResult);
        /// <summary>Compose functions by giving the output of the first function, to the input of the second function.</summary>
        public static Response<TResult> Compose<T1, T2, T3, T4, T5, TResult>(Func<Response<T1>> func1, Func<T1, Response<T2>> func2, Func<T2, Response<T3>> func3, Func<T3, Response<T4>> func4, Func<T4, Response<T5>> func5, Func<T5, Response<TResult>> funcResult) => Core.Compose.Evaluate(func1, func2, func3, func4, func5, funcResult);
        /// <summary>Compose functions by giving the output of the first function, to the input of the second function.</summary>
        public static Response<TResult> Compose<T1, T2, T3, T4, T5, T6, TResult>(Func<Response<T1>> func1, Func<T1, Response<T2>> func2, Func<T2, Response<T3>> func3, Func<T3, Response<T4>> func4, Func<T4, Response<T5>> func5, Func<T5, Response<T6>> func6, Func<T6, Response<TResult>> funcResult) => Core.Compose.Evaluate(func1, func2, func3, func4, func5, func6, funcResult);
        /// <summary>Compose functions by giving the output of the first function, to the input of the second function.</summary>
        public static Response<TResult> Compose<T1, T2, T3, T4, T5, T6, T7, TResult>(Func<Response<T1>> func1, Func<T1, Response<T2>> func2, Func<T2, Response<T3>> func3, Func<T3, Response<T4>> func4, Func<T4, Response<T5>> func5, Func<T5, Response<T6>> func6, Func<T6, Response<T7>> func7, Func<T7, Response<TResult>> funcResult) => Core.Compose.Evaluate(func1, func2, func3, func4, func5, func6, func7, funcResult);
        /// <summary>Compose functions by giving the output of the first function, to the input of the second function.</summary>
        public static Response<TResult> Compose<T1, T2, T3, T4, T5, T6, T7, T8, TResult>(Func<Response<T1>> func1, Func<T1, Response<T2>> func2, Func<T2, Response<T3>> func3, Func<T3, Response<T4>> func4, Func<T4, Response<T5>> func5, Func<T5, Response<T6>> func6, Func<T6, Response<T7>> func7, Func<T7, Response<T8>> func8, Func<T8, Response<TResult>> funcResult) => Core.Compose.Evaluate(func1, func2, func3, func4, func5, func6, func7, func8, funcResult);

        #endregion

        #region Response

        /// <summary>Compose functions by giving the output of the first function, to the input of the second function.</summary>
        public static Response Compose<T1>(Func<Response<T1>> func1, Func<T1, Response> funcResult) => Core.Compose.Evaluate(func1, funcResult);
        /// <summary>Compose functions by giving the output of the first function, to the input of the second function.</summary>
        public static Response Compose<T1, T2>(Func<Response<T1>> func1, Func<T1, Response<T2>> func2, Func<T2, Response> funcResult) => Core.Compose.Evaluate(func1, func2, funcResult);
        /// <summary>Compose functions by giving the output of the first function, to the input of the second function.</summary>
        public static Response Compose<T1, T2, T3>(Func<Response<T1>> func1, Func<T1, Response<T2>> func2, Func<T2, Response<T3>> func3, Func<T3, Response> funcResult) => Core.Compose.Evaluate(func1, func2, func3, funcResult);
        /// <summary>Compose functions by giving the output of the first function, to the input of the second function.</summary>
        public static Response Compose<T1, T2, T3, T4>(Func<Response<T1>> func1, Func<T1, Response<T2>> func2, Func<T2, Response<T3>> func3, Func<T3, Response<T4>> func4, Func<T4, Response> funcResult) => Core.Compose.Evaluate(func1, func2, func3, func4, funcResult);
        /// <summary>Compose functions by giving the output of the first function, to the input of the second function.</summary>
        public static Response Compose<T1, T2, T3, T4, T5>(Func<Response<T1>> func1, Func<T1, Response<T2>> func2, Func<T2, Response<T3>> func3, Func<T3, Response<T4>> func4, Func<T4, Response<T5>> func5, Func<T5, Response> funcResult) => Core.Compose.Evaluate(func1, func2, func3, func4, func5, funcResult);
        /// <summary>Compose functions by giving the output of the first function, to the input of the second function.</summary>
        public static Response Compose<T1, T2, T3, T4, T5, T6>(Func<Response<T1>> func1, Func<T1, Response<T2>> func2, Func<T2, Response<T3>> func3, Func<T3, Response<T4>> func4, Func<T4, Response<T5>> func5, Func<T5, Response<T6>> func6, Func<T6, Response> funcResult) => Core.Compose.Evaluate(func1, func2, func3, func4, func5, func6, funcResult);
        /// <summary>Compose functions by giving the output of the first function, to the input of the second function.</summary>
        public static Response Compose<T1, T2, T3, T4, T5, T6, T7>(Func<Response<T1>> func1, Func<T1, Response<T2>> func2, Func<T2, Response<T3>> func3, Func<T3, Response<T4>> func4, Func<T4, Response<T5>> func5, Func<T5, Response<T6>> func6, Func<T6, Response<T7>> func7, Func<T7, Response> funcResult) => Core.Compose.Evaluate(func1, func2, func3, func4, func5, func6, func7, funcResult);
        /// <summary>Compose functions by giving the output of the first function, to the input of the second function.</summary>
        public static Response Compose<T1, T2, T3, T4, T5, T6, T7, T8>(Func<Response<T1>> func1, Func<T1, Response<T2>> func2, Func<T2, Response<T3>> func3, Func<T3, Response<T4>> func4, Func<T4, Response<T5>> func5, Func<T5, Response<T6>> func6, Func<T6, Response<T7>> func7, Func<T7, Response<T8>> func8, Func<T8, Response> funcResult) => Core.Compose.Evaluate(func1, func2, func3, func4, func5, func6, func7, func8, funcResult);

        #endregion

        #endregion

        #region Asynchronous

        #region ResponseT

        /// <summary>Compose functions by giving the output of the first function, to the input of the second function.</summary>
        public static Task<Response<TResult>> ComposeAsync<T1, TResult>(Func<Task<Response<T1>>> func1, Func<T1, Task<Response<TResult>>> funcResult) => Core.ComposeAsync.EvaluateAsync(func1, funcResult);
        /// <summary>Compose functions by giving the output of the first function, to the input of the second function.</summary>
        public static Task<Response<TResult>> ComposeAsync<T1, T2, TResult>(Func<Task<Response<T1>>> func1, Func<T1, Task<Response<T2>>> func2, Func<T2, Task<Response<TResult>>> funcResult) => Core.ComposeAsync.EvaluateAsync(func1, func2, funcResult);
        /// <summary>Compose functions by giving the output of the first function, to the input of the second function.</summary>
        public static Task<Response<TResult>> ComposeAsync<T1, T2, T3, TResult>(Func<Task<Response<T1>>> func1, Func<T1, Task<Response<T2>>> func2, Func<T2, Task<Response<T3>>> func3, Func<T3, Task<Response<TResult>>> funcResult) => Core.ComposeAsync.EvaluateAsync(func1, func2, func3, funcResult);
        /// <summary>Compose functions by giving the output of the first function, to the input of the second function.</summary>
        public static Task<Response<TResult>> ComposeAsync<T1, T2, T3, T4, TResult>(Func<Task<Response<T1>>> func1, Func<T1, Task<Response<T2>>> func2, Func<T2, Task<Response<T3>>> func3, Func<T3, Task<Response<T4>>> func4, Func<T4, Task<Response<TResult>>> funcResult) => Core.ComposeAsync.EvaluateAsync(func1, func2, func3, func4, funcResult);
        /// <summary>Compose functions by giving the output of the first function, to the input of the second function.</summary>
        public static Task<Response<TResult>> ComposeAsync<T1, T2, T3, T4, T5, TResult>(Func<Task<Response<T1>>> func1, Func<T1, Task<Response<T2>>> func2, Func<T2, Task<Response<T3>>> func3, Func<T3, Task<Response<T4>>> func4, Func<T4, Task<Response<T5>>> func5, Func<T5, Task<Response<TResult>>> funcResult) => Core.ComposeAsync.EvaluateAsync(func1, func2, func3, func4, func5, funcResult);
        /// <summary>Compose functions by giving the output of the first function, to the input of the second function.</summary>
        public static Task<Response<TResult>> ComposeAsync<T1, T2, T3, T4, T5, T6, TResult>(Func<Task<Response<T1>>> func1, Func<T1, Task<Response<T2>>> func2, Func<T2, Task<Response<T3>>> func3, Func<T3, Task<Response<T4>>> func4, Func<T4, Task<Response<T5>>> func5, Func<T5, Task<Response<T6>>> func6, Func<T6, Task<Response<TResult>>> funcResult) => Core.ComposeAsync.EvaluateAsync(func1, func2, func3, func4, func5, func6, funcResult);
        /// <summary>Compose functions by giving the output of the first function, to the input of the second function.</summary>
        public static Task<Response<TResult>> ComposeAsync<T1, T2, T3, T4, T5, T6, T7, TResult>(Func<Task<Response<T1>>> func1, Func<T1, Task<Response<T2>>> func2, Func<T2, Task<Response<T3>>> func3, Func<T3, Task<Response<T4>>> func4, Func<T4, Task<Response<T5>>> func5, Func<T5, Task<Response<T6>>> func6, Func<T6, Task<Response<T7>>> func7, Func<T7, Task<Response<TResult>>> funcResult) => Core.ComposeAsync.EvaluateAsync(func1, func2, func3, func4, func5, func6, func7, funcResult);
        /// <summary>Compose functions by giving the output of the first function, to the input of the second function.</summary>
        public static Task<Response<TResult>> ComposeAsync<T1, T2, T3, T4, T5, T6, T7, T8, TResult>(Func<Task<Response<T1>>> func1, Func<T1, Task<Response<T2>>> func2, Func<T2, Task<Response<T3>>> func3, Func<T3, Task<Response<T4>>> func4, Func<T4, Task<Response<T5>>> func5, Func<T5, Task<Response<T6>>> func6, Func<T6, Task<Response<T7>>> func7, Func<T7, Task<Response<T8>>> func8, Func<T8, Task<Response<TResult>>> funcResult) => Core.ComposeAsync.EvaluateAsync(func1, func2, func3, func4, func5, func6, func7, func8, funcResult);

        #endregion

        #region Response

        /// <summary>Compose functions by giving the output of the first function, to the input of the second function.</summary>
        public static Task<Response> ComposeAsync<T1>(Func<Task<Response<T1>>> func1, Func<T1, Task<Response>> funcResult) => Core.ComposeAsync.EvaluateAsync(func1, funcResult);
        /// <summary>Compose functions by giving the output of the first function, to the input of the second function.</summary>
        public static Task<Response> ComposeAsync<T1, T2>(Func<Task<Response<T1>>> func1, Func<T1, Task<Response<T2>>> func2, Func<T2, Task<Response>> funcResult) => Core.ComposeAsync.EvaluateAsync(func1, func2, funcResult);
        /// <summary>Compose functions by giving the output of the first function, to the input of the second function.</summary>
        public static Task<Response> ComposeAsync<T1, T2, T3>(Func<Task<Response<T1>>> func1, Func<T1, Task<Response<T2>>> func2, Func<T2, Task<Response<T3>>> func3, Func<T3, Task<Response>> funcResult) => Core.ComposeAsync.EvaluateAsync(func1, func2, func3, funcResult);
        /// <summary>Compose functions by giving the output of the first function, to the input of the second function.</summary>
        public static Task<Response> ComposeAsync<T1, T2, T3, T4>(Func<Task<Response<T1>>> func1, Func<T1, Task<Response<T2>>> func2, Func<T2, Task<Response<T3>>> func3, Func<T3, Task<Response<T4>>> func4, Func<T4, Task<Response>> funcResult) => Core.ComposeAsync.EvaluateAsync(func1, func2, func3, func4, funcResult);
        /// <summary>Compose functions by giving the output of the first function, to the input of the second function.</summary>
        public static Task<Response> ComposeAsync<T1, T2, T3, T4, T5>(Func<Task<Response<T1>>> func1, Func<T1, Task<Response<T2>>> func2, Func<T2, Task<Response<T3>>> func3, Func<T3, Task<Response<T4>>> func4, Func<T4, Task<Response<T5>>> func5, Func<T5, Task<Response>> funcResult) => Core.ComposeAsync.EvaluateAsync(func1, func2, func3, func4, func5, funcResult);
        /// <summary>Compose functions by giving the output of the first function, to the input of the second function.</summary>
        public static Task<Response> ComposeAsync<T1, T2, T3, T4, T5, T6>(Func<Task<Response<T1>>> func1, Func<T1, Task<Response<T2>>> func2, Func<T2, Task<Response<T3>>> func3, Func<T3, Task<Response<T4>>> func4, Func<T4, Task<Response<T5>>> func5, Func<T5, Task<Response<T6>>> func6, Func<T6, Task<Response>> funcResult) => Core.ComposeAsync.EvaluateAsync(func1, func2, func3, func4, func5, func6, funcResult);
        /// <summary>Compose functions by giving the output of the first function, to the input of the second function.</summary>
        public static Task<Response> ComposeAsync<T1, T2, T3, T4, T5, T6, T7>(Func<Task<Response<T1>>> func1, Func<T1, Task<Response<T2>>> func2, Func<T2, Task<Response<T3>>> func3, Func<T3, Task<Response<T4>>> func4, Func<T4, Task<Response<T5>>> func5, Func<T5, Task<Response<T6>>> func6, Func<T6, Task<Response<T7>>> func7, Func<T7, Task<Response>> funcResult) => Core.ComposeAsync.EvaluateAsync(func1, func2, func3, func4, func5, func6, func7, funcResult);
        /// <summary>Compose functions by giving the output of the first function, to the input of the second function.</summary>
        public static Task<Response> ComposeAsync<T1, T2, T3, T4, T5, T6, T7, T8>(Func<Task<Response<T1>>> func1, Func<T1, Task<Response<T2>>> func2, Func<T2, Task<Response<T3>>> func3, Func<T3, Task<Response<T4>>> func4, Func<T4, Task<Response<T5>>> func5, Func<T5, Task<Response<T6>>> func6, Func<T6, Task<Response<T7>>> func7, Func<T7, Task<Response<T8>>> func8, Func<T8, Task<Response>> funcResult) => Core.ComposeAsync.EvaluateAsync(func1, func2, func3, func4, func5, func6, func7, func8, funcResult);

        #endregion

        #endregion

        #endregion
    }
}