using System;
using System.Collections.Concurrent;

namespace ContainerExpressions.Containers
{
    public static class TagExtensions
    {
        /// <summary>
        /// Tag an object with a message, based on the object's Equals, and GetHashCode methods.
        /// <para>Tag is thread safe, the object's Equals, and GetHashCode methods should not change over time.</para>
        /// </summary>
        public static Tag<TReference> Tag<TReference>(this TReference reference)
        {
            return new Tag<TReference>(reference);
        }

        /// <summary>
        /// Tag an object with a message, based on the object's address in memory.
        /// <para>Tag is thread safe, the object's Equals, and GetHashCode methods are not used to store, or retrieve the message (the address pointer is).</para>
        /// </summary>
        public static TagReference<TReference> TagReference<TReference>(this TReference reference) where TReference : class
        {
            return new TagReference<TReference>(reference);
        }
    }

    /// <summary>A DX wrapper to ergonomically infer types, or make them easier to write.</summary>
    public readonly struct Tag<TReference>
    {
        private readonly TReference _reference;

        internal Tag(TReference reference)
        {
            _reference = reference;
        }

        /// <summary>
        /// Gets the message set previously on a tagged object.
        /// <para>Tag is thread safe, the object's Equals, and GetHashCode methods are used to store, and retrieve the message.</para>
        /// </summary>
        /// <param name="removeTag">When true, the tag will be removed from the object.</param>
        /// <param name="key">When a custom key is specified, that will be used to find the tag; instead of the TReference's Equals, and GetHashCode methods.</param>
        public Response<TMessage> Get<TMessage>(bool removeTag = true, string key = null)
        {
            if (string.IsNullOrEmpty(key)) return TagBase.Get(_reference, default(TMessage), removeTag);
            return TagBase.GetKey(default(TMessage), removeTag, key);
        }

        /// <summary>
        /// Stores a message on a tagged object, if a message already exists, it is overwritten.
        /// <para>Tag is thread safe, the object's Equals, and GetHashCode methods are used to store, and retrieve the message.</para>
        /// </summary>
        /// <param name="message">The message to store on the tagged object.</param>
        /// <param name="key">When a custom key is specified, that will be used to find the tag; instead of the TReference's Equals, and GetHashCode methods.</param>
        public TReference Set<TMessage>(TMessage message, string key = null)
        {
            if (string.IsNullOrEmpty(key)) TagBase.Set(_reference, message);
            else TagBase.SetKey(message, key);
            return _reference;
        }
    }

    /// <summary>A DX wrapper to ergonomically infer types, or make them easier to write.</summary>
    public readonly struct TagReference<TReference> where TReference : class
    {
        private readonly TReference _reference;

        internal TagReference(TReference reference)
        {
            _reference = reference;
        }

        /// <summary>
        /// .Gets the message set previously on a tagged object.
        /// <para>Tag is thread safe, the object's address pointer is used to store, and retrieve the message.</para>
        /// </summary>
        /// <param name="removeTag">When true, the tag will be removed from the object.</param>
        public Response<TMessage> Get<TMessage>(bool removeTag = true)
        {
            return TagBase.GetReference(_reference, default(TMessage), removeTag);
        }

        /// <summary>
        /// Stores a message on a tagged object, if a message already exists, it is overwritten.
        /// <para>Tag is thread safe, the object's address pointer is used to store, and retrieve the message.</para>
        /// </summary>
        /// <param name="message">The message to store on the tagged object.</param>
        public TReference Set<TMessage>(TMessage message)
        {
            TagBase.SetReference(_reference, message);
            return _reference;
        }
    }

    internal abstract class TagBase
    {
        public static Response<TMessage> Get<TReference, TMessage>(TReference reference, TMessage _, bool removeTag)
        {
            return TagCache<TReference, TMessage>.Get(reference, removeTag);
        }

        public static void Set<TReference, TMessage>(TReference reference, TMessage message)
        {
            TagCache<TReference, TMessage>.Set(reference, message);
        }

        public static Response<TMessage> GetKey<TMessage>(TMessage _, bool removeTag, string key)
        {
            return TagKeyCache<TMessage>.Get(key, removeTag);
        }

        public static void SetKey<TMessage>(TMessage message, string key)
        {
            TagKeyCache<TMessage>.Set(key, message);
        }

        public static Response<TMessage> GetReference<TReference, TMessage>(TReference reference, TMessage _, bool removeTag) where TReference : class
        {
            var address = GetUnsafeReference(reference);
            return TagReferenceCache<TMessage>.Get(address, removeTag);
        }

        public static void SetReference<TReference, TMessage>(TReference reference, TMessage message) where TReference : class
        {
            var address = GetUnsafeReference(reference);
            TagReferenceCache<TMessage>.Set(address, message);
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

        internal abstract class BaseCache<TReference, TMessage>
        {
            private static readonly ConcurrentDictionary<TReference, TMessage> _dic = new(); // Thread safe implementation.

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
                _dic.AddOrUpdate(reference, message, (_, _) => message); // Always set the newest message value.
            }
        }

        // Map a key to a value, where the key is struct, or class; key matches are based on the Equals, and GetHashCode methods.
        internal sealed class TagCache<TReference, TMessage> : BaseCache<TReference, TMessage> { }

        // Created a seperate class for custom keys, so the string value won't clash when tagging normal strings.
        internal sealed class TagKeyCache<TMessage> : BaseCache<string, TMessage> { }

        // Created a seperate class for references, so the long address pointer won't clash with standard value type longs.
        internal sealed class TagReferenceCache<TMessage> : BaseCache<long, TMessage> { }
    }
}
