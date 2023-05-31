using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Threading;
using System;

namespace ContainerExpressions.Containers
{
    file sealed class Box<T> : Alias<T> { public Box(T value) : base(value) { } } // Force T on the heap, so we can mark it volatile; skipping a lock.

    // Bag of things for ResponseAsync{T}, so we can make it a readonly struct.
    file sealed class Bag<T>
    {
        /**
         * [Volatile WORM access pattern]
         * get: read > isDefault > volatile read
         * set: volatile write
        **/
        public Box<T> Result
        {
            get { return _result == default ? Volatile.Read(ref _result) : _result; }
            set { _result = value; Volatile.Write(ref _result, value); }
        }

        public bool IsCompleted
        {
            get { return _isCompleted == default ? Volatile.Read(ref _isCompleted) : _isCompleted; }
            set { _isCompleted = value; Volatile.Write(ref _isCompleted, value); }
        }

        public TaskCompletionSource<Response<T>> Tcs
        {
            get { return _tcs == default ? Volatile.Read(ref _tcs) : _tcs; }
            set { _tcs = value; Volatile.Write(ref _tcs, value); }
        }

        public Action Continuation
        {
            get { return _continuation == default ? Volatile.Read(ref _continuation) : _continuation; }
            set { _continuation = value; Volatile.Write(ref _continuation, value); }
        }

        private Box<T> _result;
        private bool _isCompleted;
        private TaskCompletionSource<Response<T>> _tcs;
        private Action _continuation;

        public Bag(T result)
        {
            Result = new Box<T>(result);
            _isCompleted = true;
            Tcs = default;
            _continuation = default;
        }

        public Bag(Exception ex)
        {
            Result = default;
            _isCompleted = true;
            Tcs = default;
            _continuation = default;
            ex.LogError();
        }

        public Bag()
        {
            Result = default;
            _isCompleted = false;
            Tcs = default;
            _continuation = default;
        }
    }

    /**
    * General flow for the task-like type ResponseAsync{T}:
    * 1) Create the custom async method builder.
    * 2) Foreach await in that method, call "OnCompleted" on said builder.
    * 3) Outside that method, get the custom awaiter (i.e. the Task); and call get result (i.e. the await keyword).
    * 4) When the method result returns (or an exception is thrown / task is canceled), inform the awaiter the task is completed; and the result is ready.
    **/
    [AsyncMethodBuilder(typeof(ResponseAsyncMethodBuilder<>))]
    public readonly struct ResponseAsync<T>
    {
        private readonly Bag<T> _bag;

        // Constructor to use when you already have the value to set (i.e. T is pre-calculated).
        public ResponseAsync(T result) { _bag = new Bag<T>(result); }

        // Constructor to use when you have an error even before trying to start the calculation.
        public ResponseAsync(Exception ex) { _bag = new Bag<T>(ex); }

        // Constructor to use when the result hasn't been calculated yet (i.e. the result, or error will be set later).
        public ResponseAsync() { _bag = new Bag<T>(); }

        // When true, the value is ready to be read.
        internal bool IsCompleted() => _bag.IsCompleted;

        internal void OnCompleted(Action continuation) => _bag.Continuation = continuation;

        // Called by the awaiter when the result is requested (either manually by getting the awaiter, or when using the await keyword).
        internal Response<T> GetValue()
        {
            if (!_bag.IsCompleted) return AsTask().GetAwaiter().GetResult();
            var result = _bag.Result;
            return result == default ? new Response<T>() : new Response<T>(result.Value); // If null, then a result was never set for this task.
        }

        // Called by the state machine when the method has returned a result.
        internal void SetValue(T value)
        {
            _bag.Result = new Box<T>(value);
            _bag.IsCompleted = true;
            _bag.Tcs?.SetResult(Response.Create(value));
            _bag.Continuation?.Invoke();
        }

        // Called by the state machine when the method has thrown an exception.
        internal void SetException(Exception ex)
        {
            ex.LogError();
            _bag.IsCompleted = true;
            _bag.Tcs?.SetResult(new Response<T>());
            _bag.Continuation?.Invoke();
        }

        // Convert ResponseAsync{T} into a Task{T} type.
        public Task<Response<T>> AsTask()
        {
            var tcs = _bag.Tcs;
            if (tcs == default)
            {
                lock (_bag)
                {
                    tcs = _bag.Tcs;
                    if (tcs == default)
                    {
                        tcs = new TaskCompletionSource<Response<T>>();
                        if (_bag.IsCompleted)
                        {
                            var result = _bag.Result;
                            if (result != default) tcs.SetResult(Response.Create(result.Value)); // Task is completed.
                            else tcs.SetResult(new Response<T>()); // There is no result, as the task generated an exception, or was canceled.
                        }
                        _bag.Tcs = tcs;
                    }
                }
            }
            return tcs.Task;
        }

        // Convert ResponseAsync{T} into a ValueTask{T} type.
        public ValueTask<Response<T>> AsValueTask()
        {
            ValueTask<Response<T>> vt;

            if (_bag.IsCompleted)
            {
                var result = _bag.Result;
                if (result != default) vt = new(Response.Create(result.Value)); // Task is completed.
                else vt = new(new Response<T>()); // There is no result, as the task generated an exception, or was canceled.
            }
            else
            {
                vt = new(AsTask()); // Task is currently running.
            }

            return vt;
        }

        // Custom awaiter to convert ResponseAsync{T} into Response{T}, when using the await keyword.
        public ResponseAsyncAwaiter<T> GetAwaiter() => new ResponseAsyncAwaiter<T>(this);

        public static implicit operator Task<Response<T>>(ResponseAsync<T> response) => response.AsTask(); // Cast to Task{T}.
        public static implicit operator ValueTask<Response<T>>(ResponseAsync<T> response) => response.AsValueTask(); // Cast to ValueTask{T}.
    }

