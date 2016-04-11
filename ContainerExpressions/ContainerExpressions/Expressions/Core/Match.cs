using ContainerExpressions.Containers;
using ContainerExpressions.Expressions.Models;

namespace ContainerExpressions.Expressions.Core
{
    internal static class Match
    {
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
    }
}
