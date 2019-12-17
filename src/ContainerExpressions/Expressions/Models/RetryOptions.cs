using System;

namespace ContainerExpressions.Expressions.Models
{
    /// <summary>Function replay, and cooldown options.</summary>
    public readonly struct RetryOptions
    {
        /// <summary>The number of times to retry the function.</summary>
        public int Retries { get; }

        /// <summary>The amount of time to wait between calling the function again.</summary>
        public int MillisecondsDelay { get; }

        /// <summary>Function replay, and cooldown options.</summary>
        /// <param name="retries">The number of retries to make before giving up.</param>
        /// <param name="millisecondsDelay">The time to wait in milliseconds before trying another attempt.</param>
        public RetryOptions(int retries, int millisecondsDelay)
        {
            if (retries < 1)
                throw new ArgumentOutOfRangeException(nameof(retries), "Must be greater than 0.");
            if (retries < 0)
                throw new ArgumentOutOfRangeException(nameof(millisecondsDelay), "Cannot be negative.");

            Retries = retries;
            MillisecondsDelay = millisecondsDelay;
        }

        /// <summary>Creates RetryOptions with sensible default values.</summary>
        public static RetryOptions Create() => new RetryOptions(1, 100);

        /// <summary>>Creates RetryOptions with custom values</summary>
        /// <param name="retries">The number of times to retry the function.</param>
        /// <param name="millisecondsDelay">The amount of time to wait between calling the function again</param>
        public static RetryOptions Create(int retries, int millisecondsDelay) => new RetryOptions(retries, millisecondsDelay);
    }
}
