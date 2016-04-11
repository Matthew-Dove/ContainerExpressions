using ContainerExpressions.Containers;
using ContainerExpressions.Expressions.Models;
using System.Threading.Tasks;

namespace ContainerExpressions.Expressions.Core
{
    internal static class MatchAsync
    {
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
    }
}
