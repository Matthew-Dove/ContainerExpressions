using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace ContainerExpressions.Containers
{
    public sealed class Loop<T> : IEnumerable<T>, IEnumerator<T>
    {
        public T this[int index] { get => _items[index]; }
        public int Length { get => _items.Length; }

        public T Current => _items[_index.Value];
        object IEnumerator.Current => _items[_index.Value];

        private const int IX = -1;
        private readonly T[] _items;
        private int? _index;

        public Loop(T[] items)
        {
            Reset();
            _items = items ?? Array.Empty<T>();
        }

        public Loop<T> ForEach(Action<T> action)
        {
            if (_items.Length == 0) return this;
            var span = new Span<T>(_items);
            ref var source = ref MemoryMarshal.GetReference(span);
            for (var offset = 0; offset < span.Length; offset++)
            {
                var item = Unsafe.Add(ref source, offset);
                action(item);
            }
            return this;
        }

        public Loop<U> Transform<U>(Func<T, U> func)
        {
            if (_items.Length == 0) return new Loop<U>(Array.Empty<U>());
            var items = new U[_items.Length];
            var span = new Span<T>(_items);
            ref var source = ref MemoryMarshal.GetReference(span);
            for (var offset = 0; offset < span.Length; offset++)
            {
                var item = Unsafe.Add(ref source, offset);
                items[offset] = func(item);
            }
            return new Loop<U>(items);
        }

        public Loop<T> Seek(int index = IX) { _index = index; if (index == IX) Reset(); return this; }

        public IEnumerator<T> GetEnumerator() { Reset(); return this; }
        IEnumerator IEnumerable.GetEnumerator() { Reset(); return this; }

        public void Dispose() { }
        public void Reset() => _index = null;
        public bool MoveNext()
        {
            if (!_index.HasValue) _index = IX;
            var hasNext = ++_index < _items.Length;
            if (!hasNext) Reset();
            return hasNext;
        }

        public static Loop<T> operator ++(Loop<T> loop) { if (loop._index.GetValueOrDefault(IX) == IX) loop._index = 0; loop._index++; return loop; }
        public static Loop<T> operator +(Loop<T> loop, int offset) { if (loop._index.GetValueOrDefault(IX) == IX) loop._index = 0; loop._index += offset; return loop; }
        public static Loop<T> operator +(int offset, Loop<T> loop) { if (loop._index.GetValueOrDefault(IX) == IX) loop._index = 0; loop._index += offset; return loop; }

        public static Loop<T> operator --(Loop<T> loop) { if (loop._index.GetValueOrDefault(IX) == IX) loop._index = 0; loop._index--; return loop; }
        public static Loop<T> operator -(Loop<T> loop, int offset) { if (loop._index.GetValueOrDefault(IX) == IX) loop._index = 0; loop._index -= offset; return loop; }
        public static Loop<T> operator -(int offset, Loop<T> loop) { if (loop._index.GetValueOrDefault(IX) == IX) loop._index = 0; loop._index -= offset; return loop; }

        public static implicit operator bool(Loop<T> loop) { if (!loop._index.HasValue) loop._index = IX; return ++loop._index < loop._items.Length; }
        public static implicit operator T(Loop<T> loop) => loop._items[loop._index.GetValueOrDefault(0)];
        public static implicit operator int(Loop<T> loop) => loop._index.GetValueOrDefault(0);
    }
}