    public static class ResponseAsync
    {
        // Static helper for the public constructor to set the result.
        public static ResponseAsync<T> FromResult<T>(T result) => new ResponseAsync<T>(result);

        // Static helper for the public constructor to set an error.
        public static ResponseAsync<T> FromError<T>(Exception ex) => new ResponseAsync<T>(ex);

        #region Task Converters

        // Safely run the ValueTask, and convert the result into a Response.
        public static ValueTask<Response> AsResponse(this ValueTask task)
        {
            if (task.IsCompleted)
            {
                if (task.IsCompletedSuccessfully) return new ValueTask<Response>(Response.Success);
                if (task.IsFaulted) task.AsTask().Exception.LogError();
                return new ValueTask<Response>(Response.Error);
            }

            return new ValueTask<Response>(task.AsTask().ContinueWith(static t =>
            {
                if (t.Status == TaskStatus.RanToCompletion) return Response.Success;
                if (t.Status == TaskStatus.Faulted) t.Exception.LogError();
                return Response.Error;
            }));
        }

        // Safely run the ValueTask{T}, and convert the T, into a Response{T}.
        public static ValueTask<Response<T>> AsResponse<T>(this ValueTask<T> task)
        {
            if (task.IsCompleted)
            {
                if (task.IsCompletedSuccessfully) return new ValueTask<Response<T>>(Response.Create(task.Result));
                if (task.IsFaulted) task.AsTask().Exception.LogError();
                return new ValueTask<Response<T>>(Response.Create<T>());
            }

            return new ValueTask<Response<T>>(task.AsTask().ContinueWith(static t =>
            {
                if (t.Status == TaskStatus.Faulted) t.Exception.LogError();
                if (t.Status == TaskStatus.RanToCompletion) return Response.Create(t.Result);
                return Response.Create<T>();
            }));
        }

        // Safely run the Task, and convert the result into a Response.
        public static Task<Response> AsResponse(this Task task)
        {
            return task.ContinueWith(static t =>
            {
                if (t.Status == TaskStatus.RanToCompletion) return new Response(true);
                if (t.Status == TaskStatus.Faulted) t.Exception.LogError();
                return new Response();
            });
        }

        // Safely run the Task{T}, and convert the T, into a Response{T}.
        public static Task<Response<T>> AsResponse<T>(this Task<T> task)
        {
            return task.ContinueWith(static t =>
            {
                if (t.Status == TaskStatus.RanToCompletion) return new Response<T>(t.Result);
                if (t.Status == TaskStatus.Faulted) t.Exception.LogError();
                return new Response<T>();
            });
        }

        #endregion

        #region Awaiters

        // Awaiter for a Func returning ResponseAsync{T}.
        public static ResponseAsyncAwaiter<T> GetAwaiter<T>(this Func<ResponseAsync<T>> func) => func().GetAwaiter();

        // Awaiter for a Func returning T.
        public static ResponseAsyncAwaiter<T> GetAwaiter<T>(this Func<T> func)
        {
            try
            {
                var result = func();
                return new ResponseAsyncAwaiter<T>(new ResponseAsync<T>(result));
            }
            catch (Exception ex)
            {
                return new ResponseAsyncAwaiter<T>(new ResponseAsync<T>(ex));
            }
        }

        // Awaiter for an Action.
        public static ResponseAsyncAwaiter<Unit> GetAwaiter(this Action action)
        {
            try
            {
                action();
                return new ResponseAsyncAwaiter<Unit>(new ResponseAsync<Unit>(Unit.Instance));
            }
            catch (Exception ex)
            {
                return new ResponseAsyncAwaiter<Unit>(new ResponseAsync<Unit>(ex));
            }
        }

