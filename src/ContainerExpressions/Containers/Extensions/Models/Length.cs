namespace ContainerExpressions.Containers
{
    /// <summary>The total number of elements in the collection.</summary>
    public readonly struct Length
    {
        public A<int> Value { get; }
        public Length(int value) { Value = new(value); }

        public static implicit operator A<int>(Length alias) => alias.Value;
        public static implicit operator Length(A<int> value) => new(value.Value);
        public static implicit operator int(Length alias) => alias.Value.Value;
        public static implicit operator Length(int value) => new(value);

        public override string ToString() => Value.ToString();
    }
}
