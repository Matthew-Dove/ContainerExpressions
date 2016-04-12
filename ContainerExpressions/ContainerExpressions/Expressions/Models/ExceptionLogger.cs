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

        public void Log(Exception ex)
        {
            if (_logger)
            {
                try
                {
                    _logger.Value(ex);
                }
                catch
                {

                }
            }
        }

        public static ExceptionLogger Create(Response<Action<Exception>> logger) => new ExceptionLogger(logger);
    }
}
