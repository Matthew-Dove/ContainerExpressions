﻿using ContainerExpressions.Containers;
using System;
using System.Text;

namespace ContainerExpressions.Expressions.Models
{
    /// <summary>Exception class for logging out custom error types that are not exceptions.</summary>
    /// <typeparam name="TError">The non-exception error type you'd like to log.</typeparam>
    public sealed class GenericErrorException<TError> : Exception
    {
        public TError Error { get; }

        internal GenericErrorException(
            TError error,
            string argumentExpression = "",
            string memberName = "",
            string filePath = "",
            int lineNumber = 0
        ) : this(error, new ErrorMessage(string.Empty), argumentExpression, memberName, filePath, lineNumber) { }

        internal GenericErrorException(
            TError error,
            ErrorMessage message,
            string argumentExpression = "",
            string memberName = "",
            string filePath = "",
            int lineNumber = 0
        ) : base(Format(error, message))
        {
            Error = error;
            this.AddCallerAttributes(argumentExpression, memberName, filePath, lineNumber);
        }

        private static string Format(TError error, string message)
        {
            return new StringBuilder()
                .AppendLine()
                .Append("Message: ").AppendLine(message)
                .Append("Error: ").AppendLine(error?.ToString() ?? string.Empty)
                .ToString();
        }
    }
}
