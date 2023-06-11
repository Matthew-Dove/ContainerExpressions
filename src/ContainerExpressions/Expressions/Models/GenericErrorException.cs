using System;

namespace ContainerExpressions.Expressions.Models
{
    /// <summary>Exception class for logging out custom error types that are not exceptions.</summary>
    /// <typeparam name="TError">The non-exception error type you'd like to log.</typeparam>
    public sealed class GenericErrorException<TError> : Exception
    {
        public TError Error { get; }

        internal GenericErrorException(TError error) : base(string.Empty) { Error = error; }

        internal GenericErrorException(TError error, string message) : base(message) { Error = error; }
    }
}
