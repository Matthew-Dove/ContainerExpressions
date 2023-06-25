using System.Threading.Tasks;
using System.Threading;
using System;
using System.Runtime.CompilerServices;

namespace ContainerExpressions.Containers
{
    public readonly struct ResponseValueTask<T> : IEquatable<ResponseValueTask<T>>, IEquatable<Task<T>>, IEquatable<ValueTask<T>>, IEquatable<Response<T>>, IEquatable<T>
    {
        public ValueTask<T> Value { get; }
        private static readonly Task<T> _canceled = Task.FromCanceled<T>(new CancellationToken(true));

        public ResponseValueTask(Task<T> task) => Value = new ValueTask<T>(task ?? _canceled);
        public ResponseValueTask(ValueTask<T> valueTask) => Value = valueTask;
        public ResponseValueTask(T value) => Value = new ValueTask<T>(value);

        public static implicit operator Task<T>(ResponseValueTask<T> response) => response.Value.AsTask();
        public static implicit operator ResponseValueTask<T>(Task<T> task) => new ResponseValueTask<T>(task);

        public static implicit operator ValueTask<T>(ResponseValueTask<T> response) => response.Value;
        public static implicit operator ResponseValueTask<T>(ValueTask<T> task) => new ResponseValueTask<T>(task);

        public static implicit operator bool(ResponseValueTask<T> response) => response.Value.IsCompletedSuccessfully;
        public static implicit operator Response<T>(ResponseValueTask<T> response)
        {
            if (response.Value.IsCompleted)
            {
                if (response.Value.IsCompletedSuccessfully) return new Response<T>(response.Value.GetAwaiter().GetResult());
                if (response.Value.IsFaulted) response.Value.AsTask().Exception.LogErrorPlain();
                return new Response<T>();
            }
            return new Response<T>(response.Value.GetAwaiter().GetResult()); // Can error if cast before awaiting.
        }

        public static implicit operator T(ResponseValueTask<T> response) => response.Value.GetAwaiter().GetResult(); // Can error if cast before awaiting.
        public static implicit operator ResponseValueTask<T>(T value) => new ResponseValueTask<T>(value);

        public override string ToString() => Value.ToString();
        public override int GetHashCode() => Value.GetHashCode();

        public bool Equals(ResponseValueTask<T> other) => Equals(other.Value);
        public bool Equals(Task<T> other) => Equals(new ValueTask<T>(other ?? _canceled));
        public bool Equals(ValueTask<T> other) => other.Equals(Value);

        public bool Equals(Response<T> other)
        {
            if (!other.IsValid) return Value.IsCompleted && (!Value.IsCompletedSuccessfully);
            return Value.IsCompletedSuccessfully && Equals(new ValueTask<T>(other.Value));
        }

        public bool Equals(T other) => Equals(new ValueTask<T>(other));

        public override bool Equals(object obj)
        {
            return obj switch
            {
                ResponseValueTask<T> other => Equals(other),
                Task<T> task => Equals(task),
                ValueTask<T> valueTask => Equals(valueTask),
                Response<T> response => Equals(response),
                T value => Equals(value),
                _ => false
            };
        }

        public static bool operator !=(ResponseValueTask<T> x, ResponseValueTask<T> y) => !(x == y);
        public static bool operator ==(ResponseValueTask<T> x, ResponseValueTask<T> y) => x.Equals(y);

        public static bool operator !=(ResponseValueTask<T> x, Task<T> y) => !(x == y);
        public static bool operator ==(ResponseValueTask<T> x, Task<T> y) => x.Equals(y);
        public static bool operator !=(Task<T> x, ResponseValueTask<T> y) => !(x == y);
        public static bool operator ==(Task<T> x, ResponseValueTask<T> y) => y.Equals(x);

        public static bool operator !=(ResponseValueTask<T> x, ValueTask<T> y) => !(x == y);
        public static bool operator ==(ResponseValueTask<T> x, ValueTask<T> y) => x.Equals(y);
        public static bool operator !=(ValueTask<T> x, ResponseValueTask<T> y) => !(x == y);
        public static bool operator ==(ValueTask<T> x, ResponseValueTask<T> y) => y.Equals(x);

        public static bool operator !=(ResponseValueTask<T> x, Response<T> y) => !(x == y);
        public static bool operator ==(ResponseValueTask<T> x, Response<T> y) => x.Equals(y);
        public static bool operator !=(Response<T> x, ResponseValueTask<T> y) => !(x == y);
        public static bool operator ==(Response<T> x, ResponseValueTask<T> y) => y.Equals(x);

        public static bool operator !=(ResponseValueTask<T> x, T y) => !(x == y);
        public static bool operator ==(ResponseValueTask<T> x, T y) => x.Equals(y);
        public static bool operator !=(T x, ResponseValueTask<T> y) => !(x == y);
        public static bool operator ==(T x, ResponseValueTask<T> y) => y.Equals(x);
    }

