using System;

namespace ContainerExpressions.Containers
{
    /// <summary>A place to put shared common functions.</summary>
    public static class Lambda
    {
        /// <summary>Maps the input directly to the output.</summary>
        public static T Identity<T>(T x) => x;

        /// <summary>Pretends to return a T (i.e. to compile), but will really throw the passed exception.</summary>
        public static T Identity<T>(Exception ex) => throw ex;

        /// <summary>Discards the function input, and returns the specified result.</summary>
        public static Func<T, T> Default<T>(T result = default) => _ => result;

        /// <summary>Discards the function input, and returns the specified result (of a different type to the input type).</summary>
        public static Func<T, TResult> Default<T, TResult>(TResult result = default) => _ => result;
    }
}
