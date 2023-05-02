using System;

namespace ContainerExpressions.Containers
{
    public sealed class Csv : Alias<string[]> {
        public Csv(string names) : base(names.Split(Instance.Of<Comma>())) { }
        private sealed class Comma : Alias<char[]> { public Comma() : base(new char[] { ',' }) { } }
        static Csv() => Instance.Create(new Comma());
    }

    // Options
    public readonly struct SmartEnum<T>
    {
        private readonly int _bitmask;

        public SmartEnum(int bitmask)
        {
            _bitmask = default;
        }

        public SmartEnum(Enum value)
        {

        }

        public SmartEnum(string name)
        {
            
        }

        public SmartEnum(Csv names) : this(names.Value) { }

        public SmartEnum(string[] names)
        {

        }
    }
}
