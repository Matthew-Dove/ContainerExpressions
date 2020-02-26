using ContainerExpressions.Containers;
using System;

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
                catch (AggregateException ae)
                {
                    Console.WriteLine("Error logging an exception in the Try Container.");
                    foreach (var e in ae.Flatten().InnerExceptions)
                    {
                        Console.WriteLine(e);
                    }
                    Console.WriteLine("The original error that was attempting to be logged is below.");
                    Console.WriteLine(ex);
                }
                catch (Exception logError)
                {
                    Console.WriteLine("Error logging an exception in the Try Container.");
                    Console.WriteLine(logError);
                    Console.WriteLine("The original error that was attempting to be logged is below.");
                    Console.WriteLine(ex);
                }
            }
        }

        public static ExceptionLogger Create(Response<Action<Exception>> logger) => new ExceptionLogger(logger);
    }
}
