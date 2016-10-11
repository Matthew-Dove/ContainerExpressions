using Repeat = ContainerExpressions.Containers.Retry;
using ContainerExpressions.Expressions.Models;
using System;

namespace ContainerExpressions.Containers
{
    /// <summary>Utility methods for the Retry Container.</summary>
    public static class RetryExtensions
    {
        /// <summary>Execute a function, and retries (using default values) if the Response is invalid.</summary>
        public static Func<Response<T>> Retry<T>(this Func<Response<T>> func) => () => Repeat.Execute(func, RetryOptions.Create());

        /// <summary>Execute a function, and retries (using custom values) if the Response is invalid.</summary>
        public static Func<Response<T>> Retry<T>(this Func<Response<T>> func, RetryOptions options) => () => Repeat.Execute(func, options);

        /// <summary>Execute a function, and retries (using default values) if the Response is invalid.</summary>
        public static Func<T, Response<TResult>> Retry<T, TResult>(this Func<T, Response<TResult>> func) => x => Repeat.Execute(x, func, RetryOptions.Create());

        /// <summary>Execute a function, and retries (using custom values) if the Response is invalid.</summary>
        public static Func<T, Response<TResult>> Retry<T, TResult>(this Func<T, Response<TResult>> func, RetryOptions options) => x => Repeat.Execute(x, func, options);
    }
}
