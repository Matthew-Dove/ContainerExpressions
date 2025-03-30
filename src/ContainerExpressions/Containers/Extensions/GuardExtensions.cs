using ContainerExpressions.Containers.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.ExceptionServices;
using System.Text;
using System.Threading.Tasks;

namespace ContainerExpressions.Containers
{
    public static class GuardExtensions
    {
        #region GetMessage

        private static string GetMessage(string message, string argument, string predicate, string caller, string path, int line, Format template)
        {
            var sb = new StringBuilder()
                .Append("Message: ").AppendLine(message)
                .Append("CallerArgumentExpression: ").AppendLine(argument)
                .Append("CallerPredicateExpression: ").AppendLine(predicate)
                .Append("CallerMemberName: ").AppendLine(caller)
                .Append("CallerFilePath: ").AppendLine(path)
                .Append("CallerLineNumber: ").Append(line);

            if (!Format.Default.Equals(template))
            {
                sb.AppendLine().Append("Template: ").Append(template.ToString());
            }

            return sb.ToString();
        }

        private static string GetMessage(string message, string argument, string caller, string path, int line, Format template)
        {
            var sb = new StringBuilder()
                .Append("Message: ").AppendLine(message)
                .Append("CallerArgumentExpression: ").AppendLine(argument)
                .Append("CallerMemberName: ").AppendLine(caller)
                .Append("CallerFilePath: ").AppendLine(path)
                .Append("CallerLineNumber: ").Append(line);

            if (!Format.Default.Equals(template))
            {
                sb.AppendLine().Append("Template: ").Append(template.ToString());
            }

            return sb.ToString();
        }

        #endregion

        #region ThrowError

        public static void ThrowError(
            this Exception ex,
            Format message = default,
            [CallerArgumentExpression(nameof(ex))] string argument = "",
            [CallerMemberName] string caller = "",
            [CallerFilePath] string path = "",
            [CallerLineNumber] int line = 0
            )
        { Throw.Exception(ex.AddCallerAttributes(argument, caller, path, line, message)); }

        public static T ThrowError<T>(
            this T ex,
            Format message = default,
            [CallerArgumentExpression(nameof(ex))] string argument = "",
            [CallerMemberName] string caller = "",
            [CallerFilePath] string path = "",
            [CallerLineNumber] int line = 0
            ) where T : Exception { Throw.Exception(ex.AddCallerAttributes(argument, caller, path, line, message)); return default; }

        public static void ThrowDispatchError(
            this ExceptionDispatchInfo ex,
            Format message = default,
            [CallerArgumentExpression(nameof(ex))] string argument = "",
            [CallerMemberName] string caller = "",
            [CallerFilePath] string path = "",
            [CallerLineNumber] int line = 0
            ) => Throw.Exception(ex.SourceException.AddCallerAttributes(argument, caller, path, line, message));

        public static T ThrowDispatchError<T>(
            this ExceptionDispatchInfo ex,
            Format message = default,
            [CallerArgumentExpression(nameof(ex))] string argument = "",
            [CallerMemberName] string caller = "",
            [CallerFilePath] string path = "",
            [CallerLineNumber] int line = 0
            ) { Throw.Exception(ex.SourceException.AddCallerAttributes(argument, caller, path, line, message)); return default; }

        #endregion

        #region ThrowIfNull

        public static T ThrowIfNull<T>(
            this T target,
            Format message = default,
            [CallerArgumentExpression(nameof(target))] string argument = "",
            [CallerMemberName] string caller = "",
            [CallerFilePath] string path = "",
            [CallerLineNumber] int line = 0
            ) where T : class
        {
            if (target is null) Throw.ArgumentNullException(argument, GetMessage($"{argument} cannot be null.", argument, caller, path, line, message));
            return target;
        }

        public static T ThrowIfNull<T, R>(
            this T target,
            Func<T, R> guard,
            Format message = default,
            [CallerArgumentExpression(nameof(target))] string argument1 = "",
            [CallerArgumentExpression(nameof(guard))] string argument2 = "",
            [CallerMemberName] string caller = "",
            [CallerFilePath] string path = "",
            [CallerLineNumber] int line = 0
            )
            where T : class
            where R : class
        {
            if (target is null) Throw.ArgumentNullException(argument1, GetMessage($"{argument1} cannot be null.", argument1, caller, path, line, message));
            if (guard(target) is null) Throw.ArgumentNullException(argument2, GetMessage($"{argument2} cannot be null.", argument2, caller, path, line, message));
            return target;
        }

        public static T[] ThrowIfSequenceIsNull<T>(
            this T[] target,
            Format message = default,
            [CallerArgumentExpression(nameof(target))] string argument = "",
            [CallerMemberName] string caller = "",
            [CallerFilePath] string path = "",
            [CallerLineNumber] int line = 0
            ) where T : class
        {
            if (target is null) Throw.ArgumentNullException(argument, GetMessage($"{argument} cannot be null.", argument, caller, path, line, message));
            foreach (var t in target)
            {
                if (t is null) Throw.ArgumentNullException(argument, GetMessage($"Element in collection {argument} cannot be null.", argument, caller, path, line, message));
            }
            return target;
        }

