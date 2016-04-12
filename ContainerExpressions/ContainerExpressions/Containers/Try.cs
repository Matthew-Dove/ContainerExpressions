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

            _func = Later.Create(() => PaddedCage(func, Try.GetExceptionLogger()));
        }

        private static Response<T> PaddedCage(Func<T> func, Response<Action<Exception>> logger)
        {
            var response = new Response<T>();

            try
            {
                var result = func();
                response = response.WithValue(result);
            }
            catch (Exception ex)
            {
                if (logger)
                {
                    Log(logger, ex);
                }
            }

            return response;
        }

        private static void Log(Action<Exception> logger, Exception ex)
        {
            try
            {
                logger(ex);
            }
            catch
            {

            }
        }

        public static implicit operator Response<T>(Try<T> func) => func;
    }

    public static class Try
    {
        internal static Response<Action<Exception>> GetExceptionLogger() => _logger;

        public static void SetExceptionLogger(Action<Exception> logger)
        {
            if (logger == null)
                throw new ArgumentNullException(nameof(logger));

            _logger = Response.Create(logger);
        }

        public static void RemoveExceptionLogger()
        {
            _logger = Response.Create<Action<Exception>>();
        }

        private static Response<Action<Exception>> _logger;

        public static Try<T> Create<T>(Func<T> func) => new Try<T>(func);
    }
}
