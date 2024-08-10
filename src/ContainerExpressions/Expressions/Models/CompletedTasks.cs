using ContainerExpressions.Containers;
using System;

namespace ContainerExpressions.Expressions.Models
{
    /// <summary>
    /// The number of Tasks awaited.
    /// <para>Note: this may be different to the number of tasks pushed, as they remove themsevles once they have finished executing.</para>
    /// </summary>
    public readonly struct CompletedTasks : IEquatable<int>
    {
        public ValueAlias<int> Value { get; }
        public CompletedTasks(int value) { Value = new(value); }
        public static implicit operator int(CompletedTasks alias) => alias.Value.Value;
        public static implicit operator CompletedTasks(int value) => new CompletedTasks(value);
        public override string ToString() => Value.ToString();
        public bool Equals(int other) => Value.Equals(other);
        public override int GetHashCode() => Value.GetHashCode();
        public override bool Equals(object obj) => (obj is CompletedTasks ct && Equals(ct.Value.Value)) || (obj is int i && Equals(i));
    }
}
