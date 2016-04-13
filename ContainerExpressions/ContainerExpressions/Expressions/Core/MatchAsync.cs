﻿using ContainerExpressions.Containers;
using ContainerExpressions.Expressions.Models;
using System.Threading.Tasks;

namespace ContainerExpressions.Expressions.Core
{
    internal static class MatchAsync
    {
        #region InputMatch

        public static async Task<Response<TResult>> PatternAsync<TInput, TResult>(TInput input, PatternAsync<TInput, TResult> pattern)
        {
            var result = pattern.Evaluate(input);
            return result ? await pattern.Execute(input) : Response.Create<TResult>();
        }

        public static Task<Response<TResult>> PatternAsync<TInput, TResult>(TInput input, PatternAsync<TInput, TResult> pattern1, PatternAsync<TInput, TResult> pattern2)
        {
            return pattern1.Evaluate(input) ? pattern1.Execute(input) : PatternAsync(input, pattern2);
        }

        public static Task<Response<TResult>> PatternAsync<TInput, TResult>(TInput input, PatternAsync<TInput, TResult> pattern1, PatternAsync<TInput, TResult> pattern2, PatternAsync<TInput, TResult> pattern3)
        {
            return pattern1.Evaluate(input) ? pattern1.Execute(input) : PatternAsync(input, pattern2, pattern3);
        }

        public static Task<Response<TResult>> PatternAsync<TInput, TResult>(TInput input, PatternAsync<TInput, TResult> pattern1, PatternAsync<TInput, TResult> pattern2, PatternAsync<TInput, TResult> pattern3, PatternAsync<TInput, TResult> pattern4)
        {
            return pattern1.Evaluate(input) ? pattern1.Execute(input) : PatternAsync(input, pattern2, pattern3, pattern4);
        }

        #endregion

        #region PivotMatch

        public static async Task<Response<TResult>> PatternAsync<TPivot, TInput, TResult>(TPivot pivot, TInput input, PatternAsync<TPivot, TInput, TResult> pattern)
        {
            var result = pattern.Evaluate(pivot);
            return result ? await pattern.Execute(input) : Response.Create<TResult>();
        }

        public static Task<Response<TResult>> PatternAsync<TPivot, TInput, TResult>(TPivot pivot, TInput input, PatternAsync<TPivot, TInput, TResult> pattern1, PatternAsync<TPivot, TInput, TResult> pattern2)
        {
            return pattern1.Evaluate(pivot) ? pattern1.Execute(input) : PatternAsync(pivot, input, pattern2);
        }

        public static Task<Response<TResult>> PatternAsync<TPivot, TInput, TResult>(TPivot pivot, TInput input, PatternAsync<TPivot, TInput, TResult> pattern1, PatternAsync<TPivot, TInput, TResult> pattern2, PatternAsync<TPivot, TInput, TResult> pattern3)
        {
            return pattern1.Evaluate(pivot) ? pattern1.Execute(input) : PatternAsync(pivot, input, pattern2, pattern3);
        }

        public static Task<Response<TResult>> PatternAsync<TPivot, TInput, TResult>(TPivot pivot, TInput input, PatternAsync<TPivot, TInput, TResult> pattern1, PatternAsync<TPivot, TInput, TResult> pattern2, PatternAsync<TPivot, TInput, TResult> pattern3, PatternAsync<TPivot, TInput, TResult> pattern4)
        {
            return pattern1.Evaluate(pivot) ? pattern1.Execute(input) : PatternAsync(pivot, input, pattern2, pattern3, pattern4);
        }

        #endregion
    }
}
