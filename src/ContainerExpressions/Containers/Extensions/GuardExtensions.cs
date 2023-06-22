using ContainerExpressions.Containers.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.ExceptionServices;
using System.Text;
using System.Threading.Tasks;

namespace ContainerExpressions.Containers.Extensions
{
    public static class GuardExtensions
    {
        public static void ThrowError<T>(this T ex) where T : Exception => Throw.Exception(ex);
        public static void ThrowError(this ExceptionDispatchInfo ex) => Throw.Exception(ex);

        private static Either<string, ErrorMessage> GetMessage(string message, string argument, string predicate, string caller, string path, int line)
        {
            if (argument == string.Empty && predicate == string.Empty && caller == string.Empty && path == string.Empty && line == 0) return new ErrorMessage(message);
            return new StringBuilder()
                .Append("Message: ").AppendLine(message)
                .Append("CallerArgumentExpression: ").AppendLine(argument)
                .Append("CallerPredicateExpression: ").AppendLine(predicate)
                .Append("CallerMemberName: ").AppendLine(caller)
                .Append("CallerFilePath: ").AppendLine(path)
                .Append("CallerLineNumber: ").Append(line)
                .ToString();
        }

        private static Either<string, NotFound> GetMessage(string caller, string path, int line)
        {
            if (caller == string.Empty && path == string.Empty && line == 0) return new NotFound();
            return new StringBuilder()
                .Append("CallerMemberName: ").AppendLine(caller)
                .Append("CallerFilePath: ").AppendLine(path)
                .Append("CallerLineNumber: ").Append(line)
                .ToString();
        }

        public static T ThrowIfNull<T>(
            this T target,
            [CallerArgumentExpression(nameof(target))] string argument = "",
            [CallerMemberName] string caller = "",
            [CallerFilePath] string path = "",
            [CallerLineNumber] int line = 0
            ) where T : class
        {
            if (target == null)
                GetMessage(caller, path, line).Match(msg => Throw.ArgumentNullException(argument, msg), _ => Throw.ArgumentNullException(argument));
            return target;
        }

        public static T ThrowIfDefault<T>(
            this T target,
            [CallerArgumentExpression(nameof(target))] string argument = "",
            [CallerMemberName] string caller = "",
            [CallerFilePath] string path = "",
            [CallerLineNumber] int line = 0
            ) where T : struct
        {
            if (EqualityComparer<T>.Default.Equals(target, default))
                GetMessage(caller, path, line).Match(msg => Throw.ArgumentOutOfRangeException(argument, msg), _ => Throw.ArgumentOutOfRangeException(argument));
            return target;
        }

        public static string ThrowIfNullOrEmpty(
            this string target,
            [CallerArgumentExpression(nameof(target))] string argument = "",
            [CallerMemberName] string caller = "",
            [CallerFilePath] string path = "",
            [CallerLineNumber] int line = 0
            )
        {
            if (string.IsNullOrEmpty(target))
                GetMessage(caller, path, line).Match(msg => Throw.ArgumentOutOfRangeException(argument, msg), _ => Throw.ArgumentOutOfRangeException(argument));
            return target;
        }

        public static T[] ThrowIfNullOrEmpty<T>(
            this T[] target,
            [CallerArgumentExpression(nameof(target))] string argument = "",
            [CallerMemberName] string caller = "",
            [CallerFilePath] string path = "",
            [CallerLineNumber] int line = 0
            )
        {
            if (target == null || target.Length == 0)
                GetMessage(caller, path, line).Match(msg => Throw.ArgumentOutOfRangeException(argument, msg), _ => Throw.ArgumentOutOfRangeException(argument));
            return target;
        }

        public static IEnumerable<T> ThrowIfNullOrEmpty<T>(
            this IEnumerable<T> target,
            [CallerArgumentExpression(nameof(target))] string argument = "",
            [CallerMemberName] string caller = "",
            [CallerFilePath] string path = "",
            [CallerLineNumber] int line = 0
            )
        {
            if (target == null || !target.Any())
                GetMessage(caller, path, line).Match(msg => Throw.ArgumentOutOfRangeException(argument, msg), _ => Throw.ArgumentOutOfRangeException(argument));
            return target;
        }

