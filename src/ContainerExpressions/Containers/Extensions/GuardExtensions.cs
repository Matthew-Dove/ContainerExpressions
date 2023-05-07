using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.ExceptionServices;

namespace ContainerExpressions.Containers.Extensions
{
    public static class GuardExtensions
    {
        // TODO: Ensure all exceptions are thrown in another class / method, to improve the chance of them having a cold jit; and these being inlined at runtime.

        public static void ThrowError<T>(
            this T ex,
            [CallerArgumentExpression(nameof(ex))] string argument = "",
            [CallerMemberName] string caller = "",
            [CallerFilePath] string path = "",
            [CallerLineNumber] int line = 0
            ) where T : Exception => throw ex;

        public static void ThrowError(
            this ExceptionDispatchInfo ex,
            [CallerArgumentExpression(nameof(ex))] string argument = "",
            [CallerMemberName] string caller = "",
            [CallerFilePath] string path = "",
            [CallerLineNumber] int line = 0
            ) => ex.Throw();

        public static T ThrowIfNull<T>(
            this T target,
            [CallerArgumentExpression(nameof(target))] string argument = "",
            [CallerMemberName] string caller = "",
            [CallerFilePath] string path = "",
            [CallerLineNumber] int line = 0
            ) where T : class
        {
            if (target == null) ArgumentNullException(argument);
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
            if (EqualityComparer<T>.Default.Equals(target, default)) ArgumentOutOfRangeException(argument);
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
            if (string.IsNullOrEmpty(target)) ArgumentOutOfRangeException(argument);
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
            if (target == null || target.Length == 0) ArgumentOutOfRangeException(argument);
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
            if (target == null || !target.Any()) ArgumentOutOfRangeException(argument);
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
            if (target == null || target.Count == 0) ArgumentOutOfRangeException(argument);
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
            if (target.CompareTo(min) < 0) ArgumentOutOfRangeException(argument);
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
            if (target.CompareTo(max) > 0) ArgumentOutOfRangeException(argument);
            return target;
        }
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