        #endregion
    }

    /**
     * OnCompleted: Called when IsCompleted is false, and the await keyword is used on the {task}.
     * GetResult: Called when the await keyword is used on the {task}, or when manually invoked by: {task}.GetAwaiter().GetResult();.
    **/
    public readonly struct ResponseAsyncAwaiter<T> : ICriticalNotifyCompletion
    {
        public bool IsCompleted => _response.IsCompleted();

        private readonly ResponseAsync<T> _response;

        public ResponseAsyncAwaiter(ResponseAsync<T> response) { _response = response; }

        public void UnsafeOnCompleted(Action continuation) => OnCompleted(continuation);

        public void OnCompleted(Action continuation) => _response.OnCompleted(continuation);

        public Response<T> GetResult() => _response.GetValue();
    }

    /**
     * Call order from the state machine:
     * 1) Create(): gets a instance.
     * 2) Start(): entry to the method.
     * 3) AwaitOnCompleted(): invoked on each occurrence of the await keyword (you can wrap the MoveNext() to setup / teardown things between awaits).
     * 4) SetResult(): exit of the method.
     * 5) SetException(): when some runtime error is thrown (including canceled tasks); in this case SetResult() is not invoked.
    **/
    public readonly struct ResponseAsyncMethodBuilder<T>
    {
        public static ResponseAsyncMethodBuilder<T> Create() => new ResponseAsyncMethodBuilder<T>();

        public ResponseAsync<T> Task => _response;
        private readonly ResponseAsync<T> _response;

        public ResponseAsyncMethodBuilder() { _response = new ResponseAsync<T>(); }

        public void SetResult(T result) => _response.SetValue(result);

        public void SetException(Exception ex) => _response.SetException(ex);

        public void SetStateMachine(IAsyncStateMachine _) { }

        public void Start<TStateMachine>(ref TStateMachine stateMachine)
            where TStateMachine : IAsyncStateMachine
            => stateMachine.MoveNext();

        public void AwaitOnCompleted<TAwaiter, TStateMachine>(ref TAwaiter awaiter, ref TStateMachine stateMachine)
            where TAwaiter : INotifyCompletion
            where TStateMachine : IAsyncStateMachine
            => awaiter.OnCompleted(stateMachine.MoveNext);

        public void AwaitUnsafeOnCompleted<TAwaiter, TStateMachine>(ref TAwaiter awaiter, ref TStateMachine stateMachine)
            where TAwaiter : ICriticalNotifyCompletion
            where TStateMachine : IAsyncStateMachine
            => awaiter.UnsafeOnCompleted(stateMachine.MoveNext);
    }

    /**
     * Use on a method returning a Task{Response{T}}, or a Task{T} type.
     * [AsyncMethodBuilder(typeof(ResponseAsyncTaskCompletionSource{}))]
    **/
    public readonly struct ResponseAsyncTaskCompletionSource<T>
    {
        public static ResponseAsyncTaskCompletionSource<T> Create() => new ResponseAsyncTaskCompletionSource<T>();

        private static readonly bool _isResponseType;

        static ResponseAsyncTaskCompletionSource() { _isResponseType = typeof(T).Equals(new Response<T>(default(T)).Value?.GetType()); }

        public Task<T> Task => _tcs.Task;

        private readonly TaskCompletionSource<T> _tcs;

        public ResponseAsyncTaskCompletionSource() { _tcs = new TaskCompletionSource<T>(); }

        public void SetResult(T result) => _tcs.SetResult(result);

        public void SetException(Exception ex) { ex.LogError(); if (_isResponseType) _tcs.SetResult(default(T)); else _tcs.SetException(ex); }

        public void SetStateMachine(IAsyncStateMachine _) { }

        public void Start<TStateMachine>(ref TStateMachine stateMachine)
            where TStateMachine : IAsyncStateMachine
            => stateMachine.MoveNext();

        public void AwaitOnCompleted<TAwaiter, TStateMachine>(ref TAwaiter awaiter, ref TStateMachine stateMachine)
            where TAwaiter : INotifyCompletion
            where TStateMachine : IAsyncStateMachine
            => awaiter.OnCompleted(stateMachine.MoveNext);

        public void AwaitUnsafeOnCompleted<TAwaiter, TStateMachine>(ref TAwaiter awaiter, ref TStateMachine stateMachine)
            where TAwaiter : ICriticalNotifyCompletion
            where TStateMachine : IAsyncStateMachine
            => awaiter.UnsafeOnCompleted(stateMachine.MoveNext);
    }
}
