using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System;
using ContainerExpressions.Containers.Internal;

namespace ContainerExpressions.Containers
{
    /**
    * General flow for the task-like type ResponseAsync{T}:
    * 1) Create the custom async method builder.
    * 2) Foreach await in that method, call "OnCompleted" on said builder.
    * 3) Outside that method, get the custom awaiter (i.e. the Task); and call get result (i.e. the await keyword).
    * 4) When the method result returns (or an exception is thrown / task is canceled), inform the awaiter the task is completed; and the result is ready.
    **/
    [AsyncMethodBuilder(typeof(ResponseAsyncMethodBuilder<>))]
    public readonly struct ResponseAsync<T> : IEquatable<ResponseAsync<T>>
    {
        private readonly Task<Response<T>> _task;
        private readonly Response<T> _result;

        public ResponseAsync(T result) { _task = null; _result = new Response<T>(result); }
        public ResponseAsync(Exception ex) { ex.LogErrorPlain(); _task = null; _result = Response<T>.Error; }
        internal ResponseAsync(Task<Response<T>> task) { _task = task; _result = default; }

        public Task<Response<T>> ToTask() => _task ?? Task.FromResult(_result);
        public ValueTask<Response<T>> ToValueTask() => _task is null ? new ValueTask<Response<T>>(_result) : new ValueTask<Response<T>>(_task);

        public ValueTaskAwaiter<Response<T>> GetAwaiter() => ToValueTask().GetAwaiter();
        public ConfiguredValueTaskAwaitable<Response<T>> ConfigureAwait(bool continueOnCapturedContext) => ToValueTask().ConfigureAwait(continueOnCapturedContext);

        public bool Equals(ResponseAsync<T> other) => ReferenceEquals(_task, other._task) && (_task is not null || _result.Equals(other._result));
        public override bool Equals(object obj) => obj is ResponseAsync<T> other && Equals(other);
        public override int GetHashCode() => _task is not null ? _task.GetHashCode() : _result.GetHashCode();

        public override string ToString()
        {
            if (_task is null) return _result.ToString();
            return _task.IsCompletedSuccessfully ? _task.Result.ToString() : string.Empty;
        }

        public static bool operator ==(ResponseAsync<T> left, ResponseAsync<T> right) => left.Equals(right);
        public static bool operator !=(ResponseAsync<T> left, ResponseAsync<T> right) => !left.Equals(right);

        public static implicit operator Task<Response<T>>(ResponseAsync<T> response) => response.ToTask();
        public static implicit operator ValueTask<Response<T>>(ResponseAsync<T> response) => response.ToValueTask();
    }

    public static class ResponseAsync
    {
        // Static helper for the public constructor to set the result.
        public static ResponseAsync<T> FromResult<T>(T result) => new ResponseAsync<T>(result);

        // Static helper for the public constructor to set an error.
        public static ResponseAsync<T> FromException<T>(Exception ex) => new ResponseAsync<T>(ex);

        // Let a ResponseAsync{T} task type run, then convert it into a Task{Response{T}}; so it can be used with extension methods targeting that type.
        public static async Task<Response<T>> AsResponse<T>(
            this ResponseAsync<T> response,
            Format message = default,
            [CallerArgumentExpression(nameof(response))] string argument = "",
            [CallerMemberName] string caller = "",
            [CallerFilePath] string path = "",
            [CallerLineNumber] int line = 0
            )
        {
            var result = await response;
            if (!result && message != default) string.Empty.LogErrorValue(message, argument, caller, path, line);
            return result;
        }

        #region Task Converters

