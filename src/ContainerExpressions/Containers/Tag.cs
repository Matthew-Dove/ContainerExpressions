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

        internal Tag(TReference reference)
        {
            _reference = reference;
        }

        public Response<TMessage> Get<TMessage>(bool removeTag = true)
        {
            return TagBase.Get(_reference, default(TMessage), removeTag);
        }

        public TReference Set<TMessage>(TMessage message)
        {
            TagBase.Set(_reference, message);
            return _reference;
        }
    }

    public readonly struct TagReference<TReference> where TReference : class
    {
        private readonly TReference _reference;

        internal TagReference(TReference reference)
        {
            _reference = reference;
        }

        public Response<TMessage> Get<TMessage>(bool removeTag = true)
        {
            return TagBase.GetReference(_reference, default(TMessage), removeTag);
        }

        public TReference Set<TMessage>(TMessage message)
        {
            TagBase.SetReference(_reference, message);
            return _reference;
        }
    }

    internal abstract class TagBase
    {
        public static Response<TMessage> Get<TReference, TMessage>(TReference reference, TMessage message, bool removeTag)
        {
            return TagCache<TReference, TMessage>.Get(reference, removeTag);
        }

        public static void Set<TReference, TMessage>(TReference reference, TMessage message)
        {
            TagCache<TReference, TMessage>.Set(reference, message);
        }

        public static Response<TMessage> GetReference<TReference, TMessage>(TReference reference, TMessage message, bool removeTag) where TReference : class
        {
            var address = GetUnsafeReference(reference);
            return TagCache<ValueAlias<long>, TMessage>.Get(ValueAlias.Create(address), removeTag);
        }

        public static void SetReference<TReference, TMessage>(TReference reference, TMessage message) where TReference : class
        {
            var address = GetUnsafeReference(reference);
            TagCache<ValueAlias<long>, TMessage>.Set(ValueAlias.Create(address), message);
        }

        private static long GetUnsafeReference<TReference>(TReference reference) where TReference : class
        {
            long address;

#pragma warning disable CS8500
            unsafe
            {
                TypedReference tr = __makeref(reference);
                IntPtr ptr = **(IntPtr**)(&tr);
                address = ptr.ToInt64();
            }
#pragma warning restore CS8500

            return address;
        }

        internal sealed class TagCache<TReference, TMessage>
        {
            private static readonly ConcurrentDictionary<TReference, TMessage> _dic = new();

            public static Response<TMessage> Get(TReference reference, bool removeTag)
            {
                bool result;
                TMessage message;

                if (removeTag) result = _dic.TryRemove(reference, out message);
                else result = _dic.TryGetValue(reference, out message);

                return result ? Response.Create(message) : Response<TMessage>.Error;
            }

            public static void Set(TReference reference, TMessage message)
            {
                _dic.AddOrUpdate(reference , message, (_, _) => message);
            }
        }
    }
}
