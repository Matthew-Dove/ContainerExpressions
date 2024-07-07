using ContainerExpressions.Containers.Internal;
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

    // WhenT# Action

    public static bool WhenT1<T1, T2>(this Either<T1, T2> either, Action<T1> action) { if (either._tag == 1) { action(either._t1); return true; } return false; }
    public static bool WhenT2<T1, T2>(this Either<T1, T2> either, Action<T2> action) { if (either._tag == 2) { action(either._t2); return true; } return false; }

    // Match Func

    /// <summary>Transform Either into one type, the function matching the internal stored type is invoked.</summary>
    public static TResult Match<T1, T2, TResult>(this Either<T1, T2> either, Func<T1, TResult> f1, Func<T2, TResult> f2)
    {
        if (either._tag == 0) Throw.InvalidOperationException(_invalidOperationMessage);
        if (either._tag == 1) return f1(either._t1);
        if (either._tag == 2) return f2(either._t2);
        return default;
    }

    /// <summary>Transform Either into one type, the function matching the internal stored type is invoked.</summary>
    public static Task<TResult> MatchAsync<T1, T2, TResult>(this Either<T1, T2> either, Func<T1, Task<TResult>> f1, Func<T2, Task<TResult>> f2)
    {
        if (either._tag == 0) Throw.InvalidOperationException(_invalidOperationMessage);
        if (either._tag == 1) return f1(either._t1);
        if (either._tag == 2) return f2(either._t2);
        return default;
    }

    // Match Action

    /// <summary>One of the actions will be called, matching the internal stored type.</summary>
    public static void Match<T1, T2>(this Either<T1, T2> either, Action<T1> a1, Action<T2> a2)
    {
        if (either._tag == 0) Throw.InvalidOperationException(_invalidOperationMessage);
        else if (either._tag == 1) a1(either._t1);
        else if (either._tag == 2) a2(either._t2);
    }

    /// <summary>One of the actions will be called, matching the internal stored type.</summary>
    public static Task MatchAsync<T1, T2>(this Either<T1, T2> either, Func<T1, Task> a1, Func<T2, Task> a2)
    {
        if (either._tag == 0) Throw.InvalidOperationException(_invalidOperationMessage);
        if (either._tag == 1) return a1(either._t1);
        if (either._tag == 2) return a2(either._t2);
        return default;
    }

    #region Other Type Permutations

    #region T3

    // TryGetT#

    public static bool TryGetT1<T1, T2, T3>(this Either<T1, T2, T3> either, out T1 t1) { if (either._tag == 1) { t1 = either._t1; return true; }; t1 = default; return false; }
    public static bool TryGetT2<T1, T2, T3>(this Either<T1, T2, T3> either, out T2 t2) { if (either._tag == 2) { t2 = either._t2; return true; }; t2 = default; return false; }
    public static bool TryGetT3<T1, T2, T3>(this Either<T1, T2, T3> either, out T3 t3) { if (either._tag == 3) { t3 = either._t3; return true; }; t3 = default; return false; }

    // WhenT# Func

    public static bool WhenT1<T1, T2, T3, TResult>(this Either<T1, T2, T3> either, Func<T1, TResult> func, out TResult result) { if (either._tag == 1) { result = func(either._t1); return true; }; result = default; return false; }
    public static bool WhenT2<T1, T2, T3, TResult>(this Either<T1, T2, T3> either, Func<T2, TResult> func, out TResult result) { if (either._tag == 2) { result = func(either._t2); return true; }; result = default; return false; }
    public static bool WhenT3<T1, T2, T3, TResult>(this Either<T1, T2, T3> either, Func<T3, TResult> func, out TResult result) { if (either._tag == 3) { result = func(either._t3); return true; }; result = default; return false; }

    // WhenT# Action

    public static bool WhenT1<T1, T2, T3>(this Either<T1, T2, T3> either, Action<T1> action) { if (either._tag == 1) { action(either._t1); return true; } return false; }
    public static bool WhenT2<T1, T2, T3>(this Either<T1, T2, T3> either, Action<T2> action) { if (either._tag == 2) { action(either._t2); return true; } return false; }
    public static bool WhenT3<T1, T2, T3>(this Either<T1, T2, T3> either, Action<T3> action) { if (either._tag == 3) { action(either._t3); return true; } return false; }

    // Match Func

    /// <summary>Transform Either into one type, the function matching the internal stored type is invoked.</summary>
    public static TResult Match<T1, T2, T3, TResult>(this Either<T1, T2, T3> either, Func<T1, TResult> f1, Func<T2, TResult> f2, Func<T3, TResult> f3)
    {
        if (either._tag == 0) Throw.InvalidOperationException(_invalidOperationMessage);
        if (either._tag == 1) return f1(either._t1);
        if (either._tag == 2) return f2(either._t2);
        if (either._tag == 3) return f3(either._t3);
        return default;
    }

    /// <summary>Transform Either into one type, the function matching the internal stored type is invoked.</summary>
    public static Task<TResult> MatchAsync<T1, T2, T3, TResult>(this Either<T1, T2, T3> either, Func<T1, Task<TResult>> f1, Func<T2, Task<TResult>> f2, Func<T3, Task<TResult>> f3)
    {
        if (either._tag == 0) Throw.InvalidOperationException(_invalidOperationMessage);
        if (either._tag == 1) return f1(either._t1);
        if (either._tag == 2) return f2(either._t2);
        if (either._tag == 3) return f3(either._t3);
        return default;
    }

    // Match Action

    /// <summary>One of the actions will be called, matching the internal stored type.</summary>
    public static void Match<T1, T2, T3>(this Either<T1, T2, T3> either, Action<T1> a1, Action<T2> a2, Action<T3> a3)
    {
        if (either._tag == 0) Throw.InvalidOperationException(_invalidOperationMessage);
        else if (either._tag == 1) a1(either._t1);
        else if (either._tag == 2) a2(either._t2);
        else if (either._tag == 3) a3(either._t3);
    }

    /// <summary>One of the actions will be called, matching the internal stored type.</summary>
    public static Task MatchAsync<T1, T2, T3>(this Either<T1, T2, T3> either, Func<T1, Task> a1, Func<T2, Task> a2, Func<T3, Task> a3)
    {
        if (either._tag == 0) Throw.InvalidOperationException(_invalidOperationMessage);
        if (either._tag == 1) return a1(either._t1);
        if (either._tag == 2) return a2(either._t2);
        if (either._tag == 3) return a3(either._t3);
        return default;
    }

    #endregion

    #region T4

    // TryGetT#

    public static bool TryGetT1<T1, T2, T3, T4>(this Either<T1, T2, T3, T4> either, out T1 t1) { if (either._tag == 1) { t1 = either._t1; return true; }; t1 = default; return false; }
    public static bool TryGetT2<T1, T2, T3, T4>(this Either<T1, T2, T3, T4> either, out T2 t2) { if (either._tag == 2) { t2 = either._t2; return true; }; t2 = default; return false; }
    public static bool TryGetT3<T1, T2, T3, T4>(this Either<T1, T2, T3, T4> either, out T3 t3) { if (either._tag == 3) { t3 = either._t3; return true; }; t3 = default; return false; }
    public static bool TryGetT4<T1, T2, T3, T4>(this Either<T1, T2, T3, T4> either, out T4 t4) { if (either._tag == 4) { t4 = either._t4; return true; }; t4 = default; return false; }

    // WhenT# Func

    public static bool WhenT1<T1, T2, T3, T4, TResult>(this Either<T1, T2, T3, T4> either, Func<T1, TResult> func, out TResult result) { if (either._tag == 1) { result = func(either._t1); return true; }; result = default; return false; }
    public static bool WhenT2<T1, T2, T3, T4, TResult>(this Either<T1, T2, T3, T4> either, Func<T2, TResult> func, out TResult result) { if (either._tag == 2) { result = func(either._t2); return true; }; result = default; return false; }
    public static bool WhenT3<T1, T2, T3, T4, TResult>(this Either<T1, T2, T3, T4> either, Func<T3, TResult> func, out TResult result) { if (either._tag == 3) { result = func(either._t3); return true; }; result = default; return false; }
    public static bool WhenT4<T1, T2, T3, T4, TResult>(this Either<T1, T2, T3, T4> either, Func<T4, TResult> func, out TResult result) { if (either._tag == 4) { result = func(either._t4); return true; }; result = default; return false; }

    // WhenT# Action

    public static bool WhenT1<T1, T2, T3, T4>(this Either<T1, T2, T3, T4> either, Action<T1> action) { if (either._tag == 1) { action(either._t1); return true; } return false; }
    public static bool WhenT2<T1, T2, T3, T4>(this Either<T1, T2, T3, T4> either, Action<T2> action) { if (either._tag == 2) { action(either._t2); return true; } return false; }
    public static bool WhenT3<T1, T2, T3, T4>(this Either<T1, T2, T3, T4> either, Action<T3> action) { if (either._tag == 3) { action(either._t3); return true; } return false; }
    public static bool WhenT4<T1, T2, T3, T4>(this Either<T1, T2, T3, T4> either, Action<T4> action) { if (either._tag == 4) { action(either._t4); return true; } return false; }

    // Match Func

    /// <summary>Transform Either into one type, the function matching the internal stored type is invoked.</summary>
    public static TResult Match<T1, T2, T3, T4, TResult>(this Either<T1, T2, T3, T4> either, Func<T1, TResult> f1, Func<T2, TResult> f2, Func<T3, TResult> f3, Func<T4, TResult> f4)
    {
        if (either._tag == 0) Throw.InvalidOperationException(_invalidOperationMessage);
        if (either._tag == 1) return f1(either._t1);
        if (either._tag == 2) return f2(either._t2);
        if (either._tag == 3) return f3(either._t3);
        if (either._tag == 4) return f4(either._t4);
        return default;
    }

    /// <summary>Transform Either into one type, the function matching the internal stored type is invoked.</summary>
    public static Task<TResult> MatchAsync<T1, T2, T3, T4, TResult>(this Either<T1, T2, T3, T4> either, Func<T1, Task<TResult>> f1, Func<T2, Task<TResult>> f2, Func<T3, Task<TResult>> f3, Func<T4, Task<TResult>> f4)
    {
        if (either._tag == 0) Throw.InvalidOperationException(_invalidOperationMessage);
        if (either._tag == 1) return f1(either._t1);
        if (either._tag == 2) return f2(either._t2);
        if (either._tag == 3) return f3(either._t3);
        if (either._tag == 4) return f4(either._t4);
        return default;
    }

    // Match Action

    /// <summary>One of the actions will be called, matching the internal stored type.</summary>
    public static void Match<T1, T2, T3, T4>(this Either<T1, T2, T3, T4> either, Action<T1> a1, Action<T2> a2, Action<T3> a3, Action<T4> a4)
    {
        if (either._tag == 0) Throw.InvalidOperationException(_invalidOperationMessage);
        else if (either._tag == 1) a1(either._t1);
        else if (either._tag == 2) a2(either._t2);
        else if (either._tag == 3) a3(either._t3);
        else if (either._tag == 4) a4(either._t4);
    }

    /// <summary>One of the actions will be called, matching the internal stored type.</summary>
    public static Task MatchAsync<T1, T2, T3, T4>(this Either<T1, T2, T3, T4> either, Func<T1, Task> a1, Func<T2, Task> a2, Func<T3, Task> a3, Func<T4, Task> a4)
    {
        if (either._tag == 0) Throw.InvalidOperationException(_invalidOperationMessage);
        if (either._tag == 1) return a1(either._t1);
        if (either._tag == 2) return a2(either._t2);
        if (either._tag == 3) return a3(either._t3);
        if (either._tag == 4) return a4(either._t4);
        return default;
    }

    #endregion

    #region T5

    // TryGetT#

    public static bool TryGetT1<T1, T2, T3, T4, T5>(this Either<T1, T2, T3, T4, T5> either, out T1 t1) { if (either._tag == 1) { t1 = either._t1; return true; }; t1 = default; return false; }
    public static bool TryGetT2<T1, T2, T3, T4, T5>(this Either<T1, T2, T3, T4, T5> either, out T2 t2) { if (either._tag == 2) { t2 = either._t2; return true; }; t2 = default; return false; }
    public static bool TryGetT3<T1, T2, T3, T4, T5>(this Either<T1, T2, T3, T4, T5> either, out T3 t3) { if (either._tag == 3) { t3 = either._t3; return true; }; t3 = default; return false; }
    public static bool TryGetT4<T1, T2, T3, T4, T5>(this Either<T1, T2, T3, T4, T5> either, out T4 t4) { if (either._tag == 4) { t4 = either._t4; return true; }; t4 = default; return false; }
    public static bool TryGetT5<T1, T2, T3, T4, T5>(this Either<T1, T2, T3, T4, T5> either, out T5 t5) { if (either._tag == 5) { t5 = either._t5; return true; }; t5 = default; return false; }

    // WhenT# Func

    public static bool WhenT1<T1, T2, T3, T4, T5, TResult>(this Either<T1, T2, T3, T4, T5> either, Func<T1, TResult> func, out TResult result) { if (either._tag == 1) { result = func(either._t1); return true; }; result = default; return false; }
    public static bool WhenT2<T1, T2, T3, T4, T5, TResult>(this Either<T1, T2, T3, T4, T5> either, Func<T2, TResult> func, out TResult result) { if (either._tag == 2) { result = func(either._t2); return true; }; result = default; return false; }
    public static bool WhenT3<T1, T2, T3, T4, T5, TResult>(this Either<T1, T2, T3, T4, T5> either, Func<T3, TResult> func, out TResult result) { if (either._tag == 3) { result = func(either._t3); return true; }; result = default; return false; }
    public static bool WhenT4<T1, T2, T3, T4, T5, TResult>(this Either<T1, T2, T3, T4, T5> either, Func<T4, TResult> func, out TResult result) { if (either._tag == 4) { result = func(either._t4); return true; }; result = default; return false; }
    public static bool WhenT5<T1, T2, T3, T4, T5, TResult>(this Either<T1, T2, T3, T4, T5> either, Func<T5, TResult> func, out TResult result) { if (either._tag == 5) { result = func(either._t5); return true; }; result = default; return false; }

    // WhenT# Action

    public static bool WhenT1<T1, T2, T3, T4, T5>(this Either<T1, T2, T3, T4, T5> either, Action<T1> action) { if (either._tag == 1) { action(either._t1); return true; } return false; }
    public static bool WhenT2<T1, T2, T3, T4, T5>(this Either<T1, T2, T3, T4, T5> either, Action<T2> action) { if (either._tag == 2) { action(either._t2); return true; } return false; }
    public static bool WhenT3<T1, T2, T3, T4, T5>(this Either<T1, T2, T3, T4, T5> either, Action<T3> action) { if (either._tag == 3) { action(either._t3); return true; } return false; }
    public static bool WhenT4<T1, T2, T3, T4, T5>(this Either<T1, T2, T3, T4, T5> either, Action<T4> action) { if (either._tag == 4) { action(either._t4); return true; } return false; }
    public static bool WhenT5<T1, T2, T3, T4, T5>(this Either<T1, T2, T3, T4, T5> either, Action<T5> action) { if (either._tag == 5) { action(either._t5); return true; } return false; }

    // Match Func

    /// <summary>Transform Either into one type, the function matching the internal stored type is invoked.</summary>
    public static TResult Match<T1, T2, T3, T4, T5, TResult>(this Either<T1, T2, T3, T4, T5> either, Func<T1, TResult> f1, Func<T2, TResult> f2, Func<T3, TResult> f3, Func<T4, TResult> f4, Func<T5, TResult> f5)
    {
        if (either._tag == 0) Throw.InvalidOperationException(_invalidOperationMessage);
        if (either._tag == 1) return f1(either._t1);
        if (either._tag == 2) return f2(either._t2);
        if (either._tag == 3) return f3(either._t3);
        if (either._tag == 4) return f4(either._t4);
        if (either._tag == 5) return f5(either._t5);
        return default;
    }

    /// <summary>Transform Either into one type, the function matching the internal stored type is invoked.</summary>
    public static Task<TResult> MatchAsync<T1, T2, T3, T4, T5, TResult>(this Either<T1, T2, T3, T4, T5> either, Func<T1, Task<TResult>> f1, Func<T2, Task<TResult>> f2, Func<T3, Task<TResult>> f3, Func<T4, Task<TResult>> f4, Func<T5, Task<TResult>> f5)
    {
        if (either._tag == 0) Throw.InvalidOperationException(_invalidOperationMessage);
        if (either._tag == 1) return f1(either._t1);
        if (either._tag == 2) return f2(either._t2);
        if (either._tag == 3) return f3(either._t3);
        if (either._tag == 4) return f4(either._t4);
        if (either._tag == 5) return f5(either._t5);
        return default;
    }

    // Match Action

    /// <summary>One of the actions will be called, matching the internal stored type.</summary>
    public static void Match<T1, T2, T3, T4, T5>(this Either<T1, T2, T3, T4, T5> either, Action<T1> a1, Action<T2> a2, Action<T3> a3, Action<T4> a4, Action<T5> a5)
    {
        if (either._tag == 0) Throw.InvalidOperationException(_invalidOperationMessage);
        else if (either._tag == 1) a1(either._t1);
        else if (either._tag == 2) a2(either._t2);
        else if (either._tag == 3) a3(either._t3);
        else if (either._tag == 4) a4(either._t4);
        else if (either._tag == 5) a5(either._t5);
    }

    /// <summary>One of the actions will be called, matching the internal stored type.</summary>
    public static Task MatchAsync<T1, T2, T3, T4, T5>(this Either<T1, T2, T3, T4, T5> either, Func<T1, Task> a1, Func<T2, Task> a2, Func<T3, Task> a3, Func<T4, Task> a4, Func<T5, Task> a5)
    {
        if (either._tag == 0) Throw.InvalidOperationException(_invalidOperationMessage);
        if (either._tag == 1) return a1(either._t1);
        if (either._tag == 2) return a2(either._t2);
        if (either._tag == 3) return a3(either._t3);
        if (either._tag == 4) return a4(either._t4);
        if (either._tag == 5) return a5(either._t5);
        return default;
    }

    #endregion

    #region T6

    // TryGetT#

    public static bool TryGetT1<T1, T2, T3, T4, T5, T6>(this Either<T1, T2, T3, T4, T5, T6> either, out T1 t1) { if (either._tag == 1) { t1 = either._t1; return true; }; t1 = default; return false; }
    public static bool TryGetT2<T1, T2, T3, T4, T5, T6>(this Either<T1, T2, T3, T4, T5, T6> either, out T2 t2) { if (either._tag == 2) { t2 = either._t2; return true; }; t2 = default; return false; }
    public static bool TryGetT3<T1, T2, T3, T4, T5, T6>(this Either<T1, T2, T3, T4, T5, T6> either, out T3 t3) { if (either._tag == 3) { t3 = either._t3; return true; }; t3 = default; return false; }
    public static bool TryGetT4<T1, T2, T3, T4, T5, T6>(this Either<T1, T2, T3, T4, T5, T6> either, out T4 t4) { if (either._tag == 4) { t4 = either._t4; return true; }; t4 = default; return false; }
    public static bool TryGetT5<T1, T2, T3, T4, T5, T6>(this Either<T1, T2, T3, T4, T5, T6> either, out T5 t5) { if (either._tag == 5) { t5 = either._t5; return true; }; t5 = default; return false; }
    public static bool TryGetT6<T1, T2, T3, T4, T5, T6>(this Either<T1, T2, T3, T4, T5, T6> either, out T6 t6) { if (either._tag == 6) { t6 = either._t6; return true; }; t6 = default; return false; }

    // WhenT# Func

    public static bool WhenT1<T1, T2, T3, T4, T5, T6, TResult>(this Either<T1, T2, T3, T4, T5, T6> either, Func<T1, TResult> func, out TResult result) { if (either._tag == 1) { result = func(either._t1); return true; }; result = default; return false; }
    public static bool WhenT2<T1, T2, T3, T4, T5, T6, TResult>(this Either<T1, T2, T3, T4, T5, T6> either, Func<T2, TResult> func, out TResult result) { if (either._tag == 2) { result = func(either._t2); return true; }; result = default; return false; }
    public static bool WhenT3<T1, T2, T3, T4, T5, T6, TResult>(this Either<T1, T2, T3, T4, T5, T6> either, Func<T3, TResult> func, out TResult result) { if (either._tag == 3) { result = func(either._t3); return true; }; result = default; return false; }
    public static bool WhenT4<T1, T2, T3, T4, T5, T6, TResult>(this Either<T1, T2, T3, T4, T5, T6> either, Func<T4, TResult> func, out TResult result) { if (either._tag == 4) { result = func(either._t4); return true; }; result = default; return false; }
    public static bool WhenT5<T1, T2, T3, T4, T5, T6, TResult>(this Either<T1, T2, T3, T4, T5, T6> either, Func<T5, TResult> func, out TResult result) { if (either._tag == 5) { result = func(either._t5); return true; }; result = default; return false; }
    public static bool WhenT6<T1, T2, T3, T4, T5, T6, TResult>(this Either<T1, T2, T3, T4, T5, T6> either, Func<T6, TResult> func, out TResult result) { if (either._tag == 6) { result = func(either._t6); return true; }; result = default; return false; }

    // WhenT# Action

    public static bool WhenT1<T1, T2, T3, T4, T5, T6>(this Either<T1, T2, T3, T4, T5, T6> either, Action<T1> action) { if (either._tag == 1) { action(either._t1); return true; } return false; }
    public static bool WhenT2<T1, T2, T3, T4, T5, T6>(this Either<T1, T2, T3, T4, T5, T6> either, Action<T2> action) { if (either._tag == 2) { action(either._t2); return true; } return false; }
    public static bool WhenT3<T1, T2, T3, T4, T5, T6>(this Either<T1, T2, T3, T4, T5, T6> either, Action<T3> action) { if (either._tag == 3) { action(either._t3); return true; } return false; }
    public static bool WhenT4<T1, T2, T3, T4, T5, T6>(this Either<T1, T2, T3, T4, T5, T6> either, Action<T4> action) { if (either._tag == 4) { action(either._t4); return true; } return false; }
    public static bool WhenT5<T1, T2, T3, T4, T5, T6>(this Either<T1, T2, T3, T4, T5, T6> either, Action<T5> action) { if (either._tag == 5) { action(either._t5); return true; } return false; }
    public static bool WhenT6<T1, T2, T3, T4, T5, T6>(this Either<T1, T2, T3, T4, T5, T6> either, Action<T6> action) { if (either._tag == 6) { action(either._t6); return true; } return false; }

    // Match Func

    /// <summary>Transform Either into one type, the function matching the internal stored type is invoked.</summary>
    public static TResult Match<T1, T2, T3, T4, T5, T6, TResult>(this Either<T1, T2, T3, T4, T5, T6> either, Func<T1, TResult> f1, Func<T2, TResult> f2, Func<T3, TResult> f3, Func<T4, TResult> f4, Func<T5, TResult> f5, Func<T6, TResult> f6)
    {
        if (either._tag == 0) Throw.InvalidOperationException(_invalidOperationMessage);
        if (either._tag == 1) return f1(either._t1);
        if (either._tag == 2) return f2(either._t2);
        if (either._tag == 3) return f3(either._t3);
        if (either._tag == 4) return f4(either._t4);
        if (either._tag == 5) return f5(either._t5);
        if (either._tag == 6) return f6(either._t6);
        return default;
    }

    /// <summary>Transform Either into one type, the function matching the internal stored type is invoked.</summary>
    public static Task<TResult> MatchAsync<T1, T2, T3, T4, T5, T6, TResult>(this Either<T1, T2, T3, T4, T5, T6> either, Func<T1, Task<TResult>> f1, Func<T2, Task<TResult>> f2, Func<T3, Task<TResult>> f3, Func<T4, Task<TResult>> f4, Func<T5, Task<TResult>> f5, Func<T6, Task<TResult>> f6)
    {
        if (either._tag == 0) Throw.InvalidOperationException(_invalidOperationMessage);
        if (either._tag == 1) return f1(either._t1);
        if (either._tag == 2) return f2(either._t2);
        if (either._tag == 3) return f3(either._t3);
        if (either._tag == 4) return f4(either._t4);
        if (either._tag == 5) return f5(either._t5);
        if (either._tag == 6) return f6(either._t6);
        return default;
    }

    // Match Action

    /// <summary>One of the actions will be called, matching the internal stored type.</summary>
    public static void Match<T1, T2, T3, T4, T5, T6>(this Either<T1, T2, T3, T4, T5, T6> either, Action<T1> a1, Action<T2> a2, Action<T3> a3, Action<T4> a4, Action<T5> a5, Action<T6> a6)
    {
        if (either._tag == 0) Throw.InvalidOperationException(_invalidOperationMessage);
        else if (either._tag == 1) a1(either._t1);
        else if (either._tag == 2) a2(either._t2);
        else if (either._tag == 3) a3(either._t3);
        else if (either._tag == 4) a4(either._t4);
        else if (either._tag == 5) a5(either._t5);
        else if (either._tag == 6) a6(either._t6);
    }

    /// <summary>One of the actions will be called, matching the internal stored type.</summary>
    public static Task MatchAsync<T1, T2, T3, T4, T5, T6>(this Either<T1, T2, T3, T4, T5, T6> either, Func<T1, Task> a1, Func<T2, Task> a2, Func<T3, Task> a3, Func<T4, Task> a4, Func<T5, Task> a5, Func<T6, Task> a6)
    {
        if (either._tag == 0) Throw.InvalidOperationException(_invalidOperationMessage);
        if (either._tag == 1) return a1(either._t1);
        if (either._tag == 2) return a2(either._t2);
        if (either._tag == 3) return a3(either._t3);
        if (either._tag == 4) return a4(either._t4);
        if (either._tag == 5) return a5(either._t5);
        if (either._tag == 6) return a6(either._t6);
        return default;
    }

    #endregion

    #region T7

    // TryGetT#

    public static bool TryGetT1<T1, T2, T3, T4, T5, T6, T7>(this Either<T1, T2, T3, T4, T5, T6, T7> either, out T1 t1) { if (either._tag == 1) { t1 = either._t1; return true; }; t1 = default; return false; }
    public static bool TryGetT2<T1, T2, T3, T4, T5, T6, T7>(this Either<T1, T2, T3, T4, T5, T6, T7> either, out T2 t2) { if (either._tag == 2) { t2 = either._t2; return true; }; t2 = default; return false; }
    public static bool TryGetT3<T1, T2, T3, T4, T5, T6, T7>(this Either<T1, T2, T3, T4, T5, T6, T7> either, out T3 t3) { if (either._tag == 3) { t3 = either._t3; return true; }; t3 = default; return false; }
    public static bool TryGetT4<T1, T2, T3, T4, T5, T6, T7>(this Either<T1, T2, T3, T4, T5, T6, T7> either, out T4 t4) { if (either._tag == 4) { t4 = either._t4; return true; }; t4 = default; return false; }
    public static bool TryGetT5<T1, T2, T3, T4, T5, T6, T7>(this Either<T1, T2, T3, T4, T5, T6, T7> either, out T5 t5) { if (either._tag == 5) { t5 = either._t5; return true; }; t5 = default; return false; }
    public static bool TryGetT6<T1, T2, T3, T4, T5, T6, T7>(this Either<T1, T2, T3, T4, T5, T6, T7> either, out T6 t6) { if (either._tag == 6) { t6 = either._t6; return true; }; t6 = default; return false; }
    public static bool TryGetT7<T1, T2, T3, T4, T5, T6, T7>(this Either<T1, T2, T3, T4, T5, T6, T7> either, out T7 t7) { if (either._tag == 7) { t7 = either._t7; return true; }; t7 = default; return false; }

    // WhenT# Func

    public static bool WhenT1<T1, T2, T3, T4, T5, T6, T7, TResult>(this Either<T1, T2, T3, T4, T5, T6, T7> either, Func<T1, TResult> func, out TResult result) { if (either._tag == 1) { result = func(either._t1); return true; }; result = default; return false; }
    public static bool WhenT2<T1, T2, T3, T4, T5, T6, T7, TResult>(this Either<T1, T2, T3, T4, T5, T6, T7> either, Func<T2, TResult> func, out TResult result) { if (either._tag == 2) { result = func(either._t2); return true; }; result = default; return false; }
    public static bool WhenT3<T1, T2, T3, T4, T5, T6, T7, TResult>(this Either<T1, T2, T3, T4, T5, T6, T7> either, Func<T3, TResult> func, out TResult result) { if (either._tag == 3) { result = func(either._t3); return true; }; result = default; return false; }
    public static bool WhenT4<T1, T2, T3, T4, T5, T6, T7, TResult>(this Either<T1, T2, T3, T4, T5, T6, T7> either, Func<T4, TResult> func, out TResult result) { if (either._tag == 4) { result = func(either._t4); return true; }; result = default; return false; }
    public static bool WhenT5<T1, T2, T3, T4, T5, T6, T7, TResult>(this Either<T1, T2, T3, T4, T5, T6, T7> either, Func<T5, TResult> func, out TResult result) { if (either._tag == 5) { result = func(either._t5); return true; }; result = default; return false; }
    public static bool WhenT6<T1, T2, T3, T4, T5, T6, T7, TResult>(this Either<T1, T2, T3, T4, T5, T6, T7> either, Func<T6, TResult> func, out TResult result) { if (either._tag == 6) { result = func(either._t6); return true; }; result = default; return false; }
    public static bool WhenT7<T1, T2, T3, T4, T5, T6, T7, TResult>(this Either<T1, T2, T3, T4, T5, T6, T7> either, Func<T7, TResult> func, out TResult result) { if (either._tag == 7) { result = func(either._t7); return true; }; result = default; return false; }

    // WhenT# Action

    public static bool WhenT1<T1, T2, T3, T4, T5, T6, T7>(this Either<T1, T2, T3, T4, T5, T6, T7> either, Action<T1> action) { if (either._tag == 1) { action(either._t1); return true; } return false; }
    public static bool WhenT2<T1, T2, T3, T4, T5, T6, T7>(this Either<T1, T2, T3, T4, T5, T6, T7> either, Action<T2> action) { if (either._tag == 2) { action(either._t2); return true; } return false; }
    public static bool WhenT3<T1, T2, T3, T4, T5, T6, T7>(this Either<T1, T2, T3, T4, T5, T6, T7> either, Action<T3> action) { if (either._tag == 3) { action(either._t3); return true; } return false; }
    public static bool WhenT4<T1, T2, T3, T4, T5, T6, T7>(this Either<T1, T2, T3, T4, T5, T6, T7> either, Action<T4> action) { if (either._tag == 4) { action(either._t4); return true; } return false; }
    public static bool WhenT5<T1, T2, T3, T4, T5, T6, T7>(this Either<T1, T2, T3, T4, T5, T6, T7> either, Action<T5> action) { if (either._tag == 5) { action(either._t5); return true; } return false; }
    public static bool WhenT6<T1, T2, T3, T4, T5, T6, T7>(this Either<T1, T2, T3, T4, T5, T6, T7> either, Action<T6> action) { if (either._tag == 6) { action(either._t6); return true; } return false; }
    public static bool WhenT7<T1, T2, T3, T4, T5, T6, T7>(this Either<T1, T2, T3, T4, T5, T6, T7> either, Action<T7> action) { if (either._tag == 7) { action(either._t7); return true; } return false; }

    // Match Func

    /// <summary>Transform Either into one type, the function matching the internal stored type is invoked.</summary>
    public static TResult Match<T1, T2, T3, T4, T5, T6, T7, TResult>(this Either<T1, T2, T3, T4, T5, T6, T7> either, Func<T1, TResult> f1, Func<T2, TResult> f2, Func<T3, TResult> f3, Func<T4, TResult> f4, Func<T5, TResult> f5, Func<T6, TResult> f6, Func<T7, TResult> f7)
    {
        if (either._tag == 0) Throw.InvalidOperationException(_invalidOperationMessage);
        if (either._tag == 1) return f1(either._t1);
        if (either._tag == 2) return f2(either._t2);
        if (either._tag == 3) return f3(either._t3);
        if (either._tag == 4) return f4(either._t4);
        if (either._tag == 5) return f5(either._t5);
        if (either._tag == 6) return f6(either._t6);
        if (either._tag == 7) return f7(either._t7);
        return default;
    }

    /// <summary>Transform Either into one type, the function matching the internal stored type is invoked.</summary>
    public static Task<TResult> MatchAsync<T1, T2, T3, T4, T5, T6, T7, TResult>(this Either<T1, T2, T3, T4, T5, T6, T7> either, Func<T1, Task<TResult>> f1, Func<T2, Task<TResult>> f2, Func<T3, Task<TResult>> f3, Func<T4, Task<TResult>> f4, Func<T5, Task<TResult>> f5, Func<T6, Task<TResult>> f6, Func<T7, Task<TResult>> f7)
    {
        if (either._tag == 0) Throw.InvalidOperationException(_invalidOperationMessage);
        if (either._tag == 1) return f1(either._t1);
        if (either._tag == 2) return f2(either._t2);
        if (either._tag == 3) return f3(either._t3);
        if (either._tag == 4) return f4(either._t4);
        if (either._tag == 5) return f5(either._t5);
        if (either._tag == 6) return f6(either._t6);
        if (either._tag == 7) return f7(either._t7);
        return default;
    }

    // Match Action

    /// <summary>One of the actions will be called, matching the internal stored type.</summary>
    public static void Match<T1, T2, T3, T4, T5, T6, T7>(this Either<T1, T2, T3, T4, T5, T6, T7> either, Action<T1> a1, Action<T2> a2, Action<T3> a3, Action<T4> a4, Action<T5> a5, Action<T6> a6, Action<T7> a7)
    {
        if (either._tag == 0) Throw.InvalidOperationException(_invalidOperationMessage);
        else if (either._tag == 1) a1(either._t1);
        else if (either._tag == 2) a2(either._t2);
        else if (either._tag == 3) a3(either._t3);
        else if (either._tag == 4) a4(either._t4);
        else if (either._tag == 5) a5(either._t5);
        else if (either._tag == 6) a6(either._t6);
        else if (either._tag == 7) a7(either._t7);
    }

    /// <summary>One of the actions will be called, matching the internal stored type.</summary>
    public static Task MatchAsync<T1, T2, T3, T4, T5, T6, T7>(this Either<T1, T2, T3, T4, T5, T6, T7> either, Func<T1, Task> a1, Func<T2, Task> a2, Func<T3, Task> a3, Func<T4, Task> a4, Func<T5, Task> a5, Func<T6, Task> a6, Func<T7, Task> a7)
    {
        if (either._tag == 0) Throw.InvalidOperationException(_invalidOperationMessage);
        if (either._tag == 1) return a1(either._t1);
        if (either._tag == 2) return a2(either._t2);
        if (either._tag == 3) return a3(either._t3);
        if (either._tag == 4) return a4(either._t4);
        if (either._tag == 5) return a5(either._t5);
        if (either._tag == 6) return a6(either._t6);
        if (either._tag == 7) return a7(either._t7);
        return default;
    }

    #endregion

    #region T8

    // TryGetT#

    public static bool TryGetT1<T1, T2, T3, T4, T5, T6, T7, T8>(this Either<T1, T2, T3, T4, T5, T6, T7, T8> either, out T1 t1) { if (either._tag == 1) { t1 = either._t1; return true; }; t1 = default; return false; }
    public static bool TryGetT2<T1, T2, T3, T4, T5, T6, T7, T8>(this Either<T1, T2, T3, T4, T5, T6, T7, T8> either, out T2 t2) { if (either._tag == 2) { t2 = either._t2; return true; }; t2 = default; return false; }
    public static bool TryGetT3<T1, T2, T3, T4, T5, T6, T7, T8>(this Either<T1, T2, T3, T4, T5, T6, T7, T8> either, out T3 t3) { if (either._tag == 3) { t3 = either._t3; return true; }; t3 = default; return false; }
    public static bool TryGetT4<T1, T2, T3, T4, T5, T6, T7, T8>(this Either<T1, T2, T3, T4, T5, T6, T7, T8> either, out T4 t4) { if (either._tag == 4) { t4 = either._t4; return true; }; t4 = default; return false; }
    public static bool TryGetT5<T1, T2, T3, T4, T5, T6, T7, T8>(this Either<T1, T2, T3, T4, T5, T6, T7, T8> either, out T5 t5) { if (either._tag == 5) { t5 = either._t5; return true; }; t5 = default; return false; }
    public static bool TryGetT6<T1, T2, T3, T4, T5, T6, T7, T8>(this Either<T1, T2, T3, T4, T5, T6, T7, T8> either, out T6 t6) { if (either._tag == 6) { t6 = either._t6; return true; }; t6 = default; return false; }
    public static bool TryGetT7<T1, T2, T3, T4, T5, T6, T7, T8>(this Either<T1, T2, T3, T4, T5, T6, T7, T8> either, out T7 t7) { if (either._tag == 7) { t7 = either._t7; return true; }; t7 = default; return false; }
    public static bool TryGetT8<T1, T2, T3, T4, T5, T6, T7, T8>(this Either<T1, T2, T3, T4, T5, T6, T7, T8> either, out T8 t8) { if (either._tag == 8) { t8 = either._t8; return true; }; t8 = default; return false; }

    // WhenT# Func

    public static bool WhenT1<T1, T2, T3, T4, T5, T6, T7, T8, TResult>(this Either<T1, T2, T3, T4, T5, T6, T7, T8> either, Func<T1, TResult> func, out TResult result) { if (either._tag == 1) { result = func(either._t1); return true; }; result = default; return false; }
    public static bool WhenT2<T1, T2, T3, T4, T5, T6, T7, T8, TResult>(this Either<T1, T2, T3, T4, T5, T6, T7, T8> either, Func<T2, TResult> func, out TResult result) { if (either._tag == 2) { result = func(either._t2); return true; }; result = default; return false; }
    public static bool WhenT3<T1, T2, T3, T4, T5, T6, T7, T8, TResult>(this Either<T1, T2, T3, T4, T5, T6, T7, T8> either, Func<T3, TResult> func, out TResult result) { if (either._tag == 3) { result = func(either._t3); return true; }; result = default; return false; }
    public static bool WhenT4<T1, T2, T3, T4, T5, T6, T7, T8, TResult>(this Either<T1, T2, T3, T4, T5, T6, T7, T8> either, Func<T4, TResult> func, out TResult result) { if (either._tag == 4) { result = func(either._t4); return true; }; result = default; return false; }
    public static bool WhenT5<T1, T2, T3, T4, T5, T6, T7, T8, TResult>(this Either<T1, T2, T3, T4, T5, T6, T7, T8> either, Func<T5, TResult> func, out TResult result) { if (either._tag == 5) { result = func(either._t5); return true; }; result = default; return false; }
    public static bool WhenT6<T1, T2, T3, T4, T5, T6, T7, T8, TResult>(this Either<T1, T2, T3, T4, T5, T6, T7, T8> either, Func<T6, TResult> func, out TResult result) { if (either._tag == 6) { result = func(either._t6); return true; }; result = default; return false; }
    public static bool WhenT7<T1, T2, T3, T4, T5, T6, T7, T8, TResult>(this Either<T1, T2, T3, T4, T5, T6, T7, T8> either, Func<T7, TResult> func, out TResult result) { if (either._tag == 7) { result = func(either._t7); return true; }; result = default; return false; }
    public static bool WhenT8<T1, T2, T3, T4, T5, T6, T7, T8, TResult>(this Either<T1, T2, T3, T4, T5, T6, T7, T8> either, Func<T8, TResult> func, out TResult result) { if (either._tag == 8) { result = func(either._t8); return true; }; result = default; return false; }

    // WhenT# Action

    public static bool WhenT1<T1, T2, T3, T4, T5, T6, T7, T8>(this Either<T1, T2, T3, T4, T5, T6, T7, T8> either, Action<T1> action) { if (either._tag == 1) { action(either._t1); return true; } return false; }
    public static bool WhenT2<T1, T2, T3, T4, T5, T6, T7, T8>(this Either<T1, T2, T3, T4, T5, T6, T7, T8> either, Action<T2> action) { if (either._tag == 2) { action(either._t2); return true; } return false; }
    public static bool WhenT3<T1, T2, T3, T4, T5, T6, T7, T8>(this Either<T1, T2, T3, T4, T5, T6, T7, T8> either, Action<T3> action) { if (either._tag == 3) { action(either._t3); return true; } return false; }
    public static bool WhenT4<T1, T2, T3, T4, T5, T6, T7, T8>(this Either<T1, T2, T3, T4, T5, T6, T7, T8> either, Action<T4> action) { if (either._tag == 4) { action(either._t4); return true; } return false; }
    public static bool WhenT5<T1, T2, T3, T4, T5, T6, T7, T8>(this Either<T1, T2, T3, T4, T5, T6, T7, T8> either, Action<T5> action) { if (either._tag == 5) { action(either._t5); return true; } return false; }
    public static bool WhenT6<T1, T2, T3, T4, T5, T6, T7, T8>(this Either<T1, T2, T3, T4, T5, T6, T7, T8> either, Action<T6> action) { if (either._tag == 6) { action(either._t6); return true; } return false; }
    public static bool WhenT7<T1, T2, T3, T4, T5, T6, T7, T8>(this Either<T1, T2, T3, T4, T5, T6, T7, T8> either, Action<T7> action) { if (either._tag == 7) { action(either._t7); return true; } return false; }
    public static bool WhenT8<T1, T2, T3, T4, T5, T6, T7, T8>(this Either<T1, T2, T3, T4, T5, T6, T7, T8> either, Action<T8> action) { if (either._tag == 8) { action(either._t8); return true; } return false; }

    // Match Func

    /// <summary>Transform Either into one type, the function matching the internal stored type is invoked.</summary>
    public static TResult Match<T1, T2, T3, T4, T5, T6, T7, T8, TResult>(this Either<T1, T2, T3, T4, T5, T6, T7, T8> either, Func<T1, TResult> f1, Func<T2, TResult> f2, Func<T3, TResult> f3, Func<T4, TResult> f4, Func<T5, TResult> f5, Func<T6, TResult> f6, Func<T7, TResult> f7, Func<T8, TResult> f8)
    {
        if (either._tag == 0) Throw.InvalidOperationException(_invalidOperationMessage);
        if (either._tag == 1) return f1(either._t1);
        if (either._tag == 2) return f2(either._t2);
        if (either._tag == 3) return f3(either._t3);
        if (either._tag == 4) return f4(either._t4);
        if (either._tag == 5) return f5(either._t5);
        if (either._tag == 6) return f6(either._t6);
        if (either._tag == 7) return f7(either._t7);
        if (either._tag == 8) return f8(either._t8);
        return default;
    }

    /// <summary>Transform Either into one type, the function matching the internal stored type is invoked.</summary>
    public static Task<TResult> MatchAsync<T1, T2, T3, T4, T5, T6, T7, T8, TResult>(this Either<T1, T2, T3, T4, T5, T6, T7, T8> either, Func<T1, Task<TResult>> f1, Func<T2, Task<TResult>> f2, Func<T3, Task<TResult>> f3, Func<T4, Task<TResult>> f4, Func<T5, Task<TResult>> f5, Func<T6, Task<TResult>> f6, Func<T7, Task<TResult>> f7, Func<T8, Task<TResult>> f8)
    {
        if (either._tag == 0) Throw.InvalidOperationException(_invalidOperationMessage);
        if (either._tag == 1) return f1(either._t1);
        if (either._tag == 2) return f2(either._t2);
        if (either._tag == 3) return f3(either._t3);
        if (either._tag == 4) return f4(either._t4);
        if (either._tag == 5) return f5(either._t5);
        if (either._tag == 6) return f6(either._t6);
        if (either._tag == 7) return f7(either._t7);
        if (either._tag == 8) return f8(either._t8);
        return default;
    }

    // Match Action

    /// <summary>One of the actions will be called, matching the internal stored type.</summary>
    public static void Match<T1, T2, T3, T4, T5, T6, T7, T8>(this Either<T1, T2, T3, T4, T5, T6, T7, T8> either, Action<T1> a1, Action<T2> a2, Action<T3> a3, Action<T4> a4, Action<T5> a5, Action<T6> a6, Action<T7> a7, Action<T8> a8)
    {
        if (either._tag == 0) Throw.InvalidOperationException(_invalidOperationMessage);
        else if (either._tag == 1) a1(either._t1);
        else if (either._tag == 2) a2(either._t2);
        else if (either._tag == 3) a3(either._t3);
        else if (either._tag == 4) a4(either._t4);
        else if (either._tag == 5) a5(either._t5);
        else if (either._tag == 6) a6(either._t6);
        else if (either._tag == 7) a7(either._t7);
        else if (either._tag == 8) a8(either._t8);
    }

    /// <summary>One of the actions will be called, matching the internal stored type.</summary>
    public static Task MatchAsync<T1, T2, T3, T4, T5, T6, T7, T8>(this Either<T1, T2, T3, T4, T5, T6, T7, T8> either, Func<T1, Task> a1, Func<T2, Task> a2, Func<T3, Task> a3, Func<T4, Task> a4, Func<T5, Task> a5, Func<T6, Task> a6, Func<T7, Task> a7, Func<T8, Task> a8)
    {
        if (either._tag == 0) Throw.InvalidOperationException(_invalidOperationMessage);
        if (either._tag == 1) return a1(either._t1);
        if (either._tag == 2) return a2(either._t2);
        if (either._tag == 3) return a3(either._t3);
        if (either._tag == 4) return a4(either._t4);
        if (either._tag == 5) return a5(either._t5);
        if (either._tag == 6) return a6(either._t6);
        if (either._tag == 7) return a7(either._t7);
        if (either._tag == 8) return a8(either._t8);
        return default;
    }

    #endregion

    #endregion
}
