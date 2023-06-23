using ContainerExpressions.Containers;
using System;

namespace ContainerExpressions.Expressions.Models
{
    internal readonly struct ExceptionLogger
    {
        private readonly Response<Action<Exception>> _logger;
        private readonly string _argument;
        private readonly string _caller;
        private readonly string _path;
        private readonly int _line;

        public ExceptionLogger(
            Response<Action<Exception>> logger,
            string argument = "",
            string caller = "",
            string path = "",
            int line = 0
            )
        {
            _logger = logger;
            _argument = argument;
            _caller = caller;
            _path = path;
            _line = line;
        }

        public void Log(Exception ex)
        {
            if (_logger.IsValid && ex != null)
            {
                try
                {
                    ex.AddCallerAttributes(ex.Message, _argument, _caller, _path, _line);
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

        public static ExceptionLogger Create(
            Response<Action<Exception>> logger,
            string argument = "",
            string caller = "",
            string path = "",
            int line = 0
            ) => new (
                logger,
                argument,
                caller,
                path,
                line
            );
    }
}
