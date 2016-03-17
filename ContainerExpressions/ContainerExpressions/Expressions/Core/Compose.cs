using ContainerExpressions.Containers;
using System;

namespace ContainerExpressions.Expressions.Core
{
    internal static class Compose
    {
        public static Response<T> Evaluate<T>(Func<Response<T>> func)
        {
            return func();
        }
    }
}
