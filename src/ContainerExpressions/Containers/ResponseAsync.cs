using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Threading;
using System;
using System.Collections.Concurrent;
using System.Runtime.ExceptionServices;
using System.Threading.Tasks.Sources;

namespace ContainerExpressions.Containers
{
    file sealed class Box<T> : Alias<T> { public Box(T value) : base(value) { } } // Force T on the heap, so we can mark it volatile; and skip a lock.

    // Bag of things for ResponseAsync{T}, so we can make it a readonly struct.
    file sealed class Bag<T>
    {
        public volatile Box<T> Box;
        public readonly ManualResetEventSlim Sync;
        public bool IsDisposed;
        public volatile TaskCompletionSource<Response<T>> Tcs;

        public Bag(T result)
        {
            Box = new Box<T>(result);
            Sync = null;
            IsDisposed = true;
            Tcs = null;
        }

        public Bag()
        {
            Box = null;
            Sync = new ManualResetEventSlim(false);
            IsDisposed = false;
            Tcs = null;
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
    public readonly struct ResponseAsync<T> : IDisposable
    {
        private readonly Bag<T> _bag;

        // Use this constructor when you already have the value to set (i.e. T is pre-calculated).
        public ResponseAsync(T result) { _bag = new Bag<T>(result); }

        public ResponseAsync() { _bag = new Bag<T>(); }

        // Static helper for the public constructor to set the result.
        public static ResponseAsync<T> FromResult(T result) => new ResponseAsync<T>(result);

        // Convert into a Task{T} type.
        public Task<Response<T>> AsTask()
        {
            if (_bag.Tcs == null)
            {
                lock (_bag.Sync) // The user calls this (from who knows where, how many times), and we only want the tcs created once.
                {
                    if (_bag.Tcs == null)
                    {
                        _bag.Tcs = new TaskCompletionSource<Response<T>>();
                        if (_bag.IsDisposed)
                        {
                            var box = _bag.Box;
                            if (box != null) _bag.Tcs.SetResult(Response.Create(box.Value)); // Task is already completed.
                            else _bag.Tcs.SetCanceled(); // There is no result, as the task generated an exception, or was canceled.

                        }
                    }
                }
            }
            return _bag.Tcs.Task;
        }

        // When true, the value is ready to be read.
        internal bool IsCompleted() { if (_bag.IsDisposed) return true; lock (_bag.Sync) { return _bag.IsDisposed; } } // Try lock free read first.

        // Called by the awaiter, after the state machine has finished, and before the result has been calculated (when using the await keyword).
        internal void Wait()
        {
            if (_bag.IsDisposed) return; // Try lock free read first.
            lock (_bag.Sync) { if (_bag.IsDisposed) return; }
            _bag.Sync.Wait();
            Dispose(disposing: true);
        }

        // Called by the awaiter when the result is requested (either manually by getting the awaiter, or when using the await keyword).
        internal Response<T> GetValue()
        {
            var box = _bag.Box;
            return box == null ? new Response<T>() : new Response<T>(box.Value); // If null, then a result was never set for this task.
        }

        // Called by the state machine when the method has returned a result.
        internal void SetValue(T value)
        {
            _bag.Box = new Box<T>(value);
            _bag.Tcs?.SetResult(Response.Create(value));
            _bag.Sync.Set();
        }

        // Called by the state machine when the method has thrown an exception.
        internal void SetException(Exception ex)
        {
            ex.LogError();
            _bag.Tcs?.SetException(ex);
            _bag.Sync.Set();
        }

        private void Dispose(bool disposing)
        {
            if (!_bag.IsDisposed)
            {
                lock (_bag.Sync)
                {
                    if (!_bag.IsDisposed) // This disposed flag also tells us the task is finished (when true).
                    {
                        if (disposing)
                        {
                            _bag.Sync.Dispose();
                        }
                        _bag.IsDisposed = true;
                    }
                }
            }
        }

        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

        // Custom awaiter to convert ResponseAsync<T> into Response<T>, when using the await keyword.
        public ResponseAsyncAwaiter<T> GetAwaiter() => new ResponseAsyncAwaiter<T>(this);

        public static implicit operator Task<Response<T>>(ResponseAsync<T> response) => response.AsTask(); // Auto-cast to Task{T}.
    }

    /**
     * OnCompleted: Called when IsCompleted is false, and the await keyword is used on the {task}.
     * GetResult: Called when the await keyword is used on the {task}, or when manually invoked by: {task}.GetAwaiter().GetResult();.
    **/
    public readonly struct ResponseAsyncAwaiter<T> : ICriticalNotifyCompletion
    {
        public bool IsCompleted => _response.IsCompleted();

        private readonly ResponseAsync<T> _response;

        internal ResponseAsyncAwaiter(ResponseAsync<T> response)
        {
            _response = response;
        }

        public void UnsafeOnCompleted(Action continuation) => OnCompleted(continuation);

        public void OnCompleted(Action continuation)
        {
            _response.Wait(); // Wait for the value to be generated, or for some exception.
            continuation(); // Get the final result.
        }

        public Response<T> GetResult()
        {
            _response.Wait(); // Callers may skip using await, and use this.GetAwaiter().GetResult() instead, so we need to call Wait() here too.
            return _response.GetValue(); // Gets the value, or an invalid response when an exception was set instead of a value.
        }
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
     * Use on a method returning a ValueTask{Response{T}} type.
     * [AsyncMethodBuilder(typeof(ResponseAsyncValueTaskCompletionSource{}))]
    **/
    public readonly struct ResponseAsyncValueTaskCompletionSource<T>
    {
        public static ResponseAsyncValueTaskCompletionSource<T> Create() => new ResponseAsyncValueTaskCompletionSource<T>();

        private static readonly bool _isResponseType;

        static ResponseAsyncValueTaskCompletionSource() { _isResponseType = typeof(T).Equals(new Response<T>(default(T)).Value?.GetType()); }

        public ValueTask<T> Task => new ValueTask<T>(_tcs.Task);

        private readonly TaskCompletionSource<T> _tcs;

        public ResponseAsyncValueTaskCompletionSource() { _tcs = new TaskCompletionSource<T>(); }

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

    /**
     * Use on a method returning a Task{Response{T}} type.
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

    file sealed class SourceBag<T>
    {
        public T Result;
        public ExceptionDispatchInfo Error;
        public ValueTaskSourceStatus Status;
        public Action<object> Continuation;
        public object State;
    }

    public readonly struct ValueTaskSource<T> : IValueTaskSource<T>
    {
        private const int MIN_TOKEN = short.MinValue;
        private const int MAX_TOKEN = 2^14;

        private static readonly ConcurrentDictionary<short, SourceBag<T>> _sources;
        private static int _token;

        static ValueTaskSource()
        {
            _sources = new ConcurrentDictionary<short, SourceBag<T>>();
            _token = MIN_TOKEN;
        }

        public short GetNextToken()
        {
            if (_token > MAX_TOKEN)
            {
                lock (_sources)
                {
                    if (_token > MAX_TOKEN)
                    {
                        _token = MIN_TOKEN;
                    }
                }
            }
            var token = (short)Interlocked.Increment(ref _token);
            _sources.TryAdd(token, new SourceBag<T>());
            return token;
        }

        public void SetResult(short token, T result)
        {
            _sources.TryGetValue(token, out SourceBag<T> source);
            source.Result = result;
            source.Status = ValueTaskSourceStatus.Succeeded;
            source.Continuation(source.State);
        }

        public void SetException(short token, ExceptionDispatchInfo ex)
        {
            _sources.TryGetValue(token, out SourceBag<T> source);
            source.Error = ex;
            source.Status = ValueTaskSourceStatus.Faulted;
            source.Continuation(source.State);
        }

        public T GetResult(short token)
        {
            _sources.TryGetValue(token, out SourceBag<T> source);
            _sources.TryRemove(token, out _);
            if (source.Error is not null) source.Error.Throw();
            return source.Result;
        }

        public ValueTaskSourceStatus GetStatus(short token)
        {
            _sources.TryGetValue(token, out SourceBag<T> source);
            return source.Status;
        }

        public void OnCompleted(Action<object> continuation, object state, short token, ValueTaskSourceOnCompletedFlags flags)
        {
            _sources.TryGetValue(token, out SourceBag<T> source);
            source.Continuation = continuation;
            source.State = state;
        }
    }

    /**
     * Use on a method returning a ValueTask{Response{T}}, or a ValueTask{T} type.
     * [AsyncMethodBuilder(typeof(ResponseAsyncValueTaskSource{}))]
     * 
     * The main difference between this, and ResponseAsyncValueTaskCompletionSource, is that ValueTaskSource is cheaper to use than TaskCompletionSource.
     * That said, both async method builders have the same logical effect.
    **/
    public readonly struct ResponseAsyncValueTaskSource<T>
    {
        public static ResponseAsyncValueTaskSource<T> Create() => new ResponseAsyncValueTaskSource<T>();

        private static readonly bool _isResponseType;
        private static readonly ValueTaskSource<T> _source;

        static ResponseAsyncValueTaskSource() { _isResponseType = typeof(T).Equals(new Response<T>(default(T)).Value?.GetType()); }

        public ValueTask<T> Task => new ValueTask<T>(_source, _token);
        private readonly short _token;

        public ResponseAsyncValueTaskSource() { _token = _source.GetNextToken(); }

        public void SetResult(T result) { _source.SetResult(_token, result); }

        public void SetException(Exception ex)
        {
            ex.LogError();
            if (_isResponseType) _source.SetResult(_token, default(T));
            else _source.SetException(_token, ExceptionDispatchInfo.Capture(ex));
        }

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