        public static T[] ThrowIfSequenceIsNull<T, R>(
            this T[] target,
            Func<T, R> guard,
            Format message = default,
            [CallerArgumentExpression(nameof(target))] string argument1 = "",
            [CallerArgumentExpression(nameof(guard))] string argument2 = "",
            [CallerMemberName] string caller = "",
            [CallerFilePath] string path = "",
            [CallerLineNumber] int line = 0
            )
            where T : class
            where R : class
        {
            if (target is null) Throw.ArgumentNullException(argument1, GetMessage($"{argument1} cannot be null.", argument1, caller, path, line, message));
            foreach (var t in target)
            {
                if (t is null) Throw.ArgumentNullException(argument1, GetMessage($"Element in collection {argument1} cannot be null.", argument1, caller, path, line, message));
                if (guard(t) is null) Throw.ArgumentNullException(argument2, GetMessage($"Element in collection {argument2} cannot be null.", argument2, caller, path, line, message));
            }
            return target;
        }

        public static IEnumerable<T> ThrowIfSequenceIsNull<T>(
            this IEnumerable<T> target,
            Format message = default,
            [CallerArgumentExpression(nameof(target))] string argument = "",
            [CallerMemberName] string caller = "",
            [CallerFilePath] string path = "",
            [CallerLineNumber] int line = 0
            ) where T : class
        {
            if (target is null) Throw.ArgumentNullException(argument, GetMessage($"{argument} cannot be null.", argument, caller, path, line, message));
            foreach (var t in target)
            {
                if (t is null) Throw.ArgumentNullException(argument, GetMessage($"Element in collection {argument} cannot be null.", argument, caller, path, line, message));
            }
            return target;
        }

        public static IEnumerable<T> ThrowIfSequenceIsNull<T, R>(
            this IEnumerable<T> target,
            Func<T, R> guard,
            Format message = default,
            [CallerArgumentExpression(nameof(target))] string argument1 = "",
            [CallerArgumentExpression(nameof(guard))] string argument2 = "",
            [CallerMemberName] string caller = "",
            [CallerFilePath] string path = "",
            [CallerLineNumber] int line = 0
            )
            where T : class
            where R : class
        {
            if (target is null) Throw.ArgumentNullException(argument1, GetMessage($"{argument1} cannot be null.", argument1, caller, path, line, message));
            foreach (var t in target)
            {
                if (t is null) Throw.ArgumentNullException(argument1, GetMessage($"Element in collection {argument1} cannot be null.", argument1, caller, path, line, message));
                if (guard(t) is null) Throw.ArgumentNullException(argument2, GetMessage($"Element in collection {argument2} cannot be null.", argument2, caller, path, line, message));
            }
            return target;
        }

        public static List<T> ThrowIfSequenceIsNull<T>(
            this List<T> target,
            Format message = default,
            [CallerArgumentExpression(nameof(target))] string argument = "",
            [CallerMemberName] string caller = "",
            [CallerFilePath] string path = "",
            [CallerLineNumber] int line = 0
            ) where T : class
        {
            if (target is null) Throw.ArgumentNullException(argument, GetMessage($"{argument} cannot be null.", argument, caller, path, line, message));
            foreach (var t in target)
            {
                if (t is null) Throw.ArgumentNullException(argument, GetMessage($"Element in collection {argument} cannot be null.", argument, caller, path, line, message));
            }
            return target;
        }

        public static List<T> ThrowIfSequenceIsNull<T, R>(
            this List<T> target,
            Func<T, R> guard,
            Format message = default,
            [CallerArgumentExpression(nameof(target))] string argument1 = "",
            [CallerArgumentExpression(nameof(guard))] string argument2 = "",
            [CallerMemberName] string caller = "",
            [CallerFilePath] string path = "",
            [CallerLineNumber] int line = 0
            )
            where T : class
            where R : class
        {
            if (target is null) Throw.ArgumentNullException(argument1, GetMessage($"{argument1} cannot be null.", argument1, caller, path, line, message));
            foreach (var t in target)
            {
                if (t is null) Throw.ArgumentNullException(argument1, GetMessage($"Element in collection {argument1} cannot be null.", argument1, caller, path, line, message));
                if (guard(t) is null) Throw.ArgumentNullException(argument2, GetMessage($"Element in collection {argument2} cannot be null.", argument2, caller, path, line, message));
            }
            return target;
        }

        public static Dictionary<K, V> ThrowIfSequenceIsNull<K, V>(
            this Dictionary<K, V> target,
            Format message = default,
            [CallerArgumentExpression(nameof(target))] string argument = "",
            [CallerMemberName] string caller = "",
            [CallerFilePath] string path = "",
            [CallerLineNumber] int line = 0
            )
            where K : class
            where V : class
        {
            if (target is null) Throw.ArgumentNullException(argument, GetMessage($"{argument} cannot be null.", argument, caller, path, line, message));
            foreach (var kv in target)
            {
                if (kv.Key is null) Throw.ArgumentNullException(argument, GetMessage($"Element in collection {argument} cannot be null.", argument, caller, path, line, message));
                if (kv.Value is null) Throw.ArgumentNullException(argument, GetMessage($"Element in collection {argument} cannot be null.", argument, caller, path, line, message));
            }
            return target;
        }

