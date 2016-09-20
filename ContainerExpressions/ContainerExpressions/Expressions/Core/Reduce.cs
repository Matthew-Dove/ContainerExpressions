using System;
using System.Collections.Generic;

namespace ContainerExpressions.Expressions.Core
{
    internal static class Reduce
    {
        public static T Fold<T>(T arg1, IEnumerable<T> values, Func<T, T, T> combine)
        {
            T result = arg1;

            foreach (var value in values)
            {
                result = combine(result, value);
            }

            return result;
        }
    }
}
