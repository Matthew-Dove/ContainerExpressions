using ContainerExpressions.Containers;
using ContainerExpressions.Expressions.Models;
using System;
using System.Threading.Tasks;

namespace ContainerExpressions.Expressions
{
    public static class Expression
    {
        #region Compose

        #region Synchronous

        public static Response<TResult> Compose<T1, TResult>(Func<Response<T1>> func1, Func<T1, Response<TResult>> funcResult) => Core.Compose.Evaluate(func1, funcResult);
        public static Response<TResult> Compose<T1, T2, TResult>(Func<Response<T1>> func1, Func<T1, Response<T2>> func2, Func<T2, Response<TResult>> funcResult) => Core.Compose.Evaluate(func1, func2, funcResult);
        public static Response<TResult> Compose<T1, T2, T3, TResult>(Func<Response<T1>> func1, Func<T1, Response<T2>> func2, Func<T2, Response<T3>> func3, Func<T3, Response<TResult>> funcResult) => Core.Compose.Evaluate(func1, func2, func3, funcResult);
        public static Response<TResult> Compose<T1, T2, T3, T4, TResult>(Func<Response<T1>> func1, Func<T1, Response<T2>> func2, Func<T2, Response<T3>> func3, Func<T3, Response<T4>> func4, Func<T4, Response<TResult>> funcResult) => Core.Compose.Evaluate(func1, func2, func3, func4, funcResult);

        #endregion

        #region Asynchronous

        public static Task<Response<TResult>> ComposeAsync<T1, TResult>(Func<Task<Response<T1>>> func1, Func<T1, Task<Response<TResult>>> funcResult) => Core.ComposeAsync.EvaluateAsync(func1, funcResult);
        public static Task<Response<TResult>> ComposeAsync<T1, T2, TResult>(Func<Task<Response<T1>>> func1, Func<T1, Task<Response<T2>>> func2, Func<T2, Task<Response<TResult>>> funcResult) => Core.ComposeAsync.EvaluateAsync(func1, func2, funcResult);
        public static Task<Response<TResult>> ComposeAsync<T1, T2, T3, TResult>(Func<Task<Response<T1>>> func1, Func<T1, Task<Response<T2>>> func2, Func<T2, Task<Response<T3>>> func3, Func<T3, Task<Response<TResult>>> funcResult) => Core.ComposeAsync.EvaluateAsync(func1, func2, func3, funcResult);
        public static Task<Response<TResult>> ComposeAsync<T1, T2, T3, T4, TResult>(Func<Task<Response<T1>>> func1, Func<T1, Task<Response<T2>>> func2, Func<T2, Task<Response<T3>>> func3, Func<T3, Task<Response<T4>>> func4, Func<T4, Task<Response<TResult>>> funcResult) => Core.ComposeAsync.EvaluateAsync(func1, func2, func3, func4, funcResult);

        #endregion

        #endregion

        #region Match

        #region Synchronous

        public static Response<TResult> Match<TInput, TResult>(TInput input, Pattern<TInput, Response<TResult>> pattern) => Core.Match.Pattern(input, pattern);
        public static Response<TResult> Match<TInput, TResult>(TInput input, Pattern<TInput, Response<TResult>> pattern1, Pattern<TInput, Response<TResult>> pattern2) => Core.Match.Pattern(input, pattern1, pattern2);
        public static Response<TResult> Match<TInput, TResult>(TInput input, Pattern<TInput, Response<TResult>> pattern1, Pattern<TInput, Response<TResult>> pattern2, Pattern<TInput, Response<TResult>> pattern3) => Core.Match.Pattern(input, pattern1, pattern2, pattern3);
        public static Response<TResult> Match<TInput, TResult>(TInput input, Pattern<TInput, Response<TResult>> pattern1, Pattern<TInput, Response<TResult>> pattern2, Pattern<TInput, Response<TResult>> pattern3, Pattern<TInput, Response<TResult>> pattern4) => Core.Match.Pattern(input, pattern1, pattern2, pattern3, pattern4);

        #endregion

        #endregion
    }
}