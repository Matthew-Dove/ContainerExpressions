using System;
namespace System.Runtime.CompilerServices { internal static class IsExternalInit { } }

namespace ContainerExpressions.Containers
{
    /// <summary>
    /// A strongly typed collection of strings that represents choices.
    /// <para>Simular to using names from an enum.</para>
    /// </summary>
    public sealed class Option : IEquatable<string>, IEquatable<Option>
    {
        /// <summary>The option's value.</summary>
        public string Value { get { return _value; } init { _value = value.ThrowIfNull(); } }
        private readonly string _value = string.Empty;

        public Option() { }

        /// <summary>Set an option value, cannot be null.</summary>
        public Option(string value) { Value = value; }

        public static implicit operator string(Option option) => option.Value;

        public override string ToString() => Value;

        public override int GetHashCode() => Value.GetHashCode();

        public bool Equals(Option other) => other is not null && other.Equals(Value);

        public bool Equals(string value) => Value.Equals(value);

        public override bool Equals(object obj) => obj != null && ((obj is Option option && Equals(option)) || (obj is string @string && Equals(@string)));

        public static bool operator !=(Option x, Option y) => !(x == y);
        public static bool operator ==(Option x, Option y)
        {
            if ((object)x == null) return (object)y == null;
            if ((object)y == null) return (object)x == null;
            return x.Equals(y);
        }

        public static bool operator !=(Option x, string y) => !(x == y);
        public static bool operator ==(Option x, string y) => (object)x != null && x.Equals(y);

        public static bool operator !=(string x, Option y) => !(x == y);
        public static bool operator ==(string x, Option y) => (object)y != null && y.Equals(x);
    }
}
