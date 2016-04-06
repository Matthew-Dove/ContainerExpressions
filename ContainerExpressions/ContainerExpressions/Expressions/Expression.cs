using ContainerExpressions.Containers;
using System;

namespace ContainerExpressions.Expressions
{
    public static class Expression
    {
        #region Response

        public static Response Compose<T1>(Func<Response<T1>> func1, Func<T1, Response> funcResult) => Core.Compose.Evaluate(func1, funcResult);
        public static Response Compose<T1, T2>(Func<Response<T1>> func1, Func<T1, Response<T2>> func2, Func<T2, Response> funcResult) => Core.Compose.Evaluate(func1, func2, funcResult);
        public static Response Compose<T1, T2, T3>(Func<Response<T1>> func1, Func<T1, Response<T2>> func2, Func<T2, Response<T3>> func3, Func<T3, Response> funcResult) => Core.Compose.Evaluate(func1, func2, func3, funcResult);
        public static Response Compose<T1, T2, T3, T4>(Func<Response<T1>> func1, Func<T1, Response<T2>> func2, Func<T2, Response<T3>> func3, Func<T3, Response<T4>> func4, Func<T4, Response> funcResult) => Core.Compose.Evaluate(func1, func2, func3, func4, funcResult);

        #endregion

        #region ResponseT

        public static Response<TResult> Compose<T1, TResult>(Func<Response<T1>> func1, Func<T1, Response<TResult>> funcResult) => Core.Compose.Evaluate(func1, funcResult);
        public static Response<TResult> Compose<T1, T2, TResult>(Func<Response<T1>> func1, Func<T1, Response<T2>> func2, Func<T2, Response<TResult>> funcResult) => Core.Compose.Evaluate(func1, func2, funcResult);
        public static Response<TResult> Compose<T1, T2, T3, TResult>(Func<Response<T1>> func1, Func<T1, Response<T2>> func2, Func<T2, Response<T3>> func3, Func<T3, Response<TResult>> funcResult) => Core.Compose.Evaluate(func1, func2, func3, funcResult);
        public static Response<TResult> Compose<T1, T2, T3, T4, TResult>(Func<Response<T1>> func1, Func<T1, Response<T2>> func2, Func<T2, Response<T3>> func3, Func<T3, Response<T4>> func4, Func<T4, Response<TResult>> funcResult) => Core.Compose.Evaluate(func1, func2, func3, func4, funcResult);

        #endregion
    }
}
