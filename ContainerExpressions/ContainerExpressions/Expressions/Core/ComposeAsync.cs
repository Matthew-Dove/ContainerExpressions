using ContainerExpressions.Containers;
using System;
using System.Threading.Tasks;

namespace ContainerExpressions.Expressions.Core
{
    internal static class ComposeAsync
    {
        public static async Task<Response<TResult>> EvaluateAsync<T1, TResult>(Func<Task<Response<T1>>> func1, Func<T1, Task<Response<TResult>>> funcResult)
        {
            var result = await func1();
            return result ? await funcResult(result) : Response.Create<TResult>();
        }

        public static async Task<Response<TResult>> EvaluateAsync<T1, T2, TResult>(Func<Task<Response<T1>>> func1, Func<T1, Task<Response<T2>>> func2, Func<T2, Task<Response<TResult>>> funcResult)
        {
            var result = await func1();
            return result ? await EvaluateAsync(() => func2(result), funcResult) : Response.Create<TResult>();
        }

        public static async Task<Response<TResult>> EvaluateAsync<T1, T2, T3, TResult>(Func<Task<Response<T1>>> func1, Func<T1, Task<Response<T2>>> func2, Func<T2, Task<Response<T3>>> func3, Func<T3, Task<Response<TResult>>> funcResult)
        {
            var result = await func1();
            return result ? await EvaluateAsync(() => func2(result), func3, funcResult) : Response.Create<TResult>();
        }

        public static async Task<Response<TResult>> EvaluateAsync<T1, T2, T3, T4, TResult>(Func<Task<Response<T1>>> func1, Func<T1, Task<Response<T2>>> func2, Func<T2, Task<Response<T3>>> func3, Func<T3, Task<Response<T4>>> func4, Func<T4, Task<Response<TResult>>> funcResult)
        {
            var result = await func1();
            return result ? await EvaluateAsync(() => func2(result), func3, func4, funcResult) : Response.Create<TResult>();
        }
    }
}
