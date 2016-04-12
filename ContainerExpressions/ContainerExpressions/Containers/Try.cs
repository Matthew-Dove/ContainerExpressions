using ContainerExpressions.Expressions.Models;
using System;

namespace ContainerExpressions.Containers
{
    public struct Try<T>
    {
        public Response<T> Value { get { return _func; } }
        private readonly Later<Response<T>> _func;

        public Try(Func<T> func)
        {
            if (func == null)
                throw new ArgumentNullException(nameof(func));

            var exceptionLogger = Try.GetExceptionLogger(); // Grabs the current Exception Logger set at the time this instance is created (incase it's changed before the value is read).
            _func = Later.Create(() => PaddedCage(func, ExceptionLogger.Create(exceptionLogger)));
        }

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
    }

    public struct Try
    {
        public Response Value { get { return _action; } }

        private static Response<Action<Exception>> _logger;
        private readonly Later<Response> _action;

        public Try(Action action)
        {
            if (action == null)
                throw new ArgumentNullException(nameof(action));

            var logger = GetExceptionLogger(); // Grabs the current Exception Logger set at the time this instance is created (incase it's changed before the value is read).
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

        public static implicit operator Response(Try action) => action.Value;

        internal static Response<Action<Exception>> GetExceptionLogger() => _logger;

        public static void RemoveExceptionLogger() => _logger = Response.Create<Action<Exception>>();

        public static void SetExceptionLogger(Action<Exception> logger)
        {
            if (logger == null)
                throw new ArgumentNullException(nameof(logger));

            _logger = Response.Create(logger);
        }
    }
}
