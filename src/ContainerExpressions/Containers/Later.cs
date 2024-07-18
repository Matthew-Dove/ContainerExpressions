using ContainerExpressions.Containers.Internal;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ContainerExpressions.Containers
{
    public struct LazyLoader<T>
    {
        public T Value { get {
            if (_func != null) {
                _value = _func();
                _func = null;
            }
            return _value;
        } }

        private T _value;
        private Func<T> _func;

        internal LazyLoader(Func<T> func) { _func = func; }
    }

    // Use this over Later{T} when you want to have a local lazy value (i.e. you won't pass this around, put it in a cache, call it asyncly, etc).
    public readonly struct ValueLater<T> : IEquatable<ValueLater<T>>, IEquatable<T>
    {
        private static readonly EqualityComparer<T> _comparer = EqualityComparer<T>.Default;
        public readonly LazyLoader<T> Lazy;

        public ValueLater(Func<T> func) { Lazy = new LazyLoader<T>(func.ThrowIfNull()); }

        public override string ToString() => Lazy.Value?.ToString() ?? string.Empty;
        public override int GetHashCode() => _comparer.GetHashCode();
        public bool Equals(ValueLater<T> other) => _comparer.Equals(other.Lazy.Value, Lazy.Value);
        public bool Equals(T other) => _comparer.Equals(other, Lazy.Value);
        public override bool Equals(object obj)
        {
            return obj switch
            {
                ValueLater<T> other => Equals(other),
                T t => Equals(t),
                _ => false
            };
        }

        public static implicit operator T(ValueLater<T> later) => later.Lazy.Value;
        public static implicit operator ValueLater<T>(Func<T> func) => new ValueLater<T>(func);

        public static bool operator ==(ValueLater<T> x, ValueLater<T> y) => x.Equals(y);
        public static bool operator !=(ValueLater<T> x, ValueLater<T> y) => !(x == y);

        public static bool operator ==(ValueLater<T> x, T y) => x.Equals(y);
        public static bool operator !=(ValueLater<T> x, T y) => !(x == y);

        public static bool operator ==(T x, ValueLater<T> y) => y.Equals(x);
        public static bool operator !=(T x, ValueLater<T> y) => !(x == y);
    }

    public static class ValueLater
    {
        public static ValueLater<T> Create<T>(Func<T> func) => new ValueLater<T>(func);
    }

    /// <summary>Loads the value only once, the first time it's accessed.</summary>
    public sealed class Later<T>
    {
        /// <summary>Gets the value form the function, if this is the first time the value is read, the function will be invoked.</summary>
        public T Value { get { if (_func != null) { _value = _func(); _func = null; } return _value; } }
        private T _value;
        private Func<T> _func;

        /// <summary>Loads the value only once, the first time it's accessed.</summary>
        /// <param name="func">A function that will be called once (and only once) to generate a value.</param>
        public Later(Func<T> func)
        {
            if (func == null)
                Throw.ArgumentNullException(nameof(func));

            _value = default;
            _func = func;
        }

        /// <summary>When compared to T, the underlying value is returned.</summary>
        public static implicit operator T(Later<T> later) => later.Value;
    }

    /// <summary>Loads the value only once in a thread safe way, the first time it's accessed.</summary>
    public sealed class LaterAsync<T>
    {
        /// <summary>Gets the value form the function, if this is the first time the value is read, the function will be invoked (in a thread-safe manner).</summary>
        public Task<T> Value { get { return GetValue(); } }
        private Task<T> _value;
        private Func<Task<T>> _func;
        private readonly object _lock;

        /// <summary>Loads the value only once in a thread safe way, the first time it's accessed.</summary>
        /// <param name="func">A function that will be called once (and only once) to generate a value.</param>
        public LaterAsync(Func<Task<T>> func)
        {
            if (func == null)
                Throw.ArgumentNullException(nameof(func));

            _lock = new object();
            _value = null;
            _func = func;
        }

        private Task<T> GetValue()
        {
            if (_value == null)
            {
                lock (_lock)
                {
                    if (_value == null)
                    {
                        _value = _func();
                        if (_value.Status == TaskStatus.Created)
                        {
                            _value.Start();
                        }
                        _func = null;
                    }
                }
            }

            return _value;
        }

        /// <summary>When compared to T, the underlying Task is returned.</summary>
        public static implicit operator Task<T>(LaterAsync<T> later) => later.Value;
    }

    /// <summary>A helper class for the Later generic class.</summary>
    public static class Later
    {
        /// <summary>Creates a Later container for the value generating function.</summary>
        /// <typeparam name="T">The type the generating function returns.</typeparam>
        /// <param name="func">A function that will be called once (and only once) to generate a value.</param>
        /// <returns></returns>
        public static Later<T> Create<T>(Func<T> func) => new Later<T>(func); // A little trick so the caller doesn't have to specify T.

        /// <summary>Loads the value only once in a thread safe way, the first time it's accessed.</summary>
        public static LaterAsync<T> CreateAsync<T>(Func<Task<T>> func) => new LaterAsync<T>(func);
    }
}