        // Safely run the ValueTask, and convert the result into a Response.
        public static ValueTask<Response> AsResponse(
            this ValueTask task,
            Format message = default,
            [CallerArgumentExpression(nameof(task))] string argument = "",
            [CallerMemberName] string caller = "",
            [CallerFilePath] string path = "",
            [CallerLineNumber] int line = 0
            )
        {
            if (task.IsCompleted)
            {
                if (task.IsCompletedSuccessfully) return new ValueTask<Response>(Response.Success);
                if (task.IsFaulted) task.AsTask().Exception.LogError(message, argument, caller, path, line);
                return new ValueTask<Response>(Response.Error);
            }

            return new ValueTask<Response>(task.AsTask().ContinueWith(t =>
            {
                if (t.Status == TaskStatus.RanToCompletion) return Response.Success;
                if (t.Status == TaskStatus.Faulted) t.Exception.LogError(message, argument, caller, path, line);
                return Response.Error;
            }));
        }

        // Safely run the ValueTask{T}, and convert the T, into a Response{T}.
        public static ValueTask<Response<T>> AsResponse<T>(
            this ValueTask<T> task,
            Format message = default,
            [CallerArgumentExpression(nameof(task))] string argument = "",
            [CallerMemberName] string caller = "",
            [CallerFilePath] string path = "",
            [CallerLineNumber] int line = 0
            )
        {
            if (task.IsCompleted)
            {
                if (task.IsCompletedSuccessfully) return new ValueTask<Response<T>>(Response.Create(task.Result));
                if (task.IsFaulted) task.AsTask().Exception.LogError(message, argument, caller, path, line);
                return new ValueTask<Response<T>>(Response.Create<T>());
            }

            return new ValueTask<Response<T>>(task.AsTask().ContinueWith(t =>
            {
                if (t.Status == TaskStatus.Faulted) t.Exception.LogError(message, argument, caller, path, line);
                if (t.Status == TaskStatus.RanToCompletion) return Response.Create(t.Result);
                return Response.Create<T>();
            }));
        }

        // Safely run the Task, and convert the result into a Response.
        public static Task<Response> AsResponse(
            this Task task,
            Format message = default,
            [CallerArgumentExpression(nameof(task))] string argument = "",
            [CallerMemberName] string caller = "",
            [CallerFilePath] string path = "",
            [CallerLineNumber] int line = 0
            )
        {
            return task.ContinueWith(t =>
            {
                if (t.Status == TaskStatus.RanToCompletion) return new Response(true);
                if (t.Status == TaskStatus.Faulted) t.Exception.LogError(message, argument, caller, path, line);
                return new Response();
            });
        }

        // Safely run the Task{T}, and convert the T, into a Response{T}.
        public static Task<Response<T>> AsResponse<T>(
            this Task<T> task,
            Format message = default,
            [CallerArgumentExpression(nameof(task))] string argument = "",
            [CallerMemberName] string caller = "",
            [CallerFilePath] string path = "",
            [CallerLineNumber] int line = 0
            )
        {
            return task.ContinueWith(t =>
            {
                if (t.Status == TaskStatus.RanToCompletion) return new Response<T>(t.Result);
                if (t.Status == TaskStatus.Faulted) t.Exception.LogError(message, argument, caller, path, line);
                return new Response<T>();
            });
        }

        #endregion

        #region Task Awaiters

        // Task
        public static TaskAwaiter<Response> GetAwaiter(this Func<Task> func)
        {
            try
            {
                var result = func == null ? default : func();
                if (result == default) return Pool.ResponseError.GetAwaiter();

                return result.ContinueWith(static t =>
                {
                    if (t.Status == TaskStatus.RanToCompletion) return new Response(true);
                    if (t.Status == TaskStatus.Faulted) t.Exception.LogErrorPlain();
                    return new Response();
                }).GetAwaiter();
            }
            catch (Exception ex)
            {
                ex.LogErrorPlain();
                return Pool.ResponseError.GetAwaiter();
            }
        }

