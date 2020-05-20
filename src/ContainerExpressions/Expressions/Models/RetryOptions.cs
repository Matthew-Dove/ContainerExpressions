using System;

namespace ContainerExpressions.Expressions.Models
{
    /// <summary>Function replay, and cooldown options.</summary>
    public readonly struct RetryOptions
    {
        /// <summary>The number of attempts the default exponential retry strategy will make before giving up.</summary>
        public const int DEFAULT_EXPONENTIAL_RETRIES = 5;

        /// <summary>The number of times to retry the function.</summary>
        public int Retries { get; }

        /// <summary>The amount of time to wait between calling the function again.</summary>
        public int MillisecondsDelay { get; }

        private readonly Func<int, int> _getMillisecondsDelay;
        private readonly static Func<int, int> _exponential = x => (int)Math.Pow(2, x - 1) * 100;

        /// <summary>Function replay, and cooldown options.</summary>
        /// <param name="retries">The number of retries to make before giving up.</param>
        /// <param name="millisecondsDelay">The time to wait in milliseconds before trying another attempt.</param>
        public RetryOptions(int retries, int millisecondsDelay)
        {
            if (retries < 1)
                throw new ArgumentOutOfRangeException(nameof(retries), "Must be greater than 0.");
            if (millisecondsDelay < 0)
                throw new ArgumentOutOfRangeException(nameof(millisecondsDelay), "Cannot be negative.");

            Retries = retries;
            MillisecondsDelay = millisecondsDelay;
            _getMillisecondsDelay = _ => millisecondsDelay;
        }

        /// <summary>Function replay, and cooldown options.</summary>
        /// <param name="retries">The number of retries to make before giving up.</param>
        /// <param name="getMillisecondsDelay">A function that takes the current retry attempt [1 - n], and returns the time to wait in milliseconds before trying again.</param>
        public RetryOptions(int retries, Func<int, int> getMillisecondsDelay)
        {
            if (retries < 1)
                throw new ArgumentOutOfRangeException(nameof(retries), "Must be greater than 0.");
            if (getMillisecondsDelay == null)
                throw new ArgumentNullException(nameof(getMillisecondsDelay));

            Retries = retries;
            MillisecondsDelay = 0;
            _getMillisecondsDelay = getMillisecondsDelay;
        }

        /// <summary>Gets the amount of milliseconds to wait until the next retry attempt.</summary>
        /// <param name="retryAttempt">The current number of retries [1 - n].</param>
        public int GetMillisecondsDelay(int retryAttempt)
        {
            if (retryAttempt < 1)
                throw new ArgumentOutOfRangeException(nameof(retryAttempt), "Must be greater than 0.");

            var millisecondsDelay = _getMillisecondsDelay(retryAttempt);

            if (millisecondsDelay < 0)
                throw new ArgumentOutOfRangeException(nameof(millisecondsDelay), "Cannot be negative.");

            return millisecondsDelay;
        }

        /// <summary>Creates RetryOptions with sensible default values.</summary>
        public static RetryOptions Create() => Create(1, 100);

        /// <summary>>Creates RetryOptions with custom values</summary>
        /// <param name="retries">The number of times to retry the function.</param>
        /// <param name="millisecondsDelay">The amount of time to wait between calling the function again</param>
        public static RetryOptions Create(int retries, int millisecondsDelay) => new RetryOptions(retries, millisecondsDelay);

        /// <summary>Defines a retry strategy with an exponential backoff, and sensible default values.</summary>
        public static RetryOptions CreateExponential() => CreateExponential(DEFAULT_EXPONENTIAL_RETRIES);

        /// <summary>
        /// Defines a retry strategy with an exponential backoff: (2 ^ retries * 100) milliseconds.
        /// <list type="number">
        ///     <item>
        ///         <description>100</description>
        ///     </item>
        ///     <item>
        ///         <description>200</description>
        ///     </item>
        ///     <item>
        ///         <description>400</description>
        ///     </item>
        ///     <item>
        ///         <description>800</description>
        ///     </item>
        ///     <item>
        ///         <description>1600</description>
        ///     </item>
        /// </list>
        /// </summary>
        /// <param name="retries">The number of attempts to make until the Response is in a valid state, before giving up.</param>
        public static RetryOptions CreateExponential(int retries) => new RetryOptions(retries, _exponential);

        /// <summary>Function replay, and cooldown options.</summary>
        /// <param name="retries">The number of retries to make before giving up.</param>
        /// <param name="getMillisecondsDelay">A function that takes the current retry attempt [1 - n], and returns the time to wait in milliseconds before trying again.</param>
        public static RetryOptions CreateExponential(int retries, Func<int, int> getMillisecondsDelay) => new RetryOptions(retries, getMillisecondsDelay);
    }
}
