using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace ContainerExpressions.Containers
{
    public sealed class Loop<T> : IEnumerable<T>
    {
        public T this[int index] { get => _items[index]; }
        public int Length { get => _items.Length; }

        private readonly T[] _items;
        private int _index;

        public Loop(T[] items)
        {
            _items = items ?? Array.Empty<T>();
        }

        public Loop<T> ForEach(Action<T> action)
        {
            if (_items.Length == 0) return this;
            var span = new Span<T>(_items);
            ref var source = ref MemoryMarshal.GetReference<T>(span);
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

        public Loop<T> Seek(int index = 0) { _index = index; return this; }

        public IEnumerator<T> GetEnumerator() => ((IEnumerable<T>)_items).GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => _items.GetEnumerator();

        public static Loop<T> operator ++(Loop<T> loop) { loop._index++; return loop; }
        public static Loop<T> operator +(Loop<T> loop, int offset) { loop._index += offset; return loop; }
        public static Loop<T> operator +(int offset, Loop<T> loop) { loop._index += offset; return loop; }

        public static Loop<T> operator --(Loop<T> loop) { loop._index--; return loop; }
        public static Loop<T> operator -(Loop<T> loop, int offset) { loop._index -= offset; return loop; }
        public static Loop<T> operator -(int offset, Loop<T> loop) { loop._index -= offset; return loop; }

        public static implicit operator bool(Loop<T> loop) { return ++loop._index < loop._items.Length; } // ++
        public static implicit operator T(Loop<T> loop) => loop._items[loop._index];
        public static implicit operator int(Loop<T> loop) => loop._index;
    }
}
