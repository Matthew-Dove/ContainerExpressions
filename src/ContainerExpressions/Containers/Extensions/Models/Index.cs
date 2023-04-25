namespace ContainerExpressions.Containers
{
    /// <summary>The current element's index.</summary>
    public readonly struct Index
    {
        public A<int> Value { get; }
        internal Index(int value) { Value = new(value); }

        public static implicit operator int(Index alias) => alias.Value.Value;

        public override string ToString() => Value.ToString();

        /// <summary>Attempts to retrieve a cached result, otherwise a new one is created.</summary>
        internal static Index From(int result)
        {
            if (result >= IndexCache.InclusiveMin && result < IndexCache.ExclusiveMax) return IndexCache.Cache[result];
            return new Index(result);
        }
    }

    internal static class IndexCache
    {
        public const int InclusiveMin = 0;
        public const int ExclusiveMax = 10;

        public static readonly Index[] Cache = GenerateCache();

        private static Index[] GenerateCache()
        {
            var cache = new Index[ExclusiveMax];
            for (int i = 0; i < cache.Length; i++)
            {
                cache[i] = new Index(i);
            }
            return cache;
        }
    }
}
