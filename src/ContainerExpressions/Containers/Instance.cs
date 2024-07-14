using ContainerExpressions.Containers.Internal;
using System;
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
        /// <summary>Gets a default instance for TValue. Create must be called first for some reference type TValue.</summary>
        public static TValue Of<TValue>() => Instance<TValue>.Value;

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
             * - Int16
             * - Int64
             * - Int128
             * - IntPtr
             * - UInt16
             * - UInt32
             * - UInt64
             * - UInt128
             * - UIntPtr
             * - DateOnly
             * - DateTime
             * - DateTimeOffset
             * - Guid
             * - BigInteger
             * - Complex
             * - SByte
             * - TimeOnly
             * - TimeSpan
             * - Enum
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
    /// <para>An instance of T may only be set once.</para>
    /// </summary>
    file static class Instance<T>
    {
        private static readonly string _setValueBeforeRetrieving = $"Must set a value for type: {typeof(T)}, before attempting to retrieve it.";
        private static readonly string _cannotSetNullValue = $"Invalid value for type: {typeof(T)}.";
        private static readonly string _onlySetValueOnce = $"The value has already been set for type: {typeof(T)}.";

        private static readonly bool _isValue = typeof(T).IsValueType;
        private static T _value = default;

        public static T Value
        {
            get
            {
                if (_isValue) return _value; // Value types cannot be set due to the generic constraints placed on Instance.Create(), so we can return them without checking (as they are not null).
                if (_value is null) Throw.InvalidOperationException(_setValueBeforeRetrieving);
                return _value;
            }
            set
            {
                if (value is null) Throw.ArgumentNullException(nameof(Value), _cannotSetNullValue);
                if (_value is not null) Throw.InvalidOperationException(_onlySetValueOnce); // Value types cannot be set, so only need to check null.
                _value = value;
            }
        }
    }

    /// <summary>Holds completed tasks for requested types.</summary>
    public static class InstanceAsync {
        public static Task<TResult> Of<TResult>() => InstanceAsync<TResult>.Result;
        public static ValueTask<TResult> ValueOf<TResult>() => ValueInstanceAsync<TResult>.Result;
        public static ResponseAsync<TResult> ResponseOf<TResult>() => InstanceResponseAsync<TResult>.Result;

        public static void Create<TResult>(TResult result) where TResult : class
        {
            if (result is null) Throw.ArgumentNullException(InstanceAsync<TResult>.CannotSetNullValue, nameof(result));
            if (InstanceAsync<TResult>._result is not null) Throw.InvalidOperationException(InstanceAsync<TResult>.OnlySetValueOnce);
            InstanceAsync<TResult>._result = Task.FromResult(result);
        }

        public static void CreateValue<TResult>(TResult result) where TResult : class
        {
            if (result is null) Throw.ArgumentNullException(ValueInstanceAsync<TResult>.CannotSetNullValue, nameof(result));
            if (!EqualityComparer<TResult>.Default.Equals(ValueInstanceAsync<TResult>._result, default)) Throw.InvalidOperationException(ValueInstanceAsync<TResult>.OnlySetValueOnce);
            ValueInstanceAsync<TResult>._result = result;
        }

        public static void CreateResponse<TResult>(TResult result) where TResult : class
        {
            if (result is null) Throw.ArgumentNullException(InstanceResponseAsync<TResult>.CannotSetNullValue, nameof(result));
            if (InstanceResponseAsync<TResult>._result is not null) Throw.InvalidOperationException(InstanceResponseAsync<TResult>.OnlySetValueOnce);
            InstanceResponseAsync<TResult>._result = new State<TResult>(result);
        }
    }

    file static class InstanceAsync<T>
    {
        internal static readonly string CannotSetNullValue = $"Invalid value for type: {typeof(T)}.";
        internal static readonly string OnlySetValueOnce = $"The value has already been set for type: {typeof(T)}.";

        private static readonly Task<T> _instance = Task.FromResult<T>(default);
        internal static Task<T> _result = default;
        public static Task<T> Result { get { return _result ?? _instance; } }
    }

    file static class ValueInstanceAsync<T>
    {
        internal static readonly string CannotSetNullValue = $"Invalid value for type: {typeof(T)}.";
        internal static readonly string OnlySetValueOnce = $"The value has already been set for type: {typeof(T)}.";

        internal static T _result = default;
        public static ValueTask<T> Result { get { return new ValueTask<T>(_result); } }
    }

    file static class InstanceResponseAsync<T>
    {
        internal static readonly string CannotSetNullValue = $"Invalid value for type: {typeof(T)}.";
        internal static readonly string OnlySetValueOnce = $"The value has already been set for type: {typeof(T)}.";

        private static readonly State<T> _instance = new(default(T));
        internal static State<T> _result = default;
        public static ResponseAsync<T> Result { get { return new ResponseAsync<T>(_result ?? _instance); } }
    }
}