    public readonly struct ResponseValueTask : IEquatable<ResponseValueTask>, IEquatable<Task>, IEquatable<ValueTask>, IEquatable<Response>
    {
        public ValueTask Value { get; }
        private static readonly Task _canceled = Task.FromCanceled(new CancellationToken(true));

        public ResponseValueTask(Task task) => Value = new ValueTask(task ?? _canceled);
        public ResponseValueTask(ValueTask valueTask) => Value = valueTask;

        public static implicit operator Task(ResponseValueTask response) => response.Value.AsTask();
        public static implicit operator ResponseValueTask(Task task) => new ResponseValueTask(task);

        public static implicit operator ValueTask(ResponseValueTask response) => response.Value;
        public static implicit operator ResponseValueTask(ValueTask task) => new ResponseValueTask(task);

        public static implicit operator bool(ResponseValueTask response) => response.Value.IsCompletedSuccessfully;
        public static implicit operator Response(ResponseValueTask response)
        {
            if (response.Value.IsCompleted)
            {
                if (response.Value.IsCompletedSuccessfully) return new Response(true);
                if (response.Value.IsFaulted) response.Value.AsTask().Exception.LogErrorPlain();
            }
            return new Response();
        }

        public override string ToString() => Value.ToString();
        public override int GetHashCode() => Value.GetHashCode();

        public bool Equals(ResponseValueTask other) => Equals(other.Value);
        public bool Equals(Task other) => Equals(new ValueTask(other ?? _canceled));
        public bool Equals(ValueTask other) => other.Equals(Value);

        public bool Equals(Response other)
        {
            if (!other.IsValid) return Value.IsCompleted && (!Value.IsCompletedSuccessfully);
            return new Response(Value.IsCompletedSuccessfully);
        }

        public override bool Equals(object obj)
        {
            return obj switch
            {
                ResponseValueTask other => Equals(other),
                Task task => Equals(task),
                ValueTask valueTask => Equals(valueTask),
                Response response => Equals(response),
                _ => false
            };
        }

        public static bool operator !=(ResponseValueTask x, ResponseValueTask y) => !(x == y);
        public static bool operator ==(ResponseValueTask x, ResponseValueTask y) => x.Equals(y);

        public static bool operator !=(ResponseValueTask x, Task y) => !(x == y);
        public static bool operator ==(ResponseValueTask x, Task y) => x.Equals(y);
        public static bool operator !=(Task x, ResponseValueTask y) => !(x == y);
        public static bool operator ==(Task x, ResponseValueTask y) => y.Equals(x);

        public static bool operator !=(ResponseValueTask x, ValueTask y) => !(x == y);
        public static bool operator ==(ResponseValueTask x, ValueTask y) => x.Equals(y);
        public static bool operator !=(ValueTask x, ResponseValueTask y) => !(x == y);
        public static bool operator ==(ValueTask x, ResponseValueTask y) => y.Equals(x);

        public static bool operator !=(ResponseValueTask x, Response y) => !(x == y);
        public static bool operator ==(ResponseValueTask x, Response y) => x.Equals(y);
        public static bool operator !=(Response x, ResponseValueTask y) => !(x == y);
        public static bool operator ==(Response x, ResponseValueTask y) => y.Equals(x);
    }

    public static class ResponseValueTaskAwaiterExtensions
    {
        // ResponseValueTask with no result.
        public static ValueTaskAwaiter<Response> GetAwaiter(this ResponseValueTask response)
        {
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

        // ResponseValueTask with T result.
        public static ValueTaskAwaiter<Response<T>> GetAwaiter<T>(this ResponseValueTask<T> response)
        {
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
    }
}
