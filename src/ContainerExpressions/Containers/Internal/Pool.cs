using System.Threading.Tasks;

namespace ContainerExpressions.Containers.Internal
{
    /// <summary>
    /// Simular to Instance, and InstanceAsync in that Pool holds static readonly object caches.
    /// <para>But Instance returns the same cache for all values in a type; Pool can return different object instances of the same type.</para>
    /// </summary>
    internal static class Pool<T>
    {
        public static readonly Task<Response<T>> ResponseSuccess = Task.FromResult(new Response<T>(default));
        public static readonly Task<Response<T>> ResponseError = Task.FromResult(new Response<T>());
    }

    internal static class Pool
    {
        public static readonly Task<Response> ResponseSuccess = Task.FromResult(new Response(true));
        public static readonly Task<Response> ResponseError = Task.FromResult(new Response(false));

        public static readonly Task<Response<Unit>> UnitResponseSuccess = Task.FromResult(new Response<Unit>(Unit.Instance));
        public static readonly Task<Response<Unit>> UnitResponseError = Task.FromResult(new Response<Unit>());
    }
}