        public static Dictionary<K, V> ThrowIfSequenceIsNull<K, V, R>(
            this Dictionary<K, V> target,
            Func<KeyValuePair<K, V>, R> guard,
            Format message = default,
            [CallerArgumentExpression(nameof(target))] string argument1 = "",
            [CallerArgumentExpression(nameof(guard))] string argument2 = "",
            [CallerMemberName] string caller = "",
            [CallerFilePath] string path = "",
            [CallerLineNumber] int line = 0
            )
            where K : class
            where V : class
            where R : class
        {
            if (target is null) Throw.ArgumentNullException(argument1, GetMessage($"{argument1} cannot be null.", argument1, caller, path, line, message));
            foreach (var kv in target)
            {
                if (kv.Key is null) Throw.ArgumentNullException(argument1, GetMessage($"Element in collection {argument1} cannot be null.", argument1, caller, path, line, message));
                if (kv.Value is null) Throw.ArgumentNullException(argument1, GetMessage($"Element in collection {argument1} cannot be null.", argument1, caller, path, line, message));
                if (guard(kv) is null) Throw.ArgumentNullException(argument2, GetMessage($"Element in collection {argument2} cannot be null.", argument2, caller, path, line, message));
            }
            return target;
        }

        #endregion

        #region ThrowIfDefault

        public static T ThrowIfDefault<T>(
            this T target,
            Format message = default,
            [CallerArgumentExpression(nameof(target))] string argument = "",
            [CallerMemberName] string caller = "",
            [CallerFilePath] string path = "",
            [CallerLineNumber] int line = 0
            ) where T : struct
        {
            if (EqualityComparer<T>.Default.Equals(target, default)) Throw.ArgumentOutOfRangeException(argument, GetMessage($"{argument} cannot be default.", argument, caller, path, line, message));
            return target;
        }

        public static T ThrowIfDefault<T, R>(
            this T target,
            Func<T, R> guard,
            Format message = default,
            [CallerArgumentExpression(nameof(target))] string argument1 = "",
            [CallerArgumentExpression(nameof(guard))] string argument2 = "",
            [CallerMemberName] string caller = "",
            [CallerFilePath] string path = "",
            [CallerLineNumber] int line = 0
            )
            where T : struct
            where R : struct
        {
            if (EqualityComparer<T>.Default.Equals(target, default)) Throw.ArgumentOutOfRangeException(argument1, GetMessage($"{argument1} cannot be default.", argument1, caller, path, line, message));
            if (EqualityComparer<R>.Default.Equals(guard(target), default)) Throw.ArgumentOutOfRangeException(argument2, GetMessage($"{argument2} cannot be default.", argument2, caller, path, line, message));
            return target;
        }

        public static T[] ThrowIfSequenceIsDefault<T>(
            this T[] target,
            Format message = default,
            [CallerArgumentExpression(nameof(target))] string argument = "",
            [CallerMemberName] string caller = "",
            [CallerFilePath] string path = "",
            [CallerLineNumber] int line = 0
            ) where T : struct
        {
            if (EqualityComparer<T[]>.Default.Equals(target, default)) Throw.ArgumentOutOfRangeException(argument, GetMessage($"{argument} cannot be default.", argument, caller, path, line, message));
            foreach (var t in target)
            {
                if (EqualityComparer<T>.Default.Equals(t, default)) Throw.ArgumentOutOfRangeException(argument, GetMessage($"Element in collection {argument} cannot be default.", argument, caller, path, line, message));
            }
            return target;
        }

        public static T[] ThrowIfSequenceIsDefault<T, R>(
            this T[] target,
            Func<T, R> guard,
            Format message = default,
            [CallerArgumentExpression(nameof(target))] string argument1 = "",
            [CallerArgumentExpression(nameof(guard))] string argument2 = "",
            [CallerMemberName] string caller = "",
            [CallerFilePath] string path = "",
            [CallerLineNumber] int line = 0
            )
            where T : struct
            where R : struct
        {
            if (EqualityComparer<T[]>.Default.Equals(target, default)) Throw.ArgumentOutOfRangeException(argument1, GetMessage($"{argument1} cannot be default.", argument1, caller, path, line, message));
            foreach (var t in target)
            {
                if (EqualityComparer<T>.Default.Equals(t, default)) Throw.ArgumentOutOfRangeException(argument1, GetMessage($"Element in collection {argument1} cannot be default.", argument1, caller, path, line, message));
                if (EqualityComparer<R>.Default.Equals(guard(t), default)) Throw.ArgumentOutOfRangeException(argument2, GetMessage($"Element in collection {argument2} cannot be default.", argument2, caller, path, line, message));
            }
            return target;
        }

        public static IEnumerable<T> ThrowIfSequenceIsDefault<T>(
            this IEnumerable<T> target,
            Format message = default,
            [CallerArgumentExpression(nameof(target))] string argument = "",
            [CallerMemberName] string caller = "",
            [CallerFilePath] string path = "",
            [CallerLineNumber] int line = 0
            ) where T : struct
        {
            if (EqualityComparer< IEnumerable<T>>.Default.Equals(target, default)) Throw.ArgumentOutOfRangeException(argument, GetMessage($"{argument} cannot be default.", argument, caller, path, line, message));
            foreach (var t in target)
            {
                if (EqualityComparer<T>.Default.Equals(t, default)) Throw.ArgumentOutOfRangeException(argument, GetMessage($"Element in collection {argument} cannot be default.", argument, caller, path, line, message));
            }
            return target;
        }

