using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Reflection;

namespace ContainerExpressions.Containers
{
    public static class SmartEnum<T> where T : SmartEnum
    {
        private static readonly HybridDictionary _smartEnumNames;
        private static readonly HybridDictionary _smartEnumValues;
        private static readonly HybridDictionary _smartEnumAliases;

        static SmartEnum()
        {
            try
            {
                var type = typeof(T);
                var staticFields = type.GetFields(BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy);

                var i = 0;
                var smartEnums = new List<T>();
                foreach (var field in staticFields)
                {
                    if (!type.IsAssignableFrom(field.FieldType)) continue;
                    var se = (T)field.GetValue(null);
                    if (se is null) new ArgumentNullException(field.Name, "SmartEnum fields must be initialised.").ThrowError("The field {field} from type {type} cannot be null.".WithArgs(field.Name, type.Name));

                    if (string.IsNullOrEmpty(se.Name)) se.Name = field.Name;
                    if (se.Value <= 0) se.Value = i;
                    if (se.Aliases is null) se.Aliases = Array.Empty<string>();

                    smartEnums.Add(se);
                    i = se.Value + 1;
                }

                _smartEnumNames = new(smartEnums.Count);
                _smartEnumValues = new(smartEnums.Count);
                _smartEnumAliases = null;

                foreach (var smartEnum in smartEnums)
                {
                    _smartEnumNames.Add(smartEnum.Name.ToLowerInvariant(), smartEnum);
                    _smartEnumValues.Add(smartEnum.Value, smartEnum);

                    foreach (var alias in smartEnum.Aliases)
                    {
                        _smartEnumAliases ??= new(smartEnums.Count);
                        _smartEnumNames.Add(alias.ToLowerInvariant(), smartEnum);
                        _smartEnumAliases.Add(alias.ToLowerInvariant(), alias);
                    }
                }

                _smartEnumAliases ??= Cache.HybridDictionary;
            }
            catch (Exception ex)
            {
                ex.LogErrorPlain("Error creating a SmartEnum container for the type \"{SmartEnumType}\": {exception}.".WithArgs(typeof(T).FullName, ex.Message));
                _smartEnumNames = new();
                _smartEnumValues = new();
                _smartEnumAliases = new();
            }
        }

        public static Dictionary<string, (int Value, string[] Aliases)> Init() => Init(FormatOptions.Lowercase);
        public static Dictionary<string, (int Value, string[] Aliases)> Init(FormatOptions format)
        {
            var objs = GetObjects();
            var init = new Dictionary<string, (int, string[])>(objs.Length);

            foreach (var obj in objs)
            {
                var name = string.Empty;
                var aliases = Array.Empty<string>();

                if (format == FormatOptions.Lowercase) { name = obj.Name.ToLowerInvariant(); aliases = obj.Aliases.Select(x => x.ToLowerInvariant()).ToArray(); }
                else if (format == FormatOptions.Uppercase) { name = obj.Name.ToUpperInvariant(); aliases = obj.Aliases.Select(x => x.ToUpperInvariant()).ToArray(); }
                else if (format == FormatOptions.Original) { name = obj.Name; aliases = obj.Aliases; }

                init.Add(name, (obj.Value, aliases));
            }

            return init;
        }

        public static T[] GetObjects()
        {
            var objs = new T[_smartEnumValues.Count];
            _smartEnumValues.Values.CopyTo(objs, 0);
            return objs;
        }

        public static Response<EnumRange<T>> FromObject(T smartEnum) => Response.Create(new EnumRange<T>(new T[] { smartEnum }));
        public static Response<EnumRange<T>> FromObjects(params T[] smartEnums) => Response.Create(new EnumRange<T>(smartEnums));

