using System;

namespace MonadicExpressions.Containers
{
    /// <summary>Loads the value only once, the first time it's accessed.</summary>
    /// <remarks>This model is thread safe.</remarks>
    public struct Later<T>
    {
        public T Value { get { return GetValue(); } }
        private T _value;
        private Func<T> _func;

        private readonly static object _lock = new object();

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

        private T GetValue()
        {
            if (_func != null)
            {
                lock (_lock)
                {
                    if (_func != null)
                    {
                        _value = _func();
                        _func = null;
                    }
                }
            }

            return _value;
        }
    }

    /// <summary>A helper class for the Later generic class.</summary>
    public static class Later
    {
        /// <summary>Creates a Later container for the value generating function.</summary>
        /// <typeparam name="T">The type the generating function returns.</typeparam>
        /// <param name="func">A function that will be called once (and only once) to generate a value.</param>
        /// <returns></returns>
        public static Later<T> Create<T>(Func<T> func) => new Later<T>(func); // A little trick so the caller doesn't have to specify T.
    }
}