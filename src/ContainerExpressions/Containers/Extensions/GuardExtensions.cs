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

        public static T ThrowIfNull<T>(
            this T target,
            Format message = default,
            [CallerArgumentExpression(nameof(target))] string argument = "",
            [CallerMemberName] string caller = "",
            [CallerFilePath] string path = "",
            [CallerLineNumber] int line = 0
            ) where T : class
        {
            if (target == null) Throw.ArgumentNullException(argument, GetMessage($"{argument} cannot be null.", argument, caller, path, line, message));
            return target;
        }

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
            if (target == null || target.Length == 0) Throw.ArgumentOutOfRangeException(argument, GetMessage($"{argument} cannot be null or empty.", argument, caller, path, line, message));
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
            if (target == null || !target.Any()) Throw.ArgumentOutOfRangeException(argument, GetMessage($"{argument} cannot be null or empty.", argument, caller, path, line, message));
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
            if (target == null || target.Count == 0) Throw.ArgumentOutOfRangeException(argument, GetMessage($"{argument} cannot be null or empty.", argument, caller, path, line, message));
            return target;
        }

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
            if (target == null) return target;
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
            if (target == null) return target;
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