        public static string[] GetNames() => GetNamesWith(FormatOptions.Lowercase);
        public static string[] GetNames(FormatOptions format) => GetNamesWith(format);
        private static string[] GetNamesWith(FormatOptions format)
        {
            var names = new string[_smartEnumNames.Count - _smartEnumAliases.Count];
            int i = 0;

            foreach (string name in _smartEnumNames.Keys)
            {
                if (!_smartEnumAliases.Contains(name))
                {
                    if (format == FormatOptions.Lowercase) names[i++] = name;
                    else if (format == FormatOptions.Uppercase) names[i++] = name.ToUpperInvariant();
                    else if (format == FormatOptions.Original) names[i++] = ((T)_smartEnumNames[name]).Name;
                }
            }

            return names;
        }

        public static Response<EnumRange<T>> FromName(string name)
        {
            if (name is not null)
            {
                var smartEnum = (T)_smartEnumNames[name.ToLowerInvariant()];
                if (smartEnum is not null) return Response.Create(new EnumRange<T>(new T[] { smartEnum }));
            }
            return Response<EnumRange<T>>.Error;
        }

        public static Response<EnumRange<T>> FromNames(string names) => FromNames(names, Cache.CharSeparator);
        public static Response<EnumRange<T>> FromNames(string names, char separator) => FromNames(names, new char[] { separator.ThrowIfDefault() });
        private static Response<EnumRange<T>> FromNames(string names, char[] separator)
        {
            var splitNames = names?.ToLowerInvariant().Split(separator, StringSplitOptions.RemoveEmptyEntries) ?? Array.Empty<string>();
            var smartEnums = new List<T>(splitNames.Length);

            foreach (var name in splitNames)
            {
                var smartEnum = (T)_smartEnumNames[name.Trim()];
                if (smartEnum is not null) smartEnums.Add(smartEnum);
            }

            return smartEnums.Count > 0 ? Response.Create(new EnumRange<T>(smartEnums)) : Response<EnumRange<T>>.Error;
        }

        public static string[] GetAliases() => GetAliasesWith(FormatOptions.Lowercase);
        public static string[] GetAliases(FormatOptions format) => GetAliasesWith(format);
        private static string[] GetAliasesWith(FormatOptions format)
        {
            var aliases = new string[_smartEnumAliases.Count];
            int i = 0;

            foreach (string alias in _smartEnumAliases.Keys)
            {
                if (format == FormatOptions.Lowercase) aliases[i++] = alias;
                else if (format == FormatOptions.Uppercase) aliases[i++] = alias.ToUpperInvariant();
                else if (format == FormatOptions.Original) aliases[i++] = (string)_smartEnumAliases[alias];
            }

            return aliases;
        }

        public static int[] GetValues()
        {
            var values = new int[_smartEnumValues.Count];
            _smartEnumValues.Keys.CopyTo(values, 0);
            return values;
        }

        public static Response<EnumRange<T>> FromValue(int value)
        {
            var smartEnum = (T)_smartEnumValues[value];
            if (smartEnum is not null) return Response.Create(new EnumRange<T>(new T[] { smartEnum }));
            return Response<EnumRange<T>>.Error;
        }

        public static Response<EnumRange<T>> FromValues(int values)
        {
            if (values == 0)
            {
                var smartEnum = (T)_smartEnumValues[0];
                if (smartEnum is not null) return Response.Create(new EnumRange<T>(new T[] { smartEnum }));
                return Response<EnumRange<T>>.Error;
            };

            var smartEnums = new List<T>();
            for (int i = 0, pow = 1; i < 32 && pow <= values; i++, pow = 1 << i)
            {
                if ((values & pow) != 0)
                {
                    var smartEnum = (T)_smartEnumValues[pow];
                    if (smartEnum is not null) smartEnums.Add(smartEnum);
                }
            }
            return smartEnums.Count > 0 ? Response.Create(new EnumRange<T>(smartEnums)) : Response<EnumRange<T>>.Error;
        }

