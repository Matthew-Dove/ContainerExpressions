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
            return result ? pattern.Execute(input) : new Response<TResult>();
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

        public static Response<TResult> Pattern<TInput, TResult>(TInput input, Pattern<TInput, TResult> pattern1, Pattern<TInput, TResult> pattern2, Pattern<TInput, TResult> pattern3, Pattern<TInput, TResult> pattern4, Pattern<TInput, TResult> pattern5)
        {
            return pattern1.Evaluate(input) ? pattern1.Execute(input) : Pattern(input, pattern2, pattern3, pattern4, pattern5);
        }

        public static Response<TResult> Pattern<TInput, TResult>(TInput input, Pattern<TInput, TResult> pattern1, Pattern<TInput, TResult> pattern2, Pattern<TInput, TResult> pattern3, Pattern<TInput, TResult> pattern4, Pattern<TInput, TResult> pattern5, Pattern<TInput, TResult> pattern6)
        {
            return pattern1.Evaluate(input) ? pattern1.Execute(input) : Pattern(input, pattern2, pattern3, pattern4, pattern5, pattern6);
        }

        public static Response<TResult> Pattern<TInput, TResult>(TInput input, Pattern<TInput, TResult> pattern1, Pattern<TInput, TResult> pattern2, Pattern<TInput, TResult> pattern3, Pattern<TInput, TResult> pattern4, Pattern<TInput, TResult> pattern5, Pattern<TInput, TResult> pattern6, Pattern<TInput, TResult> pattern7)
        {
            return pattern1.Evaluate(input) ? pattern1.Execute(input) : Pattern(input, pattern2, pattern3, pattern4, pattern5, pattern6, pattern7);
        }

        public static Response<TResult> Pattern<TInput, TResult>(TInput input, Pattern<TInput, TResult> pattern1, Pattern<TInput, TResult> pattern2, Pattern<TInput, TResult> pattern3, Pattern<TInput, TResult> pattern4, Pattern<TInput, TResult> pattern5, Pattern<TInput, TResult> pattern6, Pattern<TInput, TResult> pattern7, Pattern<TInput, TResult> pattern8)
        {
            return pattern1.Evaluate(input) ? pattern1.Execute(input) : Pattern(input, pattern2, pattern3, pattern4, pattern5, pattern6, pattern7, pattern8);
        }

        #endregion

        #region PivotMatch

        public static Response<TResult> Pattern<TPivot, TInput, TResult>(TPivot pivot, TInput input, Pattern<TPivot, TInput, TResult> pattern)
        {
            var result = pattern.Evaluate(pivot);
            return result ? pattern.Execute(input) : new Response<TResult>();
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

        public static Response<TResult> Pattern<TPivot, TInput, TResult>(TPivot pivot, TInput input, Pattern<TPivot, TInput, TResult> pattern1, Pattern<TPivot, TInput, TResult> pattern2, Pattern<TPivot, TInput, TResult> pattern3, Pattern<TPivot, TInput, TResult> pattern4, Pattern<TPivot, TInput, TResult> pattern5)
        {
            return pattern1.Evaluate(pivot) ? pattern1.Execute(input) : Pattern(pivot, input, pattern2, pattern3, pattern4, pattern5);
        }

        public static Response<TResult> Pattern<TPivot, TInput, TResult>(TPivot pivot, TInput input, Pattern<TPivot, TInput, TResult> pattern1, Pattern<TPivot, TInput, TResult> pattern2, Pattern<TPivot, TInput, TResult> pattern3, Pattern<TPivot, TInput, TResult> pattern4, Pattern<TPivot, TInput, TResult> pattern5, Pattern<TPivot, TInput, TResult> pattern6)
        {
            return pattern1.Evaluate(pivot) ? pattern1.Execute(input) : Pattern(pivot, input, pattern2, pattern3, pattern4, pattern5, pattern6);
        }

        public static Response<TResult> Pattern<TPivot, TInput, TResult>(TPivot pivot, TInput input, Pattern<TPivot, TInput, TResult> pattern1, Pattern<TPivot, TInput, TResult> pattern2, Pattern<TPivot, TInput, TResult> pattern3, Pattern<TPivot, TInput, TResult> pattern4, Pattern<TPivot, TInput, TResult> pattern5, Pattern<TPivot, TInput, TResult> pattern6, Pattern<TPivot, TInput, TResult> pattern7)
        {
            return pattern1.Evaluate(pivot) ? pattern1.Execute(input) : Pattern(pivot, input, pattern2, pattern3, pattern4, pattern5, pattern6, pattern7);
        }

        public static Response<TResult> Pattern<TPivot, TInput, TResult>(TPivot pivot, TInput input, Pattern<TPivot, TInput, TResult> pattern1, Pattern<TPivot, TInput, TResult> pattern2, Pattern<TPivot, TInput, TResult> pattern3, Pattern<TPivot, TInput, TResult> pattern4, Pattern<TPivot, TInput, TResult> pattern5, Pattern<TPivot, TInput, TResult> pattern6, Pattern<TPivot, TInput, TResult> pattern7, Pattern<TPivot, TInput, TResult> pattern8)
        {
            return pattern1.Evaluate(pivot) ? pattern1.Execute(input) : Pattern(pivot, input, pattern2, pattern3, pattern4, pattern5, pattern6, pattern7, pattern8);
        }

        #endregion
    }
}
