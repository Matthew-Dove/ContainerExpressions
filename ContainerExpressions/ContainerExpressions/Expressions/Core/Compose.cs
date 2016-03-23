using ContainerExpressions.Containers;
using System;

namespace ContainerExpressions.Expressions.Core
{
    internal static class Compose
    {
        public static Response<T> Evaluate<T>(Func<Response<T>> func) => func();

        public static Response<TResult> Evaluate<T1, TResult>(Func<Response<T1>> func1, Func<T1, Response<TResult>> funcResult)
        {
            var response = new Response<TResult>();

            var result = func1();
            if (result)
            {
                response = response.WithValue(funcResult(result));
            }

            return response;
        }
    }
}