        public static IEnumerable<T> ThrowIfSequenceIsDefault<T, R>(
            this IEnumerable<T> target,
            Func<T, R> guard,
            Format message = default,
            [CallerArgumentExpression(nameof(target))] string argument1 = "",
            [CallerArgumentExpression(nameof(guard))] string argument2 = "",
            [CallerMemberName] string caller = "",
            [CallerFilePath] string path = "",
            [CallerLineNumber] int line = 0
            )
            where T : struct
            where R : struct
        {
            if (EqualityComparer<IEnumerable<T>>.Default.Equals(target, default)) Throw.ArgumentOutOfRangeException(argument1, GetMessage($"{argument1} cannot be default.", argument1, caller, path, line, message));
            foreach (var t in target)
            {
                if (EqualityComparer<T>.Default.Equals(t, default)) Throw.ArgumentOutOfRangeException(argument1, GetMessage($"Element in collection {argument1} cannot be default.", argument1, caller, path, line, message));
                if (EqualityComparer<R>.Default.Equals(guard(t), default)) Throw.ArgumentOutOfRangeException(argument2, GetMessage($"Element in collection {argument2} cannot be default.", argument2, caller, path, line, message));
            }
            return target;
        }

        public static List<T> ThrowIfSequenceIsDefault<T>(
            this List<T> target,
            Format message = default,
            [CallerArgumentExpression(nameof(target))] string argument = "",
            [CallerMemberName] string caller = "",
            [CallerFilePath] string path = "",
            [CallerLineNumber] int line = 0
            ) where T : struct
        {
            if (EqualityComparer<List<T>>.Default.Equals(target, default)) Throw.ArgumentOutOfRangeException(argument, GetMessage($"{argument} cannot be default.", argument, caller, path, line, message));
            foreach (var t in target)
            {
                if (EqualityComparer<T>.Default.Equals(t, default)) Throw.ArgumentOutOfRangeException(argument, GetMessage($"Element in collection {argument} cannot be default.", argument, caller, path, line, message));
            }
            return target;
        }

        public static List<T> ThrowIfSequenceIsDefault<T, R>(
            this List<T> target,
            Func<T, R> guard,
            Format message = default,
            [CallerArgumentExpression(nameof(target))] string argument1 = "",
            [CallerArgumentExpression(nameof(guard))] string argument2 = "",
            [CallerMemberName] string caller = "",
            [CallerFilePath] string path = "",
            [CallerLineNumber] int line = 0
            )
            where T : struct
            where R : struct
        {
            if (EqualityComparer<List<T>>.Default.Equals(target, default)) Throw.ArgumentOutOfRangeException(argument1, GetMessage($"{argument1} cannot be default.", argument1, caller, path, line, message));
            foreach (var t in target)
            {
                if (EqualityComparer<T>.Default.Equals(t, default)) Throw.ArgumentOutOfRangeException(argument1, GetMessage($"Element in collection {argument1} cannot be default.", argument1, caller, path, line, message));
                if (EqualityComparer<R>.Default.Equals(guard(t), default)) Throw.ArgumentOutOfRangeException(argument2, GetMessage($"Element in collection {argument2} cannot be default.", argument2, caller, path, line, message));
            }
            return target;
        }

        public static Dictionary<K, V> ThrowIfSequenceIsDefault<K, V>(
            this Dictionary<K, V> target,
            Format message = default,
            [CallerArgumentExpression(nameof(target))] string argument = "",
            [CallerMemberName] string caller = "",
            [CallerFilePath] string path = "",
            [CallerLineNumber] int line = 0
            )
        {
            if (EqualityComparer<Dictionary<K, V>>.Default.Equals(target, default)) Throw.ArgumentOutOfRangeException(argument, GetMessage($"{argument} cannot be default.", argument, caller, path, line, message));
            foreach (var kv in target)
            {
                if (EqualityComparer<K>.Default.Equals(kv.Key, default)) Throw.ArgumentOutOfRangeException(argument, GetMessage($"Element in collection {argument} cannot be default.", argument, caller, path, line, message));
                if (EqualityComparer<V>.Default.Equals(kv.Value, default)) Throw.ArgumentOutOfRangeException(argument, GetMessage($"Element in collection {argument} cannot be default.", argument, caller, path, line, message));
            }
            return target;
        }

        public static Dictionary<K, V> ThrowIfSequenceIsDefault<K, V, R>(
            this Dictionary<K, V> target,
            Func<KeyValuePair<K, V>, R> guard,
            Format message = default,
            [CallerArgumentExpression(nameof(target))] string argument1 = "",
            [CallerArgumentExpression(nameof(guard))] string argument2 = "",
            [CallerMemberName] string caller = "",
            [CallerFilePath] string path = "",
            [CallerLineNumber] int line = 0
            )
        {
            if (EqualityComparer<Dictionary<K, V>>.Default.Equals(target, default)) Throw.ArgumentOutOfRangeException(argument1, GetMessage($"{argument1} cannot be default.", argument1, caller, path, line, message));
            foreach (var kv in target)
            {
                if (EqualityComparer<K>.Default.Equals(kv.Key, default)) Throw.ArgumentOutOfRangeException(argument1, GetMessage($"Element in collection {argument1} cannot be default.", argument1, caller, path, line, message));
                if (EqualityComparer<V>.Default.Equals(kv.Value, default)) Throw.ArgumentOutOfRangeException(argument1, GetMessage($"Element in collection {argument1} cannot be default.", argument1, caller, path, line, message));
                if (EqualityComparer<R>.Default.Equals(guard(kv), default)) Throw.ArgumentOutOfRangeException(argument2, GetMessage($"Element in collection {argument2} cannot be default.", argument2, caller, path, line, message));
            }
            return target;
        }

