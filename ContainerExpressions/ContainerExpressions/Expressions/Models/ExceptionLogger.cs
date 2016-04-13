using ContainerExpressions.Containers;
using System;

namespace ContainerExpressions.Expressions.Models
{
    internal struct ExceptionLogger
    {
        private readonly Response<Action<Exception>> _logger;

        public ExceptionLogger(Response<Action<Exception>> logger)
        {
            _logger = logger;
        }

        public void Log(Exception ex) => _logger.Bind(ex, RunLogger);

        private static Response RunLogger(Exception ex, Action<Exception> logger)
        {
            try
            {
                logger(ex);
            }
            catch
            {

            }

            return new Response(true);
        }

        public static ExceptionLogger Create(Response<Action<Exception>> logger) => new ExceptionLogger(logger);
    }
}
