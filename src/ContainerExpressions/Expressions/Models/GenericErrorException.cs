using ContainerExpressions.Containers;
using System;

namespace ContainerExpressions.Expressions.Models
{
    /// <summary>Exception class for logging out custom error types that are not exceptions.</summary>
    /// <typeparam name="TError">The non-exception error type you'd like to log.</typeparam>
    public sealed class GenericErrorException<TError> : Exception
    {
        public TError Error { get; }

        internal GenericErrorException(TError error) : this(error, string.Empty) { }

        internal GenericErrorException(TError error, Format message) : base(Format(error, message)) { Error = error; }

        private static string Format(TError error, Format message)
        {
            var err = error?.ToString() ?? string.Empty;
            var msg = message.ToString() ?? string.Empty;

            // Avoid creating a new string if we don't need to combine these two.
            if (msg == string.Empty) return err;
            if (err == string.Empty) return msg;

            return $"{msg} {err}";
        }
    }
}
