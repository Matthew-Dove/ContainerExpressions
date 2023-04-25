using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ContainerExpressions.Containers
{
    /// <summary>
    /// Cache a single instance per type of TValue.
    /// <para>Think of it as a generalized string.Empty.</para>
    /// <para>The main purpose of Cache is to save on garbage collected objects when using default values for references.</para>
    /// </summary>
    public static class Cache
    {
        /// <summary>Gets a default instance for TValue. Set must be called first for TValue.</summary>
        public static TValue Get<TValue>() where TValue : class => Instance<TValue>.Value;

        /// <summary>Stores a default instance for TValue. You can only call this once per type.</summary>
        public static void Set<TValue>(TValue value) where TValue : class => Instance<TValue>.Value = value;

        static Cache()
        {
            #region General Reference Types

            Set(Task.CompletedTask);
            Set(Array.Empty<Task>());
            Set(Enumerable.Empty<Task>());
            Set(new List<Task>(0));

            Set(new object());
            Set(Array.Empty<object>());
            Set(Enumerable.Empty<object>());
            Set(new List<object>(0));

            Set(string.Empty);
            Set(Array.Empty<string>());
            Set(Enumerable.Empty<string>());
            Set(new List<string>(0));

            #endregion

            #region General Value Types

            /**
             * The following Primitives / ValueTypes / Structs have not been added, as it's unlikely they'll be used for a cache.
             * They can always be added manually, using the Set<T> method at startup if required.
             * This list should not be considered exhaustive, I'm sure there will only be more to come as the language evolves over time.
             * > Int16
             * > Int64
             * > Int128
             * > IntPtr
             * > UInt16
             * > UInt32
             * > UInt64
             * > UInt128
             * > UIntPtr
             * > DateOnly
             * > DateTime
             * > DateTimeOffset
             * > Guid
             * > BigInteger
             * > Complex
             * > SByte
             * > TimeOnly
             * > TimeSpan
             * > Enum
            **/

            Set(Array.Empty<bool>());
            Set(Enumerable.Empty<bool>());
            Set(new List<bool>(0));

            Set(Array.Empty<byte>());
            Set(Enumerable.Empty<byte>());
            Set(new List<byte>(0));

            Set(Array.Empty<char>());
            Set(Enumerable.Empty<char>());
            Set(new List<char>(0));

            Set(Array.Empty<double>());
            Set(Enumerable.Empty<double>());
            Set(new List<double>(0));

            Set(Array.Empty<decimal>());
            Set(Enumerable.Empty<decimal>());
            Set(new List<decimal>(0));

            Set(Array.Empty<float>());
            Set(Enumerable.Empty<float>());
            Set(new List<float>(0));

            Set(Array.Empty<int>());
            Set(Enumerable.Empty<int>());
            Set(new List<int>(0));

            Set(Array.Empty<short>());
            Set(Enumerable.Empty<short>());
            Set(new List<short>(0));

            Set(Array.Empty<long>());
            Set(Enumerable.Empty<long>());
            Set(new List<long>(0));

            #endregion
        }
    }

    /// <summary>
    /// A holder for some cached instance of T.
    /// <para>T must be a reference type, and T cannot be null.</para>
    /// <para>This instance of T may only be set once.</para>
    /// <para>If you attempt to set some T to null, or set the same type twice, or try to get a type that has not been set first; an exception will be thrown.</para>
    /// </summary>
    file static class Instance<T> where T : class
    {
        private static T _value = null;
        public static T Value {
            get
            {
                if (_value == null) throw new InvalidOperationException($"Cache must be set for type: {typeof(T)}, before attempting to retrieve it.");
                return _value;
            }
            set
            {
                if (value == null) throw new ArgumentNullException(nameof(Value), $"Invalid value for cache type: {typeof(T)}.");
                if (_value != null) throw new InvalidOperationException($"Cache has already been set for type: {typeof(T)}.");
                _value = value;
            }
        }
    }
}