        public static List<T> ThrowIfNullOrEmpty<T>(
            this List<T> target,
            [CallerArgumentExpression(nameof(target))] string argument = "",
            [CallerMemberName] string caller = "",
            [CallerFilePath] string path = "",
            [CallerLineNumber] int line = 0
            )
        {
            if (target == null || target.Count == 0)
                GetMessage(caller, path, line).Match(msg => Throw.ArgumentOutOfRangeException(argument, msg), _ => Throw.ArgumentOutOfRangeException(argument));
            return target;
        }

        public static T ThrowIfLessThan<T>(
            this T target,
            T min,
            [CallerArgumentExpression(nameof(target))] string argument = "",
            [CallerArgumentExpression(nameof(min))] string predicate = "",
            [CallerMemberName] string caller = "",
            [CallerFilePath] string path = "",
            [CallerLineNumber] int line = 0
            ) where T : struct, IComparable<T>
        {
            if (target.CompareTo(min) < 0)
                GetMessage($"{target} < {min} is not valid, {target} must be >= {min}.", argument, predicate, caller, path, line).Match(msg => Throw.ArgumentOutOfRangeException(argument, msg), errMsg => Throw.ArgumentOutOfRangeException(argument, errMsg));
            return target;
        }

        public static T ThrowIfGreaterThan<T>(
            this T target,
            T max,
            [CallerArgumentExpression(nameof(target))] string argument = "",
            [CallerArgumentExpression(nameof(max))] string predicate = "",
            [CallerMemberName] string caller = "",
            [CallerFilePath] string path = "",
            [CallerLineNumber] int line = 0
            ) where T : struct, IComparable<T>
        {
            if (target.CompareTo(max) > 0)
                GetMessage($"{target} > {max} is not valid, {target} must be <= {max}.", argument, predicate, caller, path, line).Match(msg => Throw.ArgumentOutOfRangeException(argument, msg), errMsg => Throw.ArgumentOutOfRangeException(argument, errMsg));
            return target;
        }

        #region ThrowIfFaultedOrCanceled

        /**
         * [ThrowIfFaultedOrCanceled]
         * Task.Wait(), Task.Result, and the various awaiters already throw faulted, and canceled exceptions; if you try to access them in said state.
         * The purpose of this wrapper then, is to only throw a single exception (instead of aggregate exceptions); when there is only 1 error present.
        **/

        public static Task ThrowIfFaultedOrCanceled(this Task target)
        {
            if (target == null) return target;
            if (!target.IsCompleted) return target.ContinueWith(CheckStatus);
            CheckStatus(target);
            return target;
        }

        public static Task<T> ThrowIfFaultedOrCanceled<T>(this Task<T> target)
        {
            if (target == null) return target;
            if (!target.IsCompleted) return target.ContinueWith(static t => { CheckStatus(t); return t.Result; });
            CheckStatus(target);
            return target;
        }

        private static void CheckStatus(Task task)
        {
            if (task.Status == TaskStatus.Faulted)
            {
                if (task.Exception.InnerExceptions.Count == 1) Throw.Exception(task.Exception.InnerException);
                else Throw.Exception(task.Exception);
            }
            if (task.Status == TaskStatus.Canceled) Throw.TaskCanceledException(task);
        }

        #endregion
    }
}

#if !NETCOREAPP3_0_OR_GREATER
namespace System.Runtime.CompilerServices
{
    /// <summary>
    /// Contains the source code passed to a method's parameter, determined at compile time.
    /// <para>For example this could be a variable name (foo), an expression (foo > bar), or some object creation (new { foo = bar}).</para>
    /// </summary>
    [AttributeUsage(AttributeTargets.Parameter, AllowMultiple = false, Inherited = false)]
    internal sealed class CallerArgumentExpressionAttribute : Attribute
    {
        public string ParameterName { get; }

        public CallerArgumentExpressionAttribute(string parameterName)
        {
            ParameterName = parameterName;
        }
    }
}
#endif