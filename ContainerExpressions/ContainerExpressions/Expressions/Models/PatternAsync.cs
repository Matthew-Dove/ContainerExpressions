using ContainerExpressions.Containers;
using System;
using System.Threading.Tasks;

namespace ContainerExpressions.Expressions.Models
{
    public struct PatternAsync<TInput, TResult>
    {
        private readonly Func<TInput, bool> _evaluate;
        private readonly Func<TInput, Task<Response<TResult>>> _execute;

        public PatternAsync(Func<TInput, bool> evaluate, Func<TInput, Task<Response<TResult>>> execute)
        {
            if (evaluate == null)
                throw new ArgumentNullException(nameof(evaluate));
            if (execute == null)
                throw new ArgumentNullException(nameof(execute));

            _evaluate = evaluate;
            _execute = execute;
        }

        public bool Evaluate(TInput input) => _evaluate(input);

        public Task<Response<TResult>> Execute(TInput input) => _execute(input);
    }

    public struct PatternAsync<TPivot, TInput, TResult>
    {
        private readonly Func<TPivot, bool> _evaluate;
        private readonly Func<TInput, Task<Response<TResult>>> _execute;

        public PatternAsync(Func<TPivot, bool> evaluate, Func<TInput, Task<Response<TResult>>> execute)
        {
            if (evaluate == null)
                throw new ArgumentNullException(nameof(evaluate));
            if (execute == null)
                throw new ArgumentNullException(nameof(execute));

            _evaluate = evaluate;
            _execute = execute;
        }

        public bool Evaluate(TPivot pivot) => _evaluate(pivot);

        public Task<Response<TResult>> Execute(TInput input) => _execute(input);
    }
}
