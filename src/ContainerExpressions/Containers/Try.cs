using ContainerExpressions.Expressions.Models;
using System;
using System.Threading.Tasks;

namespace ContainerExpressions.Containers
{
    /// <summary>Wraps a function in error protecting code.</summary>
    public static class Try
    {
        /// <summary>If you'd like to log errors as they come, add your stateless error logger here, if a logger already exists, it'll be overwritten.</summary>
        public static void SetExceptionLogger(Action<Exception> logger)
        {
            if (logger == null)
                throw new ArgumentNullException(nameof(logger));

            _logger = Response.Create(logger);
        }

        internal static Response<Action<Exception>> GetExceptionLogger() => _logger;

        private static Response<Action<Exception>> _logger = new Response<Action<Exception>>();

        /// <summary>Wraps a function in error protecting code.</summary>
        public static Response Run(Action action)
        {
            if (action == null)
                throw new ArgumentNullException(nameof(action));

            var logger = GetExceptionLogger();
            return PaddedCage(action, ExceptionLogger.Create(logger));
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

        /// <summary>Wraps a function in error protecting code.</summary>
        public static Response<T> Run<T>(Func<T> func)
        {
            if (func == null)
                throw new ArgumentNullException(nameof(func));

            var exceptionLogger = GetExceptionLogger();
            return PaddedCage(func, ExceptionLogger.Create(exceptionLogger));
        }

        private static Response<T> PaddedCage<T>(Func<T> func, ExceptionLogger exceptionLogger)
        {
            var response = new Response<T>();

            try
            {
                var result = func();
                response = response.With(result);
            }
            catch (Exception ex)
            {
                exceptionLogger.Log(ex);
            }

            return response;
        }

        /// <summary>Wraps a function in error protecting code.</summary>
        public static Task<Response> RunAsync(Func<Task> action)
        {
            if (action == null)
                throw new ArgumentNullException(nameof(action));

            var logger = Try.GetExceptionLogger();
            return PaddedCageAsync(action, ExceptionLogger.Create(logger));
        }

        private static async Task<Response> PaddedCageAsync(Func<Task> func, ExceptionLogger exceptionLogger)
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
                    exceptionLogger.Log(e);
                }
            }
            catch (Exception ex)
            {
                exceptionLogger.Log(ex);
            }

            return response;
        }

        /// <summary>Wraps a function in error protecting code.</summary>
        public static Task<Response<T>> RunAsync<T>(Func<Task<T>> func)
        {
            if (func == null)
                throw new ArgumentNullException(nameof(func));

            var exceptionLogger = Try.GetExceptionLogger();
            return PaddedCageAsync(func, ExceptionLogger.Create(exceptionLogger));
        }

        private static async Task<Response<T>> PaddedCageAsync<T>(Func<Task<T>> func, ExceptionLogger exceptionLogger)
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
                    exceptionLogger.Log(e);
                }
            }
            catch (Exception ex)
            {
                exceptionLogger.Log(ex);
            }

            return response;
        }
    }
}
