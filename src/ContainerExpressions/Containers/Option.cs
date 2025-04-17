using ContainerExpressions.Containers.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace ContainerExpressions.Containers
{
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
            if (TryParse<TOption>(value, out var card)) return card;
            Throw.ArgumentOutOfRangeException(nameof(value), $"Invalid value for {typeof(TOption).Name} Option: {value}.");
            return null;
        }

        protected static bool TryParse<TOption>(string value, out TOption option) where TOption : Option
        {
            var options = OptionHelper<TOption>.GetOptions();

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

        protected static IEnumerable<string> GetValues<TOption>() where TOption : Option => OptionHelper<TOption>.GetOptions().Select(x => x.Value);

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

    file static class OptionHelper<TOption> where TOption : Option
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
}
