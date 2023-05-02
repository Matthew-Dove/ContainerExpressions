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
        protected Alias(T value)
        {
            Value = value;
        }

        public static implicit operator T(Alias<T> alias) => alias.Value;

        public bool Equals(Alias<T> other) => other != null && ((other.Value == null && Value == null) || other.Equals(Value));

        public bool Equals(T value) => (value == null && Value == null) || EqualityComparer<T>.Default.Equals(Value, value);

        public override bool Equals(object obj) => obj != null && obj is Alias<T> alias && Equals(alias);

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

    /// <summary>
    /// A struct alterative to the Alias class.
    /// <para>Note: As structs cannot be inherited, you cannot rename this type as you can for Alias.</para>
    /// <para>However you can still use this in it's raw form for method overloading, or you can have a global usings file to alias your custom type.</para>
    /// <para><![CDATA[global using XXX = ContainerExpressions.Containers.A<int>;]]></para>
    /// </summary>
    public readonly struct A<T> : IEquatable<A<T>>, IEquatable<T>
    {
        /// <summary>The underlying value of T.</summary>
        public T Value { get; }

        /// <summary>A struct alterative to the Alias class.</summary>
        public A(T value)
        {
            Value = value;
        }

        public static implicit operator T(A<T> alias) => alias.Value;
        public static implicit operator A<T>(T value) => new (value);

        public bool Equals(A<T> other) => ((other.Value == null && Value == null) || other.Equals(Value));

        public bool Equals(T value) => (value == null && Value == null) || EqualityComparer<T>.Default.Equals(Value, value);

        public override bool Equals(object obj) => obj != null && obj is A<T> a && Equals(a);

        public override int GetHashCode() => Value?.GetHashCode() ?? 0;

        public override string ToString() => Value?.ToString() ?? string.Empty;

        public static bool operator !=(A<T> x, A<T> y) => !(x == y);
        public static bool operator ==(A<T> x, A<T> y) => x.Equals(y);

        public static bool operator !=(A<T> x, T y) => !(x == y);
        public static bool operator ==(A<T> x, T y) => x.Equals(y);

        public static bool operator !=(T x, A<T> y) => !(x == y);
        public static bool operator ==(T x, A<T> y) => y.Equals(x);
    }
}
