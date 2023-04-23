using System;
using System.Collections.Generic;

namespace ContainerExpressions.Containers
{
    /// <summary>
    /// A containter for a reference type T, that ensures the provided value of T is not null.
    /// <para>Useful for wrapping method argument types, that always require a reference value.</para>
    /// <para>Use this when you want to limit the caller from creating an instance without going though the T constructor, or you want move semantics.</para>
    /// </summary>
    public sealed class NotNull<T> : IEquatable<NotNull<T>> where T : class
    {
        /// <summary>The underlying value of T, which is not null.</summary>
        public T Value { get; }

        /// <summary>Create a check for T, such that it is not null.</summary>
        /// <param name="value">The reference you want to ensure is not null.</param>
        private NotNull(T value)
        {
            if (value == null)
                throw new ArgumentNullException(nameof(value));

            Value = value;
        }

        public static implicit operator T(NotNull<T> notNull) => notNull.Value;

        public static implicit operator NotNull<T>(T value) => new NotNull<T>(value);

        public bool Equals(NotNull<T> other) => (object)other != null && other.Equals(Value);

        public bool Equals(T value) => value != null && EqualityComparer<T>.Default.Equals(Value, value);

        public override bool Equals(object obj) => obj != null && Equals(obj as NotNull<T>);

        public override int GetHashCode() => Value?.GetHashCode() ?? 0;

        public override string ToString() => Value?.ToString() ?? string.Empty;

        public static bool operator !=(NotNull<T> x, NotNull<T> y) => !(x == y);
        public static bool operator ==(NotNull<T> x, NotNull<T> y)
        {
            if ((object)x == null) return (object)y == null;
            if ((object)y == null) return (object)x == null;
            return x.Equals(y);
        }

        public static bool operator !=(NotNull<T> x, T y) => !(x == y);
        public static bool operator ==(NotNull<T> x, T y) => (object)x != null && x.Equals(y);

        public static bool operator !=(T x, NotNull<T> y) => !(x == y);
        public static bool operator ==(T x, NotNull<T> y) => (object)y != null && y.Equals(x);
    }

    /// <summary>
    /// NotNull: A containter for a reference type T, that ensures the provided value of T is not null.
    /// <para>Useful for wrapping method argument types, that always require a reference value.</para>
    /// <para>Use this when you don't mind that the caller can create an instance without going though the T constructor, or you want copy semantics.</para>
    /// </summary>
    public readonly struct NN<T> : IEquatable<NN<T>> where T : class
    {
        /// <summary>The underlying value of T, which is not null.</summary>
        public T Value { get; }

        /// <summary>Create a check for T, such that it is not null.</summary>
        /// <param name="value">The reference you want to ensure is not null.</param>
        public NN(T value)
        {
            if (value == null)
                throw new ArgumentNullException(nameof(value));

            Value = value;
        }

        public static implicit operator T(NN<T> notNull) => notNull.Value;

        public static implicit operator NN<T>(T value) => new NN<T>(value);

        public bool Equals(NN<T> other) => other.Equals(Value);

        public bool Equals(T value) => value != null && EqualityComparer<T>.Default.Equals(Value, value);

        public override bool Equals(object obj) => obj != null && obj is NN<T> notNull && Equals(notNull);

        public override int GetHashCode() => Value?.GetHashCode() ?? 0;

        public override string ToString() => Value?.ToString() ?? string.Empty;

        public static bool operator !=(NN<T> x, NN<T> y) => !(x == y);
        public static bool operator ==(NN<T> x, NN<T> y) => x.Equals(y);

        public static bool operator !=(NN<T> x, T y) => !(x == y);
        public static bool operator ==(NN<T> x, T y) => y != null && x.Equals(y);

        public static bool operator !=(T x, NN<T> y) => !(x == y);
        public static bool operator ==(T x, NN<T> y) => x != null && y.Equals(x);
    }

    public static class NN
    {
        public static NN<T> Create<T>(T value) where T : class => new NN<T>(value);
    }
}
