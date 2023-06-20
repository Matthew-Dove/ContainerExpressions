using System;

namespace ContainerExpressions.Containers.Common.Models
{
    public readonly struct Parameters
    {
        public Func<TResult> ToFunc<TResult>(Func<TResult> func) => func;

        public Action ToAction(Action action) => action;
    }

    public readonly struct Parameters<T1>
    {
        private readonly T1 _t1;

        public Parameters(T1 t1)
        {
            _t1 = t1;
        }

        public Func<TResult> ToFunc<TResult>(Func<T1, TResult> func)
        {
            var t1 = _t1;
            return () => func(t1);
        }

        public Action ToAction(Action<T1> action)
        {
            var t1 = _t1;
            return () => action(t1);
        }
    }

    #region Parameters Permutations

    public readonly struct Parameters<T1, T2>
    {
        private readonly T1 _t1;
        private readonly T2 _t2;

        public Parameters(T1 t1, T2 t2)
        {
            _t1 = t1;
            _t2 = t2;
        }

        public Func<TResult> ToFunc<TResult>(Func<T1, T2, TResult> func)
        {
            var t1 = _t1;
            var t2 = _t2;
            return () => func(t1, t2);
        }

        public Action ToAction(Action<T1, T2> action)
        {
            var t1 = _t1;
            var t2 = _t2;
            return () => action(t1, t2);
        }
    }

    public readonly struct Parameters<T1, T2, T3>
    {
        private readonly T1 _t1;
        private readonly T2 _t2;
        private readonly T3 _t3;

        public Parameters(T1 t1, T2 t2, T3 t3)
        {
            _t1 = t1;
            _t2 = t2;
            _t3 = t3;
        }

        public Func<TResult> ToFunc<TResult>(Func<T1, T2, T3, TResult> func)
        {
            var t1 = _t1;
            var t2 = _t2;
            var t3 = _t3;
            return () => func(t1, t2, t3);
        }

        public Action ToAction(Action<T1, T2, T3> action)
        {
            var t1 = _t1;
            var t2 = _t2;
            var t3 = _t3;
            return () => action(t1, t2, t3);
        }
    }

    public readonly struct Parameters<T1, T2, T3, T4>
    {
        private readonly T1 _t1;
        private readonly T2 _t2;
        private readonly T3 _t3;
        private readonly T4 _t4;

        public Parameters(T1 t1, T2 t2, T3 t3, T4 t4)
        {
            _t1 = t1;
            _t2 = t2;
            _t3 = t3;
            _t4 = t4;
        }

        public Func<TResult> ToFunc<TResult>(Func<T1, T2, T3, T4, TResult> func)
        {
            var t1 = _t1;
            var t2 = _t2;
            var t3 = _t3;
            var t4 = _t4;
            return () => func(t1, t2, t3, t4);
        }

        public Action ToAction(Action<T1, T2, T3, T4> action)
        {
            var t1 = _t1;
            var t2 = _t2;
            var t3 = _t3;
            var t4 = _t4;
            return () => action(t1, t2, t3, t4);
        }
    }

    public readonly struct Parameters<T1, T2, T3, T4, T5>
    {
        private readonly T1 _t1;
        private readonly T2 _t2;
        private readonly T3 _t3;
        private readonly T4 _t4;
        private readonly T5 _t5;

        public Parameters(T1 t1, T2 t2, T3 t3, T4 t4, T5 t5)
        {
            _t1 = t1;
            _t2 = t2;
            _t3 = t3;
            _t4 = t4;
            _t5 = t5;
        }

        public Func<TResult> ToFunc<TResult>(Func<T1, T2, T3, T4, T5, TResult> func)
        {
            var t1 = _t1;
            var t2 = _t2;
            var t3 = _t3;
            var t4 = _t4;
            var t5 = _t5;
            return () => func(t1, t2, t3, t4, t5);
        }

        public Action ToAction(Action<T1, T2, T3, T4, T5> action)
        {
            var t1 = _t1;
            var t2 = _t2;
            var t3 = _t3;
            var t4 = _t4;
            var t5 = _t5;
            return () => action(t1, t2, t3, t4, t5);
        }
    }