        #endregion

        #region ThrowIfNullOrEmpty

        public static string ThrowIfNullOrEmpty(
            this string target,
            Format message = default,
            [CallerArgumentExpression(nameof(target))] string argument = "",
            [CallerMemberName] string caller = "",
            [CallerFilePath] string path = "",
            [CallerLineNumber] int line = 0
            )
        {
            if (string.IsNullOrEmpty(target)) Throw.ArgumentOutOfRangeException(argument, GetMessage($"{argument} cannot be null or empty.", argument, caller, path, line, message));
            return target;
        }

        public static T[] ThrowIfNullOrEmpty<T>(
            this T[] target,
            Format message = default,
            [CallerArgumentExpression(nameof(target))] string argument = "",
            [CallerMemberName] string caller = "",
            [CallerFilePath] string path = "",
            [CallerLineNumber] int line = 0
            )
        {
            if (target is null || target.Length == 0) Throw.ArgumentOutOfRangeException(argument, GetMessage($"{argument} cannot be null or empty.", argument, caller, path, line, message));
            return target;
        }

        public static IEnumerable<T> ThrowIfNullOrEmpty<T>(
            this IEnumerable<T> target,
            Format message = default,
            [CallerArgumentExpression(nameof(target))] string argument = "",
            [CallerMemberName] string caller = "",
            [CallerFilePath] string path = "",
            [CallerLineNumber] int line = 0
            )
        {
            if (target is null || !target.Any()) Throw.ArgumentOutOfRangeException(argument, GetMessage($"{argument} cannot be null or empty.", argument, caller, path, line, message));
            return target;
        }

        public static List<T> ThrowIfNullOrEmpty<T>(
            this List<T> target,
            Format message = default,
            [CallerArgumentExpression(nameof(target))] string argument = "",
            [CallerMemberName] string caller = "",
            [CallerFilePath] string path = "",
            [CallerLineNumber] int line = 0
            )
        {
            if (target is null || target.Count == 0) Throw.ArgumentOutOfRangeException(argument, GetMessage($"{argument} cannot be null or empty.", argument, caller, path, line, message));
            return target;
        }

        public static Dictionary<K, V> ThrowIfNullOrEmpty<K, V>(
            this Dictionary<K, V> target,
            Format message = default,
            [CallerArgumentExpression(nameof(target))] string argument = "",
            [CallerMemberName] string caller = "",
            [CallerFilePath] string path = "",
            [CallerLineNumber] int line = 0
            )
        {
            if (target is null || target.Count == 0) Throw.ArgumentOutOfRangeException(argument, GetMessage($"{argument} cannot be null or empty.", argument, caller, path, line, message));
            return target;
        }

        public static string[] ThrowIfSequenceIsNullOrEmpty(
            this string[] target,
            Format message = default,
            [CallerArgumentExpression(nameof(target))] string argument = "",
            [CallerMemberName] string caller = "",
            [CallerFilePath] string path = "",
            [CallerLineNumber] int line = 0
            )
        {
            if (target is null) Throw.ArgumentNullException(argument, GetMessage($"{argument} cannot be null.", argument, caller, path, line, message));
            foreach (var t in target)
            {
                if (string.IsNullOrEmpty(t)) Throw.ArgumentOutOfRangeException(argument, GetMessage($"Element in collection {argument} cannot be null or empty.", argument, caller, path, line, message));
            }
            return target;
        }

        public static T[] ThrowIfSequenceIsNullOrEmpty<T>(
            this T[] target,
            Func<T, string> guard,
            Format message = default,
            [CallerArgumentExpression(nameof(target))] string argument1 = "",
            [CallerArgumentExpression(nameof(guard))] string argument2 = "",
            [CallerMemberName] string caller = "",
            [CallerFilePath] string path = "",
            [CallerLineNumber] int line = 0
            )
            where T : class
        {
            if (target is null) Throw.ArgumentNullException(argument1, GetMessage($"{argument1} cannot be null.", argument1, caller, path, line, message));
            foreach (var t in target)
            {
                if (t is null) Throw.ArgumentNullException(argument1, GetMessage($"Element in collection {argument1} cannot be null.", argument1, caller, path, line, message));
                if (string.IsNullOrEmpty(guard(t))) Throw.ArgumentOutOfRangeException(argument2, GetMessage($"Element in collection {argument2} cannot be null or empty.", argument2, caller, path, line, message));
            }
            return target;
        }

        public static IEnumerable<string> ThrowIfSequenceIsNullOrEmpty(
            this IEnumerable<string> target,
            Format message = default,
            [CallerArgumentExpression(nameof(target))] string argument = "",
            [CallerMemberName] string caller = "",
            [CallerFilePath] string path = "",
            [CallerLineNumber] int line = 0
            )
        {
            if (target is null) Throw.ArgumentNullException(argument, GetMessage($"{argument} cannot be null.", argument, caller, path, line, message));
            foreach (var t in target)
            {
                if (string.IsNullOrEmpty(t)) Throw.ArgumentOutOfRangeException(argument, GetMessage($"Element in collection {argument} cannot be null or empty.", argument, caller, path, line, message));
            }
            return target;
        }

