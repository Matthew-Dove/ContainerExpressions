using System;

namespace ContainerExpressions.Containers
{
    // Common types for reuse with containers i.e. - Either, Response, and Maybe.

    /// <summary>A marker type to use when you have nothing to return.</summary>
    public sealed class Unit : IEquatable<Unit>
    {
        /// <summary>Cached Unit Instance.</summary>
        public static readonly Unit Instance = new Unit();

        /// <summary>A Response Unit in a valid state.</summary>
        public static Response<Unit> ResponseSuccess => new Response<Unit>(Instance);

        /// <summary>A Response Unit in an invalid state.</summary>
        public static Response<Unit> ResponseError => new Response<Unit>();

        /// <summary>A Maybe Unit, with the value set.</summary>
        public static Maybe<Unit> MaybeValue => new Maybe<Unit>(Instance);

        /// <summary>A Maybe Unit, with the error set.</summary>
        public static Maybe<Unit> MaybeError(Exception ex) => new Maybe<Unit>(ex);

        private Unit() { }

        public bool Equals(Unit other) => (object)other != null;
        public override bool Equals(object obj) => obj is Unit other && Equals(other);
        public override int GetHashCode() => 1;
        public override string ToString() => string.Empty;

        public static bool operator !=(Unit x, Unit y) => !(x == y);
        public static bool operator ==(Unit x, Unit y) => x.Equals(y);
    }

    public static class Unit<T>
    {
        /// <summary>A Maybe Unit, with the value set.</summary>
        public static Maybe<Unit, T> MaybeValue => new Maybe<Unit, T>(Unit.Instance);

        /// <summary>A Maybe Unit, with the error set.</summary>
        public static Maybe<Unit, T> MaybeError(T error) => new Maybe<Unit, T>(error);
    }

    #region HTTP

    /// <summary>Ok: 200 / Created: 201 / Accepted: 202 / NoContent: 204 (Your request was successfully processed).</summary>
    public readonly struct Ok { }

    /// <summary>Ok: 200 / Created: 201 / Accepted: 202 / NoContent: 204 (Your request was successfully processed).</summary>
    public readonly struct Ok<T>
    {
        public T Value { get; }

        public Ok(T value)
        {
            Value = value;
        }
    }

    /// <summary>BadRequest: 400 (Validation error on your request).</summary>
    public readonly struct BadRequest { }

    /// <summary>BadRequest: 400 (Validation error on your request).</summary>
    public readonly struct BadRequest<T>
    {
        public T Value { get; }

        public BadRequest(T value)
        {
            Value = value;
        }
    }

    /// <summary>Unauthorized: 401 (We do not know who you are).</summary>
    public readonly struct Unauthorized { }

    /// <summary>Unauthorized: 401 (We do not know who you are).</summary>
    public readonly struct Unauthorized<T>
    {
        public T Value { get; }

        public Unauthorized(T value)
        {
            Value = value;
        }
    }

    /// <summary>Forbidden: 403 (We know who you are, but you are not allowed to be here).</summary>
    public readonly struct Forbidden { }

    /// <summary>Forbidden: 403 (We know who you are, but you are not allowed to be here).</summary>
    public readonly struct Forbidden<T>
    {
        public T Value { get; }

        public Forbidden(T value)
        {
            Value = value;
        }
    }

    /// <summary>NotFound: 404 (Requested server resource was not found).</summary>
    public readonly struct NotFound { }

    /// <summary>NotFound: 404 (Requested server resource was not found).</summary>
    public readonly struct NotFound<T>
    {
        public T Value { get; }

        public NotFound(T value)
        {
            Value = value;
        }
    }

    /// <summary>TooManyRequests: 429 (Rate Limit).</summary>
    public readonly struct TooManyRequests { }

    /// <summary>TooManyRequests: 429 (Rate Limit).</summary>
    public readonly struct TooManyRequests<T>
    {
        public T Value { get; }

        public TooManyRequests(T value)
        {
            Value = value;
        }
    }

    /// <summary>Error: 500 (Internal Server Error).</summary>
    public readonly struct Error { }

    /// <summary>Error: 500 (Server Error).</summary>
    public readonly struct Error<T>
    {
        public T Value { get; }

        public Error(T value)
        {
            Value = value;
        }
    }

    /// <summary>BadGateway: 502 (Upstream Error).</summary>
    public readonly struct BadGateway { }

    /// <summary>BadGateway: 502 (Upstream Error).</summary>
    public readonly struct BadGateway<T>
    {
        public T Value { get; }

        public BadGateway(T value)
        {
            Value = value;
        }
    }

    /// <summary>GatewayTimeout: 504 (Upstream Timeout).</summary>
    public readonly struct GatewayTimeout { }

    /// <summary>GatewayTimeout: 504 (Upstream Timeout).</summary>
    public readonly struct GatewayTimeout<T>
    {
        public T Value { get; }

        public GatewayTimeout(T value)
        {
            Value = value;
        }
    }

    #endregion
}
