using System;
using System.Collections;
using System.Collections.Generic;

namespace ContainerExpressions.Containers
{
    public sealed class Loop<T> : IEnumerable<T>
    {
        public T this[int index] { get => _items[index]; }
        public int Length { get => _items.Length; }

        internal readonly T[] _items;
        internal int _index;

        public Loop(T[] items)
        {
            _items = items ?? Array.Empty<T>();
        }

        public IEnumerator<T> GetEnumerator() => ((IEnumerable<T>)_items).GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => _items.GetEnumerator();

        public static Loop<T> operator ++(Loop<T> loop) { loop._index++; return loop; }
        public static Loop<T> operator +(Loop<T> loop, int offset) { loop._index += offset; return loop; }
        public static Loop<T> operator +(int offset, Loop<T> loop) { loop._index += offset; return loop; }

        public static Loop<T> operator --(Loop<T> loop) { loop._index--; return loop; }
        public static Loop<T> operator -(Loop<T> loop, int offset) { loop._index -= offset; return loop; }
        public static Loop<T> operator -(int offset, Loop<T> loop) { loop._index -= offset; return loop; }

        public static implicit operator bool(Loop<T> loop) { return ++loop._index < loop._items.Length; }
        public static implicit operator T(Loop<T> loop) => loop._items[loop._index];
        public static implicit operator int(Loop<T> loop) => loop._index;
    }

    public sealed class Loop
    {
        private Loop() { }

        public static Loop<T> Create<T>(T[] items) => new Loop<T>(items);
    }
}