        public static IEnumerable<T> ThrowIfSequenceIsNullOrEmpty<T>(
            this IEnumerable<T> target,
            Func<T, string> guard,
            Format message = default,
            [CallerArgumentExpression(nameof(target))] string argument1 = "",
            [CallerArgumentExpression(nameof(guard))] string argument2 = "",
            [CallerMemberName] string caller = "",
            [CallerFilePath] string path = "",
            [CallerLineNumber] int line = 0
            )
            where T : class
        {
            if (target is null) Throw.ArgumentNullException(argument1, GetMessage($"{argument1} cannot be null.", argument1, caller, path, line, message));
            foreach (var t in target)
            {
                if (t is null) Throw.ArgumentNullException(argument1, GetMessage($"Element in collection {argument1} cannot be null.", argument1, caller, path, line, message));
                if (string.IsNullOrEmpty(guard(t))) Throw.ArgumentOutOfRangeException(argument2, GetMessage($"Element in collection {argument2} cannot be null or empty.", argument2, caller, path, line, message));
            }
            return target;
        }

        public static List<string> ThrowIfSequenceIsNullOrEmpty(
            this List<string> target,
            Format message = default,
            [CallerArgumentExpression(nameof(target))] string argument = "",
            [CallerMemberName] string caller = "",
            [CallerFilePath] string path = "",
            [CallerLineNumber] int line = 0
            )
        {
            if (target is null) Throw.ArgumentNullException(argument, GetMessage($"{argument} cannot be null.", argument, caller, path, line, message));
            foreach (var t in target)
            {
                if (string.IsNullOrEmpty(t)) Throw.ArgumentOutOfRangeException(argument, GetMessage($"Element in collection {argument} cannot be null or empty.", argument, caller, path, line, message));
            }
            return target;
        }

        public static List<T> ThrowIfSequenceIsNullOrEmpty<T>(
            this List<T> target,
            Func<T, string> guard,
            Format message = default,
            [CallerArgumentExpression(nameof(target))] string argument1 = "",
            [CallerArgumentExpression(nameof(guard))] string argument2 = "",
            [CallerMemberName] string caller = "",
            [CallerFilePath] string path = "",
            [CallerLineNumber] int line = 0
            )
            where T : class
        {
            if (target is null) Throw.ArgumentNullException(argument1, GetMessage($"{argument1} cannot be null.", argument1, caller, path, line, message));
            foreach (var t in target)
            {
                if (t is null) Throw.ArgumentNullException(argument1, GetMessage($"Element in collection {argument1} cannot be null.", argument1, caller, path, line, message));
                if (string.IsNullOrEmpty(guard(t))) Throw.ArgumentOutOfRangeException(argument2, GetMessage($"Element in collection {argument2} cannot be null or empty.", argument2, caller, path, line, message));
            }
            return target;
        }

        public static Dictionary<string, string> ThrowIfSequenceIsNullOrEmpty(
            this Dictionary<string, string> target,
            Format message = default,
            [CallerArgumentExpression(nameof(target))] string argument = "",
            [CallerMemberName] string caller = "",
            [CallerFilePath] string path = "",
            [CallerLineNumber] int line = 0
            )
        {
            if (target is null) Throw.ArgumentNullException(argument, GetMessage($"{argument} cannot be null.", argument, caller, path, line, message));
            foreach (var kv in target)
            {
                if (string.IsNullOrEmpty(kv.Key)) Throw.ArgumentOutOfRangeException(argument, GetMessage($"Element in collection {argument} cannot be null or empty.", argument, caller, path, line, message));
                if (string.IsNullOrEmpty(kv.Value)) Throw.ArgumentOutOfRangeException(argument, GetMessage($"Element in collection {argument} cannot be null or empty.", argument, caller, path, line, message));
            }
            return target;
        }

        public static Dictionary<K, V> ThrowIfSequenceIsNullOrEmpty<K, V>(
            this Dictionary<K, V> target,
            Func<KeyValuePair<K, V>, string> guard,
            Format message = default,
            [CallerArgumentExpression(nameof(target))] string argument1 = "",
            [CallerArgumentExpression(nameof(guard))] string argument2 = "",
            [CallerMemberName] string caller = "",
            [CallerFilePath] string path = "",
            [CallerLineNumber] int line = 0
            )
            where K : class
            where V : class
        {
            if (target is null) Throw.ArgumentNullException(argument1, GetMessage($"{argument1} cannot be null.", argument1, caller, path, line, message));
            foreach (var kv in target)
            {
                if (kv.Key is null) Throw.ArgumentNullException(argument1, GetMessage($"Element in collection {argument1} cannot be null.", argument1, caller, path, line, message));
                if (kv.Value is null) Throw.ArgumentNullException(argument1, GetMessage($"Element in collection {argument1} cannot be null.", argument1, caller, path, line, message));
                if (string.IsNullOrEmpty(guard(kv))) Throw.ArgumentOutOfRangeException(argument2, GetMessage($"Element in collection {argument2} cannot be null or empty or empty.", argument2, caller, path, line, message));
            }
            return target;
        }

        #endregion

        #region ThrowIf

        public static T ThrowIf<T>(
            this T target,
            Func<T, bool> predicate,
            Format message = default,
            [CallerArgumentExpression(nameof(target))] string argument1 = "",
            [CallerArgumentExpression(nameof(predicate))] string argument2 = "",
            [CallerMemberName] string caller = "",
            [CallerFilePath] string path = "",
            [CallerLineNumber] int line = 0
            )
        {
            if (target is null) Throw.ArgumentNullException(argument1, GetMessage($"{argument1} cannot be null.", argument1, caller, path, line, message));
            if (predicate(target)) Throw.ArgumentOutOfRangeException(argument2, GetMessage($"Predicate failed conditional check for value: {target}.", argument2, caller, path, line, message));
            return target;
        }

