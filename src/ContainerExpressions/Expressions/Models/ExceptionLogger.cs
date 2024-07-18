using ContainerExpressions.Containers;
using System;

namespace ContainerExpressions.Expressions.Models
{
    internal readonly struct ExceptionLogger
    {
        private readonly Action<Exception> _logger;
        private readonly Action<Exception, string, object[]> _formattedLogger;
        private readonly Format _template;
        private readonly string _argument;
        private readonly string _caller;
        private readonly string _path;
        private readonly int _line;

        public ExceptionLogger(
            Format template,
            string argument,
            string caller,
            string path,
            int line
            )
        {
            _logger = Try.Logger;
            _formattedLogger = Try.FormattedLogger;
            _template = template;
            _argument = argument;
            _caller = caller;
            _path = path;
            _line = line;
        }

        public void Log(Exception ex)
        {
            if (ex == null) return;

            try
            {
                if (_formattedLogger != null)
                {
                    var template = _template;
                    if (Format.Default.Equals(template) || string.IsNullOrEmpty(template.Message))
                    {
                        template = new Format("{ExceptionMessage}", ex.Message);
                    }
                    ex.AddCallerAttributes(_argument, _caller, _path, _line, Format.Default);
                    _formattedLogger(ex, template.Message, template.Args);
                }
                else if (_logger != null)
                {
                    ex.AddCallerAttributes(_argument, _caller, _path, _line, _template);
                    _logger(ex);
                }
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

        public static ExceptionLogger Create(
            Format template,
            string argument,
            string caller,
            string path,
            int line
            ) => new (
                template,
                argument,
                caller,
                path,
                line
            );
    }
}
