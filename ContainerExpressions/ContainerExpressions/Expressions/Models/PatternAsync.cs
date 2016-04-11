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

        public bool Evaluate(TInput input)
        {
            if (_evaluate == null)
                throw new InvalidOperationException("Invalid PatternAsync creation, do not call the default construtor.");

            return _evaluate(input);
        }

        public Task<Response<TResult>> Execute(TInput input)
        {
            if (_execute == null)
                throw new InvalidOperationException("Invalid PatternAsync creation, do not call the default construtor.");

            return _execute(input);
        }
    }
}
