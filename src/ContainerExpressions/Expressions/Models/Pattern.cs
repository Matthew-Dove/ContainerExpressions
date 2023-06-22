using ContainerExpressions.Containers;
using ContainerExpressions.Containers.Internal;
using System;
using System.Threading.Tasks;

namespace ContainerExpressions.Expressions.Models
{
    /// <summary>Defines an input matcher, and a function to run, if that pattern matches.</summary>
    public readonly struct Pattern<TInput, TResult>
    {
        private readonly Func<TInput, bool> _evaluate;
        private readonly Func<TInput, Response<TResult>> _execute;

        /// <summary>Defines an input matcher, and a function to run, if that pattern matches.</summary>
        public Pattern(Func<TInput, bool> evaluate, Func<TInput, Response<TResult>> execute)
        {
            if (evaluate == null)
                Throw.ArgumentNullException(nameof(evaluate));
            if (execute == null)
                Throw.ArgumentNullException(nameof(execute));

            _evaluate = evaluate;
            _execute = execute;
        }

        /// <summary>Returns true if this patten matches.</summary>
        internal bool Evaluate(TInput input) => _evaluate(input);

        /// <summary>Invokes the function matching the pattern.</summary>
        internal Response<TResult> Execute(TInput input) => _execute(input);
    }

    /// <summary>Defines an input matcher, and a function to run, if that pattern matches.</summary>
    public readonly struct Pattern<TPivot, TInput, TResult>
    {
        private readonly Func<TPivot, bool> _evaluate;
        private readonly Func<TInput, Response<TResult>> _execute;

        /// <summary>Defines an input matcher, and a function to run, if that pattern matches.</summary>
        public Pattern(Func<TPivot, bool> evaluate, Func<TInput, Response<TResult>> execute)
        {
            if (evaluate == null)
                Throw.ArgumentNullException(nameof(evaluate));
            if (execute == null)
                Throw.ArgumentNullException(nameof(execute));

            _evaluate = evaluate;
            _execute = execute;
        }

        /// <summary>Returns true if this patten matches.</summary>
        internal bool Evaluate(TPivot pivot) => _evaluate(pivot);

        /// <summary>Invokes the function matching the pattern.</summary>
        internal Response<TResult> Execute(TInput input) => _execute(input);
    }

    /// <summary>Defines an input matcher, and a function to run, if that pattern matches.</summary>
    public static class Pattern
    {
        /// <summary>Defines an input matcher, and a function to run, if that pattern matches.</summary>
        public static Pattern<TInput, TResult> Create<TInput, TResult>(Func<TInput, bool> evaluation, Func<TInput, Response<TResult>> func) => new Pattern<TInput, TResult>(evaluation, func);
        /// <summary>Defines an input matcher, and a function to run, if that pattern matches.</summary>
        public static Pattern<TPivot, TInput, TResult> Create<TPivot, TInput, TResult>(Func<TPivot, bool> evaluation, Func<TInput, Response<TResult>> func) => new Pattern<TPivot, TInput, TResult>(evaluation, func);
        /// <summary>Defines an input matcher, and a function to run, if that pattern matches.</summary>
        public static PatternAsync<TInput, TResult> CreateAsync<TInput, TResult>(Func<TInput, bool> evaluation, Func<TInput, Task<Response<TResult>>> func) => new PatternAsync<TInput, TResult>(evaluation, func);
        /// <summary>Defines an input matcher, and a function to run, if that pattern matches.</summary>
        public static PatternAsync<TPivot, TInput, TResult> CreateAsync<TPivot, TInput, TResult>(Func<TPivot, bool> evaluation, Func<TInput, Task<Response<TResult>>> func) => new PatternAsync<TPivot, TInput, TResult>(evaluation, func);
    }
}
