using ContainerExpressions.Containers.Common.Models;
using System;
using System.Threading.Tasks;

namespace ContainerExpressions.Containers
{
    /// <summary>A place to put shared common functions.</summary>
    public static class Lambda
    {
        /// <summary>Maps the input directly to the output.</summary>
        public static T Identity<T>(T x) => x;

        /// <summary>Maps the input to the output, after wrapping it in a task.</summary>
        public static Task<T> IdentityAsync<T>(T x) => Task.FromResult(x);

        /// <summary>Pretends to return void (i.e. to compile), but will really throw the passed exception.</summary>
        public static void Throw(Exception ex) => throw ex;

        /// <summary>Pretends to return a T (i.e. to compile), but will really throw the passed exception.</summary>
        public static T Throw<T>(Exception ex) => throw ex;

        /// <summary>Pretends to return task (i.e. to compile), but will really throw the passed exception.</summary>
        public static Task ThrowAsync(Exception ex) => throw ex;

        /// <summary>Pretends to return a Task{T} (i.e. to compile), but will really throw the passed exception.</summary>
        public static Task<T> ThrowAsync<T>(Exception ex) => throw ex;

        /// <summary>Discards the function input, and returns the specified result.</summary>
        public static Func<T, T> Default<T>(T result = default) => _ => result;

        /// <summary>Discards the function input, and returns the specified result wrapped in a task.</summary>
        public static Func<T, Task<T>> DefaultAsync<T>(T result = default) => _ => Task.FromResult(result);

        /// <summary>Discards the function input, and returns the specified result (of a different type to the input type).</summary>
        public static Func<T, TResult> Default<T, TResult>(TResult result = default) => _ => result;

        /// <summary>Discards the function input, and returns the specified result wrapped in a task (of a different type to the input type).</summary>
        public static Func<T, Task<TResult>> DefaultAsync<T, TResult>(TResult result = default) => _ => Task.FromResult(result);

        // Wrap method delegates, so the complier can recognise them as Func{TResult} types. 
        public static Func<TResult> ToFunc<TResult>(this Func<TResult> func) => func;

        // Wrap method delegates, so the complier can recognise them as Action types. 
        public static Action ToAction(this Action action) => action;

        // Create a function wrapper that takes 0 args.
        public static Parameters Args() => new();

        // Create a function wrapper that takes 1 arg.
        public static Parameters<T1> Args<T1>(T1 t1) => new(t1);

        #region Parameters Permutations

        // Create a function wrapper that takes n args.
        public static Parameters<T1, T2> Args<T1, T2>(T1 t1, T2 t2) => new(t1, t2);

        public static Parameters<T1, T2, T3> Args<T1, T2, T3>(T1 t1, T2 t2, T3 t3) => new(t1, t2, t3);

        public static Parameters<T1, T2, T3, T4> Args<T1, T2, T3, T4>(T1 t1, T2 t2, T3 t3, T4 t4) => new(t1, t2, t3, t4);

        public static Parameters<T1, T2, T3, T4, T5> Args<T1, T2, T3, T4, T5>(T1 t1, T2 t2, T3 t3, T4 t4, T5 t5) => new(t1, t2, t3, t4, t5);

        public static Parameters<T1, T2, T3, T4, T5, T6> Args<T1, T2, T3, T4, T5, T6>(T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6) => new(t1, t2, t3, t4, t5, t6);

        public static Parameters<T1, T2, T3, T4, T5, T6, T7> Args<T1, T2, T3, T4, T5, T6, T7>(T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7) => new(t1, t2, t3, t4, t5, t6, t7);

        public static Parameters<T1, T2, T3, T4, T5, T6, T7, T8> Args<T1, T2, T3, T4, T5, T6, T7, T8>(T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8) => new(t1, t2, t3, t4, t5, t6, t7, t8);

        #endregion
    }
}
