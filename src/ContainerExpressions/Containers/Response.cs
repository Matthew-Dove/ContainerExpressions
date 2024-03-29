﻿using ContainerExpressions.Containers.Internal;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace ContainerExpressions.Containers
{
    /// <summary>
    /// A wrapper around the real value a method will return.
    /// <para>Using this pattern you can tell if a returned value is the correct one, or if some error happened trying to get the real value.</para>
    /// </summary>
    /// <typeparam name="T">The value to return.</typeparam>
    public readonly struct Response<T> : IEquatable<Response<T>>, IEquatable<T>
    {
        /// <summary>Creates a new Response T, in an invalid state.</summary>
        public static Response<T> Error { get => new Response<T>(); }

        /// <summary>Creates a new Response T, in a valid state.</summary>
        public static Response<T> Success(T value) => new Response<T>(value);

        /// <summary>True if the value was set correctly, false if some error occurred getting the value.</summary>
        public bool IsValid { get; }

        /// <summary>The value that was calculated, with the guarantee it's in a valid state, throws InvalidOperationException if Response is in an invalid state.</summary>
        public T Value { get { if (!IsValid) Throw.InvalidOperationException("Cannot access the value when the container is not valid."); return _value; } }
        private readonly T _value;

        /// <summary>Create a response container in a valid state.</summary>
        /// <param name="value">The response's value.</param>
        public Response(T value)
        {
            _value = value;
            IsValid = true;
        }

        /// <summary>When compared to a bool, the IsValid property value will be used.</summary>
        public static implicit operator bool(Response<T> response) => response.IsValid;

        /// <summary>When compared to T, the Value property will be used.</summary>
        public static implicit operator T(Response<T> response) => response.Value;

        /// <summary>When compared to Response, the IsValid property will be used to create the response model.</summary>
        public static implicit operator Response(Response<T> response) => new Response(response.IsValid);

        /// <summary>Returns the underlying value's string representation.</summary>
        public override string ToString() => IsValid ? _value?.ToString() ?? string.Empty : string.Empty;

        /// <summary>If both response types are valid, then the values are compared, otherwise the IsValid property is compared.</summary>
        public bool Equals(Response<T> other)
        {
            if (other.IsValid && IsValid)
            {
                if (_value == null) return other._value == null;
                if (other._value == null) return _value == null;
                return EqualityComparer<T>.Default.Equals(_value, other._value);
            }
            return other.IsValid == IsValid; // i.e. are both response containers invalid (false).
        }

        /// <summary>Compares the provided value, to the Response's Value, throws InvalidOperationException if Response is in an invalid state.</summary>
        public bool Equals(T value)
        {
            if (!IsValid) Throw.InvalidOperationException("Cannot access the value when the container is not valid.");
            if (_value == null) return value == null;
            return EqualityComparer<T>.Default.Equals(_value, value);
        }

        /// <summary>Compares obj if it is of type Response T, or of type T. Throws InvalidOperationException if Response is in an invalid state, and obj is a T value.</summary>
        public override bool Equals(object obj) => obj is T value && Equals(value) || obj is Response<T> response && Equals(response);

        /// <summary>Returns the Value's hash code (T), or 0 if there is no value.</summary>
        public override int GetHashCode() => IsValid ? Value?.GetHashCode() ?? 0 : 0;

        public static bool operator !=(Response<T> x, Response<T> y) => !(x == y);
        public static bool operator ==(Response<T> x, Response<T> y) => x.Equals(y);

        public static bool operator !=(Response<T> x, T y) => !(x == y);
        public static bool operator ==(Response<T> x, T y) => x.Equals(y);

        public static bool operator !=(T x, Response<T> y) => !(x == y);
        public static bool operator ==(T x, Response<T> y) => y.Equals(x);
    }

    /// <summary>A helper class for the Response generic class.</summary>
    public readonly struct Response : IEquatable<Response>, IEquatable<bool>
    {
        /// <summary>Creates a new Response, in a valid state.</summary>
        public static Response Success { get => new Response(true); }

        /// <summary>Creates a new Response, in an invalid state.</summary>
        public static Response Error { get => new Response(false); }

        /// <summary>True if the container is in a valid state, otherwise the operation didn't run successfully.</summary>
        public bool IsValid { get; }

        /// <summary>Create a response container in an valid state.</summary>
        public Response(bool isValid)
        {
            IsValid = isValid;
        }

        /// <summary>Create a response container in a valid state.</summary>
        /// <param name="value">The response's value.</param>
        public static Response<T> Create<T>(T value) => new Response<T>(value);

        /// <summary>Create a response container in an invalid state.</summary>
        public static Response<T> Create<T>() => new Response<T>();

        /// <summary>Gets the string value for if this response is valid or not.</summary>
        /// <returns>The bool string value for the IsValid Property.</returns>
        public override string ToString() => IsValid.ToString();

        /// <summary>When compared to a bool, the IsValid properties value will be used.</summary>
        public static implicit operator bool(Response response) => response.IsValid;

        /// <summary>When compared to Response Unit, the IsValid property will be used to select a success, or error Response Unit instance.</summary>
        public static implicit operator Response<Unit>(Response response) => response.IsValid ? Unit.ResponseSuccess : Unit.ResponseError;

        public override int GetHashCode() => IsValid.GetHashCode();

        public bool Equals(Response other) => other.IsValid == IsValid;
        public bool Equals(bool value) => value == IsValid;
        public override bool Equals(object obj) => obj is Response value && Equals(value);

        public static bool operator !=(Response x, Response y) => !(x == y);
        public static bool operator ==(Response x, Response y) => x.Equals(y);

        public static bool operator !=(Response x, bool y) => !(x == y);
        public static bool operator ==(Response x, bool y) => x.Equals(y);
        public static bool operator !=(bool x, Response y) => !(x == y);
        public static bool operator ==(bool x, Response y) => y.Equals(x);
    }

    public static class ResponseAwaiterExtensions
    {
        #region Response

        // ValueTask with no result.
        public static ValueTaskAwaiter<Response> GetAwaiter(this Response<ValueTask> response)
        {
            if (!response) return new ValueTask<Response>(Response.Error).GetAwaiter();

            if (response.Value.IsCompleted)
            {
                if (response.Value.IsCompletedSuccessfully) return new ValueTask<Response>(Response.Success).GetAwaiter();
                if (response.Value.IsFaulted) response.Value.AsTask().Exception.LogErrorPlain();
                return new ValueTask<Response>(Response.Error).GetAwaiter();
            }

            return new ValueTask<Response>(response.Value.AsTask().ContinueWith(static t =>
            {
                if (t.Status == TaskStatus.Faulted) t.Exception.LogErrorPlain();
                if (t.Status == TaskStatus.RanToCompletion) return Response.Success;
                return Response.Error;
            })).GetAwaiter();
        }

        // ValueTask with T result.
        public static ValueTaskAwaiter<Response<T>> GetAwaiter<T>(this Response<ValueTask<T>> response)
        {
            if (!response) return new ValueTask<Response<T>>(Response<T>.Error).GetAwaiter();

            if (response.Value.IsCompleted)
            {
                if (response.Value.IsCompletedSuccessfully) return new ValueTask<Response<T>>(Response.Create(response.Value.Result)).GetAwaiter();
                if (response.Value.IsFaulted) response.Value.AsTask().Exception.LogErrorPlain();
                return new ValueTask<Response<T>>(Response.Create<T>()).GetAwaiter();
            }

            return new ValueTask<Response<T>>(response.Value.AsTask().ContinueWith(static t =>
            {
                if (t.Status == TaskStatus.Faulted) t.Exception.LogErrorPlain();
                if (t.Status == TaskStatus.RanToCompletion) return Response.Create(t.Result);
                return new Response<T>();
            })).GetAwaiter();
        }

        // Task with no result.
        public static TaskAwaiter<Response> GetAwaiter(this Response<Task> response)
        {
            if (!response) return InstanceAsync.Of<Response>().GetAwaiter();
            return response.Value.ContinueWith(static t => {
                if (t.Status == TaskStatus.Faulted) t.Exception.LogErrorPlain();
                if (t.Status == TaskStatus.RanToCompletion) return Response.Success;
                return Response.Error;
            }).GetAwaiter();
        }

        // Task with T result.
        public static TaskAwaiter<Response<T>> GetAwaiter<T>(this Response<Task<T>> response)
        {
            if (!response) return InstanceAsync.Of<Response<T>>().GetAwaiter();
            return response.Value.ContinueWith(static t => {
                if (t.Status == TaskStatus.Faulted) t.Exception.LogErrorPlain();
                if (t.Status == TaskStatus.RanToCompletion) return Response.Create(t.Result);
                return new Response<T>();
            }).GetAwaiter();
        }

        #endregion

        #region Unpack

        public static TaskAwaiter<Response> GetAwaiter(this Response<Task<Response<Response>>> response)
        {
            if (!response) return InstanceAsync.Of<Response>().GetAwaiter();
            return response.Value.UnpackAsync().GetAwaiter();
        }

        public static TaskAwaiter<Response> GetAwaiter(this Response<Task<Response<Task<Response>>>> response)
        {
            if (!response) return InstanceAsync.Of<Response>().GetAwaiter();
            return response.Value.UnpackAsync().GetAwaiter();
        }

        public static TaskAwaiter<Response<T>> GetAwaiter<T>(this Response<Task<Response<Response<T>>>> response)
        {
            if (!response) return InstanceAsync.Of<Response<T>>().GetAwaiter();
            return response.Value.UnpackAsync().GetAwaiter();
        }

        public static TaskAwaiter<Response<T>> GetAwaiter<T>(this Response<Task<Response<Task<Response<T>>>>> response)
        {
            if (!response) return InstanceAsync.Of<Response<T>>().GetAwaiter();
            return response.Value.UnpackAsync().GetAwaiter();
        }

        #endregion
    }
}