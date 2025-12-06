using System;
using System.Linq;

namespace ContainerExpressions.Containers
{
    /// <summary>A formatted message to trace via the logger.</summary>
    public readonly struct Format : IEquatable<string>, IEquatable<Format>
    {
        public static readonly Format Default = default;

        internal readonly string Message { get; }
        internal readonly object[] Args { get; }

        public Format(string message, params object[] args) { Message = message; Args = args; }

        public override bool Equals(object obj)
        {
            if (obj is Format format) return Equals(format);
            if (obj is string str) return Equals(str);
            return false;
        }

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

            if (Message is null && Args is null) return other.Message is null && other.Args is null;
            if (Message is null) return other.Message is null && Args.Length == other.Args.Length && (Args.Length == 0 || Args.SequenceEqual(other.Args));
            if ((Args is null || Args.Length == 0) && (other.Args is null || other.Args.Length == 0)) return Message.Equals(other.Message);

            return
                (other.Args.Length == Args.Length && (other.Args.Length == 0 || other.Args.SequenceEqual(Args)) && other.Message.Equals(Message))
                ||
                (other.Args.Length == 0 && other.Message.Equals(GetMessageTemplate(Message, Args)))
                ||
                (Args.Length == 0 && Message.Equals(GetMessageTemplate(other.Message, other.Args)));
        }

        public override string ToString() => GetMessageTemplate(Message, Args);

        private static string GetMessageTemplate(string message, object[] args)
        {
            if (message is not null && args is not null && args.Length > 0) return FormatHelper.NamedPlaceholders(message, args);
            return message;
        }

        public static implicit operator Format(string message) => new Format(message, Array.Empty<string>());

        public static bool operator !=(Format x, Format y) => !(x == y);
        public static bool operator ==(Format x, Format y) => x.Equals(y);

        public override int GetHashCode()
        {
            unchecked
            {
                var hash = 17;
                hash = hash * 23 + (Message?.GetHashCode() ?? 0);
                if (Args != null)
                {
                    foreach (var arg in Args)
                    {
                        hash = hash * 23 + (arg?.GetHashCode() ?? 0);
                    }
                }
                return hash;
            }
        }
    }

    public static class FormatExtensions
    {
        public static Format WithArgs(this string message, params object[] args) => new Format(message, args);
    }
}