    public readonly struct Parameters<T1, T2, T3, T4, T5, T6>
    {
        private readonly T1 _t1;
        private readonly T2 _t2;
        private readonly T3 _t3;
        private readonly T4 _t4;
        private readonly T5 _t5;
        private readonly T6 _t6;

        public Parameters(T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6)
        {
            _t1 = t1;
            _t2 = t2;
            _t3 = t3;
            _t4 = t4;
            _t5 = t5;
            _t6 = t6;
        }

        public Func<TResult> ToFunc<TResult>(Func<T1, T2, T3, T4, T5, T6, TResult> func)
        {
            var t1 = _t1;
            var t2 = _t2;
            var t3 = _t3;
            var t4 = _t4;
            var t5 = _t5;
            var t6 = _t6;
            return () => func(t1, t2, t3, t4, t5, t6);
        }

        public Action ToAction(Action<T1, T2, T3, T4, T5, T6> action)
        {
            var t1 = _t1;
            var t2 = _t2;
            var t3 = _t3;
            var t4 = _t4;
            var t5 = _t5;
            var t6 = _t6;
            return () => action(t1, t2, t3, t4, t5, t6);
        }
    }

    public readonly struct Parameters<T1, T2, T3, T4, T5, T6, T7>
    {
        private readonly T1 _t1;
        private readonly T2 _t2;
        private readonly T3 _t3;
        private readonly T4 _t4;
        private readonly T5 _t5;
        private readonly T6 _t6;
        private readonly T7 _t7;

        public Parameters(T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7)
        {
            _t1 = t1;
            _t2 = t2;
            _t3 = t3;
            _t4 = t4;
            _t5 = t5;
            _t6 = t6;
            _t7 = t7;
        }

        public Func<TResult> ToFunc<TResult>(Func<T1, T2, T3, T4, T5, T6, T7, TResult> func)
        {
            var t1 = _t1;
            var t2 = _t2;
            var t3 = _t3;
            var t4 = _t4;
            var t5 = _t5;
            var t6 = _t6;
            var t7 = _t7;
            return () => func(t1, t2, t3, t4, t5, t6, t7);
        }

        public Action ToAction(Action<T1, T2, T3, T4, T5, T6, T7> action)
        {
            var t1 = _t1;
            var t2 = _t2;
            var t3 = _t3;
            var t4 = _t4;
            var t5 = _t5;
            var t6 = _t6;
            var t7 = _t7;
            return () => action(t1, t2, t3, t4, t5, t6, t7);
        }
    }

    public readonly struct Parameters<T1, T2, T3, T4, T5, T6, T7, T8>
    {
        private readonly T1 _t1;
        private readonly T2 _t2;
        private readonly T3 _t3;
        private readonly T4 _t4;
        private readonly T5 _t5;
        private readonly T6 _t6;
        private readonly T7 _t7;
        private readonly T8 _t8;

        public Parameters(T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8)
        {
            _t1 = t1;
            _t2 = t2;
            _t3 = t3;
            _t4 = t4;
            _t5 = t5;
            _t6 = t6;
            _t7 = t7;
            _t8 = t8;
        }

        public Func<TResult> ToFunc<TResult>(Func<T1, T2, T3, T4, T5, T6, T7, T8, TResult> func)
        {
            var t1 = _t1;
            var t2 = _t2;
            var t3 = _t3;
            var t4 = _t4;
            var t5 = _t5;
            var t6 = _t6;
            var t7 = _t7;
            var t8 = _t8;
            return () => func(t1, t2, t3, t4, t5, t6, t7, t8);
        }

        public Action ToAction(Action<T1, T2, T3, T4, T5, T6, T7, T8> action)
        {
            var t1 = _t1;
            var t2 = _t2;
            var t3 = _t3;
            var t4 = _t4;
            var t5 = _t5;
            var t6 = _t6;
            var t7 = _t7;
            var t8 = _t8;
            return () => action(t1, t2, t3, t4, t5, t6, t7, t8);
        }
    }

    #endregion
}
