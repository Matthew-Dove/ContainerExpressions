using ContainerExpressions.Containers.Internal;
using System;
using System.Text.RegularExpressions;

namespace ContainerExpressions.Containers
{
    /// <summary>Logs the result, and output of Response types.</summary>
    public static class Trace
    {
        private static Action<string> _logger = null;
        private static Action<string, object[]> _formattedLogger = null;

        internal static void Log(Format format) => Log(format.Message, format.Args);

        private static void Log(string message, params object[] args)
        {
            if (_formattedLogger != null)
            {
                _formattedLogger(message, args);
            }
            else if (_logger != null)
            {
                if (args != null && args.Length != 0)
                {
                    message = FormatHelper.NamedPlaceholders(message, args);
                }
                _logger(message);
            }
        }

        /// <summary>
        /// Set your desired logger implementation here.
        /// <para>It is recommend that the logger be stateless.</para>
        /// <para>Only one logger instance is used at a time, the formatted logger takes precedence.</para>
        /// </summary>
        /// <param name="logger">A function that will log the incoming message.</param>
        public static void SetLogger(Action<string> logger)
        {
            if (logger == null) Throw.ArgumentNullException(nameof(logger));

            _logger = logger;
        }

        /// <summary>
        /// Formats and writes a log message.
        /// <para>First argument is the message - format string of the log message in message template format: i.e. "User {User} logged in!"</para>
        /// <para>Second argument is the message args - an object array that contains zero or more objects to format.</para>
        /// <para>Only one logger instance is used at a time, the formatted logger takes precedence.</para>
        /// </summary>
        public static void SetFormattedLogger(Action<string, object[]> formattedLogger)
        {
            if (formattedLogger == null) Throw.ArgumentNullException(nameof(formattedLogger));

            _formattedLogger = formattedLogger;
        }
    }

    internal static class FormatHelper
    {
        private const string LEN_MISMATCH_ERR = "The number of args ({0}) does not match the number of parameters ({1}) in the message template.";

        public static string NamedPlaceholders(string template, params object[] args)
        {
            var regex = new Regex(@"{(\w+)}");
            var matches = regex.Matches(template);

            if (matches.Count != args.Length) Throw.ArgumentOutOfRangeException(nameof(args), string.Format(LEN_MISMATCH_ERR, args.Length, matches.Count));

            for (int i = 0; i < args.Length; i++)
            {
                var target = matches[i].Value;
                var value = args[i].ToString();
                template = template.Replace(target, value);
            }

            return template;
        }
    }
}
