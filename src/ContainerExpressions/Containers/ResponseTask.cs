using System.Threading.Tasks;
using System.Threading;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace ContainerExpressions.Containers
{
    public readonly struct ResponseTask<T> : IEquatable<ResponseTask<T>>, IEquatable<Task<T>>
    {
        public Task<T> Value { get; }
        private static readonly Task<T> _canceled = Task.FromCanceled<T>(new CancellationToken(true));

        public ResponseTask(Task<T> task) => Value = task ?? _canceled;

        internal static bool IsCompletedSuccessfully(Task<T> t) => t.IsCompleted && !(t.IsFaulted || t.IsCanceled);

        public static implicit operator Task<T>(ResponseTask<T> response) => response.Value;
        public static implicit operator ResponseTask<T>(Task<T> task) => new ResponseTask<T>(task);

        public static implicit operator bool(ResponseTask<T> response) => IsCompletedSuccessfully(response.Value);

        public override string ToString() => Value.ToString();
        public override int GetHashCode() => Value.GetHashCode();

        public bool Equals(ResponseTask<T> other) => Equals(other.Value);
        public bool Equals(Task<T> other)
        {
            if (Value == null) return other == null;
            if (other == null) return false;

            if (Value.IsCompleted && other.IsCompleted)
            {
                if (Value.IsCanceled) return other.IsCanceled;
                if (Value.IsFaulted) return other.IsFaulted;
                if (IsCompletedSuccessfully(Value))
                {
                    if (!IsCompletedSuccessfully(other)) return false;
                    return EqualityComparer<T>.Default.Equals(Value.GetAwaiter().GetResult(), other.GetAwaiter().GetResult());
                }
            }

            return Value.Equals(other);
        }

        public override bool Equals(object obj)
        {
            return obj switch
            {
                ResponseTask<T> other => Equals(other),
                Task<T> task => Equals(task),
                _ => false
            };
        }

        public static bool operator !=(ResponseTask<T> x, ResponseTask<T> y) => !(x == y);
        public static bool operator ==(ResponseTask<T> x, ResponseTask<T> y) => x.Equals(y);

        public static bool operator !=(ResponseTask<T> x, Task<T> y) => !(x == y);
        public static bool operator ==(ResponseTask<T> x, Task<T> y) => x.Equals(y);
        public static bool operator !=(Task<T> x, ResponseTask<T> y) => !(x == y);
        public static bool operator ==(Task<T> x, ResponseTask<T> y) => y.Equals(x);
    }

    public readonly struct ResponseTask : IEquatable<ResponseTask>, IEquatable<Task>
    {
        public Task Value { get; }
        private static readonly Task _canceled = Task.FromCanceled(new CancellationToken(true));

        public ResponseTask(Task task) => Value = task ?? _canceled;

        internal static bool IsCompletedSuccessfully(Task t) => t.IsCompleted && !(t.IsFaulted || t.IsCanceled);

        public static implicit operator Task(ResponseTask response) => response.Value;
        public static implicit operator ResponseTask(Task task) => new ResponseTask(task);

        public static implicit operator bool(ResponseTask response) => IsCompletedSuccessfully(response.Value);

        public override string ToString() => Value.ToString();
        public override int GetHashCode() => Value.GetHashCode();

        public bool Equals(ResponseTask other) => Equals(other.Value);
        public bool Equals(Task other)
        {
            if (Value == null) return other == null;
            if (other == null) return false;

            if (Value.IsCompleted && other.IsCompleted)
            {
                if (Value.IsCanceled) return other.IsCanceled;
                if (Value.IsFaulted) return other.IsFaulted;
                if (IsCompletedSuccessfully(Value)) return IsCompletedSuccessfully(other);
            }

            return Value.Equals(other);
        }

        public override bool Equals(object obj)
        {
            return obj switch
            {
                ResponseTask other => Equals(other),
                Task task => Equals(task),
                _ => false
            };
        }

        public static bool operator !=(ResponseTask x, ResponseTask y) => !(x == y);
        public static bool operator ==(ResponseTask x, ResponseTask y) => x.Equals(y);

        public static bool operator !=(ResponseTask x, Task y) => !(x == y);
        public static bool operator ==(ResponseTask x, Task y) => x.Equals(y);
        public static bool operator !=(Task x, ResponseTask y) => !(x == y);
        public static bool operator ==(Task x, ResponseTask y) => y.Equals(x);
    }

    public static class ResponseTaskAwaiterExtensions
    {
        private static readonly Task<Response> _success = Task.FromResult(Response.Success);

        // ResponseTask with no result.
        public static TaskAwaiter<Response> GetAwaiter(this ResponseTask response)
        {
            if (response.Value.IsCompleted)
            {
                if (response.Value.Status == TaskStatus.Faulted) response.Value.Exception.LogError();
                if (response.Value.Status == TaskStatus.RanToCompletion) return _success.GetAwaiter();
                return InstanceAsync.Of<Response>().GetAwaiter();
            }

            return response.Value.ContinueWith(static t =>
            {
                if (t.Status == TaskStatus.Faulted) t.Exception.LogError();
                if (t.Status == TaskStatus.RanToCompletion) return Response.Success;
                return Response.Error;
            }).GetAwaiter();
        }

        // ResponseTask with T result.
        public static TaskAwaiter<Response<T>> GetAwaiter<T>(this ResponseTask<T> response)
        {
            if (response.Value.IsCompleted)
            {
                if (response.Value.Status == TaskStatus.Faulted) response.Value.Exception.LogError();
                if (response.Value.Status == TaskStatus.RanToCompletion) return Task.FromResult(Response<T>.Success(response.Value.GetAwaiter().GetResult())).GetAwaiter();
                return InstanceAsync.Of<Response<T>>().GetAwaiter();
            }

            return response.Value.ContinueWith(static t =>
            {
                if (t.Status == TaskStatus.Faulted) t.Exception.LogError();
                if (t.Status == TaskStatus.RanToCompletion) return Response<T>.Success(t.GetAwaiter().GetResult());
                return Response<T>.Error;
            }).GetAwaiter();
        }
    }
}
