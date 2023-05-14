using System.Threading.Tasks;
using System.Threading;
using System;
using System.Runtime.CompilerServices;

namespace ContainerExpressions.Containers
{
    public readonly struct ResponseTask<T> : IEquatable<ResponseTask<T>>, IEquatable<Task<T>>, IEquatable<ValueTask<T>>, IEquatable<Response<T>>, IEquatable<T>
    {
        public ValueTask<T> ValueTask { get; }
        private static readonly Task<T> _canceled = Task.FromCanceled<T>(new CancellationToken(true));

        public ResponseTask(Task<T> task) => ValueTask = new ValueTask<T>(task ?? _canceled);
        public ResponseTask(ValueTask<T> valueTask) => ValueTask = valueTask;
        public ResponseTask(T value) => ValueTask = new ValueTask<T>(value);

        public static implicit operator Task<T>(ResponseTask<T> response) => response.ValueTask.AsTask();
        public static implicit operator ResponseTask<T>(Task<T> task) => new ResponseTask<T>(task);

        public static implicit operator ValueTask<T>(ResponseTask<T> response) => response.ValueTask;
        public static implicit operator ResponseTask<T>(ValueTask<T> task) => new ResponseTask<T>(task);

        public static implicit operator bool(ResponseTask<T> response) => response.ValueTask.IsCompletedSuccessfully;
        public static implicit operator Response<T>(ResponseTask<T> response)
        {
            if (response.ValueTask.IsCompleted)
            {
                if (response.ValueTask.IsCompletedSuccessfully) return new Response<T>(response.ValueTask.GetAwaiter().GetResult());
                if (response.ValueTask.IsFaulted) response.ValueTask.AsTask().Exception.LogError();
                return new Response<T>();
            }
            return new Response<T>(response.ValueTask.GetAwaiter().GetResult()); // Can error if cast before awaiting.
        }

        public static implicit operator T(ResponseTask<T> response) => response.ValueTask.GetAwaiter().GetResult(); // Can error if cast before awaiting.
        public static implicit operator ResponseTask<T>(T value) => new ResponseTask<T>(value);

        public override string ToString() => ValueTask.ToString();
        public override int GetHashCode() => ValueTask.GetHashCode();

        public bool Equals(ResponseTask<T> other) => Equals(other.ValueTask);
        public bool Equals(Task<T> other) => Equals(new ValueTask<T>(other ?? _canceled));
        public bool Equals(ValueTask<T> other) => other.Equals(ValueTask);

        public bool Equals(Response<T> other)
        {
            if (!other.IsValid) return ValueTask.IsCompleted && (!ValueTask.IsCompletedSuccessfully);
            return ValueTask.IsCompletedSuccessfully && Equals(new ValueTask<T>(other.Value));
        }

        public bool Equals(T other) => Equals(new ValueTask<T>(other));

        public override bool Equals(object obj)
        {
            return obj switch
            {
                ResponseTask<T> other => Equals(other),
                Task<T> task => Equals(task),
                ValueTask<T> valueTask => Equals(valueTask),
                Response<T> response => Equals(response),
                T value => Equals(value),
                _ => false
            };
        }

        public static bool operator !=(ResponseTask<T> x, ResponseTask<T> y) => !(x == y);
        public static bool operator ==(ResponseTask<T> x, ResponseTask<T> y) => x.Equals(y);

        public static bool operator !=(ResponseTask<T> x, Task<T> y) => !(x == y);
        public static bool operator ==(ResponseTask<T> x, Task<T> y) => x.Equals(y);
        public static bool operator !=(Task<T> x, ResponseTask<T> y) => !(x == y);
        public static bool operator ==(Task<T> x, ResponseTask<T> y) => y.Equals(x);

        public static bool operator !=(ResponseTask<T> x, ValueTask<T> y) => !(x == y);
        public static bool operator ==(ResponseTask<T> x, ValueTask<T> y) => x.Equals(y);
        public static bool operator !=(ValueTask<T> x, ResponseTask<T> y) => !(x == y);
        public static bool operator ==(ValueTask<T> x, ResponseTask<T> y) => y.Equals(x);

        public static bool operator !=(ResponseTask<T> x, Response<T> y) => !(x == y);
        public static bool operator ==(ResponseTask<T> x, Response<T> y) => x.Equals(y);
        public static bool operator !=(Response<T> x, ResponseTask<T> y) => !(x == y);
        public static bool operator ==(Response<T> x, ResponseTask<T> y) => y.Equals(x);

        public static bool operator !=(ResponseTask<T> x, T y) => !(x == y);
        public static bool operator ==(ResponseTask<T> x, T y) => x.Equals(y);
        public static bool operator !=(T x, ResponseTask<T> y) => !(x == y);
        public static bool operator ==(T x, ResponseTask<T> y) => y.Equals(x);
    }

