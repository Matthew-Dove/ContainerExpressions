namespace ContainerExpressions.Containers
{
    /// <summary>The total number of elements in the collection.</summary>
    public readonly struct Length
    {
        public A<int> Value { get; }
        internal Length(int value) { Value = new(value); }

        public static implicit operator int(Length alias) => alias.Value.Value;

        public override string ToString() => Value.ToString();

        /// <summary>Attempts to retrieve a cached result, otherwise a new one is created.</summary>
        internal static Length From(int result)
        {
            if (result >= LengthCache.InclusiveMin && result < LengthCache.ExclusiveMax) return LengthCache.Cache[result];
            return new Length(result);
        }
    }

    internal static class LengthCache
    {
        public const int InclusiveMin = 0;
        public const int ExclusiveMax = 10;

        public static readonly Length[] Cache = GenerateCache();

        private static Length[] GenerateCache()
        {
            var cache = new Length[ExclusiveMax];
            for (int i = 0; i < cache.Length; i++)
            {
                cache[i] = new Length(i);
            }
            return cache;
        }
    }
}