        // Task{T}
        public static TaskAwaiter<Response<T>> GetAwaiter<T>(this Func<Task<T>> func)
        {
            try
            {
                var result = func == null ? default : func();
                if (result == default) return Pool<T>.ResponseError.GetAwaiter();

                return result.ContinueWith(static t =>
                {
                    if (t.Status == TaskStatus.RanToCompletion) return new Response<T>(t.Result);
                    if (t.Status == TaskStatus.Faulted) t.Exception.LogErrorPlain();
                    return new Response<T>();
                }).GetAwaiter();
            }
            catch (Exception ex)
            {
                ex.LogErrorPlain();
                return Pool<T>.ResponseError.GetAwaiter();
            }
        }

        // Task{Response}
        public static TaskAwaiter<Response> GetAwaiter(this Func<Task<Response>> func)
        {
            try
            {
                var result = func == null ? default : func();
                if (result == default) return Pool.ResponseError.GetAwaiter();

                return result.ContinueWith(static t =>
                {
                    if (t.Status == TaskStatus.RanToCompletion && t.Result.IsValid) return t.Result;
                    if (t.Status == TaskStatus.Faulted) t.Exception.LogErrorPlain();
                    return new Response();
                }).GetAwaiter();
            }
            catch (Exception ex)
            {
                ex.LogErrorPlain();
                return Pool.ResponseError.GetAwaiter();
            }
        }

        // Task{Response{T}}
        public static TaskAwaiter<Response<T>> GetAwaiter<T>(this Func<Task<Response<T>>> func)
        {
            try
            {
                var result = func == null ? default : func();
                if (result == default) return Pool<T>.ResponseError.GetAwaiter();

                return result.ContinueWith(static t =>
                {
                    if (t.Status == TaskStatus.RanToCompletion && t.Result.IsValid) return t.Result;
                    if (t.Status == TaskStatus.Faulted) t.Exception.LogErrorPlain();
                    return new Response<T>();
                }).GetAwaiter();
            }
            catch (Exception ex)
            {
                ex.LogErrorPlain();
                return Pool<T>.ResponseError.GetAwaiter();
            }
        }

        // ValueTask
        public static ValueTaskAwaiter<Response> GetAwaiter(this Func<ValueTask> func)
        {
            try
            {
                if (func == null) return new ValueTask<Response>(Response.Error).GetAwaiter();
                var result = func();

                if (result.IsCompleted)
                {
                    if (result.IsCompletedSuccessfully) return new ValueTask<Response>(Response.Success).GetAwaiter();
                    if (result.IsFaulted) result.AsTask().Exception.LogErrorPlain();
                    return new ValueTask<Response>(Response.Error).GetAwaiter();
                }

                return new ValueTask<Response>(result.AsTask().ContinueWith(static t =>
                {
                    if (t.Status == TaskStatus.RanToCompletion) return Response.Success;
                    if (t.Status == TaskStatus.Faulted) t.Exception.LogErrorPlain();
                    return Response.Error;
                })).GetAwaiter();
            }
            catch (Exception ex)
            {
                ex.LogErrorPlain();
                return new ValueTask<Response>(Response.Error).GetAwaiter();
            }
        }

        // ValueTask{T}
        public static ValueTaskAwaiter<Response<T>> GetAwaiter<T>(this Func<ValueTask<T>> func)
        {
            try
            {
                if (func == null) return new ValueTask<Response<T>>(Response<T>.Error).GetAwaiter();
                var result = func();

                if (result.IsCompleted)
                {
                    if (result.IsCompletedSuccessfully) return new ValueTask<Response<T>>(Response.Create(result.Result)).GetAwaiter();
                    if (result.IsFaulted) result.AsTask().Exception.LogErrorPlain();
                    return new ValueTask<Response<T>>(Response<T>.Error).GetAwaiter();
                }

                return new ValueTask<Response<T>>(result.AsTask().ContinueWith(static t =>
                {
                    if (t.Status == TaskStatus.RanToCompletion) return Response.Create(t.Result);
                    if (t.Status == TaskStatus.Faulted) t.Exception.LogErrorPlain();
                    return new Response<T>();
                })).GetAwaiter();
            }
            catch (Exception ex)
            {
                ex.LogErrorPlain();
                return new ValueTask<Response<T>>(Response<T>.Error).GetAwaiter();
            }
        }

