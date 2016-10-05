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
            return result ? await funcResult(result) : new Response<TResult>();
        }

        public static async Task<Response<TResult>> EvaluateAsync<T1, T2, TResult>(Func<Task<Response<T1>>> func1, Func<T1, Task<Response<T2>>> func2, Func<T2, Task<Response<TResult>>> funcResult)
        {
            var result = await func1();
            return result ? await EvaluateAsync(() => func2(result), funcResult) : new Response<TResult>();
        }

        public static async Task<Response<TResult>> EvaluateAsync<T1, T2, T3, TResult>(Func<Task<Response<T1>>> func1, Func<T1, Task<Response<T2>>> func2, Func<T2, Task<Response<T3>>> func3, Func<T3, Task<Response<TResult>>> funcResult)
        {
            var result = await func1();
            return result ? await EvaluateAsync(() => func2(result), func3, funcResult) : new Response<TResult>();
        }

        public static async Task<Response<TResult>> EvaluateAsync<T1, T2, T3, T4, TResult>(Func<Task<Response<T1>>> func1, Func<T1, Task<Response<T2>>> func2, Func<T2, Task<Response<T3>>> func3, Func<T3, Task<Response<T4>>> func4, Func<T4, Task<Response<TResult>>> funcResult)
        {
            var result = await func1();
            return result ? await EvaluateAsync(() => func2(result), func3, func4, funcResult) : new Response<TResult>();
        }

        public static async Task<Response<TResult>> EvaluateAsync<T1, T2, T3, T4, T5, TResult>(Func<Task<Response<T1>>> func1, Func<T1, Task<Response<T2>>> func2, Func<T2, Task<Response<T3>>> func3, Func<T3, Task<Response<T4>>> func4, Func<T4, Task<Response<T5>>> func5, Func<T5, Task<Response<TResult>>> funcResult)
        {
            var result = await func1();
            return result ? await EvaluateAsync(() => func2(result), func3, func4, func5, funcResult) : new Response<TResult>();
        }

        public static async Task<Response<TResult>> EvaluateAsync<T1, T2, T3, T4, T5, T6, TResult>(Func<Task<Response<T1>>> func1, Func<T1, Task<Response<T2>>> func2, Func<T2, Task<Response<T3>>> func3, Func<T3, Task<Response<T4>>> func4, Func<T4, Task<Response<T5>>> func5, Func<T5, Task<Response<T6>>> func6, Func<T6, Task<Response<TResult>>> funcResult)
        {
            var result = await func1();
            return result ? await EvaluateAsync(() => func2(result), func3, func4, func5, func6, funcResult) : new Response<TResult>();
        }

        public static async Task<Response<TResult>> EvaluateAsync<T1, T2, T3, T4, T5, T6, T7, TResult>(Func<Task<Response<T1>>> func1, Func<T1, Task<Response<T2>>> func2, Func<T2, Task<Response<T3>>> func3, Func<T3, Task<Response<T4>>> func4, Func<T4, Task<Response<T5>>> func5, Func<T5, Task<Response<T6>>> func6, Func<T6, Task<Response<T7>>> func7, Func<T7, Task<Response<TResult>>> funcResult)
        {
            var result = await func1();
            return result ? await EvaluateAsync(() => func2(result), func3, func4, func5, func6, func7, funcResult) : new Response<TResult>();
        }

        public static async Task<Response<TResult>> EvaluateAsync<T1, T2, T3, T4, T5, T6, T7, T8, TResult>(Func<Task<Response<T1>>> func1, Func<T1, Task<Response<T2>>> func2, Func<T2, Task<Response<T3>>> func3, Func<T3, Task<Response<T4>>> func4, Func<T4, Task<Response<T5>>> func5, Func<T5, Task<Response<T6>>> func6, Func<T6, Task<Response<T7>>> func7, Func<T7, Task<Response<T8>>> func8, Func<T8, Task<Response<TResult>>> funcResult)
        {
            var result = await func1();
            return result ? await EvaluateAsync(() => func2(result), func3, func4, func5, func6, func7, func8, funcResult) : new Response<TResult>();
        }
    }
}
