using ContainerExpressions.Containers;
using System;

namespace ContainerExpressions.Expressions
{
    public static class Expression
    {
        public static Response<T> Compose<T>(Func<Response<T>> func) => Core.Compose.Evaluate(func);
        public static Response<TResult> Compose<T1, TResult>(Func<Response<T1>> func1, Func<T1, Response<TResult>> funcResult) => Core.Compose.Evaluate(func1, funcResult);
    }
}
