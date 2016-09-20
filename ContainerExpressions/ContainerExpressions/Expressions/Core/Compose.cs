using ContainerExpressions.Containers;
using System;

namespace ContainerExpressions.Expressions.Core
{
    internal static class Compose
    {
        public static Response<TResult> Evaluate<T1, TResult>(Func<Response<T1>> func1, Func<T1, Response<TResult>> funcResult)
        {
            var result = func1();
            return result ? funcResult(result) : new Response<TResult>();
        }

        public static Response<TResult> Evaluate<T1, T2, TResult>(Func<Response<T1>> func1, Func<T1, Response<T2>> func2, Func<T2, Response<TResult>> funcResult)
        {
            var result = func1();
            return result ? Evaluate(() => func2(result), funcResult) : new Response<TResult>();
        }

        public static Response<TResult> Evaluate<T1, T2, T3, TResult>(Func<Response<T1>> func1, Func<T1, Response<T2>> func2, Func<T2, Response<T3>> func3, Func<T3, Response<TResult>> funcResult)
        {
            var result = func1();
            return result ? Evaluate(() => func2(result), func3, funcResult) : new Response<TResult>();
        }

        public static Response<TResult> Evaluate<T1, T2, T3, T4, TResult>(Func<Response<T1>> func1, Func<T1, Response<T2>> func2, Func<T2, Response<T3>> func3, Func<T3, Response<T4>> func4, Func<T4, Response<TResult>> funcResult)
        {
            var result = func1();
            return result ? Evaluate(() => func2(result), func3, func4, funcResult) : new Response<TResult>();
        }

        public static Response<TResult> Evaluate<T1, T2, T3, T4, T5, TResult>(Func<Response<T1>> func1, Func<T1, Response<T2>> func2, Func<T2, Response<T3>> func3, Func<T3, Response<T4>> func4, Func<T4, Response<T5>> func5, Func<T5, Response<TResult>> funcResult)
        {
            var result = func1();
            return result ? Evaluate(() => func2(result), func3, func4, func5, funcResult) : new Response<TResult>();
        }

        public static Response<TResult> Evaluate<T1, T2, T3, T4, T5, T6, TResult>(Func<Response<T1>> func1, Func<T1, Response<T2>> func2, Func<T2, Response<T3>> func3, Func<T3, Response<T4>> func4, Func<T4, Response<T5>> func5, Func<T5, Response<T6>> func6, Func<T6, Response<TResult>> funcResult)
        {
            var result = func1();
            return result ? Evaluate(() => func2(result), func3, func4, func5, func6, funcResult) : new Response<TResult>();
        }

        public static Response<TResult> Evaluate<T1, T2, T3, T4, T5, T6, T7, TResult>(Func<Response<T1>> func1, Func<T1, Response<T2>> func2, Func<T2, Response<T3>> func3, Func<T3, Response<T4>> func4, Func<T4, Response<T5>> func5, Func<T5, Response<T6>> func6, Func<T6, Response<T7>> func7, Func<T7, Response<TResult>> funcResult)
        {
            var result = func1();
            return result ? Evaluate(() => func2(result), func3, func4, func5, func6, func7, funcResult) : new Response<TResult>();
        }

        public static Response<TResult> Evaluate<T1, T2, T3, T4, T5, T6, T7, T8, TResult>(Func<Response<T1>> func1, Func<T1, Response<T2>> func2, Func<T2, Response<T3>> func3, Func<T3, Response<T4>> func4, Func<T4, Response<T5>> func5, Func<T5, Response<T6>> func6, Func<T6, Response<T7>> func7, Func<T7, Response<T8>> func8, Func<T8, Response<TResult>> funcResult)
        {
            var result = func1();
            return result ? Evaluate(() => func2(result), func3, func4, func5, func6, func7, func8, funcResult) : new Response<TResult>();
        }
    }
}