        public static Response<EnumRange<T>> Parse(Either<string, int, object> input) => Parse(input, Cache.CharSeparator);
        public static Response<EnumRange<T>> Parse(Either<string, int, object> input, char separator) => Parse(input, new char[] { separator.ThrowIfDefault() });
        private static Response<EnumRange<T>> Parse(Either<string, int, object> input, char[] separator)
        {
            Response<EnumRange<T>> ParseName(string name, char[] sep)
            {
                var smartEnum = (T)_smartEnumNames[name.ToLowerInvariant()];
                if (smartEnum is not null) return Response.Create(new EnumRange<T>(new T[] { smartEnum }));

                var names = FromNames(name, sep);
                if (names) return names;

                if (int.TryParse(name, out var i)) return ParseValue(i);

                return Response<EnumRange<T>>.Error;
            }

            Response<EnumRange<T>> ParseValue(int value)
            {
                var smartEnum = (T)_smartEnumValues[value];
                if (smartEnum is not null) return Response.Create(new EnumRange<T>(new T[] { smartEnum }));

                var values = FromValues(value);
                if (values) return values;

                return Response<EnumRange<T>>.Error;
            }

            Response<EnumRange<T>> ParseObject(object obj, char[] sep)
            {
                if (obj is string s) return ParseName(s, sep);

                if (obj is int i) return ParseValue(i);

                if (obj is T t && t is not null) return Response.Create(new EnumRange<T>(new[] { t }));

                if (obj is ICollection<T> tc && tc is not null) return Response.Create(new EnumRange<T>(tc));

                if (obj is EnumRange<T> tr && tr.Objects is not null) return Response.Create(tr);

                return Response<EnumRange<T>>.Error;
            }

            return input.Match(
                name => ParseName(name, separator),
                value => ParseValue(value),
                obj => ParseObject(obj, separator)
            );
        }

        public static bool TryParse(Either<string, int, object> input, out EnumRange<T> enumRange) => TryParse(input, Cache.CharSeparator, out enumRange);
        public static bool TryParse(Either<string, int, object> input, char separator, out EnumRange<T> enumRange) => TryParse(input, new char[] { separator.ThrowIfDefault() }, out enumRange);
        private static bool TryParse(Either<string, int, object> input, char[] separator, out EnumRange<T> enumRange)
        {
            enumRange = new EnumRange<T>(Array.Empty<T>());

            var parse = Parse(input, separator);
            if (parse) enumRange = parse;

            return parse;
        }
    }

    public abstract class SmartEnum : IEquatable<SmartEnum>
    {
        public string Name { get; internal set; }
        public int Value { get; internal set; }
        public string[] Aliases { get; internal set; }

        protected SmartEnum(string name) : this(name, -1, null) { }
        protected SmartEnum(string name, int value) : this(name, value, null) { }
        protected SmartEnum(string name, string[] aliases) : this(name, -1, aliases) { }

        protected SmartEnum(int value) : this(null, value, null) { }
        protected SmartEnum(int value, string[] aliases) : this(null, value, aliases) { }

        protected SmartEnum(string[] aliases) : this(null, -1, aliases) { }

        protected SmartEnum(string name, int value, string[] aliases)
        {
            Name = name ?? string.Empty;
            Value = value >= 0 ? value : -1;
            Aliases = (aliases ?? Array.Empty<string>()).ThrowIfSequenceIsNullOrEmpty("Alias must have a name.");
        }

        protected SmartEnum() { }

        public override string ToString() => Name;
        public override bool Equals(object obj) => obj is SmartEnum r && Equals(r);
        public bool Equals(SmartEnum smartEnum) => smartEnum is not null && smartEnum.Value.Equals(Value);
        public override int GetHashCode() => Name.GetHashCode();

        public static bool operator !=(SmartEnum x, SmartEnum y) => !(x == y);
        public static bool operator ==(SmartEnum x, SmartEnum y)
        {
            if ((object)x == null) return (object)y == null;
            if ((object)y == null) return (object)x == null;
            return x.Equals(y);
        }

        public static implicit operator string(SmartEnum smartEnum) => smartEnum.Name;
        public static implicit operator int(SmartEnum smartEnum) => smartEnum.Value;
    }

    file abstract class Cache
    {
        internal static readonly HybridDictionary HybridDictionary = new(0);
        internal static readonly string StringSeparator = ",";
        internal static readonly char[] CharSeparator = new char[] { ',' };
    }

