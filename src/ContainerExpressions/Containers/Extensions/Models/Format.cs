using System;
using System.Linq;

namespace ContainerExpressions.Containers
{
    /// <summary>A formatted message to trace via the logger.</summary>
    public readonly struct Format : IEquatable<string>, IEquatable<Format>
    {
        internal readonly string Message { get; }
        internal readonly object[] Args { get; }

        public Format(string message, params object[] args) { Message = message; Args = args; }

        public bool Equals(string other)
        {
            if (other is null) return Message is null;
            if (Args is not null && Args.Length > 0) return other.Equals(GetMessageTemplate(Message, Args));
            return other.Equals(Message);
        }

        public bool Equals(Format other)
        {
            if (other.Message is null && other.Args is null) return Message is null && Args is null;
            if (other.Message is null) return Message is null && other.Args.Length == Args.Length && (other.Args.Length == 0 || other.Args.SequenceEqual(Args));
            if ((other.Args is null || other.Args.Length == 0) && (Args is null || Args.Length == 0)) return other.Message.Equals(Message);
            
            return
                (other.Message.Equals(Message) && other.Args.Length == Args.Length && (other.Args.Length == 0 || other.Args.SequenceEqual(Args)))
                ||
                ((other.Args is null || other.Args.Length == 0) && other.Message.Equals(GetMessageTemplate(Message, Args)))
                ||
                ((Args is null || Args.Length == 0) && Message.Equals(GetMessageTemplate(other.Message, other.Args)));
        }

        public override string ToString() => GetMessageTemplate(Message, Args);

        private static string GetMessageTemplate(string message, object[] args)
        {
            if (message is not null && args is not null && args.Length > 0) return FormatHelper.NamedPlaceholders(message, args);
            return message;
        }

        public static implicit operator Format(string message) => new Format(message, Array.Empty<string>());
    }

    public static class FormatExtensions
    {
        public static Format WithArgs(this string message, params object[] args) => new Format(message, args);
    }
}
