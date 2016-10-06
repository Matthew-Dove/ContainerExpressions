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
                if (values[i])
                {
                    result = combine(result, values[i]);
                }
            }

            return result;
        }

        public static async Task<T> FoldAsync<T>(Func<T, T, T> combine, T arg1, params Task<Response<T>>[] values)
        {
            await Task.WhenAll(values);

            T result = arg1;

            for (int i = 0; i < values.Length; i++)
            {
                if (values[i].Result)
                {
                    result = combine(result, values[i].Result);
                }
            }

            return result;
        }

        public static async Task<T> FoldAsync<T>(Func<T, T, T> combine, Task<T> arg1, params Task<Response<T>>[] values)
        {
            var tasks = new Task[values.Length + 1];
            Array.Copy(values, tasks, values.Length);
            tasks[values.Length + 1] = arg1;

            await Task.WhenAll(tasks);

            T result = arg1.Result;

            for (int i = 0; i < values.Length; i++)
            {
                if (values[i].Result)
                {
                    result = combine(result, values[i].Result);
                }
            }

            return result;
        }
    }
}
