using ContainerExpressions.Containers.Internal;
using System;

namespace ContainerExpressions.Containers
{
    /// <summary>Logs the result, and output of Response types.</summary>
    public readonly struct Trace
    {
        private static Action<string> _logger = x => { };

        /// <summary>
        /// Set your desired logger implementation here.
        /// <para>It is recommend that the logger be stateless.</para>
        /// </summary>
        /// <param name="logger">A function that will log the incoming message.</param>
        public static void SetLogger(Action<string> logger)
        {
            if (logger == null)
                Throw.ArgumentNullException(nameof(logger));

            _logger = logger;
        }

        internal static void Log(string message) => _logger(message);
    }
}
