using System;
using System.Threading.Tasks;

namespace ContainerExpressions.Containers;

public static class EitherExtensions
{
    private static ValueTask<T> Create<T>(T value) => new (value);
    private static ValueTask<T> Create<T>(Task<T> value) => new (value);

    public static Response TryGetT1<T1, T2>(this Either<T1, T2> either, out T1 t1) { if (either._tag == 1) { t1 = either._t1; return Response.Success; }; t1 = default; return Response.Error; }
    public static Response TryGetT2<T1, T2>(this Either<T1, T2> either, out T2 t2) { if (either._tag == 2) { t2 = either._t2; return Response.Success; }; t2 = default; return Response.Error; }

    public static Response<TResult> WhenT1<T1, T2, TResult>(this Either<T1, T2> either, Func<T1, TResult> func) => either._tag == 1 ? new Response<TResult>(func(either._t1)) : new Response<TResult>();
    public static Response<TResult> WhenT2<T1, T2, TResult>(this Either<T1, T2> either, Func<T2, TResult> func) => either._tag == 2 ? new Response<TResult>(func(either._t2)) : new Response<TResult>();

    public static ValueTask<Response<TResult>> WhenAsyncT1<T1, T2, TResult>(this Either<T1, T2> either, Func<T1, Task<TResult>> func)
    {
        ValueTask<Response<TResult>> vt = default;

        // new ValueTask<Response<TResult>>(func.LiftAsync()(either._t1));
        // new ValueTask<Response<TResult>>(new Response<TResult>());

        if (either._tag == 1)
        {
            vt = Create(func.LiftAsync()(either._t1));
        }
        else
        {
            vt = Create(new Response<TResult>());
        }

        return vt;
    }

    public static Response<TResult> WhenAsyncT2<T1, T2, TResult>(this Either<T1, T2> either, Func<T2, TResult> func) => either._tag == 2 ? new Response<TResult>(func(either._t2)) : new Response<TResult>();

    public static Response WhenT1<T1, T2>(this Either<T1, T2> either, Action<T1> action) { if (either._tag == 1) { action(either._t1); return Unit.ResponseSuccess; } return Unit.ResponseError; }
    public static Response WhenT2<T1, T2>(this Either<T1, T2> either, Action<T2> action) { if (either._tag == 2) { action(either._t2); return Unit.ResponseSuccess; } return Unit.ResponseError; }

    /// <summary>Transform Either into one type, the function matching the internal stored type is invoked.</summary>
    public static TResult Match<T1, T2, TResult>(this Either<T1, T2> either, Func<T1, TResult> f1, Func<T2, TResult> f2)
    {
        if (either._tag == 1) return f1(either._t1);
        if (either._tag == 2) return f2(either._t2);
        throw new InvalidOperationException("The internal type was not set, you must assign Either a type at least once.");
    }

    /// <summary>One of the actions will be called, matching the internal stored type.</summary>
    public static void Match<T1, T2>(this Either<T1, T2> either, Action<T1> a1, Action<T2> a2)
    {
             if (either._tag == 1) a1(either._t1);
        else if (either._tag == 2) a2(either._t2);
    }

    #region Other Type Permutations



    #endregion
}
