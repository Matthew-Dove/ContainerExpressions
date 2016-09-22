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

    #region OtherTypes

    /// <summary>
    /// This type holds a single instance of a selection of types.
    /// <para>While Either can take on more than one type, it is only ever a single type at a time.</para>
    /// </summary>
    public struct Either<T1, T2, T3>
    {
        private readonly int _tag;
        private readonly T1 _t1;
        private readonly T2 _t2;
        private readonly T3 _t3;

        /// <summary>A container that internally holds one, of many possible types.</summary>
        public Either(T1 t1)
        {
            _tag = 1;
            _t1 = t1;
            _t2 = default(T2);
            _t3 = default(T3);
        }

        /// <summary>A container that internally holds one, of many possible types.</summary>
        public Either(T2 t2)
        {
            _tag = 2;
            _t1 = default(T1);
            _t2 = t2;
            _t3 = default(T3);
        }

        /// <summary>A container that internally holds one, of many possible types.</summary>
        public Either(T3 t3)
        {
            _tag = 3;
            _t1 = default(T1);
            _t2 = default(T2);
            _t3 = t3;
        }

        /// <summary>
        /// Gets a type that each type of Either can transform to.
        /// <para>Only one function will be invoked depending on what type is internally stored.</para>
        /// </summary>
        public TResult Match<TResult>(Func<T1, TResult> f1, Func<T2, TResult> f2, Func<T3, TResult> f3)
        {
            if (_tag == 1)
                return f1(_t1);
            if (_tag == 2)
                return f2(_t2);
            if (_tag == 3)
                return f3(_t3);

            throw new InvalidOperationException("The internal type was not set, you must assign Either a type at least once.");
        }

        /// <summary>Sets the current internal type to that of the value.</summary>
        public static implicit operator Either<T1, T2, T3>(T1 value) => new Either<T1, T2, T3>(value);

        /// <summary>Sets the current internal type to that of the value.</summary>
        public static implicit operator Either<T1, T2, T3>(T2 value) => new Either<T1, T2, T3>(value);

        /// <summary>Sets the current internal type to that of the value.</summary>
        public static implicit operator Either<T1, T2, T3>(T3 value) => new Either<T1, T2, T3>(value);

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
            else if (_tag == 3 && _t3 != null)
            {
                value = _t3.ToString();
            }

            return value;
        }
    }

    /// <summary>
    /// This type holds a single instance of a selection of types.
    /// <para>While Either can take on more than one type, it is only ever a single type at a time.</para>
    /// </summary>
    public struct Either<T1, T2, T3, T4>
    {
        private readonly int _tag;
        private readonly T1 _t1;
        private readonly T2 _t2;
        private readonly T3 _t3;
        private readonly T4 _t4;

        /// <summary>A container that internally holds one, of many possible types.</summary>
        public Either(T1 t1)
        {
            _tag = 1;
            _t1 = t1;
            _t2 = default(T2);
            _t3 = default(T3);
            _t4 = default(T4);
        }

        /// <summary>A container that internally holds one, of many possible types.</summary>
        public Either(T2 t2)
        {
            _tag = 2;
            _t1 = default(T1);
            _t2 = t2;
            _t3 = default(T3);
            _t4 = default(T4);
        }

        /// <summary>A container that internally holds one, of many possible types.</summary>
        public Either(T3 t3)
        {
            _tag = 3;
            _t1 = default(T1);
            _t2 = default(T2);
            _t3 = t3;
            _t4 = default(T4);
        }

        /// <summary>A container that internally holds one, of many possible types.</summary>
        public Either(T4 t4)
        {
            _tag = 4;
            _t1 = default(T1);
            _t2 = default(T2);
            _t3 = default(T3);
            _t4 = t4;
        }

        /// <summary>
        /// Gets a type that each type of Either can transform to.
        /// <para>Only one function will be invoked depending on what type is internally stored.</para>
        /// </summary>
        public TResult Match<TResult>(Func<T1, TResult> f1, Func<T2, TResult> f2, Func<T3, TResult> f3, Func<T4, TResult> f4)
        {
            if (_tag == 1)
                return f1(_t1);
            if (_tag == 2)
                return f2(_t2);
            if (_tag == 3)
                return f3(_t3);
            if (_tag == 4)
                return f4(_t4);

            throw new InvalidOperationException("The internal type was not set, you must assign Either a type at least once.");
        }

        /// <summary>Sets the current internal type to that of the value.</summary>
        public static implicit operator Either<T1, T2, T3, T4>(T1 value) => new Either<T1, T2, T3, T4>(value);

        /// <summary>Sets the current internal type to that of the value.</summary>
        public static implicit operator Either<T1, T2, T3, T4>(T2 value) => new Either<T1, T2, T3, T4>(value);

        /// <summary>Sets the current internal type to that of the value.</summary>
        public static implicit operator Either<T1, T2, T3, T4>(T3 value) => new Either<T1, T2, T3, T4>(value);

        /// <summary>Sets the current internal type to that of the value.</summary>
        public static implicit operator Either<T1, T2, T3, T4>(T4 value) => new Either<T1, T2, T3, T4>(value);

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
            else if (_tag == 3 && _t3 != null)
            {
                value = _t3.ToString();
            }
            else if (_tag == 4 && _t4 != null)
            {
                value = _t4.ToString();
            }

            return value;
        }
    }

    #endregion
}
