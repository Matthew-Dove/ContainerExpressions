using ContainerExpressions.Containers;
using System;

namespace ContainerExpressions.Expressions.Core
{
    internal static class Compose
    {
        public static Response<TResult> Evaluate<TResult>(Func<Response<TResult>> func) => func();

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

        public static Response<TResult> Evaluate<T1, T2, TResult>(Func<Response<T1>> func1, Func<T1, Response<T2>> func2, Func<T2, Response<TResult>> funcResult)
        {
            var response = new Response<TResult>();

            var result1 = func1();
            if (result1)
            {
                var result2 = func2(result1);
                if (result2)
                {
                    response = response.WithValue(funcResult(result2));
                }
            }

            return response;
        }

        public static Response<TResult> Evaluate<T1, T2, T3, TResult>(Func<Response<T1>> func1, Func<T1, Response<T2>> func2, Func<T2, Response<T3>> func3, Func<T3, Response<TResult>> funcResult)
        {
            var response = new Response<TResult>();

            var result1 = func1();
            if (result1)
            {
                var result2 = func2(result1);
                if (result2)
                {
                    var result3 = func3(result2);
                    if (result3)
                    {
                        response = response.WithValue(funcResult(result3));
                    }
                }
            }

            return response;
        }
    }
}