        public static Task<T> ThrowIfAsync<T>(
            this Task<T> target,
            Func<T, bool> predicate,
            Format message = default,
            [CallerArgumentExpression(nameof(target))] string argument1 = "",
            [CallerArgumentExpression(nameof(predicate))] string argument2 = "",
            [CallerMemberName] string caller = "",
            [CallerFilePath] string path = "",
            [CallerLineNumber] int line = 0
            )
        {
            if (target is null) Throw.ArgumentNullException(argument1, GetMessage($"{argument1} cannot be null.", argument1, caller, path, line, message));
            if (target.IsCompleted) {
                if (predicate(target.ThrowIfFaultedOrCanceled().Result)) { Throw.ArgumentOutOfRangeException(argument2, GetMessage($"Predicate failed conditional check for value: {target.Result}.", argument2, caller, path, line, message)); }
                return target;
            }
            return target.ContinueWith(t => {
                if (predicate(t.ThrowIfFaultedOrCanceled().Result)) { Throw.ArgumentOutOfRangeException(argument2, GetMessage($"Predicate failed conditional check for value: {t.Result}.", argument2, caller, path, line, message)); }
                return t.Result;
            });
        }

        public static Task<T> ThrowIfAsync<T>(
            this Task<T> target,
            Func<T, Task<bool>> predicate,
            Format message = default,
            [CallerArgumentExpression(nameof(target))] string argument1 = "",
            [CallerArgumentExpression(nameof(predicate))] string argument2 = "",
            [CallerMemberName] string caller = "",
            [CallerFilePath] string path = "",
            [CallerLineNumber] int line = 0
            )
        {
            if (target is null) Throw.ArgumentNullException(argument1, GetMessage($"{argument1} cannot be null.", argument1, caller, path, line, message));
            if (target.IsCompleted)
            {
                var result = predicate(target.ThrowIfFaultedOrCanceled().Result);
                if (result.ThrowIfFaultedOrCanceled().Result) { Throw.ArgumentOutOfRangeException(argument2, GetMessage($"Predicate failed conditional check for value: {target.Result}.", argument2, caller, path, line, message)); }
                return target;
            }
            return target.ContinueWith(t => {
                var r = predicate(t.ThrowIfFaultedOrCanceled().Result);
                if (r.ThrowIfFaultedOrCanceled().Result) { Throw.ArgumentOutOfRangeException(argument2, GetMessage($"Predicate failed conditional check for value: {t.Result}.", argument2, caller, path, line, message)); }
                return t.Result;
            });
        }

        public static Task<T> ThrowIfAsync<T>(
            this T target,
            Func<T, Task<bool>> predicate,
            Format message = default,
            [CallerArgumentExpression(nameof(target))] string argument1 = "",
            [CallerArgumentExpression(nameof(predicate))] string argument2 = "",
            [CallerMemberName] string caller = "",
            [CallerFilePath] string path = "",
            [CallerLineNumber] int line = 0
            )
        {
            if (target is null) Throw.ArgumentNullException(argument1, GetMessage($"{argument1} cannot be null.", argument1, caller, path, line, message));
            var result = predicate(target);
            if (result.IsCompleted) {
                if (result.ThrowIfFaultedOrCanceled().Result) { Throw.ArgumentOutOfRangeException(argument2, GetMessage($"Predicate failed conditional check for value: {target}.", argument2, caller, path, line, message)); }
                return Task.FromResult(target);
            }
            return result.ContinueWith(t => {
                if (t.ThrowIfFaultedOrCanceled().Result) { Throw.ArgumentOutOfRangeException(argument2, GetMessage($"Predicate failed conditional check for value: {target}.", argument2, caller, path, line, message)); }
                return target;
            });
        }

        public static T[] ThrowIfSequence<T>(
            this T[] target,
            Func<T, bool> predicate,
            Format message = default,
            [CallerArgumentExpression(nameof(target))] string argument1 = "",
            [CallerArgumentExpression(nameof(predicate))] string argument2 = "",
            [CallerMemberName] string caller = "",
            [CallerFilePath] string path = "",
            [CallerLineNumber] int line = 0
            )
        {
            if (target is null) Throw.ArgumentNullException(argument1, GetMessage($"{argument1} cannot be null.", argument1, caller, path, line, message));
            foreach (var t in target)
            {
                if (predicate(t)) Throw.ArgumentOutOfRangeException(argument2, GetMessage($"Predicate failed conditional check for value: {t}.", argument2, caller, path, line, message));
            }
            return target;
        }

        public static IEnumerable<T> ThrowIfSequence<T>(
            this IEnumerable<T> target,
            Func<T, bool> predicate,
            Format message = default,
            [CallerArgumentExpression(nameof(target))] string argument1 = "",
            [CallerArgumentExpression(nameof(predicate))] string argument2 = "",
            [CallerMemberName] string caller = "",
            [CallerFilePath] string path = "",
            [CallerLineNumber] int line = 0
            )
        {
            if (target is null) Throw.ArgumentNullException(argument1, GetMessage($"{argument1} cannot be null.", argument1, caller, path, line, message));
            foreach (var t in target)
            {
                if (predicate(t)) Throw.ArgumentOutOfRangeException(argument2, GetMessage($"Predicate failed conditional check for value: {t}.", argument2, caller, path, line, message));
            }
            return target;
        }

