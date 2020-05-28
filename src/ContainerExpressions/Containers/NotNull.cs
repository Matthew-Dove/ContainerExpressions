using System;

namespace ContainerExpressions.Containers
{
    /// <summary>
    /// A containter for a reference type T, that ensures the provided value of T is not null.
    /// <para>Useful for wrapping method argument types, that always require a reference value.</para>
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

        public bool Equals(NotNull<T> other) => (object)other != null && other.Value.Equals(Value);

        public bool Equals(T value) => value != null && Value.Equals(value);

        public override bool Equals(object obj) => obj != null && Equals(obj as NotNull<T>);

        public override int GetHashCode() => Value.GetHashCode();

        public override string ToString() => Value.ToString();

        public static bool operator !=(NotNull<T> x, NotNull<T> y) => !(x == y);

        public static bool operator ==(NotNull<T> x, NotNull<T> y)
        {
            if ((object)x == y) return true;
            if ((object)x == null) return false;
            return x.Equals(y);
        }

        public static bool operator !=(NotNull<T> x, T y) => !(x == y);
        public static bool operator ==(NotNull<T> x, T y) => (object)x != null && x.Value.Equals(y);

        public static bool operator !=(T x, NotNull<T> y) => !(x == y);
        public static bool operator ==(T x, NotNull<T> y) => (object)y != null && y.Value.Equals(x);
    }
}
