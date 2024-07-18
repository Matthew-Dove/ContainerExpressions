using System;
using System.Collections.Concurrent;

namespace ContainerExpressions.Containers
{
    public static class TagExtensions
    {
        public static Tag<TReference> Tag<TReference>(this TReference reference)
        {
            return new Tag<TReference>(reference);
        }

        public static TagReference<TReference> TagReference<TReference>(this TReference reference) where TReference : class
        {
            return new TagReference<TReference>(reference);
        }
    }

    public readonly struct Tag<TReference>
    {
        private readonly TReference _reference;

        public Tag(TReference reference)
        {
            _reference = reference;
        }

        public TReference Set<TMessage>(TMessage message)
        {
            TagBase.Set(_reference, message);
            return _reference;
        }

        public Response<TMessage> Get<TMessage>()
        {
            return TagBase.Get(_reference, default(TMessage));
        }
    }

    public readonly struct TagReference<TReference> where TReference : class
    {
        private readonly TReference _reference;

        public TagReference(TReference reference)
        {
            _reference = reference;
        }

        public TReference Set<TMessage>(TMessage message)
        {
            TagBase.SetReference(_reference, message);
            return _reference;
        }

        public Response<TMessage> Get<TMessage>()
        {
            return TagBase.GetReference(_reference, default(TMessage));
        }
    }

    internal sealed class TagCache<TReference, TMessage>
    {
        private static readonly ConcurrentDictionary<TReference, TMessage> _dic = new();

        public static void Set(TReference reference, TMessage message)
        {
            _dic.TryAdd(reference, message);
        }

        public static Response<TMessage> Get(TReference reference)
        {
            var result = _dic.TryRemove(reference, out TMessage message);
            return result ? Response.Create(message) : Response<TMessage>.Error;
        }
    }

    internal abstract class TagBase
    {
        public static void Set<TReference, TMessage>(TReference reference, TMessage message)
        {
            TagCache<TReference, TMessage>.Set(reference, message);
        }

        public static Response<TMessage> Get<TReference, TMessage>(TReference reference, TMessage message)
        {
            return TagCache<TReference, TMessage>.Get(reference);
        }

        private static long GetUnsafeReference<TReference>(TReference reference) where TReference : class
        {
            long address;

            unsafe
            {
#pragma warning disable CS8500
                TypedReference tr = __makeref(reference);
                IntPtr ptr = **(IntPtr**)(&tr);
                address = ptr.ToInt64();
#pragma warning restore CS8500
            }

            return address;
        }

        public static void SetReference<TReference, TMessage>(TReference reference, TMessage message) where TReference : class
        {
            var address = GetUnsafeReference(reference);
            TagCache<ValueAlias<long>, TMessage>.Set(ValueAlias.Create(address), message);
        }

        public static Response<TMessage> GetReference<TReference, TMessage>(TReference reference, TMessage message) where TReference : class
        {
            var address = GetUnsafeReference(reference);
            return TagCache<ValueAlias<long>, TMessage>.Get(ValueAlias.Create(address));
        }
    }
}
