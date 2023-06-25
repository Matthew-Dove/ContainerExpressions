using ContainerExpressions.Containers;
using System;
using System.Threading.Tasks;

namespace ContainerExpressions.Expressions.Core
{
    internal static class Reduce
    {
        public static T Fold<T>(Func<T, T, T> combine, T arg1, params Response<T>[] values)
        {
            T result = arg1;

            for (int i = 0; i < values.Length; i++)
            {
                if (values[i]) result = combine(result, values[i]);
            }

            return result;
        }

        public static async Task<T> FoldAsync<T>(Func<T, T, T> combine, T arg1, params Task<Response<T>>[] values)
        {
            var results = await Task.WhenAll(values);
            T result = arg1;

            for (int i = 0; i < results.Length; i++)
            {
                if (results[i]) result = combine(result, results[i]);
            }

            return result;
        }
    }
}
