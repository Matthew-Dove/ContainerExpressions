using System;
using System.Threading.Tasks;

namespace ContainerExpressions.Containers
{
    /// <summary>Loads the value only once, the first time it's accessed.</summary>
    public struct Later<T>
    {
        public T Value { get { if (_func != null) { _value = _func(); _func = null; } return _value; } }
        private T _value;
        private Func<T> _func;

        /// <summary>Loads the value only once, the first time it's accessed.</summary>
        /// <param name="func">A function that will be called once (and only once) to generate a value.</param>
        public Later(Func<T> func)
        {
            if (func == null)
                throw new ArgumentNullException(nameof(func));

            _value = default(T);
            _func = func;
        }

        /// <summary>When compared to T, the underlying value is returned.</summary>
        public static implicit operator T(Later<T> later) => later.Value;
    }

    /// <summary>A helper class for the Later generic class.</summary>
    public static class Later
    {
        /// <summary>Creates a Later container for the value generating function.</summary>
        /// <typeparam name="T">The type the generating function returns.</typeparam>
        /// <param name="func">A function that will be called once (and only once) to generate a value.</param>
        /// <returns></returns>
        public static Later<T> Create<T>(Func<T> func) => new Later<T>(func); // A little trick so the caller doesn't have to specify T.

        /// <summary>Loads the value only once in a thread safe way, the first time it's accessed.</summary>
        public static LaterAsync<T> CreateAsync<T>(Func<Task<T>> func) => new LaterAsync<T>(func);
    }
}