using ContainerExpressions.Containers.Internal;
using ContainerExpressions.Expressions.Models;
using System;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace ContainerExpressions.Containers
{
    /// <summary>Wraps a function in error protecting code.</summary>
    public static class Try
    {
        /// <summary>Caller attributes are added the the exception's data dictionary, with this as the key.</summary>
        public const string DataKey = "ContainerExpressions.Caller";

        /// <summary>Gets the caller values, or empty string if they don't exist on the exception.</summary>
        /// <param name="ex">The exception sent to the logger from the Try container.</param>
        public static string GetCallerAttributes(this Exception ex)
        {
            if (ex?.Data is not null && ex.Data.Contains(DataKey)) return ex.Data[DataKey].ToString();
            return string.Empty;
        }

        internal static T AddCallerAttributes<T>(this T ex, string argumentExpression, string memberName, string filePath, int lineNumber) where T : Exception
        {
            if (ex is null) return ex;
            return AddCallerAttributes(ex, ex.Message, argumentExpression, memberName, filePath, lineNumber);
        }

        internal static T AddCallerAttributes<T>(this T ex, string message, string argumentExpression, string memberName, string filePath, int lineNumber) where T : Exception
        {
            var data = new StringBuilder()
                .Append("Message: ").AppendLine(message)
                .Append("CallerArgumentExpression: ").AppendLine(argumentExpression)
                .Append("CallerMemberName: ").AppendLine(memberName)
                .Append("CallerFilePath: ").AppendLine(filePath)
                .Append("CallerLineNumber: ").Append(lineNumber)
                .ToString();

            if (ex?.Data is not null && !ex.Data.Contains(DataKey)) ex.Data.Add(DataKey, data);
            return ex;
        }

        internal static Response<Action<Exception>> GetExceptionLogger() => _logger;
        private static Response<Action<Exception>> _logger = new Response<Action<Exception>>();

        /// <summary>If you'd like to log errors as they come, add your stateless error logger here, if a logger already exists, it'll be overwritten.</summary>
        public static void SetExceptionLogger(Action<Exception> logger)
        {
            if (logger == null) Throw.ArgumentNullException(nameof(logger));
            _logger = Response.Create(logger);
        }

        /// <summary>Wraps a function in error protecting code.</summary>
        public static Response Run(
            Action action,
            [CallerArgumentExpression(nameof(action))] string argument = "",
            [CallerMemberName] string caller = "",
            [CallerFilePath] string path = "",
            [CallerLineNumber] int line = 0
            )
        {
            if (action == null) Throw.ArgumentNullException(nameof(action));
            return PaddedCage(action, ExceptionLogger.Create(_logger, argument, caller, path, line));
        }

        private static Response PaddedCage(Action action, ExceptionLogger error)
        {
            var response = new Response();

            try
            {
                action();
                response = response.AsValid();
            }
            catch (AggregateException ae)
            {
                foreach (var e in ae.Flatten().InnerExceptions)
                {
                    error.Log(e);
                }
            }
            catch (Exception ex)
            {
                error.Log(ex);
            }

            return response;
        }

        /// <summary>Wraps a function in error protecting code.</summary>
        public static Response<T> Run<T>(
            Func<T> func,
            [CallerArgumentExpression(nameof(func))] string argument = "",
            [CallerMemberName] string caller = "",
            [CallerFilePath] string path = "",
            [CallerLineNumber] int line = 0
            )
        {
            if (func == null) Throw.ArgumentNullException(nameof(func));
            return PaddedCage(func, ExceptionLogger.Create(_logger, argument, caller, path, line));
        }

        private static Response<T> PaddedCage<T>(Func<T> func, ExceptionLogger error)
        {
            var response = new Response<T>();

            try
            {
                var result = func();
                response = response.With(result);
            }
            catch (AggregateException ae)
            {
                foreach (var e in ae.Flatten().InnerExceptions)
                {
                    error.Log(e);
                }
            }
            catch (Exception ex)
            {
                error.Log(ex);
            }

            return response;
        }

        /// <summary>Wraps a function in error protecting code.</summary>
        public static Task<Response> RunAsync(
            Func<Task> action,
            [CallerArgumentExpression(nameof(action))] string argument = "",
            [CallerMemberName] string caller = "",
            [CallerFilePath] string path = "",
            [CallerLineNumber] int line = 0
            )
        {
            if (action == null) Throw.ArgumentNullException(nameof(action));
            return PaddedCageAsync(action, ExceptionLogger.Create(_logger, argument, caller, path, line));
        }

        private static async Task<Response> PaddedCageAsync(Func<Task> func, ExceptionLogger error)
        {
            var response = new Response();

            try
            {
                await func();
                response = response.AsValid();
            }
            catch (AggregateException ae)
            {
                foreach (var e in ae.Flatten().InnerExceptions)
                {
                    error.Log(e);
                }
            }
            catch (Exception ex)
            {
                error.Log(ex);
            }

            return response;
        }

        /// <summary>Wraps a function in error protecting code.</summary>
        public static Task<Response<T>> RunAsync<T>(
            Func<Task<T>> func,
            [CallerArgumentExpression(nameof(func))] string argument = "",
            [CallerMemberName] string caller = "",
            [CallerFilePath] string path = "",
            [CallerLineNumber] int line = 0
            )
        {
            if (func == null) Throw.ArgumentNullException(nameof(func));
            return PaddedCageAsync(func, ExceptionLogger.Create(_logger, argument, caller, path, line));
        }

        private static async Task<Response<T>> PaddedCageAsync<T>(Func<Task<T>> func, ExceptionLogger error)
        {
            var response = new Response<T>();

            try
            {
                var result = await func();
                response = response.With(result);
            }
            catch (AggregateException ae)
            {
                foreach (var e in ae.Flatten().InnerExceptions)
                {
                    error.Log(e);
                }
            }
            catch (Exception ex)
            {
                error.Log(ex);
            }

            return response;
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
