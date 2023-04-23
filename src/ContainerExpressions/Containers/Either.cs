using System;
using System.Collections.Generic;

namespace ContainerExpressions.Containers
{
    /// <summary>
    /// This type holds a single instance of a selection of types.
    /// <para>While Either can take on more than one type, it is only ever a single type at a time.</para>
    /// </summary>
    public readonly struct Either<T1, T2> : IEquatable<Either<T1, T2>>
    {
        private readonly int _tag;
        private readonly T1 _t1;
        private readonly T2 _t2;

        /// <summary>A container that internally holds one, of many possible types.</summary>
        public Either(T1 t1)
        {
            _tag = 1;
            _t1 = t1;
            _t2 = default;
        }

        /// <summary>A container that internally holds one, of many possible types.</summary>
        public Either(T2 t2)
        {
            _tag = 2;
            _t1 = default;
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

        public bool TryGetT1(out T1 t1) { if (_tag == 1) { t1 = _t1; return true; }; t1 = default; return false; }
        public bool TryGetT2(out T2 t2) { if (_tag == 2) { t2 = _t2; return true; }; t2 = default; return false; }

        /// <summary>Returns the underlying type's value's string representation.</summary>
        public override string ToString()
        {
            string value = null;

            if (_tag == 1)
            {
                value = _t1?.ToString();
            }
            else if (_tag == 2)
            {
                value = _t2?.ToString();
            }

            return value ?? string.Empty;
        }

        public static implicit operator Either<T1, T2>(T1 value) => new Either<T1, T2>(value);
        public static implicit operator Either<T1, T2>(T2 value) => new Either<T1, T2>(value);

        public static bool operator !=(Either<T1, T2> x, T1 y) => !(x == y);
        public static bool operator ==(Either<T1, T2> x, T1 y) => x.Equals(y);

        public static bool operator !=(T1 x, Either<T1, T2> y) => !(x == y);
        public static bool operator ==(T1 x, Either<T1, T2> y) => y.Equals(x);

        public static bool operator !=(Either<T1, T2> x, T2 y) => !(x == y);
        public static bool operator ==(Either<T1, T2> x, T2 y) => x.Equals(y);

        public static bool operator !=(T2 x, Either<T1, T2> y) => !(x == y);
        public static bool operator ==(T2 x, Either<T1, T2> y) => y.Equals(x);

        public static bool operator !=(Either<T1, T2> x, Either<T1, T2> y) => !(x == y);
        public static bool operator ==(Either<T1, T2> x, Either<T1, T2> y) => x.Equals(y);

        public bool Equals(T1 other) => new Either<T1, T2>(other).Equals(this);
        public bool Equals(T2 other) => new Either<T1, T2>(other).Equals(this);

        public override bool Equals(object obj) => obj != null && obj is Either<T1, T2> other && Equals(other);

        public bool Equals(Either<T1, T2> other)
        {
            if (_tag != other._tag) return false;

            if (_tag == 1) return EqualityComparer<T1>.Default.Equals(_t1, other._t1);
            if (_tag == 2) return EqualityComparer<T2>.Default.Equals(_t2, other._t2);

            return _tag == 0 && other._tag == 0;
        }

        public override int GetHashCode()
        {
            if (_tag == 1) return _t1?.GetHashCode() ?? 0;
            else if (_tag == 2) return _t2?.GetHashCode() ?? 0;
            else return 0;
        }
    }

    #region Other Type Permutations

    /// <summary>
    /// This type holds a single instance of a selection of types.
    /// <para>While Either can take on more than one type, it is only ever a single type at a time.</para>
    /// </summary>
    public readonly struct Either<T1, T2, T3>
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
            _t2 = default;
            _t3 = default;
        }

        /// <summary>A container that internally holds one, of many possible types.</summary>
        public Either(T2 t2)
        {
            _tag = 2;
            _t1 = default;
            _t2 = t2;
            _t3 = default;
        }

