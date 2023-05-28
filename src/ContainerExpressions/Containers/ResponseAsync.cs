﻿using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Threading;
using System;
using System.Collections.Concurrent;
using System.Runtime.ExceptionServices;
using System.Threading.Tasks.Sources;
using ContainerExpressions.Containers.Common;
using System.Diagnostics;

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
            set { Volatile.Write(ref _result, value); }
        }

        public TaskCompletionSource<Response<T>> Tcs
        {
            get { return _tcs == default ? Volatile.Read(ref _tcs) : _tcs; }
            set { Volatile.Write(ref _tcs, value); }
        }

        public readonly ManualResetEventSlim Sync;
        public bool IsDisposed;

        private Box<T> _result;
        private TaskCompletionSource<Response<T>> _tcs;

        public Bag(T result)
        {
            Result = new Box<T>(result);
            Sync = default;
            IsDisposed = true;
            Tcs = default;
        }

        public Bag(Exception ex)
        {
            Result = default;
            Sync = default;
            IsDisposed = true;
            Tcs = default;
            ex.LogError();
        }

        public Bag()
        {
            Result = default;
            Sync = new ManualResetEventSlim(false);
            IsDisposed = false;
            Tcs = default;
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

        // Constructor to use when you already have the value to set (i.e. T is pre-calculated).
        public ResponseAsync(T result) { _bag = new Bag<T>(result); }

        // Constructor to use when you have an error even before trying to start the calculation.
        public ResponseAsync(Exception ex) { _bag = new Bag<T>(ex); }

        // Constructor to use when the result hasn't been calculated yet (i.e. the result, or error will be set later).
        public ResponseAsync() { _bag = new Bag<T>(); }

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
            var result = _bag.Result;
            return result == null ? new Response<T>() : new Response<T>(result.Value); // If null, then a result was never set for this task.
        }

        // Called by the state machine when the method has returned a result.
        internal void SetValue(T value)
        {
            _bag.Result = new Box<T>(value);
            _bag.Tcs?.SetResult(Response.Create(value));
            _bag.Sync.Set();
        }

        // Called by the state machine when the method has thrown an exception.
        internal void SetException(Exception ex)
        {
            ex.LogError();
            _bag.Tcs?.SetResult(new Response<T>());
            _bag.Sync.Set();
        }

        // Convert ResponseAsync{T} into a Task{T} type.
        public Task<Response<T>> AsTask()
        {
            if (_bag.Tcs == null)
            {
                lock (_bag.Sync) // The user calls this (from who knows where, how many times), and we only want the tcs created once.
                {
                    if (_bag.Tcs == null)
                    {
                        _bag.Tcs = new TaskCompletionSource<Response<T>>();
                        if (_bag.IsDisposed || _bag.Result != default)
                        {
                            if (_bag.Result != default) _bag.Tcs.SetResult(Response.Create(_bag.Result.Value)); // Task is completed.
                            else _bag.Tcs.SetResult(new Response<T>()); // There is no result, as the task generated an exception, or was canceled.
                        }
                    }
                }
            }
            return _bag.Tcs.Task;
        }

        // Convert ResponseAsync{T} into a ValueTask{T} type.
        public ValueTask<Response<T>> AsValueTask()
        {
            ValueTask<Response<T>> vt;

            if (_bag.IsDisposed || _bag.Result != default)
            {
                if (_bag.Result != default) vt = new(Response.Create(_bag.Result.Value)); // Task is completed.
                else vt = new(new Response<T>()); // There is no result, as the task generated an exception, or was canceled.
            }
            else
            {
                vt = new(AsTask()); // Task is currently running.
            }

            return vt;
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
     * Use on a method returning a ValueTask{Response{T}}, or a ValueTask{T} type.
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

    // Bag of things for ValueTaskSource{T}, so we can make it a readonly struct.
    file sealed class SourceBag<T>
    {
        /**
         * [Volatile WORM access pattern]
         * get: read > isDefault > volatile read
         * set: volatile write
        **/
        public Box<T> Result
        {
            get { return _result == default ? Volatile.Read(ref _result) : _result; }
            set { Volatile.Write(ref _result, value); }
        }

        public ExceptionDispatchInfo Error
        {
            get { return _error == default ? Volatile.Read(ref _error) : _error; }
            set { Volatile.Write(ref _error, value); }
        }

        public Box<ValueTaskSourceStatus> Status
        {
            get { return _status == default ? Volatile.Read(ref _status) : _status; }
            set { Volatile.Write(ref _status, value); }
        }

        public Action<object> Continuation
        {
            get { return _continuation == default ? Volatile.Read(ref _continuation) : _continuation; }
            set { Volatile.Write(ref _continuation, value); }
        }

        public object State
        {
            get { return _state == default ? Volatile.Read(ref _state) : _state; }
            set { Volatile.Write(ref _state, value); }
        }

        private Box<T> _result = default;
        private ExceptionDispatchInfo _error = default;
        private Box<ValueTaskSourceStatus> _status = default;
        private Action<object> _continuation = default;
        private object _state = default;
    }

    // A non-generic class to ensure unique short tokens are generated for all active ValueTaskSource{T} instances.
    file static class ValueTaskSource
    {
        private const int MIN_TOKEN = short.MinValue;
        private const int MAX_TOKEN = 2 ^ 14;

        private static int _token = MIN_TOKEN;
        private static object _lock = new object();

        public static short GetNextToken()
        {
            // Reset the token count when we start to run out of space, this will happen every ~50K tokens.
            if (_token > MAX_TOKEN)
            {
                lock (_lock)
                {
                    if (_token > MAX_TOKEN)
                    {
                        _token = MIN_TOKEN;
                    }
                }
            }
            return (short)Interlocked.Increment(ref _token);
        }
    }

    public sealed class ValueTaskSource<T> : IValueTaskSource<T>
    {
        private static readonly ConcurrentDictionary<short, SourceBag<T>> _sources;

        static ValueTaskSource() { _sources = new ConcurrentDictionary<short, SourceBag<T>>(); }

        public short GetToken() => ValueTaskSource.GetNextToken();

        private static bool WaitFor(short token)
        {
            if (_sources.ContainsKey(token)) return true;
            return SpinWait.SpinUntil(() => _sources.ContainsKey(token), 250);
        }

        public void SetResult(short token, T result)
        {
            var source = new SourceBag<T>
            {
                Result = new Box<T>(result),
                Status = new Box<ValueTaskSourceStatus>(ValueTaskSourceStatus.Succeeded)
            };
            _sources.TryAdd(token, source);
        }

        public void SetException(short token, ExceptionDispatchInfo ex)
        {
            var source = new SourceBag<T>
            {
                Error = ex,
                Status = new Box<ValueTaskSourceStatus>(ValueTaskSourceStatus.Faulted)
            };
            _sources.TryAdd(token, source);
        }

        public T GetResult(short token)
        {
            WaitFor(token);

            _sources.TryRemove(token, out SourceBag<T> source);

            var result = source?.Result;
            if (result is not null) return result.Value;

            var error = source?.Error;
            if (error is not null) source.Error.Throw();

            Throw.InvalidOperationException($"{nameof(IValueTaskSource<T>)} finished with no matching result, or error; for the token: {token}.");
            return default;
        }

        public ValueTaskSourceStatus GetStatus(short token)
        {
            _sources.TryGetValue(token, out SourceBag<T> source);
            return source?.Status?.Value ?? ValueTaskSourceStatus.Pending;
        }

        public void OnCompleted(Action<object> continuation, object state, short token, ValueTaskSourceOnCompletedFlags flags)
        {
            WaitFor(token);
            continuation(state);
        }
    }

    /**
     * Use on a method returning a ValueTask{Response{T}}, or a ValueTask{T} type.
     * [AsyncMethodBuilder(typeof(ResponseAsyncValueTaskSource{}))]
     * 
     * The main difference between this, and ResponseAsyncValueTaskCompletionSource{T}, is that ValueTaskSource{T} avoids creating a TaskCompletionSource.
     * That said, both async method builders have the same logical effect.
    **/
    public readonly struct ResponseAsyncValueTaskSource<T>
    {
        public static ResponseAsyncValueTaskSource<T> Create() => new ResponseAsyncValueTaskSource<T>();

        private static readonly bool _isResponseType;
        private static readonly ValueTaskSource<T> _source;

        static ResponseAsyncValueTaskSource() { _isResponseType = typeof(T).Equals(new Response<T>(default(T)).Value?.GetType()); _source = new ValueTaskSource<T>(); }

        public ValueTask<T> Task => new ValueTask<T>(_source, _token);
        private readonly short _token;

        public ResponseAsyncValueTaskSource() { _token = _source.GetToken(); }

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
