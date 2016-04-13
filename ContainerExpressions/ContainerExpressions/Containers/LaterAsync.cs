using System;
using System.Threading.Tasks;

namespace ContainerExpressions.Containers
{
    /// <summary>Loads the value only once in a thread safe way, the first time it's accessed.</summary>
    public struct LaterAsync<T>
    {
        public Task<T> Value { get { return GetValue(); } }
        private Task<T> _value;
        private Func<Task<T>> _func;
        private readonly object _lock;

        /// <summary>Loads the value only once in a thread safe way, the first time it's accessed.</summary>
        /// <param name="func">A function that will be called once (and only once) to generate a value.</param>
        public LaterAsync(Func<Task<T>> func)
        {
            if (func == null)
                throw new ArgumentNullException(nameof(func));

            _lock = new object();
            _value = null;
            _func = func;
        }

        private Task<T> GetValue()
        {
            if (_value == null)
            {
                lock (_lock)
                {
                    if (_value == null)
                    {
                        _value = _func();
                        if (_value.Status == TaskStatus.Created)
                        {
                            _value.Start();
                        }
                        _func = null;
                    }
                }
            }

            return _value;
        }

        /// <summary>When compared to T, the underlying Task is returned.</summary>
        public static implicit operator Task<T>(LaterAsync<T> later) => later.Value;
    }
}
