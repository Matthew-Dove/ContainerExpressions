using ContainerExpressions.Containers;
using System;
using System.Threading.Tasks;

namespace ContainerExpressions.Expressions.Models
{
    public struct Pattern<TInput, TResult>
    {
        private readonly Func<TInput, bool> _evaluate;
        private readonly Func<TInput, Response<TResult>> _execute;

        public Pattern(Func<TInput, bool> evaluate, Func<TInput, Response<TResult>> execute)
        {
            if (evaluate == null)
                throw new ArgumentNullException(nameof(evaluate));
            if (execute == null)
                throw new ArgumentNullException(nameof(execute));

            _evaluate = evaluate;
            _execute = execute;
        }

        public bool Evaluate(TInput input) => _evaluate(input);

        public Response<TResult> Execute(TInput input) => _execute(input);
    }

    public struct Pattern<TPivot, TInput, TResult>
    {
        private readonly Func<TPivot, bool> _evaluate;
        private readonly Func<TInput, Response<TResult>> _execute;

        public Pattern(Func<TPivot, bool> evaluate, Func<TInput, Response<TResult>> execute)
        {
            if (evaluate == null)
                throw new ArgumentNullException(nameof(evaluate));
            if (execute == null)
                throw new ArgumentNullException(nameof(execute));

            _evaluate = evaluate;
            _execute = execute;
        }

        public bool Evaluate(TPivot pivot) => _evaluate(pivot);

        public Response<TResult> Execute(TInput input) => _execute(input);
    }

    public static class Pattern
    {
        public static Pattern<TInput, TResult> Create<TInput, TResult>(Func<TInput, bool> evaluation, Func<TInput, Response<TResult>> func) => new Pattern<TInput, TResult>(evaluation, func);
        public static Pattern<TPivot, TInput, TResult> Create<TPivot, TInput, TResult>(Func<TPivot, bool> evaluation, Func<TInput, Response<TResult>> func) => new Pattern<TPivot, TInput, TResult>(evaluation, func);
        public static PatternAsync<TInput, TResult> CreateAsync<TInput, TResult>(Func<TInput, bool> evaluation, Func<TInput, Task<Response<TResult>>> func) => new PatternAsync<TInput, TResult>(evaluation, func);
        public static PatternAsync<TPivot, TInput, TResult> CreateAsync<TPivot, TInput, TResult>(Func<TPivot, bool> evaluation, Func<TInput, Task<Response<TResult>>> func) => new PatternAsync<TPivot, TInput, TResult>(evaluation, func);
    }
}
