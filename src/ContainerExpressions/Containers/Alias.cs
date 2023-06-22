using System;
using System.Collections.Generic;

namespace ContainerExpressions.Containers
{
    /// <summary>Allows you to name a type.</summary>
    public abstract class Alias<T> : IEquatable<Alias<T>>, IEquatable<T>
    {
        /// <summary>The underlying value of T.</summary>
        public T Value { get; }

        /// <summary>Allows you to name a type.</summary>
        protected Alias(T value) { Value = value; }

        public static implicit operator T(Alias<T> alias) => alias.Value;

        public override string ToString() => Value?.ToString() ?? string.Empty;

        public override int GetHashCode() => Value?.GetHashCode() ?? 0;

        public bool Equals(Alias<T> other) => other is not null && other.Equals(Value);

        public bool Equals(T value) => EqualityComparer<T>.Default.Equals(Value, value);

        public override bool Equals(object obj) => obj != null && obj is Alias<T> alias && Equals(alias);

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

    /// <summary>
    /// A struct alterative to the Alias class.
    /// <para>Note: As structs cannot be inherited, you cannot rename this type as you can for Alias.</para>
    /// <para>However you can still use this in it's raw form for method overloading, or you can have a global usings file to alias your custom type.</para>
    /// </summary>
    public readonly struct ValueAlias<T> : IEquatable<ValueAlias<T>>, IEquatable<T>
    {
        /// <summary>The underlying value of T.</summary>
        public T Value { get; }

        /// <summary>A struct alterative to the Alias class.</summary>
        public ValueAlias(T value) { Value = value; }

        public static implicit operator T(ValueAlias<T> alias) => alias.Value;

        public override string ToString() => Value?.ToString() ?? string.Empty;

        public override int GetHashCode() => Value?.GetHashCode() ?? 0;

        public bool Equals(ValueAlias<T> other) => other.Equals(Value);

        public bool Equals(T value) => EqualityComparer<T>.Default.Equals(Value, value);

        public override bool Equals(object obj) => (obj is ValueAlias<T> va && Equals(va)) || (obj is T v && Equals(v));

        public static bool operator !=(ValueAlias<T> x, ValueAlias<T> y) => !(x == y);
        public static bool operator ==(ValueAlias<T> x, ValueAlias<T> y) => x.Equals(y);

        public static bool operator !=(ValueAlias<T> x, T y) => !(x == y);
        public static bool operator ==(ValueAlias<T> x, T y) => x.Equals(y);

        public static bool operator !=(T x, ValueAlias<T> y) => !(x == y);
        public static bool operator ==(T x, ValueAlias<T> y) => y.Equals(x);
    }

    public static class ValueAlias
    {
        public static ValueAlias<T> Create<T>(T value) => new(value);
    }
}
