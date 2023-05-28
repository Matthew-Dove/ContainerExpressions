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

        private readonly T1 _t1;

        public Parameters(T1 t1)
        {
            _t1 = t1;
        }
    }

    #region Parameters Permutations

    public readonly struct Parameters<T1, T2>
    {
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

        private readonly T1 _t1;
        private readonly T2 _t2;

        public Parameters(T1 t1, T2 t2)
        {
            _t1 = t1;
            _t2 = t2;
        }
    }

    #endregion
}
