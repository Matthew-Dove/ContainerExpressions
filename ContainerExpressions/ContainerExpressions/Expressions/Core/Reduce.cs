using ContainerExpressions.Containers;
using System;

namespace ContainerExpressions.Expressions.Core
{
    internal static class Reduce
    {
        public static T Fold<T>(Func<T, T, T> combine, T arg1, params Response<T>[] values)
        {
            T result = arg1;

            for (int i = 0; i < values.Length; i++)
            {
                if (values[i])
                {
                    result = combine(result, values[i]);
                }
            }

            return result;
        }
    }
}
