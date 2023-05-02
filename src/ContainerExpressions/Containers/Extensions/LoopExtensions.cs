using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System;

namespace ContainerExpressions.Containers
{
    public static class LoopExtensions
    {
        public static T[] ToArray<T>(this Loop<T> loop) => loop._items;

        public static Loop<T> Seek<T>(this Loop<T> loop, int index = 0) { loop._index = index; return loop; }

        public static Loop<T> ForEach<T>(this Loop<T> loop, Action<T> action)
        {
            if (loop._items.Length == 0) return loop;
            var span = new ReadOnlySpan<T>(loop._items);
            ref var source = ref MemoryMarshal.GetReference<T>(span);
            for (var offset = 0; offset < span.Length; offset++)
            {
                var item = Unsafe.Add(ref source, offset);
                action(item);
            }
            return loop;
        }

        public static Loop<TResult> Transform<TSource, TResult>(this Loop<TSource> loop, Func<TSource, TResult> map)
        {
            if (loop._items.Length == 0) return new Loop<TResult>(Array.Empty<TResult>());
            var items = new TResult[loop.Length];
            var span = new ReadOnlySpan<TSource>(loop._items);
            ref var source = ref MemoryMarshal.GetReference<TSource>(span);
            for (var offset = 0; offset < span.Length; offset++)
            {
                var item = Unsafe.Add(ref source, offset);
                items[offset] = map(item);
            }
            return new Loop<TResult>(items);
        }

        public static TResult Flattern<TSource, TResult>(this Loop<TSource> loop, TResult identity, Func<TSource, TResult, TResult> accumulate)
        {
            if (loop._items.Length == 0) return default;
            var aggregation = identity;
            var span = new ReadOnlySpan<TSource>(loop._items);
            ref var source = ref MemoryMarshal.GetReference<TSource>(span);
            for (var offset = 0; offset < span.Length; offset++)
            {
                var item = Unsafe.Add(ref source, offset);
                aggregation = accumulate(item, aggregation);
            }
            return aggregation;
        }

        // Filter<T>(Func<T, bool> predicate) | SelectMany | Concat (Another Loop, or an array) | Distinct<T>(IEqualityComparer<T> compare)
    }
}
