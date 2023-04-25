namespace ContainerExpressions.Containers
{
    // Common types for reuse with containers i.e. - Either, Response, and Maybe.

    /// <summary>A marker type to use when you have no return type, or result.</summary>
    public readonly struct Unit
    {
        /// <summary>A cached Response Unit in a valid state.</summary>
        public static readonly Response<Unit> Success = new Response<Unit>(new Unit());

        /// <summary>A cached Response Unit in an invalid state.</summary>
        public static readonly Response<Unit> Error = new Response<Unit>();
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
