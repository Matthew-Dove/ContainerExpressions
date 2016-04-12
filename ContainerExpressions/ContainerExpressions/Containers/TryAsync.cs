using ContainerExpressions.Expressions.Models;
using System;
using System.Threading.Tasks;

namespace ContainerExpressions.Containers
{
    public struct TryAsync<T>
    {
        /// <summary>Runs the function the first time this is accessed, returns a valid Response if the code ran without any errors.</summary>
        public Task<Response<T>> Value { get { return _func; } }
        private readonly Later<Task<Response<T>>> _func;

        /// <summary>Wraps a function in error protecting code.</summary>
        public TryAsync(Func<Task<T>> func)
        {
            if (func == null)
                throw new ArgumentNullException(nameof(func));

            var exceptionLogger = Try.GetExceptionLogger(); // Grabs the current Exception Logger set at the time this instance is created (incase it's changed before the value is read).
            _func = Later.Create(() => PaddedCage(func, ExceptionLogger.Create(exceptionLogger)));
        }

        /// <summary>Run the code in a safe manner, optionally logging any errors as they occur.</summary>
        private static async Task<Response<T>> PaddedCage(Func<Task<T>> func, ExceptionLogger exceptionLogger)
        {
            var response = new Response<T>();

            try
            {
                var result = await func();
                response = response.WithValue(result);
            }
            catch (Exception ex)
            {
                exceptionLogger.Log(ex);
            }

            return response;
        }

        public static implicit operator Task<Response<T>>(TryAsync<T> func) => func.Value;
    }

    /// <summary>Wraps a function in error protecting code.</summary>
    public struct TryAsync
    {
        /// <summary>Runs the function the first time this is accessed, returns a valid Response if the code ran without any errors.</summary>
        public Task<Response> Value { get { return _func; } }

        private readonly Later<Task<Response>> _func;

        /// <summary>Wraps a function in error protecting code.</summary>
        public TryAsync(Func<Task> action)
        {
            if (action == null)
                throw new ArgumentNullException(nameof(action));

            var logger = Try.GetExceptionLogger();
            _func = Later.Create(() => PaddedCage(action, ExceptionLogger.Create(logger)));
        }

        private static async Task<Response> PaddedCage(Func<Task> func, ExceptionLogger exceptionLogger)
        {
            var response = new Response();

            try
            {
                await func();
                response = response.AsValid();
            }
            catch (Exception ex)
            {
                exceptionLogger.Log(ex);
            }

            return response;
        }

        public static implicit operator Task<Response>(TryAsync func) => func.Value;
    }
}
