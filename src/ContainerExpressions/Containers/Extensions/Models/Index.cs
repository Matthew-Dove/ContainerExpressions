namespace ContainerExpressions.Containers
{
    /// <summary>The current element's index.</summary>
    public readonly struct Index
    {
        public A<int> Value { get; }
        public Index(int value) { Value = new(value); }

        public static implicit operator A<int>(Index alias) => alias.Value;
        public static implicit operator Index(A<int> value) => new(value.Value);
        public static implicit operator int(Index alias) => alias.Value.Value;
        public static implicit operator Index(int value) => new(value);

        public override string ToString() => Value.ToString();
    }
}
