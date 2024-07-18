using System.Collections.Concurrent;

namespace ContainerExpressions.Containers
{
    public static class MessageExtensions
    {
        public static Message<TReference> Message<TReference>(this TReference reference)
        {
            return new Message<TReference>(reference);
        }
    }

    public readonly struct Message<TReference>
    {
        private readonly TReference _reference;

        public Message(TReference reference)
        {
            _reference = reference;
        }

        public TReference Set<TMessage>(TMessage message)
        {
            MessageBase.Set(_reference, message);
            return _reference;
        }

        public TMessage Get<TMessage>()
        {
            return MessageBase.Get(_reference, default(TMessage));
        }
    }

    internal sealed class MessageCache<TReference, TMessage>
    {
        private static readonly ConcurrentDictionary<TReference, TMessage> _dic = new();

        public static void Set(TReference reference, TMessage message)
        {
            _dic.TryAdd(reference, message);
        }

        public static TMessage Get(TReference reference)
        {
            _dic.TryRemove(reference, out TMessage message);
            return message;
        }
    }

    internal abstract class MessageBase
    {
        public static void Set<TReference, TMessage>(TReference reference, TMessage message)
        {
            MessageCache<TReference, TMessage>.Set(reference, message);
        }

        public static TMessage Get<TReference, TMessage>(TReference reference, TMessage message)
        {
            return MessageCache<TReference, TMessage>.Get(reference);
        }
    }
}