    public readonly struct ResponseTask : IEquatable<ResponseTask>, IEquatable<Task>, IEquatable<ValueTask>, IEquatable<Response>
    {
        public ValueTask ValueTask { get; }
        private static readonly Task _canceled = Task.FromCanceled(new CancellationToken(true));

        public ResponseTask(Task task) => ValueTask = new ValueTask(task ?? _canceled);
        public ResponseTask(ValueTask valueTask) => ValueTask = valueTask;

        public static implicit operator Task(ResponseTask response) => response.ValueTask.AsTask();
        public static implicit operator ResponseTask(Task task) => new ResponseTask(task);

        public static implicit operator ValueTask(ResponseTask response) => response.ValueTask;
        public static implicit operator ResponseTask(ValueTask task) => new ResponseTask(task);

        public static implicit operator bool(ResponseTask response) => response.ValueTask.IsCompletedSuccessfully;
        public static implicit operator Response(ResponseTask response)
        {
            if (response.ValueTask.IsCompleted)
            {
                if (response.ValueTask.IsCompletedSuccessfully) return new Response(true);
                if (response.ValueTask.IsFaulted) response.ValueTask.AsTask().Exception.LogError();
            }
            return new Response();
        }

        public override string ToString() => ValueTask.ToString();
        public override int GetHashCode() => ValueTask.GetHashCode();

        public bool Equals(ResponseTask other) => Equals(other.ValueTask);
        public bool Equals(Task other) => Equals(new ValueTask(other ?? _canceled));
        public bool Equals(ValueTask other) => other.Equals(ValueTask);

        public bool Equals(Response other)
        {
            if (!other.IsValid) return ValueTask.IsCompleted && (!ValueTask.IsCompletedSuccessfully);
            return new Response(ValueTask.IsCompletedSuccessfully);
        }

        public override bool Equals(object obj)
        {
            return obj switch
            {
                ResponseTask other => Equals(other),
                Task task => Equals(task),
                ValueTask valueTask => Equals(valueTask),
                Response response => Equals(response),
                _ => false
            };
        }

        public static bool operator !=(ResponseTask x, ResponseTask y) => !(x == y);
        public static bool operator ==(ResponseTask x, ResponseTask y) => x.Equals(y);

        public static bool operator !=(ResponseTask x, Task y) => !(x == y);
        public static bool operator ==(ResponseTask x, Task y) => x.Equals(y);
        public static bool operator !=(Task x, ResponseTask y) => !(x == y);
        public static bool operator ==(Task x, ResponseTask y) => y.Equals(x);

        public static bool operator !=(ResponseTask x, ValueTask y) => !(x == y);
        public static bool operator ==(ResponseTask x, ValueTask y) => x.Equals(y);
        public static bool operator !=(ValueTask x, ResponseTask y) => !(x == y);
        public static bool operator ==(ValueTask x, ResponseTask y) => y.Equals(x);

        public static bool operator !=(ResponseTask x, Response y) => !(x == y);
        public static bool operator ==(ResponseTask x, Response y) => x.Equals(y);
        public static bool operator !=(Response x, ResponseTask y) => !(x == y);
        public static bool operator ==(Response x, ResponseTask y) => y.Equals(x);
    }

    public static class ResponseTaskAwaiterExtensions
    {
        // ResponseTask with no result.
        public static ValueTaskAwaiter<Response> GetAwaiter(this ResponseTask response)
        {
            if (response.ValueTask.IsCompleted)
            {
                if (response.ValueTask.IsCompletedSuccessfully) return new ValueTask<Response>(Response.Success).GetAwaiter();
                if (response.ValueTask.IsFaulted) response.ValueTask.AsTask().Exception.LogError();
                return new ValueTask<Response>(Response.Error).GetAwaiter();
            }

            return new ValueTask<Response>(response.ValueTask.AsTask().ContinueWith(static t =>
            {
                if (t.Status == TaskStatus.Faulted) t.Exception.LogError();
                if (t.Status == TaskStatus.RanToCompletion) return Response.Success;
                return Response.Error;
            })).GetAwaiter();
        }

        // ResponseTask with T result.
        public static ValueTaskAwaiter<Response<T>> GetAwaiter<T>(this ResponseTask<T> response)
        {
            if (response.ValueTask.IsCompleted)
            {
                if (response.ValueTask.IsCompletedSuccessfully) return new ValueTask<Response<T>>(Response.Create(response.ValueTask.Result)).GetAwaiter();
                if (response.ValueTask.IsFaulted) response.ValueTask.AsTask().Exception.LogError();
                return new ValueTask<Response<T>>(Response.Create<T>()).GetAwaiter();
            }

            return new ValueTask<Response<T>>(response.ValueTask.AsTask().ContinueWith(static t =>
            {
                if (t.Status == TaskStatus.Faulted) t.Exception.LogError();
                if (t.Status == TaskStatus.RanToCompletion) return Response.Create(t.Result);
                return new Response<T>();
            })).GetAwaiter();
        }
    }
}
