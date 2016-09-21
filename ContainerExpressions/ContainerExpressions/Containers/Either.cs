using System;

namespace ContainerExpressions.Containers
{
    /// <summary>
    /// This type holds a single instance of a selection of types.
    /// <para>While Either can take on more than one type, it is only ever a single type at a time.</para>
    /// </summary>
    public struct Either<T1, T2>
    {
        private readonly int _tag;
        private readonly T1 _t1;
        private readonly T2 _t2;

        /// <summary>A container that internally holds one, of many possible types.</summary>
        public Either(T1 t1)
        {
            _tag = 1;
            _t1 = t1;
            _t2 = default(T2);
        }

        /// <summary>A container that internally holds one, of many possible types.</summary>
        public Either(T2 t2)
        {
            _tag = 2;
            _t1 = default(T1);
            _t2 = t2;
        }

        /// <summary>A container that internally holds one, of many possible types.</summary>
        public Either(Either<T1, T2> either)
        {
            _tag = either._tag;
            _t1 = either._t1;
            _t2 = either._t2;
        }

        /// <summary>
        /// Gets a type that each type of Either can transform to.
        /// <para>Only one function will be invoked depending on what type is internally stored.</para>
        /// </summary>
        public TResult Match<TResult>(Func<T1, TResult> f1, Func<T2, TResult> f2)
        {
            if (_tag == 1)
                return f1(_t1);
            if (_tag == 2)
                return f2(_t2);

            throw new InvalidOperationException("The internal type was not set, you must assign Either a type at least once.");
        }

        /// <summary>Sets the current internal type to that of the value.</summary>
        public static implicit operator Either<T1, T2>(T1 value) => new Either<T1, T2>(value);

        /// <summary>Sets the current internal type to that of the value.</summary>
        public static implicit operator Either<T1, T2>(T2 value) => new Either<T1, T2>(value);

        /// <summary>Returns the underlying type's value's string representation.</summary>
        public override string ToString()
        {
            string value = null;

            if (_tag == 1 && _t1 != null)
            {
                value = _t1.ToString();
            }
            else if (_tag == 2 && _t2 != null)
            {
                value = _t2.ToString();
            }

            return value;
        }
    }

    /// <summary>A helper class for the Either generic class.</summary>
    public static class Either
    {
        /// <summary>A container that internally holds one, of many possible types.</summary>
        public static Either<T1, T2> Create<T1, T2>(T1 value) => new Either<T1, T2>(value);

        /// <summary>A container that internally holds one, of many possible types.</summary>
        public static Either<T1, T2> Create<T1, T2>(T2 value) => new Either<T1, T2>(value);

        /// <summary>A helper class to convert different types into an Either type.</summary>
        public static EitherChain<T1> Create<T1>(T1 value) => new EitherChain<T1>(value);

        /// <summary>A container that internally holds one, of many possible types.</summary>
        public static Either<T1, T2> Create<T1, T2>(Either<T1, T2> either) => new Either<T1, T2>(either);
    }

    /// <summary>A helper class to convert different types into an Either type.</summary>
    public struct EitherChain<T1>
    {
        /// <summary>A container that internally holds one, of many possible types.</summary>
        public Either<T1, T2> WithType<T2>() => new Either<T1, T2>(_value);

        private readonly T1 _value;

        internal EitherChain(T1 value)
        {
            _value = value;
        }
    }
}
