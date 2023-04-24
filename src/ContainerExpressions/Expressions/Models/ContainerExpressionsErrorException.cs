using System;

namespace ContainerExpressions.Expressions.Models
{
    /// <summary>Exception class for logging out custom error types that are not exceptions.</summary>
    /// <typeparam name="TError">The non-exception error type you'd like to log.</typeparam>
    public sealed class ContainerExpressionsErrorException<TError> : Exception
    {
        public TError Error { get; }

        internal ContainerExpressionsErrorException(TError error) : base(string.Empty)
        {
            Error = error;
        }

        internal ContainerExpressionsErrorException(TError error, string message) : base(message)
        {
            Error = error;
        }
    }
}