    public enum FormatOptions { Lowercase, Uppercase, Original }

    public readonly struct EnumRange<T> where T : SmartEnum
    {
        public T[] Objects { get; }

        public EnumRange() : this(Array.Empty<T>()) { }
        internal EnumRange(T[] objects)
        {
            if (objects.Length < 2) Objects = objects;
            else Objects = objects.Distinct().OrderBy(x => x.Value).ToArray();
        }
        internal EnumRange(IEnumerable<T> objects)
        {
            Objects = objects.Distinct().OrderBy(x => x.Value).ToArray();
        }

        public override string ToString() => ToStringWith(FormatOptions.Lowercase, Cache.StringSeparator);
        public string ToString(FormatOptions format) => ToStringWith(format, Cache.StringSeparator);
        public string ToString(char separator) => ToStringWith(FormatOptions.Lowercase, char.ToString(separator));
        public string ToString(FormatOptions format, char separator) => ToStringWith(format, char.ToString(separator));
        private string ToStringWith(FormatOptions format, string separator)
        {
            var names = new string[Objects.Length];
            var i = 0;

            foreach (var obj in Objects)
            {
                if (format == FormatOptions.Lowercase) names[i++] = obj.Name.ToLowerInvariant();
                else if (format == FormatOptions.Uppercase) names[i++] = obj.Name.ToUpperInvariant();
                else if (format == FormatOptions.Original) names[i++] = obj.Name;
            }

            return string.Join(separator, names);
        }

        public override int GetHashCode()
        {
            if (Objects.Length == 0) return 0;
            if (Objects.Length == 1) return Objects[0].GetHashCode();

            unchecked
            {
                var hash = Objects[0].GetHashCode();
                foreach (var obj in Objects.Skip(1))
                {
                    hash = (hash * 397) ^ obj.GetHashCode();
                }
                return hash;
            }
        }

        public override bool Equals(object obj) => obj is EnumRange<T> er && Equals(er) || obj is T se && Equals(se);
        public bool Equals(T smartEnum) => Objects.Length == 1 && Objects[0].Equals(smartEnum);
        public bool Equals(EnumRange<T> range)
        {
            if (Objects.Length == 0 && range.Objects.Length == 0) return true;
            if (Objects.Length == 1 && range.Objects.Length == 1) return Objects[0].Equals(range.Objects[0]);
            return Objects.Length == range.Objects.Length && Objects.SequenceEqual(range.Objects);
        }

        public static bool operator !=(EnumRange<T> x, EnumRange<T> y) => !(x == y);
        public static bool operator ==(EnumRange<T> x, EnumRange<T> y) => x.Equals(y);
        public static bool operator !=(EnumRange<T> x, T y) => !(x == y);
        public static bool operator ==(EnumRange<T> x, T y) => x.Equals(y);
        public static bool operator !=(T x, EnumRange<T> y) => !(x == y);
        public static bool operator ==(T x, EnumRange<T> y) => y.Equals(x);

        public static implicit operator string(EnumRange<T> range) => range.ToString();
        public static implicit operator int(EnumRange<T> range) => range.Objects.Sum(x => x.Value);

        // Bitwise OR: add target if it doesn't exist.
        public static EnumRange<T> operator |(EnumRange<T> x, EnumRange<T> y)
        {
            if (x.Equals(y)) return y;

            var addFlags = SmartEnum<T>.FromValues((int)x | y);
            if (addFlags) return addFlags;

            var none = SmartEnum<T>.FromValue(0);
            if (none) return none;

            return new EnumRange<T>();
        }
        public static EnumRange<T> operator |(EnumRange<T> x, T y)
        {
            var hasFlag = false;

            for (int i = 0; i < x.Objects.Length; i++)
            {
                if (x.Objects[i].Equals(y)) { hasFlag = true; break; }
            }

            if (!hasFlag)
            {
                var objs = new T[x.Objects.Length + 1];
                Array.Copy(x.Objects, objs, x.Objects.Length);
                objs[objs.Length - 1] = y;
                return new EnumRange<T>(objs);
            }

            return x;
        }

        // Bitwise AND: check if the target exists.
        public static EnumRange<T> operator &(EnumRange<T> x, EnumRange<T> y)
        {
            if (x.Equals(y)) return y;

            var hasFlags = SmartEnum<T>.FromValues((int)x & y);
            if (hasFlags) return hasFlags;

            var none = SmartEnum<T>.FromValue(0);
            if (none) return none;

            return new EnumRange<T>();
        }
        public static T operator &(EnumRange<T> x, T y)
        {
            for (int i = 0; i < x.Objects.Length; i++)
            {
                if (x.Objects[i].Equals(y)) return y;
            }

            var none = SmartEnum<T>.FromValue(0);
            if (none) return none.Value.Objects[0];

            return null;
        }

        /**
         * Bitwise NOT: remove target if it exists.
         * Note: Bitwise Complement (~) isn't overloaded here, as it's a unary operator.
         * EnumRange is readonly, and does not store "negative" state (i.e. what has been removed / negated).
         * Which would be required in order to interact with future binary operators such as: "Bitwise AND" (i.e. to remove the target if it exists).
        **/

        // Bitwise XOR: remove the target if it exists, otherwise add it.
        public static EnumRange<T> operator ^(EnumRange<T> x, EnumRange<T> y) => x.HasFlags(y) ? x.RemoveFlags(y) : x.AddFlags(y);
        public static EnumRange<T> operator ^(EnumRange<T> x, T y) => x.HasFlag(y) ? x.RemoveFlag(y) : x.AddFlag(y);
    }

