using System;
using System.Diagnostics;

namespace ContainerExpressions.Containers
{
    /// <summary>Logs the result, and output of Response types.</summary>
    public struct Trace
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
                throw new ArgumentNullException(nameof(logger));

            _logger = logger;
        }

        internal static void Log(string message)
        {
            var log = _logger;

            try
            {
                log(message);
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Error in the Log Container calling the custom log function.");
                Debug.WriteLine(ex);
            }
        }
    }
}