        public static List<T> ThrowIfSequence<T>(
            this List<T> target,
            Func<T, bool> predicate,
            Format message = default,
            [CallerArgumentExpression(nameof(target))] string argument1 = "",
            [CallerArgumentExpression(nameof(predicate))] string argument2 = "",
            [CallerMemberName] string caller = "",
            [CallerFilePath] string path = "",
            [CallerLineNumber] int line = 0
            )
        {
            if (target is null) Throw.ArgumentNullException(argument1, GetMessage($"{argument1} cannot be null.", argument1, caller, path, line, message));
            foreach (var t in target)
            {
                if (predicate(t)) Throw.ArgumentOutOfRangeException(argument2, GetMessage($"Predicate failed conditional check for value: {t}.", argument2, caller, path, line, message));
            }
            return target;
        }

        public static Dictionary<K, V> ThrowIfSequence<K, V>(
            this Dictionary<K, V> target,
            Func<KeyValuePair<K, V>, bool> predicate,
            Format message = default,
            [CallerArgumentExpression(nameof(target))] string argument1 = "",
            [CallerArgumentExpression(nameof(predicate))] string argument2 = "",
            [CallerMemberName] string caller = "",
            [CallerFilePath] string path = "",
            [CallerLineNumber] int line = 0
            )
        {
            if (target is null) Throw.ArgumentNullException(argument1, GetMessage($"{argument1} cannot be null.", argument1, caller, path, line, message));
            foreach (var t in target)
            {
                if (predicate(t)) Throw.ArgumentOutOfRangeException(argument2, GetMessage($"Predicate failed conditional check for value: {t}.", argument2, caller, path, line, message));
            }
            return target;
        }

        #endregion

        #region ThrowIfLessThanGreaterThan

        public static T ThrowIfLessThan<T>(
            this T target,
            T min,
            Format message = default,
            [CallerArgumentExpression(nameof(target))] string argument = "",
            [CallerArgumentExpression(nameof(min))] string predicate = "",
            [CallerMemberName] string caller = "",
            [CallerFilePath] string path = "",
            [CallerLineNumber] int line = 0
            ) where T : struct, IComparable<T>
        {
            if (target.CompareTo(min) < 0) Throw.ArgumentOutOfRangeException(argument, GetMessage($"{target} < {min} is not valid, {target} must be >= {min}.", argument, predicate, caller, path, line, message));
            return target;
        }

        public static T ThrowIfGreaterThan<T>(
            this T target,
            T max,
            Format message = default,
            [CallerArgumentExpression(nameof(target))] string argument = "",
            [CallerArgumentExpression(nameof(max))] string predicate = "",
            [CallerMemberName] string caller = "",
            [CallerFilePath] string path = "",
            [CallerLineNumber] int line = 0
            ) where T : struct, IComparable<T>
        {
            if (target.CompareTo(max) > 0) Throw.ArgumentOutOfRangeException(argument, GetMessage($"{target} > {max} is not valid, {target} must be <= {max}.", argument, predicate, caller, path, line, message));
            return target;
        }

        #endregion

        #region ThrowIfFaultedOrCanceled

        /**
         * [ThrowIfFaultedOrCanceled]
         * Task.Wait(), Task.Result, and the various awaiters already throw faulted, and canceled exceptions; if you try to access them in said state.
         * The purpose of this wrapper then, is to only throw a single exception (instead of aggregate exceptions); when there is only 1 error present.
        **/

        public static Task ThrowIfFaultedOrCanceled(
            this Task target,
            Format message = default,
            [CallerArgumentExpression(nameof(target))] string argument = "",
            [CallerMemberName] string caller = "",
            [CallerFilePath] string path = "",
            [CallerLineNumber] int line = 0
            )
        {
            if (target is null) Throw.ArgumentNullException(argument, GetMessage($"{argument} cannot be null.", argument, caller, path, line, message));
            if (!target.IsCompleted) return target.ContinueWith(t => CheckStatus(t, argument, caller, path, line, message));
            CheckStatus(target, argument, caller, path, line, message);
            return target;
        }

        public static Task<T> ThrowIfFaultedOrCanceled<T>(
            this Task<T> target,
            Format message = default,
            [CallerArgumentExpression(nameof(target))] string argument = "",
            [CallerMemberName] string caller = "",
            [CallerFilePath] string path = "",
            [CallerLineNumber] int line = 0
            )
        {
            if (target is null) Throw.ArgumentNullException(argument, GetMessage($"{argument} cannot be null.", argument, caller, path, line, message));
            if (!target.IsCompleted) return target.ContinueWith(t => { CheckStatus(t, argument, caller, path, line, message); return t.Result; });
            CheckStatus(target, argument, caller, path, line, message);
            return target;
        }

        private static void CheckStatus(Task task, string argument, string caller, string path, int line, Format message)
        {
            if (task.Status == TaskStatus.Faulted)
            {
                if (task.Exception.InnerExceptions.Count == 1) Throw.Exception(task.Exception.InnerException.AddCallerAttributes(argument, caller, path, line, message));
                else Throw.Exception(task.Exception.AddCallerAttributes(argument, caller, path, line, message));
            }
            if (task.Status == TaskStatus.Canceled) Throw.TaskCanceledException(GetMessage("A task was canceled.", argument, caller, path, line, message));
        }

        #endregion
    }
}
