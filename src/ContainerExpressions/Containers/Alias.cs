using System;
using System.Collections.Generic;

namespace ContainerExpressions.Containers
{
    /// <summary>Allows you to name a type.</summary>
    public abstract class Alias<T> : IEquatable<Alias<T>>
    {
        /// <summary>The underlying value of T.</summary>
        public T Value { get; }

        /// <summary>Allows you to name a type.</summary>
        protected Alias(T value)
        {
            Value = value;
        }

        public static implicit operator T(Alias<T> alias) => alias.Value;

        public bool Equals(Alias<T> other) => other != null && ((other.Value == null && Value == null) || other.Equals(Value));

        public bool Equals(T value) => (value == null && Value == null) || EqualityComparer<T>.Default.Equals(Value, value);

        public override bool Equals(object obj) => obj != null && obj is Alias<T> && Equals(obj as Alias<T>);

        public override int GetHashCode() => Value?.GetHashCode() ?? 0;

        public override string ToString() => Value?.ToString() ?? string.Empty;

        public static bool operator !=(Alias<T> x, Alias<T> y) => !(x == y);
        public static bool operator ==(Alias<T> x, Alias<T> y)
        {
            if ((object)x == null) return (object)y == null;
            if ((object)y == null) return (object)x == null;
            return x.Equals(y);
        }

        public static bool operator !=(Alias<T> x, T y) => !(x == y);
        public static bool operator ==(Alias<T> x, T y) => (object)x != null && x.Equals(y);

        public static bool operator !=(T x, Alias<T> y) => !(x == y);
        public static bool operator ==(T x, Alias<T> y) => (object)y != null && y.Equals(x);
    }
}
