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
        internal readonly int _tag;
        internal readonly T1 _t1;
        internal readonly T2 _t2;

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

        /// <summary>Returns the underlying type's value's string representation.</summary>
        public override string ToString()
        {
            if (_tag == 0) return string.Empty;

            if (_tag == 1) return _t1?.ToString() ?? string.Empty;
            if (_tag == 2) return _t2?.ToString() ?? string.Empty;

            return string.Empty;
        }

        public static implicit operator Either<T1, T2>(T1 value) => new Either<T1, T2>(value);
        public static implicit operator Either<T1, T2>(T2 value) => new Either<T1, T2>(value);

        public static bool operator !=(Either<T1, T2> x, Either<T1, T2> y) => !(x == y);
        public static bool operator ==(Either<T1, T2> x, Either<T1, T2> y) => x.Equals(y);

        public static bool operator !=(T1 x, Either<T1, T2> y) => !(x == y);
        public static bool operator ==(T1 x, Either<T1, T2> y) => y.Equals(x);
        public static bool operator !=(Either<T1, T2> x, T1 y) => !(x == y);
        public static bool operator ==(Either<T1, T2> x, T1 y) => x.Equals(y);

        public static bool operator !=(T2 x, Either<T1, T2> y) => !(x == y);
        public static bool operator ==(T2 x, Either<T1, T2> y) => y.Equals(x);
        public static bool operator !=(Either<T1, T2> x, T2 y) => !(x == y);
        public static bool operator ==(Either<T1, T2> x, T2 y) => x.Equals(y);

        public bool Equals(T1 other) => new Either<T1, T2>(other).Equals(this);
        public bool Equals(T2 other) => new Either<T1, T2>(other).Equals(this);

        public override bool Equals(object obj)
        {
            return obj switch
            {
                Either<T1, T2> other => Equals(other),
                T1 t1 => Equals(t1),
                T2 t2 => Equals(t2),
                _ => false
            };
        }

        public bool Equals(Either<T1, T2> other)
        {
            if (_tag != other._tag) return false;

            if (_tag == 1) return EqualityComparer<T1>.Default.Equals(_t1, other._t1);
            if (_tag == 2) return EqualityComparer<T2>.Default.Equals(_t2, other._t2);

            return _tag == 0 && other._tag == 0;
        }

        public override int GetHashCode()
        {
            if (_tag == 0) return 0;

            if (_tag == 1) return _t1?.GetHashCode() ?? 0;
            if (_tag == 2) return _t2?.GetHashCode() ?? 0;

            return 0;
        }
    }

    #region Other Type Permutations

    /// <summary>
    /// This type holds a single instance of a selection of types.
    /// <para>While Either can take on more than one type, it is only ever a single type at a time.</para>
    /// </summary>
    public readonly struct Either<T1, T2, T3> : IEquatable<Either<T1, T2, T3>>
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

        public bool TryGetT1(out T1 t1) { if (_tag == 1) { t1 = _t1; return true; }; t1 = default; return false; }
        public bool TryGetT2(out T2 t2) { if (_tag == 2) { t2 = _t2; return true; }; t2 = default; return false; }
        public bool TryGetT3(out T3 t3) { if (_tag == 3) { t3 = _t3; return true; }; t3 = default; return false; }

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

        public static implicit operator Either<T1, T2, T3>(T1 value) => new Either<T1, T2, T3>(value);
        public static implicit operator Either<T1, T2, T3>(T2 value) => new Either<T1, T2, T3>(value);
        public static implicit operator Either<T1, T2, T3>(T3 value) => new Either<T1, T2, T3>(value);

        public static bool operator !=(Either<T1, T2, T3> x, Either<T1, T2, T3> y) => !(x == y);
        public static bool operator ==(Either<T1, T2, T3> x, Either<T1, T2, T3> y) => x.Equals(y);

        public static bool operator !=(T1 x, Either<T1, T2, T3> y) => !(x == y);
        public static bool operator ==(T1 x, Either<T1, T2, T3> y) => y.Equals(x);
        public static bool operator !=(Either<T1, T2, T3> x, T1 y) => !(x == y);
        public static bool operator ==(Either<T1, T2, T3> x, T1 y) => x.Equals(y);

        public static bool operator !=(T2 x, Either<T1, T2, T3> y) => !(x == y);
        public static bool operator ==(T2 x, Either<T1, T2, T3> y) => y.Equals(x);
        public static bool operator !=(Either<T1, T2, T3> x, T2 y) => !(x == y);
        public static bool operator ==(Either<T1, T2, T3> x, T2 y) => x.Equals(y);

        public static bool operator !=(T3 x, Either<T1, T2, T3> y) => !(x == y);
        public static bool operator ==(T3 x, Either<T1, T2, T3> y) => y.Equals(x);
        public static bool operator !=(Either<T1, T2, T3> x, T3 y) => !(x == y);
        public static bool operator ==(Either<T1, T2, T3> x, T3 y) => x.Equals(y);

        public bool Equals(T1 other) => new Either<T1, T2, T3>(other).Equals(this);
        public bool Equals(T2 other) => new Either<T1, T2, T3>(other).Equals(this);
        public bool Equals(T3 other) => new Either<T1, T2, T3>(other).Equals(this);

        public override bool Equals(object obj)
        {
            return obj switch
            {
                Either<T1, T2, T3> other => Equals(other),
                T1 t1 => Equals(t1),
                T2 t2 => Equals(t2),
                T3 t3 => Equals(t3),
                _ => false
            };
        }

        public bool Equals(Either<T1, T2, T3> other)
        {
            if (_tag != other._tag) return false;

            if (_tag == 1) return EqualityComparer<T1>.Default.Equals(_t1, other._t1);
            if (_tag == 2) return EqualityComparer<T2>.Default.Equals(_t2, other._t2);
            if (_tag == 3) return EqualityComparer<T3>.Default.Equals(_t3, other._t3);

            return _tag == 0 && other._tag == 0;
        }

        public override int GetHashCode()
        {
            if (_tag == 0) return 0;

            if (_tag == 1) return _t1?.GetHashCode() ?? 0;
            if (_tag == 2) return _t2?.GetHashCode() ?? 0;
            if (_tag == 3) return _t3?.GetHashCode() ?? 0;

            return 0;
        }
    }

    /// <summary>
    /// This type holds a single instance of a selection of types.
    /// <para>While Either can take on more than one type, it is only ever a single type at a time.</para>
    /// </summary>
    public readonly struct Either<T1, T2, T3, T4> : IEquatable<Either<T1, T2, T3, T4>>
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

        public bool TryGetT1(out T1 t1) { if (_tag == 1) { t1 = _t1; return true; }; t1 = default; return false; }
        public bool TryGetT2(out T2 t2) { if (_tag == 2) { t2 = _t2; return true; }; t2 = default; return false; }
        public bool TryGetT3(out T3 t3) { if (_tag == 3) { t3 = _t3; return true; }; t3 = default; return false; }
        public bool TryGetT4(out T4 t4) { if (_tag == 4) { t4 = _t4; return true; }; t4 = default; return false; }

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

        public static implicit operator Either<T1, T2, T3, T4>(T1 value) => new Either<T1, T2, T3, T4>(value);
        public static implicit operator Either<T1, T2, T3, T4>(T2 value) => new Either<T1, T2, T3, T4>(value);
        public static implicit operator Either<T1, T2, T3, T4>(T3 value) => new Either<T1, T2, T3, T4>(value);
        public static implicit operator Either<T1, T2, T3, T4>(T4 value) => new Either<T1, T2, T3, T4>(value);

        public static bool operator !=(Either<T1, T2, T3, T4> x, Either<T1, T2, T3, T4> y) => !(x == y);
        public static bool operator ==(Either<T1, T2, T3, T4> x, Either<T1, T2, T3, T4> y) => x.Equals(y);

        public static bool operator !=(T1 x, Either<T1, T2, T3, T4> y) => !(x == y);
        public static bool operator ==(T1 x, Either<T1, T2, T3, T4> y) => y.Equals(x);
        public static bool operator !=(Either<T1, T2, T3, T4> x, T1 y) => !(x == y);
        public static bool operator ==(Either<T1, T2, T3, T4> x, T1 y) => x.Equals(y);

        public static bool operator !=(T2 x, Either<T1, T2, T3, T4> y) => !(x == y);
        public static bool operator ==(T2 x, Either<T1, T2, T3, T4> y) => y.Equals(x);
        public static bool operator !=(Either<T1, T2, T3, T4> x, T2 y) => !(x == y);
        public static bool operator ==(Either<T1, T2, T3, T4> x, T2 y) => x.Equals(y);

        public static bool operator !=(T3 x, Either<T1, T2, T3, T4> y) => !(x == y);
        public static bool operator ==(T3 x, Either<T1, T2, T3, T4> y) => y.Equals(x);
        public static bool operator !=(Either<T1, T2, T3, T4> x, T3 y) => !(x == y);
        public static bool operator ==(Either<T1, T2, T3, T4> x, T3 y) => x.Equals(y);

        public static bool operator !=(T4 x, Either<T1, T2, T3, T4> y) => !(x == y);
        public static bool operator ==(T4 x, Either<T1, T2, T3, T4> y) => y.Equals(x);
        public static bool operator !=(Either<T1, T2, T3, T4> x, T4 y) => !(x == y);
        public static bool operator ==(Either<T1, T2, T3, T4> x, T4 y) => x.Equals(y);

        public bool Equals(T1 other) => new Either<T1, T2, T3, T4>(other).Equals(this);
        public bool Equals(T2 other) => new Either<T1, T2, T3, T4>(other).Equals(this);
        public bool Equals(T3 other) => new Either<T1, T2, T3, T4>(other).Equals(this);
        public bool Equals(T4 other) => new Either<T1, T2, T3, T4>(other).Equals(this);

        public override bool Equals(object obj)
        {
            return obj switch
            {
                Either<T1, T2, T3, T4> other => Equals(other),
                T1 t1 => Equals(t1),
                T2 t2 => Equals(t2),
                T3 t3 => Equals(t3),
                T4 t4 => Equals(t4),
                _ => false
            };
        }

        public bool Equals(Either<T1, T2, T3, T4> other)
        {
            if (_tag != other._tag) return false;

            if (_tag == 1) return EqualityComparer<T1>.Default.Equals(_t1, other._t1);
            if (_tag == 2) return EqualityComparer<T2>.Default.Equals(_t2, other._t2);
            if (_tag == 3) return EqualityComparer<T3>.Default.Equals(_t3, other._t3);
            if (_tag == 4) return EqualityComparer<T4>.Default.Equals(_t4, other._t4);

            return _tag == 0 && other._tag == 0;
        }

        public override int GetHashCode()
        {
            if (_tag == 0) return 0;

            if (_tag == 1) return _t1?.GetHashCode() ?? 0;
            if (_tag == 2) return _t2?.GetHashCode() ?? 0;
            if (_tag == 3) return _t3?.GetHashCode() ?? 0;
            if (_tag == 4) return _t4?.GetHashCode() ?? 0;

            return 0;
        }
    }

    /// <summary>
    /// This type holds a single instance of a selection of types.
    /// <para>While Either can take on more than one type, it is only ever a single type at a time.</para>
    /// </summary>
    public readonly struct Either<T1, T2, T3, T4, T5> : IEquatable<Either<T1, T2, T3, T4, T5>>
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

        public bool TryGetT1(out T1 t1) { if (_tag == 1) { t1 = _t1; return true; }; t1 = default; return false; }
        public bool TryGetT2(out T2 t2) { if (_tag == 2) { t2 = _t2; return true; }; t2 = default; return false; }
        public bool TryGetT3(out T3 t3) { if (_tag == 3) { t3 = _t3; return true; }; t3 = default; return false; }
        public bool TryGetT4(out T4 t4) { if (_tag == 4) { t4 = _t4; return true; }; t4 = default; return false; }
        public bool TryGetT5(out T5 t5) { if (_tag == 5) { t5 = _t5; return true; }; t5 = default; return false; }

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

        public static implicit operator Either<T1, T2, T3, T4, T5>(T1 value) => new Either<T1, T2, T3, T4, T5>(value);
        public static implicit operator Either<T1, T2, T3, T4, T5>(T2 value) => new Either<T1, T2, T3, T4, T5>(value);
        public static implicit operator Either<T1, T2, T3, T4, T5>(T3 value) => new Either<T1, T2, T3, T4, T5>(value);
        public static implicit operator Either<T1, T2, T3, T4, T5>(T4 value) => new Either<T1, T2, T3, T4, T5>(value);
        public static implicit operator Either<T1, T2, T3, T4, T5>(T5 value) => new Either<T1, T2, T3, T4, T5>(value);

        public static bool operator !=(Either<T1, T2, T3, T4, T5> x, Either<T1, T2, T3, T4, T5> y) => !(x == y);
        public static bool operator ==(Either<T1, T2, T3, T4, T5> x, Either<T1, T2, T3, T4, T5> y) => x.Equals(y);

        public static bool operator !=(T1 x, Either<T1, T2, T3, T4, T5> y) => !(x == y);
        public static bool operator ==(T1 x, Either<T1, T2, T3, T4, T5> y) => y.Equals(x);
        public static bool operator !=(Either<T1, T2, T3, T4, T5> x, T1 y) => !(x == y);
        public static bool operator ==(Either<T1, T2, T3, T4, T5> x, T1 y) => x.Equals(y);

        public static bool operator !=(T2 x, Either<T1, T2, T3, T4, T5> y) => !(x == y);
        public static bool operator ==(T2 x, Either<T1, T2, T3, T4, T5> y) => y.Equals(x);
        public static bool operator !=(Either<T1, T2, T3, T4, T5> x, T2 y) => !(x == y);
        public static bool operator ==(Either<T1, T2, T3, T4, T5> x, T2 y) => x.Equals(y);

        public static bool operator !=(T3 x, Either<T1, T2, T3, T4, T5> y) => !(x == y);
        public static bool operator ==(T3 x, Either<T1, T2, T3, T4, T5> y) => y.Equals(x);
        public static bool operator !=(Either<T1, T2, T3, T4, T5> x, T3 y) => !(x == y);
        public static bool operator ==(Either<T1, T2, T3, T4, T5> x, T3 y) => x.Equals(y);

        public static bool operator !=(T4 x, Either<T1, T2, T3, T4, T5> y) => !(x == y);
        public static bool operator ==(T4 x, Either<T1, T2, T3, T4, T5> y) => y.Equals(x);
        public static bool operator !=(Either<T1, T2, T3, T4, T5> x, T4 y) => !(x == y);
        public static bool operator ==(Either<T1, T2, T3, T4, T5> x, T4 y) => x.Equals(y);

        public static bool operator !=(T5 x, Either<T1, T2, T3, T4, T5> y) => !(x == y);
        public static bool operator ==(T5 x, Either<T1, T2, T3, T4, T5> y) => y.Equals(x);
        public static bool operator !=(Either<T1, T2, T3, T4, T5> x, T5 y) => !(x == y);
        public static bool operator ==(Either<T1, T2, T3, T4, T5> x, T5 y) => x.Equals(y);

        public bool Equals(T1 other) => new Either<T1, T2, T3, T4, T5>(other).Equals(this);
        public bool Equals(T2 other) => new Either<T1, T2, T3, T4, T5>(other).Equals(this);
        public bool Equals(T3 other) => new Either<T1, T2, T3, T4, T5>(other).Equals(this);
        public bool Equals(T4 other) => new Either<T1, T2, T3, T4, T5>(other).Equals(this);
        public bool Equals(T5 other) => new Either<T1, T2, T3, T4, T5>(other).Equals(this);

        public override bool Equals(object obj)
        {
            return obj switch
            {
                Either<T1, T2, T3, T4, T5> other => Equals(other),
                T1 t1 => Equals(t1),
                T2 t2 => Equals(t2),
                T3 t3 => Equals(t3),
                T4 t4 => Equals(t4),
                T5 t5 => Equals(t5),
                _ => false
            };
        }

        public bool Equals(Either<T1, T2, T3, T4, T5> other)
        {
            if (_tag != other._tag) return false;

            if (_tag == 1) return EqualityComparer<T1>.Default.Equals(_t1, other._t1);
            if (_tag == 2) return EqualityComparer<T2>.Default.Equals(_t2, other._t2);
            if (_tag == 3) return EqualityComparer<T3>.Default.Equals(_t3, other._t3);
            if (_tag == 4) return EqualityComparer<T4>.Default.Equals(_t4, other._t4);
            if (_tag == 5) return EqualityComparer<T5>.Default.Equals(_t5, other._t5);

            return _tag == 0 && other._tag == 0;
        }

        public override int GetHashCode()
        {
            if (_tag == 0) return 0;

            if (_tag == 1) return _t1?.GetHashCode() ?? 0;
            if (_tag == 2) return _t2?.GetHashCode() ?? 0;
            if (_tag == 3) return _t3?.GetHashCode() ?? 0;
            if (_tag == 4) return _t4?.GetHashCode() ?? 0;
            if (_tag == 5) return _t5?.GetHashCode() ?? 0;

            return 0;
        }
    }

    /// <summary>
    /// This type holds a single instance of a selection of types.
    /// <para>While Either can take on more than one type, it is only ever a single type at a time.</para>
    /// </summary>
    public readonly struct Either<T1, T2, T3, T4, T5, T6> : IEquatable<Either<T1, T2, T3, T4, T5, T6>>
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

        public bool TryGetT1(out T1 t1) { if (_tag == 1) { t1 = _t1; return true; }; t1 = default; return false; }
        public bool TryGetT2(out T2 t2) { if (_tag == 2) { t2 = _t2; return true; }; t2 = default; return false; }
        public bool TryGetT3(out T3 t3) { if (_tag == 3) { t3 = _t3; return true; }; t3 = default; return false; }
        public bool TryGetT4(out T4 t4) { if (_tag == 4) { t4 = _t4; return true; }; t4 = default; return false; }
        public bool TryGetT5(out T5 t5) { if (_tag == 5) { t5 = _t5; return true; }; t5 = default; return false; }
        public bool TryGetT6(out T6 t6) { if (_tag == 6) { t6 = _t6; return true; }; t6 = default; return false; }

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

        public static implicit operator Either<T1, T2, T3, T4, T5, T6>(T1 value) => new Either<T1, T2, T3, T4, T5, T6>(value);
        public static implicit operator Either<T1, T2, T3, T4, T5, T6>(T2 value) => new Either<T1, T2, T3, T4, T5, T6>(value);
        public static implicit operator Either<T1, T2, T3, T4, T5, T6>(T3 value) => new Either<T1, T2, T3, T4, T5, T6>(value);
        public static implicit operator Either<T1, T2, T3, T4, T5, T6>(T4 value) => new Either<T1, T2, T3, T4, T5, T6>(value);
        public static implicit operator Either<T1, T2, T3, T4, T5, T6>(T5 value) => new Either<T1, T2, T3, T4, T5, T6>(value);
        public static implicit operator Either<T1, T2, T3, T4, T5, T6>(T6 value) => new Either<T1, T2, T3, T4, T5, T6>(value);

        public static bool operator !=(Either<T1, T2, T3, T4, T5, T6> x, Either<T1, T2, T3, T4, T5, T6> y) => !(x == y);
        public static bool operator ==(Either<T1, T2, T3, T4, T5, T6> x, Either<T1, T2, T3, T4, T5, T6> y) => x.Equals(y);

        public static bool operator !=(T1 x, Either<T1, T2, T3, T4, T5, T6> y) => !(x == y);
        public static bool operator ==(T1 x, Either<T1, T2, T3, T4, T5, T6> y) => y.Equals(x);
        public static bool operator !=(Either<T1, T2, T3, T4, T5, T6> x, T1 y) => !(x == y);
        public static bool operator ==(Either<T1, T2, T3, T4, T5, T6> x, T1 y) => x.Equals(y);

        public static bool operator !=(T2 x, Either<T1, T2, T3, T4, T5, T6> y) => !(x == y);
        public static bool operator ==(T2 x, Either<T1, T2, T3, T4, T5, T6> y) => y.Equals(x);
        public static bool operator !=(Either<T1, T2, T3, T4, T5, T6> x, T2 y) => !(x == y);
        public static bool operator ==(Either<T1, T2, T3, T4, T5, T6> x, T2 y) => x.Equals(y);

        public static bool operator !=(T3 x, Either<T1, T2, T3, T4, T5, T6> y) => !(x == y);
        public static bool operator ==(T3 x, Either<T1, T2, T3, T4, T5, T6> y) => y.Equals(x);
        public static bool operator !=(Either<T1, T2, T3, T4, T5, T6> x, T3 y) => !(x == y);
        public static bool operator ==(Either<T1, T2, T3, T4, T5, T6> x, T3 y) => x.Equals(y);

        public static bool operator !=(T4 x, Either<T1, T2, T3, T4, T5, T6> y) => !(x == y);
        public static bool operator ==(T4 x, Either<T1, T2, T3, T4, T5, T6> y) => y.Equals(x);
        public static bool operator !=(Either<T1, T2, T3, T4, T5, T6> x, T4 y) => !(x == y);
        public static bool operator ==(Either<T1, T2, T3, T4, T5, T6> x, T4 y) => x.Equals(y);

        public static bool operator !=(T5 x, Either<T1, T2, T3, T4, T5, T6> y) => !(x == y);
        public static bool operator ==(T5 x, Either<T1, T2, T3, T4, T5, T6> y) => y.Equals(x);
        public static bool operator !=(Either<T1, T2, T3, T4, T5, T6> x, T5 y) => !(x == y);
        public static bool operator ==(Either<T1, T2, T3, T4, T5, T6> x, T5 y) => x.Equals(y);

        public static bool operator !=(T6 x, Either<T1, T2, T3, T4, T5, T6> y) => !(x == y);
        public static bool operator ==(T6 x, Either<T1, T2, T3, T4, T5, T6> y) => y.Equals(x);
        public static bool operator !=(Either<T1, T2, T3, T4, T5, T6> x, T6 y) => !(x == y);
        public static bool operator ==(Either<T1, T2, T3, T4, T5, T6> x, T6 y) => x.Equals(y);

        public bool Equals(T1 other) => new Either<T1, T2, T3, T4, T5, T6>(other).Equals(this);
        public bool Equals(T2 other) => new Either<T1, T2, T3, T4, T5, T6>(other).Equals(this);
        public bool Equals(T3 other) => new Either<T1, T2, T3, T4, T5, T6>(other).Equals(this);
        public bool Equals(T4 other) => new Either<T1, T2, T3, T4, T5, T6>(other).Equals(this);
        public bool Equals(T5 other) => new Either<T1, T2, T3, T4, T5, T6>(other).Equals(this);
        public bool Equals(T6 other) => new Either<T1, T2, T3, T4, T5, T6>(other).Equals(this);

        public override bool Equals(object obj)
        {
            return obj switch
            {
                Either<T1, T2, T3, T4, T5, T6> other => Equals(other),
                T1 t1 => Equals(t1),
                T2 t2 => Equals(t2),
                T3 t3 => Equals(t3),
                T4 t4 => Equals(t4),
                T5 t5 => Equals(t5),
                T6 t6 => Equals(t6),
                _ => false
            };
        }

        public bool Equals(Either<T1, T2, T3, T4, T5, T6> other)
        {
            if (_tag != other._tag) return false;

            if (_tag == 1) return EqualityComparer<T1>.Default.Equals(_t1, other._t1);
            if (_tag == 2) return EqualityComparer<T2>.Default.Equals(_t2, other._t2);
            if (_tag == 3) return EqualityComparer<T3>.Default.Equals(_t3, other._t3);
            if (_tag == 4) return EqualityComparer<T4>.Default.Equals(_t4, other._t4);
            if (_tag == 5) return EqualityComparer<T5>.Default.Equals(_t5, other._t5);
            if (_tag == 6) return EqualityComparer<T6>.Default.Equals(_t6, other._t6);

            return _tag == 0 && other._tag == 0;
        }

        public override int GetHashCode()
        {
            if (_tag == 0) return 0;

            if (_tag == 1) return _t1?.GetHashCode() ?? 0;
            if (_tag == 2) return _t2?.GetHashCode() ?? 0;
            if (_tag == 3) return _t3?.GetHashCode() ?? 0;
            if (_tag == 4) return _t4?.GetHashCode() ?? 0;
            if (_tag == 5) return _t5?.GetHashCode() ?? 0;
            if (_tag == 6) return _t6?.GetHashCode() ?? 0;

            return 0;
        }
    }

    /// <summary>
    /// This type holds a single instance of a selection of types.
    /// <para>While Either can take on more than one type, it is only ever a single type at a time.</para>
    /// </summary>
    public readonly struct Either<T1, T2, T3, T4, T5, T6, T7> : IEquatable<Either<T1, T2, T3, T4, T5, T6, T7>>
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

        public bool TryGetT1(out T1 t1) { if (_tag == 1) { t1 = _t1; return true; }; t1 = default; return false; }
        public bool TryGetT2(out T2 t2) { if (_tag == 2) { t2 = _t2; return true; }; t2 = default; return false; }
        public bool TryGetT3(out T3 t3) { if (_tag == 3) { t3 = _t3; return true; }; t3 = default; return false; }
        public bool TryGetT4(out T4 t4) { if (_tag == 4) { t4 = _t4; return true; }; t4 = default; return false; }
        public bool TryGetT5(out T5 t5) { if (_tag == 5) { t5 = _t5; return true; }; t5 = default; return false; }
        public bool TryGetT6(out T6 t6) { if (_tag == 6) { t6 = _t6; return true; }; t6 = default; return false; }
        public bool TryGetT7(out T7 t7) { if (_tag == 7) { t7 = _t7; return true; }; t7 = default; return false; }

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

        public static implicit operator Either<T1, T2, T3, T4, T5, T6, T7>(T1 value) => new Either<T1, T2, T3, T4, T5, T6, T7>(value);
        public static implicit operator Either<T1, T2, T3, T4, T5, T6, T7>(T2 value) => new Either<T1, T2, T3, T4, T5, T6, T7>(value);
        public static implicit operator Either<T1, T2, T3, T4, T5, T6, T7>(T3 value) => new Either<T1, T2, T3, T4, T5, T6, T7>(value);
        public static implicit operator Either<T1, T2, T3, T4, T5, T6, T7>(T4 value) => new Either<T1, T2, T3, T4, T5, T6, T7>(value);
        public static implicit operator Either<T1, T2, T3, T4, T5, T6, T7>(T5 value) => new Either<T1, T2, T3, T4, T5, T6, T7>(value);
        public static implicit operator Either<T1, T2, T3, T4, T5, T6, T7>(T6 value) => new Either<T1, T2, T3, T4, T5, T6, T7>(value);
        public static implicit operator Either<T1, T2, T3, T4, T5, T6, T7>(T7 value) => new Either<T1, T2, T3, T4, T5, T6, T7>(value);

        public static bool operator !=(Either<T1, T2, T3, T4, T5, T6, T7> x, Either<T1, T2, T3, T4, T5, T6, T7> y) => !(x == y);
        public static bool operator ==(Either<T1, T2, T3, T4, T5, T6, T7> x, Either<T1, T2, T3, T4, T5, T6, T7> y) => x.Equals(y);

        public static bool operator !=(T1 x, Either<T1, T2, T3, T4, T5, T6, T7> y) => !(x == y);
        public static bool operator ==(T1 x, Either<T1, T2, T3, T4, T5, T6, T7> y) => y.Equals(x);
        public static bool operator !=(Either<T1, T2, T3, T4, T5, T6, T7> x, T1 y) => !(x == y);
        public static bool operator ==(Either<T1, T2, T3, T4, T5, T6, T7> x, T1 y) => x.Equals(y);

        public static bool operator !=(T2 x, Either<T1, T2, T3, T4, T5, T6, T7> y) => !(x == y);
        public static bool operator ==(T2 x, Either<T1, T2, T3, T4, T5, T6, T7> y) => y.Equals(x);
        public static bool operator !=(Either<T1, T2, T3, T4, T5, T6, T7> x, T2 y) => !(x == y);
        public static bool operator ==(Either<T1, T2, T3, T4, T5, T6, T7> x, T2 y) => x.Equals(y);

        public static bool operator !=(T3 x, Either<T1, T2, T3, T4, T5, T6, T7> y) => !(x == y);
        public static bool operator ==(T3 x, Either<T1, T2, T3, T4, T5, T6, T7> y) => y.Equals(x);
        public static bool operator !=(Either<T1, T2, T3, T4, T5, T6, T7> x, T3 y) => !(x == y);
        public static bool operator ==(Either<T1, T2, T3, T4, T5, T6, T7> x, T3 y) => x.Equals(y);

        public static bool operator !=(T4 x, Either<T1, T2, T3, T4, T5, T6, T7> y) => !(x == y);
        public static bool operator ==(T4 x, Either<T1, T2, T3, T4, T5, T6, T7> y) => y.Equals(x);
        public static bool operator !=(Either<T1, T2, T3, T4, T5, T6, T7> x, T4 y) => !(x == y);
        public static bool operator ==(Either<T1, T2, T3, T4, T5, T6, T7> x, T4 y) => x.Equals(y);

        public static bool operator !=(T5 x, Either<T1, T2, T3, T4, T5, T6, T7> y) => !(x == y);
        public static bool operator ==(T5 x, Either<T1, T2, T3, T4, T5, T6, T7> y) => y.Equals(x);
        public static bool operator !=(Either<T1, T2, T3, T4, T5, T6, T7> x, T5 y) => !(x == y);
        public static bool operator ==(Either<T1, T2, T3, T4, T5, T6, T7> x, T5 y) => x.Equals(y);

        public static bool operator !=(T6 x, Either<T1, T2, T3, T4, T5, T6, T7> y) => !(x == y);
        public static bool operator ==(T6 x, Either<T1, T2, T3, T4, T5, T6, T7> y) => y.Equals(x);
        public static bool operator !=(Either<T1, T2, T3, T4, T5, T6, T7> x, T6 y) => !(x == y);
        public static bool operator ==(Either<T1, T2, T3, T4, T5, T6, T7> x, T6 y) => x.Equals(y);

        public static bool operator !=(T7 x, Either<T1, T2, T3, T4, T5, T6, T7> y) => !(x == y);
        public static bool operator ==(T7 x, Either<T1, T2, T3, T4, T5, T6, T7> y) => y.Equals(x);
        public static bool operator !=(Either<T1, T2, T3, T4, T5, T6, T7> x, T7 y) => !(x == y);
        public static bool operator ==(Either<T1, T2, T3, T4, T5, T6, T7> x, T7 y) => x.Equals(y);

        public bool Equals(T1 other) => new Either<T1, T2, T3, T4, T5, T6, T7>(other).Equals(this);
        public bool Equals(T2 other) => new Either<T1, T2, T3, T4, T5, T6, T7>(other).Equals(this);
        public bool Equals(T3 other) => new Either<T1, T2, T3, T4, T5, T6, T7>(other).Equals(this);
        public bool Equals(T4 other) => new Either<T1, T2, T3, T4, T5, T6, T7>(other).Equals(this);
        public bool Equals(T5 other) => new Either<T1, T2, T3, T4, T5, T6, T7>(other).Equals(this);
        public bool Equals(T6 other) => new Either<T1, T2, T3, T4, T5, T6, T7>(other).Equals(this);
        public bool Equals(T7 other) => new Either<T1, T2, T3, T4, T5, T6, T7>(other).Equals(this);

        public override bool Equals(object obj)
        {
            return obj switch
            {
                Either<T1, T2, T3, T4, T5, T6, T7> other => Equals(other),
                T1 t1 => Equals(t1),
                T2 t2 => Equals(t2),
                T3 t3 => Equals(t3),
                T4 t4 => Equals(t4),
                T5 t5 => Equals(t5),
                T6 t6 => Equals(t6),
                T7 t7 => Equals(t7),
                _ => false
            };
        }

        public bool Equals(Either<T1, T2, T3, T4, T5, T6, T7> other)
        {
            if (_tag != other._tag) return false;

            if (_tag == 1) return EqualityComparer<T1>.Default.Equals(_t1, other._t1);
            if (_tag == 2) return EqualityComparer<T2>.Default.Equals(_t2, other._t2);
            if (_tag == 3) return EqualityComparer<T3>.Default.Equals(_t3, other._t3);
            if (_tag == 4) return EqualityComparer<T4>.Default.Equals(_t4, other._t4);
            if (_tag == 5) return EqualityComparer<T5>.Default.Equals(_t5, other._t5);
            if (_tag == 6) return EqualityComparer<T6>.Default.Equals(_t6, other._t6);
            if (_tag == 7) return EqualityComparer<T7>.Default.Equals(_t7, other._t7);

            return _tag == 0 && other._tag == 0;
        }

        public override int GetHashCode()
        {
            if (_tag == 0) return 0;

            if (_tag == 1) return _t1?.GetHashCode() ?? 0;
            if (_tag == 2) return _t2?.GetHashCode() ?? 0;
            if (_tag == 3) return _t3?.GetHashCode() ?? 0;
            if (_tag == 4) return _t4?.GetHashCode() ?? 0;
            if (_tag == 5) return _t5?.GetHashCode() ?? 0;
            if (_tag == 6) return _t6?.GetHashCode() ?? 0;
            if (_tag == 7) return _t7?.GetHashCode() ?? 0;

            return 0;
        }
    }

    /// <summary>
    /// This type holds a single instance of a selection of types.
    /// <para>While Either can take on more than one type, it is only ever a single type at a time.</para>
    /// </summary>
    public readonly struct Either<T1, T2, T3, T4, T5, T6, T7, T8> : IEquatable<Either<T1, T2, T3, T4, T5, T6, T7, T8>>
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

        public bool TryGetT1(out T1 t1) { if (_tag == 1) { t1 = _t1; return true; }; t1 = default; return false; }
        public bool TryGetT2(out T2 t2) { if (_tag == 2) { t2 = _t2; return true; }; t2 = default; return false; }
        public bool TryGetT3(out T3 t3) { if (_tag == 3) { t3 = _t3; return true; }; t3 = default; return false; }
        public bool TryGetT4(out T4 t4) { if (_tag == 4) { t4 = _t4; return true; }; t4 = default; return false; }
        public bool TryGetT5(out T5 t5) { if (_tag == 5) { t5 = _t5; return true; }; t5 = default; return false; }
        public bool TryGetT6(out T6 t6) { if (_tag == 6) { t6 = _t6; return true; }; t6 = default; return false; }
        public bool TryGetT7(out T7 t7) { if (_tag == 7) { t7 = _t7; return true; }; t7 = default; return false; }
        public bool TryGetT8(out T8 t8) { if (_tag == 8) { t8 = _t8; return true; }; t8 = default; return false; }

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

        public static implicit operator Either<T1, T2, T3, T4, T5, T6, T7, T8>(T1 value) => new Either<T1, T2, T3, T4, T5, T6, T7, T8>(value);
        public static implicit operator Either<T1, T2, T3, T4, T5, T6, T7, T8>(T2 value) => new Either<T1, T2, T3, T4, T5, T6, T7, T8>(value);
        public static implicit operator Either<T1, T2, T3, T4, T5, T6, T7, T8>(T3 value) => new Either<T1, T2, T3, T4, T5, T6, T7, T8>(value);
        public static implicit operator Either<T1, T2, T3, T4, T5, T6, T7, T8>(T4 value) => new Either<T1, T2, T3, T4, T5, T6, T7, T8>(value);
        public static implicit operator Either<T1, T2, T3, T4, T5, T6, T7, T8>(T5 value) => new Either<T1, T2, T3, T4, T5, T6, T7, T8>(value);
        public static implicit operator Either<T1, T2, T3, T4, T5, T6, T7, T8>(T6 value) => new Either<T1, T2, T3, T4, T5, T6, T7, T8>(value);
        public static implicit operator Either<T1, T2, T3, T4, T5, T6, T7, T8>(T7 value) => new Either<T1, T2, T3, T4, T5, T6, T7, T8>(value);
        public static implicit operator Either<T1, T2, T3, T4, T5, T6, T7, T8>(T8 value) => new Either<T1, T2, T3, T4, T5, T6, T7, T8>(value);

        public static bool operator !=(Either<T1, T2, T3, T4, T5, T6, T7, T8> x, Either<T1, T2, T3, T4, T5, T6, T7, T8> y) => !(x == y);
        public static bool operator ==(Either<T1, T2, T3, T4, T5, T6, T7, T8> x, Either<T1, T2, T3, T4, T5, T6, T7, T8> y) => x.Equals(y);

        public static bool operator !=(T1 x, Either<T1, T2, T3, T4, T5, T6, T7, T8> y) => !(x == y);
        public static bool operator ==(T1 x, Either<T1, T2, T3, T4, T5, T6, T7, T8> y) => y.Equals(x);
        public static bool operator !=(Either<T1, T2, T3, T4, T5, T6, T7, T8> x, T1 y) => !(x == y);
        public static bool operator ==(Either<T1, T2, T3, T4, T5, T6, T7, T8> x, T1 y) => x.Equals(y);

        public static bool operator !=(T2 x, Either<T1, T2, T3, T4, T5, T6, T7, T8> y) => !(x == y);
        public static bool operator ==(T2 x, Either<T1, T2, T3, T4, T5, T6, T7, T8> y) => y.Equals(x);
        public static bool operator !=(Either<T1, T2, T3, T4, T5, T6, T7, T8> x, T2 y) => !(x == y);
        public static bool operator ==(Either<T1, T2, T3, T4, T5, T6, T7, T8> x, T2 y) => x.Equals(y);

        public static bool operator !=(T3 x, Either<T1, T2, T3, T4, T5, T6, T7, T8> y) => !(x == y);
        public static bool operator ==(T3 x, Either<T1, T2, T3, T4, T5, T6, T7, T8> y) => y.Equals(x);
        public static bool operator !=(Either<T1, T2, T3, T4, T5, T6, T7, T8> x, T3 y) => !(x == y);
        public static bool operator ==(Either<T1, T2, T3, T4, T5, T6, T7, T8> x, T3 y) => x.Equals(y);

        public static bool operator !=(T4 x, Either<T1, T2, T3, T4, T5, T6, T7, T8> y) => !(x == y);
        public static bool operator ==(T4 x, Either<T1, T2, T3, T4, T5, T6, T7, T8> y) => y.Equals(x);
        public static bool operator !=(Either<T1, T2, T3, T4, T5, T6, T7, T8> x, T4 y) => !(x == y);
        public static bool operator ==(Either<T1, T2, T3, T4, T5, T6, T7, T8> x, T4 y) => x.Equals(y);

        public static bool operator !=(T5 x, Either<T1, T2, T3, T4, T5, T6, T7, T8> y) => !(x == y);
        public static bool operator ==(T5 x, Either<T1, T2, T3, T4, T5, T6, T7, T8> y) => y.Equals(x);
        public static bool operator !=(Either<T1, T2, T3, T4, T5, T6, T7, T8> x, T5 y) => !(x == y);
        public static bool operator ==(Either<T1, T2, T3, T4, T5, T6, T7, T8> x, T5 y) => x.Equals(y);

        public static bool operator !=(T6 x, Either<T1, T2, T3, T4, T5, T6, T7, T8> y) => !(x == y);
        public static bool operator ==(T6 x, Either<T1, T2, T3, T4, T5, T6, T7, T8> y) => y.Equals(x);
        public static bool operator !=(Either<T1, T2, T3, T4, T5, T6, T7, T8> x, T6 y) => !(x == y);
        public static bool operator ==(Either<T1, T2, T3, T4, T5, T6, T7, T8> x, T6 y) => x.Equals(y);

        public static bool operator !=(T7 x, Either<T1, T2, T3, T4, T5, T6, T7, T8> y) => !(x == y);
        public static bool operator ==(T7 x, Either<T1, T2, T3, T4, T5, T6, T7, T8> y) => y.Equals(x);
        public static bool operator !=(Either<T1, T2, T3, T4, T5, T6, T7, T8> x, T7 y) => !(x == y);
        public static bool operator ==(Either<T1, T2, T3, T4, T5, T6, T7, T8> x, T7 y) => x.Equals(y);

        public static bool operator !=(T8 x, Either<T1, T2, T3, T4, T5, T6, T7, T8> y) => !(x == y);
        public static bool operator ==(T8 x, Either<T1, T2, T3, T4, T5, T6, T7, T8> y) => y.Equals(x);
        public static bool operator !=(Either<T1, T2, T3, T4, T5, T6, T7, T8> x, T8 y) => !(x == y);
        public static bool operator ==(Either<T1, T2, T3, T4, T5, T6, T7, T8> x, T8 y) => x.Equals(y);

        public bool Equals(T1 other) => new Either<T1, T2, T3, T4, T5, T6, T7, T8>(other).Equals(this);
        public bool Equals(T2 other) => new Either<T1, T2, T3, T4, T5, T6, T7, T8>(other).Equals(this);
        public bool Equals(T3 other) => new Either<T1, T2, T3, T4, T5, T6, T7, T8>(other).Equals(this);
        public bool Equals(T4 other) => new Either<T1, T2, T3, T4, T5, T6, T7, T8>(other).Equals(this);
        public bool Equals(T5 other) => new Either<T1, T2, T3, T4, T5, T6, T7, T8>(other).Equals(this);
        public bool Equals(T6 other) => new Either<T1, T2, T3, T4, T5, T6, T7, T8>(other).Equals(this);
        public bool Equals(T7 other) => new Either<T1, T2, T3, T4, T5, T6, T7, T8>(other).Equals(this);
        public bool Equals(T8 other) => new Either<T1, T2, T3, T4, T5, T6, T7, T8>(other).Equals(this);

        public override bool Equals(object obj)
        {
            return obj switch
            {
                Either<T1, T2, T3, T4, T5, T6, T7, T8> other => Equals(other),
                T1 t1 => Equals(t1),
                T2 t2 => Equals(t2),
                T3 t3 => Equals(t3),
                T4 t4 => Equals(t4),
                T5 t5 => Equals(t5),
                T6 t6 => Equals(t6),
                T7 t7 => Equals(t7),
                T8 t8 => Equals(t8),
                _ => false
            };
        }

        public bool Equals(Either<T1, T2, T3, T4, T5, T6, T7, T8> other)
        {
            if (_tag != other._tag) return false;

            if (_tag == 1) return EqualityComparer<T1>.Default.Equals(_t1, other._t1);
            if (_tag == 2) return EqualityComparer<T2>.Default.Equals(_t2, other._t2);
            if (_tag == 3) return EqualityComparer<T3>.Default.Equals(_t3, other._t3);
            if (_tag == 4) return EqualityComparer<T4>.Default.Equals(_t4, other._t4);
            if (_tag == 5) return EqualityComparer<T5>.Default.Equals(_t5, other._t5);
            if (_tag == 6) return EqualityComparer<T6>.Default.Equals(_t6, other._t6);
            if (_tag == 7) return EqualityComparer<T7>.Default.Equals(_t7, other._t7);
            if (_tag == 8) return EqualityComparer<T8>.Default.Equals(_t8, other._t8);

            return _tag == 0 && other._tag == 0;
        }

        public override int GetHashCode()
        {
            if (_tag == 0) return 0;

            if (_tag == 1) return _t1?.GetHashCode() ?? 0;
            if (_tag == 2) return _t2?.GetHashCode() ?? 0;
            if (_tag == 3) return _t3?.GetHashCode() ?? 0;
            if (_tag == 4) return _t4?.GetHashCode() ?? 0;
            if (_tag == 5) return _t5?.GetHashCode() ?? 0;
            if (_tag == 6) return _t6?.GetHashCode() ?? 0;
            if (_tag == 7) return _t7?.GetHashCode() ?? 0;
            if (_tag == 8) return _t8?.GetHashCode() ?? 0;

            return 0;
        }
    }

    #endregion
}
