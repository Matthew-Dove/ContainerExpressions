namespace ContainerExpressions.Containers
{
    /// <summary>A message to trace via the logger.</summary>
    public readonly struct Message
    {
        private readonly string _message;
        public Message(string message) { _message = message; }
        public override string ToString() => _message.ToString();
        public static implicit operator string(Message message) => message._message;
    }
}
