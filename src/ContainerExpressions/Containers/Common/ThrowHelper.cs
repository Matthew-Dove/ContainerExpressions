using System;

namespace ContainerExpressions.Containers.Common
{
    internal static class ThrowHelper
    {
        public static void ArgumentOutOfRangeException(string name) => throw new ArgumentOutOfRangeException(name);

        public static void ArgumentNullException(string name) => throw new ArgumentNullException(name);
    }
}
