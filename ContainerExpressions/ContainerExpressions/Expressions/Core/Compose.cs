using ContainerExpressions.Containers;
using System;

namespace ContainerExpressions.Expressions.Core
{
    internal static class Compose
    {
        public static Response<TResult> Evaluate<T1, TResult>(Func<Response<T1>> func1, Func<T1, Response<TResult>> funcResult)
        {
            var result = func1();
            return result ? funcResult(result) : Response.Create<TResult>();
        }

        public static Response<TResult> Evaluate<T1, T2, TResult>(Func<Response<T1>> func1, Func<T1, Response<T2>> func2, Func<T2, Response<TResult>> funcResult)
        {
            var result = func1();
            return result ? Evaluate(() => func2(result), funcResult) : Response.Create<TResult>();
        }

        public static Response<TResult> Evaluate<T1, T2, T3, TResult>(Func<Response<T1>> func1, Func<T1, Response<T2>> func2, Func<T2, Response<T3>> func3, Func<T3, Response<TResult>> funcResult)
        {
            var result = func1();
            return result ? Evaluate(() => func2(result), func3, funcResult) : Response.Create<TResult>();
        }

        public static Response<TResult> Evaluate<T1, T2, T3, T4, TResult>(Func<Response<T1>> func1, Func<T1, Response<T2>> func2, Func<T2, Response<T3>> func3, Func<T3, Response<T4>> func4, Func<T4, Response<TResult>> funcResult)
        {
            var result = func1();
            return result ? Evaluate(() => func2(result), func3, func4, funcResult) : Response.Create<TResult>();
        }
    }
}
