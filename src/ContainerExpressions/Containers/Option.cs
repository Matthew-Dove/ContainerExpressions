﻿using ContainerExpressions.Containers.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace ContainerExpressions.Containers
{
    #region String Option

    /// <summary>
    /// A strongly typed collection of strings that represents choices.
    /// <para>Simular to using names from an enum.</para>
    /// </summary>
    public abstract class Option : IEquatable<string>, IEquatable<Option>
    {
        /// <summary>The option's value.</summary>
        public string Value { get; }

        protected Option() { Value = string.Empty; }

        /// <summary>Set an option value, cannot be null.</summary>
        protected Option(string value) { Value = value.ThrowIfNull(); }

        public static implicit operator string(Option option) => option.Value;

        public override string ToString() => Value;

        public override int GetHashCode() => Value.GetHashCode();

        public bool Equals(Option other) => other is not null && other.Equals(Value);

        public bool Equals(string value) => Value.Equals(value, StringComparison.OrdinalIgnoreCase);

        public override bool Equals(object obj) => obj != null && ((obj is Option option && Equals(option)) || (obj is string @string && Equals(@string)));

        protected static TOption Parse<TOption>(string value) where TOption : Option
        {
            if (TryParse<TOption>(value, out var option)) return option;
            Throw.ArgumentOutOfRangeException(nameof(value), $"Invalid value for {typeof(TOption).Name} Option: {value}.");
            return null;
        }

        protected static bool TryParse<TOption>(string value, out TOption option) where TOption : Option
        {
            var options = StringOptionHelper<TOption>.GetOptions();

            foreach (var opt in options)
            {
                if (opt.Value.Equals(value, StringComparison.OrdinalIgnoreCase))
                {
                    option = opt;
                    return true;
                }
            }

            option = null;
            return false;
        }

        protected static IEnumerable<string> GetValues<TOption>() where TOption : Option => StringOptionHelper<TOption>.GetOptions().Select(x => x.Value);

        public static bool operator !=(Option x, Option y) => !(x == y);
        public static bool operator ==(Option x, Option y)
        {
            if ((object)x == null) return (object)y == null;
            if ((object)y == null) return (object)x == null;
            return x.Equals(y);
        }

        public static bool operator !=(Option x, string y) => !(x == y);
        public static bool operator ==(Option x, string y) => (object)x != null && x.Equals(y);

        public static bool operator !=(string x, Option y) => !(x == y);
        public static bool operator ==(string x, Option y) => (object)y != null && y.Equals(x);
    }

    file static class StringOptionHelper<TOption> where TOption : Option
    {
        private static TOption[] _options = null;

        public static TOption[] GetOptions()
        {
            if (_options == null) _options = BuildOptions();
            return _options;
        }

        private static TOption[] BuildOptions()
        {
            var optionType = typeof(TOption);
            var instances = new List<TOption>();

            var optionFields = typeof(TOption)
                .GetFields(BindingFlags.Public | BindingFlags.Static)
                .Where(f => f.FieldType == typeof(TOption))
                .Select(f => (TOption)f.GetValue(null))
                .Where(f => f != null);

            var optionProperties = typeof(TOption)
                .GetProperties(BindingFlags.Public | BindingFlags.Static)
                .Where(p => p.PropertyType == typeof(TOption))
                .Select(p => (TOption)p.GetValue(null))
                .Where(p => p != null);

            var ctor = optionType.GetConstructor(BindingFlags.NonPublic | BindingFlags.Instance, null, [typeof(string)], null);

            if (ctor != null)
            {
                foreach (var option in optionFields.Concat(optionProperties))
                {
                    instances.Add(option);
                }
            }

            return instances.ToArray();
        }
    }

    #endregion

    #region Generic Option

    /// <summary>
    /// A strongly typed collection of T that represents choices.
    /// <para>Simular to using values from an enum.</para>
    /// </summary>
    public abstract class Option<TValue> : IEquatable<TValue>, IEquatable<Option<TValue>>
    {
        /// <summary>The option's value.</summary>
        public TValue Value { get; }

        protected Option() { Value = default; }

        /// <summary>Set an option value, cannot be null.</summary>
        protected Option(TValue value) { Value = value is null ? (TValue)((object)value).ThrowIfNull() : value; }

        public static implicit operator TValue(Option<TValue> option) => option.Value;

        public override string ToString() => Value?.ToString() ?? string.Empty;

        public override int GetHashCode() => Value is null ? 0 : EqualityComparer<TValue>.Default.GetHashCode(Value);

        public bool Equals(Option<TValue> other) => other is not null && other.Equals(Value);

        public bool Equals(TValue value) => EqualityComparer<TValue>.Default.Equals(Value, value);

        public override bool Equals(object obj) => obj is not null && ((obj is Option<TValue> option && Equals(option)) || (obj is TValue value && Equals(value)));

        protected static TOption Parse<TOption>(TValue value) where TOption : Option<TValue>
        {
            if (TryParse<TOption>(value, out var option)) return option;
            Throw.ArgumentOutOfRangeException(nameof(value), $"Invalid value for {typeof(TOption).Name} Option: {value}.");
            return null;
        }

        protected static bool TryParse<TOption>(TValue value, out TOption option) where TOption : Option<TValue>
        {
            var options = GenericOptionHelper<TOption, TValue>.GetOptions();

            foreach (var opt in options)
            {
                if (EqualityComparer<TValue>.Default.Equals(opt.Value, value))
                {
                    option = opt;
                    return true;
                }
            }

            option = null!;
            return false;
        }

        protected static IEnumerable<TValue> GetValues<TOption>() where TOption : Option<TValue> => GenericOptionHelper<TOption, TValue>.GetOptions().Select(x => x.Value);

        public static bool operator !=(Option<TValue> x, Option<TValue> y) => !(x == y);
        public static bool operator ==(Option<TValue> x, Option<TValue> y)
        {
            if (x is null) return y is null;
            if (y is null) return x is null;
            return x.Equals(y);
        }

        public static bool operator !=(Option<TValue> x, TValue y) => !(x == y);
        public static bool operator ==(Option<TValue> x, TValue y) => x is not null && x.Equals(y);

        public static bool operator !=(TValue x, Option<TValue> y) => !(x == y);
        public static bool operator ==(TValue x, Option<TValue> y) => y is not null && y.Equals(x);
    }

    file static class GenericOptionHelper<TOption, TValue> where TOption : Option<TValue>
    {
        private static TOption[] _options;

        public static TOption[] GetOptions()
        {
            if (_options == null) _options = BuildOptions();
            return _options;
        }

        private static TOption[] BuildOptions()
        {
            var optionType = typeof(TOption);
            var instances = new List<TOption>();

            var optionFields = optionType
                .GetFields(BindingFlags.Public | BindingFlags.Static)
                .Where(f => f.FieldType == typeof(TOption))
                .Select(f => (TOption)f.GetValue(null))
                .Where(f => f != null);

            var optionProperties = optionType
                .GetProperties(BindingFlags.Public | BindingFlags.Static)
                .Where(p => p.PropertyType == typeof(TOption))
                .Select(p => (TOption)p.GetValue(null))
                .Where(p => p != null);

            var ctor = optionType.GetConstructor(BindingFlags.NonPublic | BindingFlags.Instance, null, [typeof(TValue)], null);

            if (ctor != null)
            {
                foreach (var option in optionFields.Concat(optionProperties))
                {
                    instances.Add(option);
                }
            }

            return instances.ToArray();
        }
    }

    #endregion
}
