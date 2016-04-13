using ContainerExpressions.Containers;
using ContainerExpressions.Expressions.Models;

namespace ContainerExpressions.Expressions.Core
{
    internal static class Match
    {
        #region InputMatch

        public static Response<TResult> Pattern<TInput, TResult>(TInput input, Pattern<TInput, TResult> pattern)
        {
            var result = pattern.Evaluate(input);
            return result ? pattern.Execute(input) : Response.Create<TResult>();
        }

        public static Response<TResult> Pattern<TInput, TResult>(TInput input, Pattern<TInput, TResult> pattern1, Pattern<TInput, TResult> pattern2)
        {
            return pattern1.Evaluate(input) ? pattern1.Execute(input) : Pattern(input, pattern2);
        }

        public static Response<TResult> Pattern<TInput, TResult>(TInput input, Pattern<TInput, TResult> pattern1, Pattern<TInput, TResult> pattern2, Pattern<TInput, TResult> pattern3)
        {
            return pattern1.Evaluate(input) ? pattern1.Execute(input) : Pattern(input, pattern2, pattern3);
        }

        public static Response<TResult> Pattern<TInput, TResult>(TInput input, Pattern<TInput, TResult> pattern1, Pattern<TInput, TResult> pattern2, Pattern<TInput, TResult> pattern3, Pattern<TInput, TResult> pattern4)
        {
            return pattern1.Evaluate(input) ? pattern1.Execute(input) : Pattern(input, pattern2, pattern3, pattern4);
        }

        #endregion

        #region PivotMatch

        public static Response<TResult> Pattern<TPivot, TInput, TResult>(TPivot pivot, TInput input, Pattern<TPivot, TInput, TResult> pattern)
        {
            var result = pattern.Evaluate(pivot);
            return result ? pattern.Execute(input) : Response.Create<TResult>();
        }

        public static Response<TResult> Pattern<TPivot, TInput, TResult>(TPivot pivot, TInput input, Pattern<TPivot, TInput, TResult> pattern1, Pattern<TPivot, TInput, TResult> pattern2)
        {
            return pattern1.Evaluate(pivot) ? pattern1.Execute(input) : Pattern(pivot, input, pattern2);
        }

        public static Response<TResult> Pattern<TPivot, TInput, TResult>(TPivot pivot, TInput input, Pattern<TPivot, TInput, TResult> pattern1, Pattern<TPivot, TInput, TResult> pattern2, Pattern<TPivot, TInput, TResult> pattern3)
        {
            return pattern1.Evaluate(pivot) ? pattern1.Execute(input) : Pattern(pivot, input, pattern2, pattern3);
        }

        public static Response<TResult> Pattern<TPivot, TInput, TResult>(TPivot pivot, TInput input, Pattern<TPivot, TInput, TResult> pattern1, Pattern<TPivot, TInput, TResult> pattern2, Pattern<TPivot, TInput, TResult> pattern3, Pattern<TPivot, TInput, TResult> pattern4)
        {
            return pattern1.Evaluate(pivot) ? pattern1.Execute(input) : Pattern(pivot, input, pattern2, pattern3, pattern4);
        }

        #endregion
    }
}
