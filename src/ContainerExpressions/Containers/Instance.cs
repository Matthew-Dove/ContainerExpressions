﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ContainerExpressions.Containers
{
    /// <summary>
    /// Store a single instance per type of TValue.
    /// <para>Think of it as a generalized string.Empty.</para>
    /// <para>The main purpose of Instance is to save on garbage collected objects when using default values for references.</para>
    /// </summary>
    public static class Instance
    {
        /// <summary>Gets a default instance for TValue. Create must be called first for TValue.</summary>
        public static TValue Of<TValue>() where TValue : class => Instance<TValue>.Value;

        /// <summary>Stores a default instance for TValue. You can only call this once per type of TValue.</summary>
        public static void Create<TValue>(TValue value) where TValue : class => Instance<TValue>.Value = value;

        static Instance()
        {
            #region General Reference Types

            Create(Task.CompletedTask);
            Create(Array.Empty<Task>());
            Create(Enumerable.Empty<Task>());
            Create(new List<Task>(0));

            Create(new object());
            Create(Array.Empty<object>());
            Create(Enumerable.Empty<object>());
            Create(new List<object>(0));

            Create(string.Empty);
            Create(Array.Empty<string>());
            Create(Enumerable.Empty<string>());
            Create(new List<string>(0));

            #endregion

            #region General Value Types

            /**
             * The following Primitives / ValueTypes / Structs have not been added, as it's unlikely they'll be used.
             * They can always be added manually, using the Create() method at startup if required.
             * This list should not be considered exhaustive, there will only be more to come as the language evolves over time.
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

            Create(Array.Empty<bool>());
            Create(Enumerable.Empty<bool>());
            Create(new List<bool>(0));

            Create(Array.Empty<byte>());
            Create(Enumerable.Empty<byte>());
            Create(new List<byte>(0));

            Create(Array.Empty<char>());
            Create(Enumerable.Empty<char>());
            Create(new List<char>(0));

            Create(Array.Empty<double>());
            Create(Enumerable.Empty<double>());
            Create(new List<double>(0));

            Create(Array.Empty<decimal>());
            Create(Enumerable.Empty<decimal>());
            Create(new List<decimal>(0));

            Create(Array.Empty<float>());
            Create(Enumerable.Empty<float>());
            Create(new List<float>(0));

            Create(Array.Empty<int>());
            Create(Enumerable.Empty<int>());
            Create(new List<int>(0));

            Create(Array.Empty<short>());
            Create(Enumerable.Empty<short>());
            Create(new List<short>(0));

            Create(Array.Empty<long>());
            Create(Enumerable.Empty<long>());
            Create(new List<long>(0));

            #endregion
        }
    }

    /// <summary>
    /// A holder for some instance of T.
    /// <para>T must be a reference type, and T cannot be null.</para>
    /// <para>Each instance of T may only be set once.</para>
    /// <para>If you attempt to set some Instance T to null, set the same type twice, or try to get a type that has not been set first; an exception will be thrown.</para>
    /// </summary>
    file static class Instance<T> where T : class
    {
        private static T _value = null;
        public static T Value {
            get
            {
                if (_value == null) throw new InvalidOperationException($"Must set a value for type: {typeof(T)}, before attempting to retrieve it.");
                return _value;
            }
            set
            {
                if (value == null) throw new ArgumentNullException(nameof(Value), $"Invalid value for type: {typeof(T)}.");
                if (_value != null) throw new InvalidOperationException($"The value has already been set for type: {typeof(T)}.");
                _value = value;
            }
        }
    }
}