    public static class EnumRangeExtensions
    {
        public static EnumRange<T> AddFlag<T>(this EnumRange<T> range, T flag) where T : SmartEnum => range | flag;
        public static EnumRange<T> AddFlags<T>(this EnumRange<T> range, params T[] flags) where T : SmartEnum => AddFlags(range, new EnumRange<T>(flags));
        public static EnumRange<T> AddFlags<T>(this EnumRange<T> range, EnumRange<T> flags) where T : SmartEnum => range | flags;

        public static bool HasFlag<T>(this EnumRange<T> range, T flag) where T : SmartEnum => flag.Equals(range & flag);
        public static bool HasFlags<T>(this EnumRange<T> range, params T[] flags) where T : SmartEnum => HasFlags(range, new EnumRange<T>(flags));
        public static bool HasFlags<T>(this EnumRange<T> range, EnumRange<T> flags) where T : SmartEnum => flags.Equals(range & flags);

        public static EnumRange<T> RemoveFlag<T>(this EnumRange<T> range, T flag) where T : SmartEnum
        {
            var removeFlag = SmartEnum<T>.FromValues(range & (~flag));
            if (removeFlag) return removeFlag;

            var none = SmartEnum<T>.FromValue(0);
            if (none) return none;

            return new EnumRange<T>();
        }
        public static EnumRange<T> RemoveFlags<T>(this EnumRange<T> range, params T[] flags) where T : SmartEnum => RemoveFlags(range, new EnumRange<T>(flags));
        public static EnumRange<T> RemoveFlags<T>(this EnumRange<T> range, EnumRange<T> flags) where T : SmartEnum
        {
            var removeFlags = SmartEnum<T>.FromValues(range & (~flags));
            if (removeFlags) return removeFlags;

            var none = SmartEnum<T>.FromValue(0);
            if (none) return none;

            return new EnumRange<T>();
        }

        public static EnumRange<T> ToggleFlag<T>(this EnumRange<T> range, T flag) where T : SmartEnum => range ^ flag;
        public static EnumRange<T> ToggleFlags<T>(this EnumRange<T> range, params T[] flags) where T : SmartEnum => ToggleFlags(range, new EnumRange<T>(flags));
        public static EnumRange<T> ToggleFlags<T>(this EnumRange<T> range, EnumRange<T> flags) where T : SmartEnum => range ^ flags;
    }
}