        // ValueTask{Response}
        public static ValueTaskAwaiter<Response> GetAwaiter(this Func<ValueTask<Response>> func)
        {
            try
            {
                if (func == null) return new ValueTask<Response>(Response.Error).GetAwaiter();
                var result = func();

                if (result.IsCompleted)
                {
                    if (result.IsCompletedSuccessfully) return result.GetAwaiter();
                    if (result.IsFaulted) result.AsTask().Exception.LogErrorPlain();
                    return new ValueTask<Response>(new Response()).GetAwaiter();
                }

                return new ValueTask<Response>(result.AsTask().ContinueWith(static t =>
                {
                    if (t.Status == TaskStatus.RanToCompletion && t.Result.IsValid) return t.Result;
                    if (t.Status == TaskStatus.Faulted) t.Exception.LogErrorPlain();
                    return new Response();
                })).GetAwaiter();
            }
            catch (Exception ex)
            {
                ex.LogErrorPlain();
                return new ValueTask<Response>(new Response()).GetAwaiter();
            }
        }

        // ValueTask{Response{T}}
        public static ValueTaskAwaiter<Response<T>> GetAwaiter<T>(this Func<ValueTask<Response<T>>> func)
        {
            try
            {
                if (func == null) return new ValueTask<Response<T>>(Response<T>.Error).GetAwaiter();
                var result = func();

                if (result.IsCompleted)
                {
                    if (result.IsCompletedSuccessfully) return result.GetAwaiter();
                    if (result.IsFaulted) result.AsTask().Exception.LogErrorPlain();
                    return new ValueTask<Response<T>>(new Response<T>()).GetAwaiter();
                }

                return new ValueTask<Response<T>>(result.AsTask().ContinueWith(static t =>
                {
                    if (t.Status == TaskStatus.RanToCompletion && t.Result.IsValid) return t.Result;
                    if (t.Status == TaskStatus.Faulted) t.Exception.LogErrorPlain();
                    return new Response<T>();
                })).GetAwaiter();
            }
            catch (Exception ex)
            {
                ex.LogErrorPlain();
                return new ValueTask<Response<T>>(new Response<T>()).GetAwaiter();
            }
        }

        #endregion

        #region Response Async Awaiters

        // Awaiter for a Func returning ResponseAsync{T}.
        public static ValueTaskAwaiter<Response<T>> GetAwaiter<T>(this Func<ResponseAsync<T>> func)
        {
            try
            {
                return func is null
                    ? new ValueTask<Response<T>>(Response<T>.Error).GetAwaiter()
                    : func().GetAwaiter();
            }
            catch (Exception ex)
            {
                ex.LogErrorPlain();
                return new ValueTask<Response<T>>(Response<T>.Error).GetAwaiter();
            }
        }

        // Awaiter for a Func returning T.
        public static ValueTaskAwaiter<Response<T>> GetAwaiter<T>(this Func<T> func)
        {
            try
            {
                if (func is null) return new ValueTask<Response<T>>(Response<T>.Error).GetAwaiter();
                return new ValueTask<Response<T>>(new Response<T>(func())).GetAwaiter();
            }
            catch (Exception ex)
            {
                ex.LogErrorPlain();
                return new ValueTask<Response<T>>(Response<T>.Error).GetAwaiter();
            }
        }

        // Awaiter for an Action.
        public static ValueTaskAwaiter<Response<Unit>> GetAwaiter(this Action action)
        {
            try
            {
                if (action is null) return new ValueTask<Response<Unit>>(Unit.ResponseError).GetAwaiter();
                action();
                return new ValueTask<Response<Unit>>(Unit.ResponseSuccess).GetAwaiter();
            }
            catch (Exception ex)
            {
                ex.LogErrorPlain();
                return new ValueTask<Response<Unit>>(Unit.ResponseError).GetAwaiter();
            }
        }

