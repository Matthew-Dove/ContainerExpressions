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
        public static Response<TResult> Compose<T1, T2, T3, T4,T5, TResult>(Func<Response<T1>> func1, Func<T1, Response<T2>> func2, Func<T2, Response<T3>> func3, Func<T3, Response<T4>> func4, Func<T4, Response<T5>> func5, Func<T5, Response<TResult>> funcResult) => Core.Compose.Evaluate(func1, func2, func3, func4, func5, funcResult);
        public static Response<TResult> Compose<T1, T2, T3, T4, T5, T6, TResult>(Func<Response<T1>> func1, Func<T1, Response<T2>> func2, Func<T2, Response<T3>> func3, Func<T3, Response<T4>> func4, Func<T4, Response<T5>> func5, Func<T5, Response<T6>> func6, Func<T6, Response<TResult>> funcResult) => Core.Compose.Evaluate(func1, func2, func3, func4, func5, func6, funcResult);
        public static Response<TResult> Compose<T1, T2, T3, T4, T5, T6, T7, TResult>(Func<Response<T1>> func1, Func<T1, Response<T2>> func2, Func<T2, Response<T3>> func3, Func<T3, Response<T4>> func4, Func<T4, Response<T5>> func5, Func<T5, Response<T6>> func6, Func<T6, Response<T7>> func7, Func<T7, Response<TResult>> funcResult) => Core.Compose.Evaluate(func1, func2, func3, func4, func5, func6, func7, funcResult);
        public static Response<TResult> Compose<T1, T2, T3, T4, T5, T6, T7, T8, TResult>(Func<Response<T1>> func1, Func<T1, Response<T2>> func2, Func<T2, Response<T3>> func3, Func<T3, Response<T4>> func4, Func<T4, Response<T5>> func5, Func<T5, Response<T6>> func6, Func<T6, Response<T7>> func7, Func<T7, Response<T8>> func8, Func<T8, Response<TResult>> funcResult) => Core.Compose.Evaluate(func1, func2, func3, func4, func5, func6, func7, func8, funcResult);

        #endregion

        #region Asynchronous

        public static Task<Response<TResult>> ComposeAsync<T1, TResult>(Func<Task<Response<T1>>> func1, Func<T1, Task<Response<TResult>>> funcResult) => Core.ComposeAsync.EvaluateAsync(func1, funcResult);
        public static Task<Response<TResult>> ComposeAsync<T1, T2, TResult>(Func<Task<Response<T1>>> func1, Func<T1, Task<Response<T2>>> func2, Func<T2, Task<Response<TResult>>> funcResult) => Core.ComposeAsync.EvaluateAsync(func1, func2, funcResult);
        public static Task<Response<TResult>> ComposeAsync<T1, T2, T3, TResult>(Func<Task<Response<T1>>> func1, Func<T1, Task<Response<T2>>> func2, Func<T2, Task<Response<T3>>> func3, Func<T3, Task<Response<TResult>>> funcResult) => Core.ComposeAsync.EvaluateAsync(func1, func2, func3, funcResult);
        public static Task<Response<TResult>> ComposeAsync<T1, T2, T3, T4, TResult>(Func<Task<Response<T1>>> func1, Func<T1, Task<Response<T2>>> func2, Func<T2, Task<Response<T3>>> func3, Func<T3, Task<Response<T4>>> func4, Func<T4, Task<Response<TResult>>> funcResult) => Core.ComposeAsync.EvaluateAsync(func1, func2, func3, func4, funcResult);
        public static Task<Response<TResult>> ComposeAsync<T1, T2, T3, T4, T5, TResult>(Func<Task<Response<T1>>> func1, Func<T1, Task<Response<T2>>> func2, Func<T2, Task<Response<T3>>> func3, Func<T3, Task<Response<T4>>> func4, Func<T4, Task<Response<T5>>> func5, Func<T5, Task<Response<TResult>>> funcResult) => Core.ComposeAsync.EvaluateAsync(func1, func2, func3, func4, func5, funcResult);
        public static Task<Response<TResult>> ComposeAsync<T1, T2, T3, T4, T5, T6, TResult>(Func<Task<Response<T1>>> func1, Func<T1, Task<Response<T2>>> func2, Func<T2, Task<Response<T3>>> func3, Func<T3, Task<Response<T4>>> func4, Func<T4, Task<Response<T5>>> func5, Func<T5, Task<Response<T6>>> func6, Func<T6, Task<Response<TResult>>> funcResult) => Core.ComposeAsync.EvaluateAsync(func1, func2, func3, func4, func5, func6, funcResult);
        public static Task<Response<TResult>> ComposeAsync<T1, T2, T3, T4, T5, T6, T7, TResult>(Func<Task<Response<T1>>> func1, Func<T1, Task<Response<T2>>> func2, Func<T2, Task<Response<T3>>> func3, Func<T3, Task<Response<T4>>> func4, Func<T4, Task<Response<T5>>> func5, Func<T5, Task<Response<T6>>> func6, Func<T6, Task<Response<T7>>> func7, Func<T7, Task<Response<TResult>>> funcResult) => Core.ComposeAsync.EvaluateAsync(func1, func2, func3, func4, func5, func6, func7, funcResult);
        public static Task<Response<TResult>> ComposeAsync<T1, T2, T3, T4, T5, T6, T7, T8, TResult>(Func<Task<Response<T1>>> func1, Func<T1, Task<Response<T2>>> func2, Func<T2, Task<Response<T3>>> func3, Func<T3, Task<Response<T4>>> func4, Func<T4, Task<Response<T5>>> func5, Func<T5, Task<Response<T6>>> func6, Func<T6, Task<Response<T7>>> func7, Func<T7, Task<Response<T8>>> func8, Func<T8, Task<Response<TResult>>> funcResult) => Core.ComposeAsync.EvaluateAsync(func1, func2, func3, func4, func5, func6, func7, func8, funcResult);

        #endregion

        #endregion

        #region Match

        #region Synchronous

        #region InputMatch

        public static Response<TResult> Match<TInput, TResult>(TInput input, Pattern<TInput, TResult> pattern) => Core.Match.Pattern(input, pattern);
        public static Response<TResult> Match<TInput, TResult>(TInput input, Pattern<TInput, TResult> pattern1, Pattern<TInput, TResult> pattern2) => Core.Match.Pattern(input, pattern1, pattern2);
        public static Response<TResult> Match<TInput, TResult>(TInput input, Pattern<TInput, TResult> pattern1, Pattern<TInput, TResult> pattern2, Pattern<TInput, TResult> pattern3) => Core.Match.Pattern(input, pattern1, pattern2, pattern3);
        public static Response<TResult> Match<TInput, TResult>(TInput input, Pattern<TInput, TResult> pattern1, Pattern<TInput, TResult> pattern2, Pattern<TInput, TResult> pattern3, Pattern<TInput, TResult> pattern4) => Core.Match.Pattern(input, pattern1, pattern2, pattern3, pattern4);
        public static Response<TResult> Match<TInput, TResult>(TInput input, Pattern<TInput, TResult> pattern1, Pattern<TInput, TResult> pattern2, Pattern<TInput, TResult> pattern3, Pattern<TInput, TResult> pattern4, Pattern<TInput, TResult> pattern5) => Core.Match.Pattern(input, pattern1, pattern2, pattern3, pattern4, pattern5);
        public static Response<TResult> Match<TInput, TResult>(TInput input, Pattern<TInput, TResult> pattern1, Pattern<TInput, TResult> pattern2, Pattern<TInput, TResult> pattern3, Pattern<TInput, TResult> pattern4, Pattern<TInput, TResult> pattern5, Pattern<TInput, TResult> pattern6) => Core.Match.Pattern(input, pattern1, pattern2, pattern3, pattern4, pattern5, pattern6);
        public static Response<TResult> Match<TInput, TResult>(TInput input, Pattern<TInput, TResult> pattern1, Pattern<TInput, TResult> pattern2, Pattern<TInput, TResult> pattern3, Pattern<TInput, TResult> pattern4, Pattern<TInput, TResult> pattern5, Pattern<TInput, TResult> pattern6, Pattern<TInput, TResult> pattern7) => Core.Match.Pattern(input, pattern1, pattern2, pattern3, pattern4, pattern5, pattern6, pattern7);
        public static Response<TResult> Match<TInput, TResult>(TInput input, Pattern<TInput, TResult> pattern1, Pattern<TInput, TResult> pattern2, Pattern<TInput, TResult> pattern3, Pattern<TInput, TResult> pattern4, Pattern<TInput, TResult> pattern5, Pattern<TInput, TResult> pattern6, Pattern<TInput, TResult> pattern7, Pattern<TInput, TResult> pattern8) => Core.Match.Pattern(input, pattern1, pattern2, pattern3, pattern4, pattern5, pattern6, pattern7, pattern8);

        #endregion

        #region PivotMatch

        public static Response<TResult> Match<TPivot, TInput, TResult>(TPivot pivot, TInput input, Pattern<TPivot, TInput, TResult> pattern) => Core.Match.Pattern(pivot, input, pattern);
        public static Response<TResult> Match<TPivot, TInput, TResult>(TPivot pivot, TInput input, Pattern<TPivot, TInput, TResult> pattern1, Pattern<TPivot, TInput, TResult> pattern2) => Core.Match.Pattern(pivot, input, pattern1, pattern2);
        public static Response<TResult> Match<TPivot, TInput, TResult>(TPivot pivot, TInput input, Pattern<TPivot, TInput, TResult> pattern1, Pattern<TPivot, TInput, TResult> pattern2, Pattern<TPivot, TInput, TResult> pattern3) => Core.Match.Pattern(pivot, input, pattern1, pattern2, pattern3);
        public static Response<TResult> Match<TPivot, TInput, TResult>(TPivot pivot, TInput input, Pattern<TPivot, TInput, TResult> pattern1, Pattern<TPivot, TInput, TResult> pattern2, Pattern<TPivot, TInput, TResult> pattern3, Pattern<TPivot, TInput, TResult> pattern4) => Core.Match.Pattern(pivot, input, pattern1, pattern2, pattern3, pattern4);
        public static Response<TResult> Match<TPivot, TInput, TResult>(TPivot pivot, TInput input, Pattern<TPivot, TInput, TResult> pattern1, Pattern<TPivot, TInput, TResult> pattern2, Pattern<TPivot, TInput, TResult> pattern3, Pattern<TPivot, TInput, TResult> pattern4, Pattern<TPivot, TInput, TResult> pattern5) => Core.Match.Pattern(pivot, input, pattern1, pattern2, pattern3, pattern4, pattern5);
        public static Response<TResult> Match<TPivot, TInput, TResult>(TPivot pivot, TInput input, Pattern<TPivot, TInput, TResult> pattern1, Pattern<TPivot, TInput, TResult> pattern2, Pattern<TPivot, TInput, TResult> pattern3, Pattern<TPivot, TInput, TResult> pattern4, Pattern<TPivot, TInput, TResult> pattern5, Pattern<TPivot, TInput, TResult> pattern6) => Core.Match.Pattern(pivot, input, pattern1, pattern2, pattern3, pattern4, pattern5, pattern6);
        public static Response<TResult> Match<TPivot, TInput, TResult>(TPivot pivot, TInput input, Pattern<TPivot, TInput, TResult> pattern1, Pattern<TPivot, TInput, TResult> pattern2, Pattern<TPivot, TInput, TResult> pattern3, Pattern<TPivot, TInput, TResult> pattern4, Pattern<TPivot, TInput, TResult> pattern5, Pattern<TPivot, TInput, TResult> pattern6, Pattern<TPivot, TInput, TResult> pattern7) => Core.Match.Pattern(pivot, input, pattern1, pattern2, pattern3, pattern4, pattern5, pattern6, pattern7);
        public static Response<TResult> Match<TPivot, TInput, TResult>(TPivot pivot, TInput input, Pattern<TPivot, TInput, TResult> pattern1, Pattern<TPivot, TInput, TResult> pattern2, Pattern<TPivot, TInput, TResult> pattern3, Pattern<TPivot, TInput, TResult> pattern4, Pattern<TPivot, TInput, TResult> pattern5, Pattern<TPivot, TInput, TResult> pattern6, Pattern<TPivot, TInput, TResult> pattern7, Pattern<TPivot, TInput, TResult> pattern8) => Core.Match.Pattern(pivot, input, pattern1, pattern2, pattern3, pattern4, pattern5, pattern6, pattern7, pattern8);

        #endregion

        #endregion

        #region Asynchronous

        #region InputMatch

        public static Task<Response<TResult>> MatchAsync<TInput, TResult>(TInput input, PatternAsync<TInput, TResult> pattern) => Core.MatchAsync.PatternAsync(input, pattern);
        public static Task<Response<TResult>> MatchAsync<TInput, TResult>(TInput input, PatternAsync<TInput, TResult> pattern1, PatternAsync<TInput, TResult> pattern2) => Core.MatchAsync.PatternAsync(input, pattern1, pattern2);
        public static Task<Response<TResult>> MatchAsync<TInput, TResult>(TInput input, PatternAsync<TInput, TResult> pattern1, PatternAsync<TInput, TResult> pattern2, PatternAsync<TInput, TResult> pattern3) => Core.MatchAsync.PatternAsync(input, pattern1, pattern2, pattern3);
        public static Task<Response<TResult>> MatchAsync<TInput, TResult>(TInput input, PatternAsync<TInput, TResult> pattern1, PatternAsync<TInput, TResult> pattern2, PatternAsync<TInput, TResult> pattern3, PatternAsync<TInput, TResult> pattern4) => Core.MatchAsync.PatternAsync(input, pattern1, pattern2, pattern3, pattern4);
        public static Task<Response<TResult>> MatchAsync<TInput, TResult>(TInput input, PatternAsync<TInput, TResult> pattern1, PatternAsync<TInput, TResult> pattern2, PatternAsync<TInput, TResult> pattern3, PatternAsync<TInput, TResult> pattern4, PatternAsync<TInput, TResult> pattern5) => Core.MatchAsync.PatternAsync(input, pattern1, pattern2, pattern3, pattern4, pattern5);
        public static Task<Response<TResult>> MatchAsync<TInput, TResult>(TInput input, PatternAsync<TInput, TResult> pattern1, PatternAsync<TInput, TResult> pattern2, PatternAsync<TInput, TResult> pattern3, PatternAsync<TInput, TResult> pattern4, PatternAsync<TInput, TResult> pattern5, PatternAsync<TInput, TResult> pattern6) => Core.MatchAsync.PatternAsync(input, pattern1, pattern2, pattern3, pattern4, pattern5, pattern6);
        public static Task<Response<TResult>> MatchAsync<TInput, TResult>(TInput input, PatternAsync<TInput, TResult> pattern1, PatternAsync<TInput, TResult> pattern2, PatternAsync<TInput, TResult> pattern3, PatternAsync<TInput, TResult> pattern4, PatternAsync<TInput, TResult> pattern5, PatternAsync<TInput, TResult> pattern6, PatternAsync<TInput, TResult> pattern7) => Core.MatchAsync.PatternAsync(input, pattern1, pattern2, pattern3, pattern4, pattern5, pattern6, pattern7);
        public static Task<Response<TResult>> MatchAsync<TInput, TResult>(TInput input, PatternAsync<TInput, TResult> pattern1, PatternAsync<TInput, TResult> pattern2, PatternAsync<TInput, TResult> pattern3, PatternAsync<TInput, TResult> pattern4, PatternAsync<TInput, TResult> pattern5, PatternAsync<TInput, TResult> pattern6, PatternAsync<TInput, TResult> pattern7, PatternAsync<TInput, TResult> pattern8) => Core.MatchAsync.PatternAsync(input, pattern1, pattern2, pattern3, pattern4, pattern5, pattern6, pattern7, pattern8);

        #endregion

        #region PivotMatch

        public static Task<Response<TResult>> MatchAsync<TPivot, TInput, TResult>(TPivot pivot, TInput input, PatternAsync<TPivot, TInput, TResult> pattern) => Core.MatchAsync.PatternAsync(pivot, input, pattern);
        public static Task<Response<TResult>> MatchAsync<TPivot, TInput, TResult>(TPivot pivot, TInput input, PatternAsync<TPivot, TInput, TResult> pattern1, PatternAsync<TPivot, TInput, TResult> pattern2) => Core.MatchAsync.PatternAsync(pivot, input, pattern1, pattern2);
        public static Task<Response<TResult>> MatchAsync<TPivot, TInput, TResult>(TPivot pivot, TInput input, PatternAsync<TPivot, TInput, TResult> pattern1, PatternAsync<TPivot, TInput, TResult> pattern2, PatternAsync<TPivot, TInput, TResult> pattern3) => Core.MatchAsync.PatternAsync(pivot, input, pattern1, pattern2, pattern3);
        public static Task<Response<TResult>> MatchAsync<TPivot, TInput, TResult>(TPivot pivot, TInput input, PatternAsync<TPivot, TInput, TResult> pattern1, PatternAsync<TPivot, TInput, TResult> pattern2, PatternAsync<TPivot, TInput, TResult> pattern3, PatternAsync<TPivot, TInput, TResult> pattern4) => Core.MatchAsync.PatternAsync(pivot, input, pattern1, pattern2, pattern3, pattern4);
        public static Task<Response<TResult>> MatchAsync<TPivot, TInput, TResult>(TPivot pivot, TInput input, PatternAsync<TPivot, TInput, TResult> pattern1, PatternAsync<TPivot, TInput, TResult> pattern2, PatternAsync<TPivot, TInput, TResult> pattern3, PatternAsync<TPivot, TInput, TResult> pattern4, PatternAsync<TPivot, TInput, TResult> pattern5) => Core.MatchAsync.PatternAsync(pivot, input, pattern1, pattern2, pattern3, pattern4, pattern5);
        public static Task<Response<TResult>> MatchAsync<TPivot, TInput, TResult>(TPivot pivot, TInput input, PatternAsync<TPivot, TInput, TResult> pattern1, PatternAsync<TPivot, TInput, TResult> pattern2, PatternAsync<TPivot, TInput, TResult> pattern3, PatternAsync<TPivot, TInput, TResult> pattern4, PatternAsync<TPivot, TInput, TResult> pattern5, PatternAsync<TPivot, TInput, TResult> pattern6) => Core.MatchAsync.PatternAsync(pivot, input, pattern1, pattern2, pattern3, pattern4, pattern5, pattern6);
        public static Task<Response<TResult>> MatchAsync<TPivot, TInput, TResult>(TPivot pivot, TInput input, PatternAsync<TPivot, TInput, TResult> pattern1, PatternAsync<TPivot, TInput, TResult> pattern2, PatternAsync<TPivot, TInput, TResult> pattern3, PatternAsync<TPivot, TInput, TResult> pattern4, PatternAsync<TPivot, TInput, TResult> pattern5, PatternAsync<TPivot, TInput, TResult> pattern6, PatternAsync<TPivot, TInput, TResult> pattern7) => Core.MatchAsync.PatternAsync(pivot, input, pattern1, pattern2, pattern3, pattern4, pattern5, pattern6, pattern7);
        public static Task<Response<TResult>> MatchAsync<TPivot, TInput, TResult>(TPivot pivot, TInput input, PatternAsync<TPivot, TInput, TResult> pattern1, PatternAsync<TPivot, TInput, TResult> pattern2, PatternAsync<TPivot, TInput, TResult> pattern3, PatternAsync<TPivot, TInput, TResult> pattern4, PatternAsync<TPivot, TInput, TResult> pattern5, PatternAsync<TPivot, TInput, TResult> pattern6, PatternAsync<TPivot, TInput, TResult> pattern7, PatternAsync<TPivot, TInput, TResult> pattern8) => Core.MatchAsync.PatternAsync(pivot, input, pattern1, pattern2, pattern3, pattern4, pattern5, pattern6, pattern7, pattern8);

        #endregion

        #endregion

        #endregion
    }
}