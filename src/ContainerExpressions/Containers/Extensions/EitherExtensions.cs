using System;
using System.Threading.Tasks;

namespace ContainerExpressions.Containers;

public static class EitherExtensions
{
    private const string _invalidOperationMessage = "The internal type was not set, you must assign Either a type at least once.";

    // TryGetT#

    public static bool TryGetT1<T1, T2>(this Either<T1, T2> either, out T1 t1) { if (either._tag == 1) { t1 = either._t1; return true; }; t1 = default; return false; }
    public static bool TryGetT2<T1, T2>(this Either<T1, T2> either, out T2 t2) { if (either._tag == 2) { t2 = either._t2; return true; }; t2 = default; return false; }

    // WhenT# Func

    public static bool WhenT1<T1, T2, TResult>(this Either<T1, T2> either, Func<T1, TResult> func, out TResult result) { if (either._tag == 1) { result = func(either._t1); return true; }; result = default; return false; }
    public static bool WhenT2<T1, T2, TResult>(this Either<T1, T2> either, Func<T2, TResult> func, out TResult result) { if (either._tag == 2) { result = func(either._t2); return true; }; result = default; return false; }

    public static bool WhenAsyncT1<T1, T2, TResult>(this Either<T1, T2> either, Func<T1, Task<TResult>> func, out Task<TResult> result) { if (either._tag == 1) { result = func(either._t1); return true; }; result = default; return false; }
    public static bool WhenAsyncT2<T1, T2, TResult>(this Either<T1, T2> either, Func<T2, Task<TResult>> func, out Task<TResult> result) { if (either._tag == 2) { result = func(either._t2); return true; }; result = default; return false; }

    // WhenT# Action

    public static bool WhenT1<T1, T2>(this Either<T1, T2> either, Action<T1> action) { if (either._tag == 1) { action(either._t1); return true; } return false; }
    public static bool WhenT2<T1, T2>(this Either<T1, T2> either, Action<T2> action) { if (either._tag == 2) { action(either._t2); return true; } return false; }

    public static bool WhenAsyncT1<T1, T2>(this Either<T1, T2> either, Func<T1, Task> action, out Task task) { if (either._tag == 1) { task = action(either._t1); return true; } task = default; return false; }
    public static bool WhenAsyncT2<T1, T2>(this Either<T1, T2> either, Func<T2, Task> action, out Task task) { if (either._tag == 2) { task = action(either._t2); return true; } task = default;  return false; }

    // Match Func

    /// <summary>Transform Either into one type, the function matching the internal stored type is invoked.</summary>
    public static TResult Match<T1, T2, TResult>(this Either<T1, T2> either, Func<T1, TResult> f1, Func<T2, TResult> f2)
    {
        if (either._tag == 0) InvalidOperationException(_invalidOperationMessage);
        if (either._tag == 1) return f1(either._t1);
        if (either._tag == 2) return f2(either._t2);
        return default;
    }

    /// <summary>Transform Either into one type, the function matching the internal stored type is invoked.</summary>
    public static Task<TResult> MatchAsync<T1, T2, TResult>(this Either<T1, T2> either, Func<T1, Task<TResult>> f1, Func<T2, Task<TResult>> f2)
    {
        if (either._tag == 0) InvalidOperationException(_invalidOperationMessage);
        if (either._tag == 1) return f1(either._t1);
        if (either._tag == 2) return f2(either._t2);
        return default;
    }

    // Match Action

    /// <summary>One of the actions will be called, matching the internal stored type.</summary>
    public static void Match<T1, T2>(this Either<T1, T2> either, Action<T1> a1, Action<T2> a2)
    {
        if (either._tag == 0) InvalidOperationException(_invalidOperationMessage);
        else if (either._tag == 1) a1(either._t1);
        else if (either._tag == 2) a2(either._t2);
    }

    /// <summary>One of the actions will be called, matching the internal stored type.</summary>
    public static Task MatchAsync<T1, T2>(this Either<T1, T2> either, Func<T1, Task> a1, Func<T2, Task> a2)
    {
        if (either._tag == 0) InvalidOperationException(_invalidOperationMessage);
        if (either._tag == 1) return a1(either._t1);
        if (either._tag == 2) return a2(either._t2);
        return default;
    }

    #region Other Type Permutations



    #endregion
}
