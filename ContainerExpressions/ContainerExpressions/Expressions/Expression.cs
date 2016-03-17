using ContainerExpressions.Containers;
using System;

namespace ContainerExpressions.Expressions
{
    public static class Expression
    {
        public static Response<T> Compose<T>(Func<Response<T>> func) => Core.Compose.Evaluate(func);
    }
}
