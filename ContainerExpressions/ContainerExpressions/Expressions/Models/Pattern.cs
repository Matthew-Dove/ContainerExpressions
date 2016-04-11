﻿using ContainerExpressions.Containers;
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

        public bool Evaluate(TInput input)
        {
            if (_evaluate == null)
                throw new InvalidOperationException($"Invalid Pattern creation, do not call the default construtor.");

            return _evaluate(input);
        }

        public Response<TResult> Execute(TInput input)
        {
            if (_execute == null)
                throw new InvalidOperationException($"Invalid Pattern creation, do not call the default construtor.");

            return _execute(input);
        }
    }

    public static class Pattern
    {
        public static Pattern<TInput, TResult> Create<TInput, TResult>(Func<TInput, bool> evaluation, Func<TInput, Response<TResult>> func) => new Pattern<TInput, TResult>(evaluation, func);
        public static PatternAsync<TInput, TResult> CreateAsync<TInput, TResult>(Func<TInput, bool> evaluation, Func<TInput, Task<Response<TResult>>> func) => new PatternAsync<TInput, TResult>(evaluation, func);
    }
}