        #endregion
    }

    /**
     * Call order from the state machine:
     * 1) Create(): gets a instance.
     * 2) Start(): entry to the method.
     * 3) AwaitOnCompleted(): invoked on each occurrence of the await keyword (you can wrap the MoveNext() to setup / teardown things between awaits).
     * 4) SetResult(): exit of the method.
     * 5) SetException(): when some runtime error is thrown (including canceled tasks); in this case SetResult() is not invoked.
    **/
    public struct ResponseAsyncMethodBuilder<T>
    {
        private AsyncTaskMethodBuilder<Response<T>> _builder;

        public static ResponseAsyncMethodBuilder<T> Create() => new() { _builder = AsyncTaskMethodBuilder<Response<T>>.Create() };

        public ResponseAsync<T> Task => new(_builder.Task);

        public void Start<TStateMachine>(ref TStateMachine stateMachine) where TStateMachine : IAsyncStateMachine
            => _builder.Start(ref stateMachine);

        public void SetStateMachine(IAsyncStateMachine stateMachine)
            => _builder.SetStateMachine(stateMachine);

        public void SetResult(T result)
            => _builder.SetResult(new Response<T>(result));

        public void SetException(Exception ex)
        {
            ex.LogErrorPlain();
            _builder.SetResult(Response<T>.Error);
        }

        public void AwaitOnCompleted<TAwaiter, TStateMachine>(ref TAwaiter awaiter, ref TStateMachine stateMachine)
            where TAwaiter : INotifyCompletion
            where TStateMachine : IAsyncStateMachine
            => _builder.AwaitOnCompleted(ref awaiter, ref stateMachine);

        public void AwaitUnsafeOnCompleted<TAwaiter, TStateMachine>(ref TAwaiter awaiter, ref TStateMachine stateMachine)
            where TAwaiter : ICriticalNotifyCompletion
            where TStateMachine : IAsyncStateMachine
            => _builder.AwaitUnsafeOnCompleted(ref awaiter, ref stateMachine);
    }

    /**
     * Use on a method returning a Task{Response{T}}, or a Task{T} type.
     * [AsyncMethodBuilder(typeof(ResponseAsyncTaskCompletionSource{}))]
    **/
    public struct ResponseAsyncTaskCompletionSource<T>
    {
        private static readonly bool _isResponseType =
            typeof(T) == typeof(Response)
            || (typeof(T).IsGenericType && typeof(T).GetGenericTypeDefinition() == typeof(Response<>));

        private AsyncTaskMethodBuilder<T> _builder;

        public static ResponseAsyncTaskCompletionSource<T> Create()
            => new() { _builder = AsyncTaskMethodBuilder<T>.Create() };

        public Task<T> Task => _builder.Task;

        public void SetResult(T result) => _builder.SetResult(result);

        public void SetException(Exception ex)
        {
            ex.LogErrorPlain();
            if (_isResponseType) _builder.SetResult(default);
            else _builder.SetException(ex);
        }

        public void SetStateMachine(IAsyncStateMachine stateMachine) => _builder.SetStateMachine(stateMachine);

        public void Start<TStateMachine>(ref TStateMachine stateMachine)
            where TStateMachine : IAsyncStateMachine
            => _builder.Start(ref stateMachine);

        public void AwaitOnCompleted<TAwaiter, TStateMachine>(ref TAwaiter awaiter, ref TStateMachine stateMachine)
            where TAwaiter : INotifyCompletion
            where TStateMachine : IAsyncStateMachine
            => _builder.AwaitOnCompleted(ref awaiter, ref stateMachine);

        public void AwaitUnsafeOnCompleted<TAwaiter, TStateMachine>(ref TAwaiter awaiter, ref TStateMachine stateMachine)
            where TAwaiter : ICriticalNotifyCompletion
            where TStateMachine : IAsyncStateMachine
            => _builder.AwaitUnsafeOnCompleted(ref awaiter, ref stateMachine);
    }
}
