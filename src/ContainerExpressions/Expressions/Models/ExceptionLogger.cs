using ContainerExpressions.Containers;
using System;
using System.Diagnostics;

namespace ContainerExpressions.Expressions.Models
{
    internal readonly struct ExceptionLogger
    {
        private readonly Response<Action<Exception>> _logger;

        public ExceptionLogger(Response<Action<Exception>> logger)
        {
            _logger = logger;
        }

        public void Log(Exception ex)
        {
            if (_logger.IsValid)
            {
                try
                {
                    _logger.Value(ex);
                }
                catch (Exception logError)
                {
                    Debug.WriteLine("Error logging an exception in the Try Container.");
                    Debug.WriteLine(logError);
                    Debug.WriteLine("The original error that was attempting to be logged is below.");
                    Debug.WriteLine(ex);
                }
            }
        }

        public static ExceptionLogger Create(Response<Action<Exception>> logger) => new ExceptionLogger(logger);
    }
}