        /// <summary>A container that internally holds one, of many possible types.</summary>
        public Either(T3 t3)
        {
            _tag = 3;
            _t1 = default;
            _t2 = default;
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

            if (_tag == 1)
            {
                value = _t1?.ToString();
            }
            else if (_tag == 2)
            {
                value = _t2?.ToString();
            }
            else if (_tag == 3)
            {
                value = _t3?.ToString();
            }

            return value ?? string.Empty;
        }
    }

    /// <summary>
    /// This type holds a single instance of a selection of types.
    /// <para>While Either can take on more than one type, it is only ever a single type at a time.</para>
    /// </summary>
    public readonly struct Either<T1, T2, T3, T4>
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
            _t2 = default;
            _t3 = default;
            _t4 = default;
        }

        /// <summary>A container that internally holds one, of many possible types.</summary>
        public Either(T2 t2)
        {
            _tag = 2;
            _t1 = default;
            _t2 = t2;
            _t3 = default;
            _t4 = default;
        }

        /// <summary>A container that internally holds one, of many possible types.</summary>
        public Either(T3 t3)
        {
            _tag = 3;
            _t1 = default;
            _t2 = default;
            _t3 = t3;
            _t4 = default;
        }

        /// <summary>A container that internally holds one, of many possible types.</summary>
        public Either(T4 t4)
        {
            _tag = 4;
            _t1 = default;
            _t2 = default;
            _t3 = default;
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

            if (_tag == 1)
            {
                value = _t1?.ToString();
            }
            else if (_tag == 2)
            {
                value = _t2?.ToString();
            }
            else if (_tag == 3)
            {
                value = _t3?.ToString();
            }
            else if (_tag == 4)
            {
                value = _t4?.ToString();
            }

            return value ?? string.Empty;
        }
    }

    /// <summary>
    /// This type holds a single instance of a selection of types.
    /// <para>While Either can take on more than one type, it is only ever a single type at a time.</para>
    /// </summary>
    public readonly struct Either<T1, T2, T3, T4, T5>
    {
        private readonly int _tag;
        private readonly T1 _t1;
        private readonly T2 _t2;
        private readonly T3 _t3;
        private readonly T4 _t4;
        private readonly T5 _t5;

        /// <summary>A container that internally holds one, of many possible types.</summary>
        public Either(T1 t1)
        {
            _tag = 1;
            _t1 = t1;
            _t2 = default;
            _t3 = default;
            _t4 = default;
            _t5 = default;
        }

        /// <summary>A container that internally holds one, of many possible types.</summary>
        public Either(T2 t2)
        {
            _tag = 2;
            _t1 = default;
            _t2 = t2;
            _t3 = default;
            _t4 = default;
            _t5 = default;
        }

        /// <summary>A container that internally holds one, of many possible types.</summary>
        public Either(T3 t3)
        {
            _tag = 3;
            _t1 = default;
            _t2 = default;
            _t3 = t3;
            _t4 = default;
            _t5 = default;
        }

        /// <summary>A container that internally holds one, of many possible types.</summary>
        public Either(T4 t4)
        {
            _tag = 4;
            _t1 = default;
            _t2 = default;
            _t3 = default;
            _t4 = t4;
            _t5 = default;
        }

        /// <summary>A container that internally holds one, of many possible types.</summary>
        public Either(T5 t5)
        {
            _tag = 5;
            _t1 = default;
            _t2 = default;
            _t3 = default;
            _t4 = default;
            _t5 = t5;
        }

        /// <summary>
        /// Gets a type that each type of Either can transform to.
        /// <para>Only one function will be invoked depending on what type is internally stored.</para>
        /// </summary>
        public TResult Match<TResult>(Func<T1, TResult> f1, Func<T2, TResult> f2, Func<T3, TResult> f3, Func<T4, TResult> f4, Func<T5, TResult> f5)
        {
            if (_tag == 1)
                return f1(_t1);
            if (_tag == 2)
                return f2(_t2);
            if (_tag == 3)
                return f3(_t3);
            if (_tag == 4)
                return f4(_t4);
            if (_tag == 5)
                return f5(_t5);

            throw new InvalidOperationException("The internal type was not set, you must assign Either a type at least once.");
        }

        /// <summary>Sets the current internal type to that of the value.</summary>
        public static implicit operator Either<T1, T2, T3, T4, T5>(T1 value) => new Either<T1, T2, T3, T4, T5>(value);

        /// <summary>Sets the current internal type to that of the value.</summary>
        public static implicit operator Either<T1, T2, T3, T4, T5>(T2 value) => new Either<T1, T2, T3, T4, T5>(value);

        /// <summary>Sets the current internal type to that of the value.</summary>
        public static implicit operator Either<T1, T2, T3, T4, T5>(T3 value) => new Either<T1, T2, T3, T4, T5>(value);

        /// <summary>Sets the current internal type to that of the value.</summary>
        public static implicit operator Either<T1, T2, T3, T4, T5>(T4 value) => new Either<T1, T2, T3, T4, T5>(value);

        /// <summary>Sets the current internal type to that of the value.</summary>
        public static implicit operator Either<T1, T2, T3, T4, T5>(T5 value) => new Either<T1, T2, T3, T4, T5>(value);

        /// <summary>Returns the underlying type's value's string representation.</summary>
        public override string ToString()
        {
            string value = null;

            if (_tag == 1)
            {
                value = _t1?.ToString();
            }
            else if (_tag == 2)
            {
                value = _t2?.ToString();
            }
            else if (_tag == 3)
            {
                value = _t3?.ToString();
            }
            else if (_tag == 4)
            {
                value = _t4?.ToString();
            }
            else if (_tag == 5)
            {
                value = _t5?.ToString();
            }

            return value ?? string.Empty;
        }
    }

    /// <summary>
    /// This type holds a single instance of a selection of types.
    /// <para>While Either can take on more than one type, it is only ever a single type at a time.</para>
    /// </summary>
    public readonly struct Either<T1, T2, T3, T4, T5, T6>
    {
        private readonly int _tag;
        private readonly T1 _t1;
        private readonly T2 _t2;
        private readonly T3 _t3;
        private readonly T4 _t4;
        private readonly T5 _t5;
        private readonly T6 _t6;

        /// <summary>A container that internally holds one, of many possible types.</summary>
        public Either(T1 t1)
        {
            _tag = 1;
            _t1 = t1;
            _t2 = default;
            _t3 = default;
            _t4 = default;
            _t5 = default;
            _t6 = default;
        }

        /// <summary>A container that internally holds one, of many possible types.</summary>
        public Either(T2 t2)
        {
            _tag = 2;
            _t1 = default;
            _t2 = t2;
            _t3 = default;
            _t4 = default;
            _t5 = default;
            _t6 = default;
        }

        /// <summary>A container that internally holds one, of many possible types.</summary>
        public Either(T3 t3)
        {
            _tag = 3;
            _t1 = default;
            _t2 = default;
            _t3 = t3;
            _t4 = default;
            _t5 = default;
            _t6 = default;
        }

        /// <summary>A container that internally holds one, of many possible types.</summary>
        public Either(T4 t4)
        {
            _tag = 4;
            _t1 = default;
            _t2 = default;
            _t3 = default;
            _t4 = t4;
            _t5 = default;
            _t6 = default;
        }

        /// <summary>A container that internally holds one, of many possible types.</summary>
        public Either(T5 t5)
        {
            _tag = 5;
            _t1 = default;
            _t2 = default;
            _t3 = default;
            _t4 = default;
            _t5 = t5;
            _t6 = default;
        }

        /// <summary>A container that internally holds one, of many possible types.</summary>
        public Either(T6 t6)
        {
            _tag = 6;
            _t1 = default;
            _t2 = default;
            _t3 = default;
            _t4 = default;
            _t5 = default;
            _t6 = t6;
        }

        /// <summary>
        /// Gets a type that each type of Either can transform to.
        /// <para>Only one function will be invoked depending on what type is internally stored.</para>
        /// </summary>
        public TResult Match<TResult>(Func<T1, TResult> f1, Func<T2, TResult> f2, Func<T3, TResult> f3, Func<T4, TResult> f4, Func<T5, TResult> f5, Func<T6, TResult> f6)
        {
            if (_tag == 1)
                return f1(_t1);
            if (_tag == 2)
                return f2(_t2);
            if (_tag == 3)
                return f3(_t3);
            if (_tag == 4)
                return f4(_t4);
            if (_tag == 5)
                return f5(_t5);
            if (_tag == 6)
                return f6(_t6);

            throw new InvalidOperationException("The internal type was not set, you must assign Either a type at least once.");
        }

        /// <summary>Sets the current internal type to that of the value.</summary>
        public static implicit operator Either<T1, T2, T3, T4, T5, T6>(T1 value) => new Either<T1, T2, T3, T4, T5, T6>(value);

        /// <summary>Sets the current internal type to that of the value.</summary>
        public static implicit operator Either<T1, T2, T3, T4, T5, T6>(T2 value) => new Either<T1, T2, T3, T4, T5, T6>(value);

        /// <summary>Sets the current internal type to that of the value.</summary>
        public static implicit operator Either<T1, T2, T3, T4, T5, T6>(T3 value) => new Either<T1, T2, T3, T4, T5, T6>(value);

        /// <summary>Sets the current internal type to that of the value.</summary>
        public static implicit operator Either<T1, T2, T3, T4, T5, T6>(T4 value) => new Either<T1, T2, T3, T4, T5, T6>(value);

        /// <summary>Sets the current internal type to that of the value.</summary>
        public static implicit operator Either<T1, T2, T3, T4, T5, T6>(T5 value) => new Either<T1, T2, T3, T4, T5, T6>(value);

        /// <summary>Sets the current internal type to that of the value.</summary>
        public static implicit operator Either<T1, T2, T3, T4, T5, T6>(T6 value) => new Either<T1, T2, T3, T4, T5, T6>(value);

        /// <summary>Returns the underlying type's value's string representation.</summary>
        public override string ToString()
        {
            string value = null;

            if (_tag == 1)
            {
                value = _t1?.ToString();
            }
            else if (_tag == 2)
            {
                value = _t2?.ToString();
            }
            else if (_tag == 3)
            {
                value = _t3?.ToString();
            }
            else if (_tag == 4)
            {
                value = _t4?.ToString();
            }
            else if (_tag == 5)
            {
                value = _t5?.ToString();
            }
            else if (_tag == 6)
            {
                value = _t6?.ToString();
            }

            return value ?? string.Empty;
        }
    }

    /// <summary>
    /// This type holds a single instance of a selection of types.
    /// <para>While Either can take on more than one type, it is only ever a single type at a time.</para>
    /// </summary>
    public readonly struct Either<T1, T2, T3, T4, T5, T6, T7>
    {
        private readonly int _tag;
        private readonly T1 _t1;
        private readonly T2 _t2;
        private readonly T3 _t3;
        private readonly T4 _t4;
        private readonly T5 _t5;
        private readonly T6 _t6;
        private readonly T7 _t7;

        /// <summary>A container that internally holds one, of many possible types.</summary>
        public Either(T1 t1)
        {
            _tag = 1;
            _t1 = t1;
            _t2 = default;
            _t3 = default;
            _t4 = default;
            _t5 = default;
            _t6 = default;
            _t7 = default;
        }

        /// <summary>A container that internally holds one, of many possible types.</summary>
        public Either(T2 t2)
        {
            _tag = 2;
            _t1 = default;
            _t2 = t2;
            _t3 = default;
            _t4 = default;
            _t5 = default;
            _t6 = default;
            _t7 = default;
        }

        /// <summary>A container that internally holds one, of many possible types.</summary>
        public Either(T3 t3)
        {
            _tag = 3;
            _t1 = default;
            _t2 = default;
            _t3 = t3;
            _t4 = default;
            _t5 = default;
            _t6 = default;
            _t7 = default;
        }

        /// <summary>A container that internally holds one, of many possible types.</summary>
        public Either(T4 t4)
        {
            _tag = 4;
            _t1 = default;
            _t2 = default;
            _t3 = default;
            _t4 = t4;
            _t5 = default;
            _t6 = default;
            _t7 = default;
        }

        /// <summary>A container that internally holds one, of many possible types.</summary>
        public Either(T5 t5)
        {
            _tag = 5;
            _t1 = default;
            _t2 = default;
            _t3 = default;
            _t4 = default;
            _t5 = t5;
            _t6 = default;
            _t7 = default;
        }

        /// <summary>A container that internally holds one, of many possible types.</summary>
        public Either(T6 t6)
        {
            _tag = 6;
            _t1 = default;
            _t2 = default;
            _t3 = default;
            _t4 = default;
            _t5 = default;
            _t6 = t6;
            _t7 = default;
        }

        /// <summary>A container that internally holds one, of many possible types.</summary>
        public Either(T7 t7)
        {
            _tag = 7;
            _t1 = default;
            _t2 = default;
            _t3 = default;
            _t4 = default;
            _t5 = default;
            _t6 = default;
            _t7 = t7;
        }

        /// <summary>
        /// Gets a type that each type of Either can transform to.
        /// <para>Only one function will be invoked depending on what type is internally stored.</para>
        /// </summary>
        public TResult Match<TResult>(Func<T1, TResult> f1, Func<T2, TResult> f2, Func<T3, TResult> f3, Func<T4, TResult> f4, Func<T5, TResult> f5, Func<T6, TResult> f6, Func<T7, TResult> f7)
        {
            if (_tag == 1)
                return f1(_t1);
            if (_tag == 2)
                return f2(_t2);
            if (_tag == 3)
                return f3(_t3);
            if (_tag == 4)
                return f4(_t4);
            if (_tag == 5)
                return f5(_t5);
            if (_tag == 6)
                return f6(_t6);
            if (_tag == 7)
                return f7(_t7);

            throw new InvalidOperationException("The internal type was not set, you must assign Either a type at least once.");
        }

        /// <summary>Sets the current internal type to that of the value.</summary>
        public static implicit operator Either<T1, T2, T3, T4, T5, T6, T7>(T1 value) => new Either<T1, T2, T3, T4, T5, T6, T7>(value);

        /// <summary>Sets the current internal type to that of the value.</summary>
        public static implicit operator Either<T1, T2, T3, T4, T5, T6, T7>(T2 value) => new Either<T1, T2, T3, T4, T5, T6, T7>(value);

        /// <summary>Sets the current internal type to that of the value.</summary>
        public static implicit operator Either<T1, T2, T3, T4, T5, T6, T7>(T3 value) => new Either<T1, T2, T3, T4, T5, T6, T7>(value);

        /// <summary>Sets the current internal type to that of the value.</summary>
        public static implicit operator Either<T1, T2, T3, T4, T5, T6, T7>(T4 value) => new Either<T1, T2, T3, T4, T5, T6, T7>(value);

        /// <summary>Sets the current internal type to that of the value.</summary>
        public static implicit operator Either<T1, T2, T3, T4, T5, T6, T7>(T5 value) => new Either<T1, T2, T3, T4, T5, T6, T7>(value);

        /// <summary>Sets the current internal type to that of the value.</summary>
        public static implicit operator Either<T1, T2, T3, T4, T5, T6, T7>(T6 value) => new Either<T1, T2, T3, T4, T5, T6, T7>(value);

        /// <summary>Sets the current internal type to that of the value.</summary>
        public static implicit operator Either<T1, T2, T3, T4, T5, T6, T7>(T7 value) => new Either<T1, T2, T3, T4, T5, T6, T7>(value);

        /// <summary>Returns the underlying type's value's string representation.</summary>
        public override string ToString()
        {
            string value = null;

            if (_tag == 1)
            {
                value = _t1?.ToString();
            }
            else if (_tag == 2)
            {
                value = _t2?.ToString();
            }
            else if (_tag == 3)
            {
                value = _t3?.ToString();
            }
            else if (_tag == 4)
            {
                value = _t4?.ToString();
            }
            else if (_tag == 5)
            {
                value = _t5?.ToString();
            }
            else if (_tag == 6)
            {
                value = _t6?.ToString();
            }
            else if (_tag == 7)
            {
                value = _t7?.ToString();
            }

            return value ?? string.Empty;
        }
    }

    /// <summary>
    /// This type holds a single instance of a selection of types.
    /// <para>While Either can take on more than one type, it is only ever a single type at a time.</para>
    /// </summary>
    public readonly struct Either<T1, T2, T3, T4, T5, T6, T7, T8>
    {
        private readonly int _tag;
        private readonly T1 _t1;
        private readonly T2 _t2;
        private readonly T3 _t3;
        private readonly T4 _t4;
        private readonly T5 _t5;
        private readonly T6 _t6;
        private readonly T7 _t7;
        private readonly T8 _t8;

        /// <summary>A container that internally holds one, of many possible types.</summary>
        public Either(T1 t1)
        {
            _tag = 1;
            _t1 = t1;
            _t2 = default;
            _t3 = default;
            _t4 = default;
            _t5 = default;
            _t6 = default;
            _t7 = default;
            _t8 = default;
        }

        /// <summary>A container that internally holds one, of many possible types.</summary>
        public Either(T2 t2)
        {
            _tag = 2;
            _t1 = default;
            _t2 = t2;
            _t3 = default;
            _t4 = default;
            _t5 = default;
            _t6 = default;
            _t7 = default;
            _t8 = default;
        }

        /// <summary>A container that internally holds one, of many possible types.</summary>
        public Either(T3 t3)
        {
            _tag = 3;
            _t1 = default;
            _t2 = default;
            _t3 = t3;
            _t4 = default;
            _t5 = default;
            _t6 = default;
            _t7 = default;
            _t8 = default;
        }

        /// <summary>A container that internally holds one, of many possible types.</summary>
        public Either(T4 t4)
        {
            _tag = 4;
            _t1 = default;
            _t2 = default;
            _t3 = default;
            _t4 = t4;
            _t5 = default;
            _t6 = default;
            _t7 = default;
            _t8 = default;
        }

        /// <summary>A container that internally holds one, of many possible types.</summary>
        public Either(T5 t5)
        {
            _tag = 5;
            _t1 = default;
            _t2 = default;
            _t3 = default;
            _t4 = default;
            _t5 = t5;
            _t6 = default;
            _t7 = default;
            _t8 = default;
        }

        /// <summary>A container that internally holds one, of many possible types.</summary>
        public Either(T6 t6)
        {
            _tag = 6;
            _t1 = default;
            _t2 = default;
            _t3 = default;
            _t4 = default;
            _t5 = default;
            _t6 = t6;
            _t7 = default;
            _t8 = default;
        }

        /// <summary>A container that internally holds one, of many possible types.</summary>
        public Either(T7 t7)
        {
            _tag = 7;
            _t1 = default;
            _t2 = default;
            _t3 = default;
            _t4 = default;
            _t5 = default;
            _t6 = default;
            _t7 = t7;
            _t8 = default;
        }

        /// <summary>A container that internally holds one, of many possible types.</summary>
        public Either(T8 t8)
        {
            _tag = 7;
            _t1 = default;
            _t2 = default;
            _t3 = default;
            _t4 = default;
            _t5 = default;
            _t6 = default;
            _t7 = default;
            _t8 = t8;
        }

        /// <summary>
        /// Gets a type that each type of Either can transform to.
        /// <para>Only one function will be invoked depending on what type is internally stored.</para>
        /// </summary>
        public TResult Match<TResult>(Func<T1, TResult> f1, Func<T2, TResult> f2, Func<T3, TResult> f3, Func<T4, TResult> f4, Func<T5, TResult> f5, Func<T6, TResult> f6, Func<T7, TResult> f7, Func<T8, TResult> f8)
        {
            if (_tag == 1)
                return f1(_t1);
            if (_tag == 2)
                return f2(_t2);
            if (_tag == 3)
                return f3(_t3);
            if (_tag == 4)
                return f4(_t4);
            if (_tag == 5)
                return f5(_t5);
            if (_tag == 6)
                return f6(_t6);
            if (_tag == 7)
                return f7(_t7);
            if (_tag == 8)
                return f8(_t8);

            throw new InvalidOperationException("The internal type was not set, you must assign Either a type at least once.");
        }

        /// <summary>Sets the current internal type to that of the value.</summary>
        public static implicit operator Either<T1, T2, T3, T4, T5, T6, T7, T8>(T1 value) => new Either<T1, T2, T3, T4, T5, T6, T7, T8>(value);

        /// <summary>Sets the current internal type to that of the value.</summary>
        public static implicit operator Either<T1, T2, T3, T4, T5, T6, T7, T8>(T2 value) => new Either<T1, T2, T3, T4, T5, T6, T7, T8>(value);

        /// <summary>Sets the current internal type to that of the value.</summary>
        public static implicit operator Either<T1, T2, T3, T4, T5, T6, T7, T8>(T3 value) => new Either<T1, T2, T3, T4, T5, T6, T7, T8>(value);

        /// <summary>Sets the current internal type to that of the value.</summary>
        public static implicit operator Either<T1, T2, T3, T4, T5, T6, T7, T8>(T4 value) => new Either<T1, T2, T3, T4, T5, T6, T7, T8>(value);

        /// <summary>Sets the current internal type to that of the value.</summary>
        public static implicit operator Either<T1, T2, T3, T4, T5, T6, T7, T8>(T5 value) => new Either<T1, T2, T3, T4, T5, T6, T7, T8>(value);

        /// <summary>Sets the current internal type to that of the value.</summary>
        public static implicit operator Either<T1, T2, T3, T4, T5, T6, T7, T8>(T6 value) => new Either<T1, T2, T3, T4, T5, T6, T7, T8>(value);

        /// <summary>Sets the current internal type to that of the value.</summary>
        public static implicit operator Either<T1, T2, T3, T4, T5, T6, T7, T8>(T7 value) => new Either<T1, T2, T3, T4, T5, T6, T7, T8>(value);

        /// <summary>Sets the current internal type to that of the value.</summary>
        public static implicit operator Either<T1, T2, T3, T4, T5, T6, T7, T8>(T8 value) => new Either<T1, T2, T3, T4, T5, T6, T7, T8>(value);

        /// <summary>Returns the underlying type's value's string representation.</summary>
        public override string ToString()
        {
            string value = null;

            if (_tag == 1)
            {
                value = _t1?.ToString();
            }
            else if (_tag == 2)
            {
                value = _t2?.ToString();
            }
            else if (_tag == 3)
            {
                value = _t3?.ToString();
            }
            else if (_tag == 4)
            {
                value = _t4?.ToString();
            }
            else if (_tag == 5)
            {
                value = _t5?.ToString();
            }
            else if (_tag == 6)
            {
                value = _t6?.ToString();
            }
            else if (_tag == 7)
            {
                value = _t7?.ToString();
            }
            else if (_tag == 8)
            {
                value = _t8?.ToString();
            }

            return value ?? string.Empty;
        }
    }

    #endregion
}
