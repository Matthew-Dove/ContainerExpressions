using System;

namespace ContainerExpressions.Containers
{
    public sealed class NotNull<T> : IEquatable<NotNull<T>> where T : class
    {
        public T Value { get; }

        private NotNull(T value)
        {
            if (value == null)
                throw new ArgumentNullException(nameof(value));

            Value = value;
        }

        public static implicit operator T(NotNull<T> notNull) => notNull.Value;

        public static implicit operator NotNull<T>(T value) => new NotNull<T>(value);

        public bool Equals(NotNull<T> other) => other != null && other.Value.Equals(Value);

        public override bool Equals(object obj) => Equals(obj as NotNull<T>);

        public override int GetHashCode() => Value.GetHashCode();

        public override string ToString() => Value.ToString();

        public static bool operator !=(NotNull<T> x, NotNull<T> y) => !(x == y);

        public static bool operator ==(NotNull<T> x, NotNull<T> y)
        {
            if (x == null && y == null) return true;
            if (x == null || y == null) return false;
            return x.Value.Equals(y.Value);
        }
    }
}
