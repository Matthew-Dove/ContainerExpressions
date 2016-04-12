using ContainerExpressions.Expressions.Models;
using System;
using System.Threading.Tasks;

namespace ContainerExpressions.Containers
{
    /// <summary>Wraps a function in error protecting code.</summary>
    public struct Try<T>
    {
        /// <summary>Runs the function the first time this is accessed, returns a valid Response if the code ran without any errors.</summary>
        public Response<T> Value { get { return _func; } }
        private readonly Later<Response<T>> _func; // Lazily evaluated to the caller can't just wrap the function and run fot the hills, without at least checking the value first.

        /// <summary>Wraps a function in error protecting code.</summary>
        public Try(Func<T> func)
        {
            if (func == null)
                throw new ArgumentNullException(nameof(func));

            var exceptionLogger = Try.GetExceptionLogger(); // Grabs the current Exception Logger set at the time this instance is created (incase it's changed before the value is read).
            _func = Later.Create(() => PaddedCage(func, ExceptionLogger.Create(exceptionLogger)));
        }

        /// <summary>Run the code in a safe manner, optionally logging any errors as they occur.</summary>
        private static Response<T> PaddedCage(Func<T> func, ExceptionLogger exceptionLogger)
        {
            var response = new Response<T>();

            try
            {
                var result = func();
                response = response.WithValue(result);
            }
            catch (Exception ex)
            {
                exceptionLogger.Log(ex);
            }

            return response;
        }

        public static implicit operator Response<T>(Try<T> func) => func.Value;

        public static implicit operator bool(Try<T> func) => func.Value.IsValid;

        public static implicit operator T(Try<T> func) => func.Value.Value;
    }

    /// <summary>Wraps a function in error protecting code.</summary>
    public struct Try
    {
        /// <summary>Runs the function the first time this is accessed, returns a valid Response if the code ran without any errors.</summary>
        public Response Value { get { return _action; } }

        private static Response<Action<Exception>> _logger;
        private readonly Later<Response> _action;

        /// <summary>Wraps a function in error protecting code.</summary>
        public Try(Action action)
        {
            if (action == null)
                throw new ArgumentNullException(nameof(action));

            var logger = GetExceptionLogger();
            _action = Later.Create(() => PaddedCage(action, ExceptionLogger.Create(logger)));
        }

        private static Response PaddedCage(Action action, ExceptionLogger exceptionLogger)
        {
            var response = new Response();

            try
            {
                action();
                response = response.AsValid();
            }
            catch (Exception ex)
            {
                exceptionLogger.Log(ex);
            }

            return response;
        }

        public static Try<T> Create<T>(Func<T> func) => new Try<T>(func);

        public static Try Create(Action action) => new Try(action);

        public static TryAsync<T> CreateAsync<T>(Func<Task<T>> func) => new TryAsync<T>(func);

        public static TryAsync CreateAsync(Func<Task> func) => new TryAsync(func);

        public static implicit operator Response(Try action) => action.Value;

        public static implicit operator bool(Try action) => action.Value.IsValid;

        internal static Response<Action<Exception>> GetExceptionLogger() => _logger;

        /// <summary>Removes any logger that was previously set.</summary>
        public static void RemoveExceptionLogger() => _logger = Response.Create<Action<Exception>>();

        /// <summary>If you'd like to log errors as they come, add your stateless error logger here, if a logger already exists, it'll be overwritten.</summary>
        public static void SetExceptionLogger(Action<Exception> logger)
        {
            if (logger == null)
                throw new ArgumentNullException(nameof(logger));

            _logger = Response.Create(logger);
        }
    }
}
