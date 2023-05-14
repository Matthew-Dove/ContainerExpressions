using ContainerExpressions.Containers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Net;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace Tests.ContainerExpressions.Containers
{
    [TestClass]
    public class AwaiterTests
    {
        [TestMethod]
        public async Task Await_On_Response_TaskT()
        {
            var number = 100;

            var response = Response.Create(Task.FromResult(number));
            var result = await response;

            Assert.IsTrue(result);
            Assert.AreEqual(number, result);
        }

        [TestMethod]
        public async Task Await_On_Response_ValueTask()
        {
            var response = Response.Create(new ValueTask(Task.CompletedTask));

            var result = await response;

            Assert.IsTrue(result);
        }

        [TestMethod]
        public async Task Await_On_Response_ValueTaskT()
        {
            var response = Response.Create(new ValueTask<int>(1));
            var result = await response;
            Assert.AreEqual(1, result);
        }

        [TestMethod]
        public async Task Await_On_Response_Task()
        {
            var response = Response.Create(Task.CompletedTask);

            var result  = await response;

            Assert.IsTrue(result);
        }

        [TestMethod]
        public async Task Await_On_Response_Unpack()
        {
            var response = Response.Create(Task.FromResult(Response.Create(Response.Create(1))));
            var result = await response;
            Assert.IsTrue(result);
            Assert.AreEqual(1, result);
        }

        [TestMethod]
        public async Task Runtime_Errors_Are_Handled()
        {
            // A few different ways of turning a Task<T>, into a Response<Task<T>>
            var success = await Response.Create(Divide(1, 1));
            var error1 = await Divide(1, 0).ToResponse();
            var error2 = await new Response<Task<int>>(Divide(1, 0));

            Assert.IsTrue(success);
            Assert.AreEqual(1, success);
            Assert.IsFalse(error1);
            Assert.IsFalse(error2);
        }

        [TestMethod]
        public async Task Runtime_Errors_Are_Handled_Async()
        {
            ResponseTask successResponse = Divide(1, 1);
            Response success = await successResponse;

            Response err = await new ResponseTask(Divide(1, 0));
            ResponseTask errorResponse = Divide(1, 0);
            Response error = await errorResponse;

            Assert.IsTrue(success);
            Assert.IsFalse(error);
            Assert.IsFalse(err);
        }

        [TestMethod]
        public async Task Runtime_Errors_WithT_Are_Handled_Async()
        {
            ResponseTask<int> successResponse = Divide(1, 1);
            Response<int> success = await successResponse;

            ResponseTask<int> errorResponse = Divide(1, 0);
            Response<int> error = await errorResponse;
            Response<int> err = await new ResponseTask<int>(Divide(1, 0));

            Assert.IsTrue(success);
            Assert.AreEqual(1, success);
            Assert.IsFalse(error);
            Assert.IsFalse(err);
        }

        private static async Task<int> Divide(int numerator, int denominator)
        {
            var quotient = numerator / denominator;
            await Task.Delay(1); // The method must use async await, otherwise the exception will be thrown at runtime instead of being handled.
            return quotient;
        }

        [TestMethod]
        public async Task Custom_Sleep_Awaiter()
        {
            var sleep = new Sleep(1);

            var naptime = await sleep;

            Assert.AreEqual(1, naptime);
        }

        [Ignore]
        [TestMethod]
        public async Task Custom_Uri_Awaiter()
        {
            Response<string> body = await new Uri("https://www.example.com/"); // This one is just for the lols.
            Assert.IsTrue(body);
            Assert.IsFalse(string.IsNullOrWhiteSpace(body));
        }

        [TestMethod]
        public async Task Unpack_BaseResponse_Task_Awaiter()
        {
            var input = Response.Create(Task.FromResult(new Response<Response>(new Response(true))));

            var result = await input;

            Assert.IsTrue(result);
        }

        [TestMethod]
        public async Task Unpack_BaseResponse_Task_Async_Awaiter()
        {
            var input = Response.Create(Task.FromResult(new Response<Task<Response>>(Task.FromResult(new Response(true)))));

            var result = await input;

            Assert.IsTrue(result);
        }

        [TestMethod]
        public async Task Unpack_Response_Task_Awaiter()
        {
            var input = Response.Create(Task.FromResult(new Response<Response<int>>(new Response<int>(1))));

            var result = await input;

            Assert.IsTrue(result);
            Assert.AreEqual(1, result);
        }

        [TestMethod]
        public async Task Unpack_Response_Task_Async_Awaiter()
        {
            var input = Response.Create(Task.FromResult(new Response<Task<Response<int>>>(Task.FromResult(new Response<int>(1)))));

            var result = await input;

            Assert.IsTrue(result);
            Assert.AreEqual(1, result);
        }
    }

    #region Awaiter Extensions

    public static class AwaiterExtensions
    {
        #region Response

        // Example using Response ValueTask with no result.
        public static ValueTaskAwaiter<Response> GetAwaiter(this Response<ValueTask> response)
        {
            if (!response) return new ValueTask<Response>(Response.Error).GetAwaiter();

            if (response.Value.IsCompleted)
            {
                if (response.Value.IsCompletedSuccessfully) return new ValueTask<Response>(Response.Success).GetAwaiter();
                if (response.Value.IsFaulted) response.Value.AsTask().Exception.LogError();
                return new ValueTask<Response>(Response.Error).GetAwaiter();
            }

            return new ValueTask<Response>(response.Value.AsTask().ContinueWith(static t =>
            {
                if (t.Status == TaskStatus.Faulted) t.Exception.LogError();
                if (t.Status == TaskStatus.RanToCompletion) return Response.Success;
                return Response.Error;
            })).GetAwaiter();
        }

        // Example using Response ValueTask with T.
        public static ValueTaskAwaiter<Response<T>> GetAwaiter<T>(this Response<ValueTask<T>> response)
        {
            if (!response) return new ValueTask<Response<T>>(Response<T>.Error).GetAwaiter();

            if (response.Value.IsCompleted)
            {
                if (response.Value.IsCompletedSuccessfully) return new ValueTask<Response<T>>(Response.Create(response.Value.Result)).GetAwaiter();
                if (response.Value.IsFaulted) response.Value.AsTask().Exception.LogError();
                return new ValueTask<Response<T>>(Response.Create<T>()).GetAwaiter();
            }

            return new ValueTask<Response<T>>(response.Value.AsTask().ContinueWith(static t =>
            {
                if (t.Status == TaskStatus.Faulted) t.Exception.LogError();
                if (t.Status == TaskStatus.RanToCompletion) return Response.Create(t.Result);
                return new Response<T>();
            })).GetAwaiter();
        }

        // Example using Response Task with no result.
        public static TaskAwaiter<Response> GetAwaiter(this Response<Task> response)
        {
            if (!response) return InstanceAsync.Of<Response>().GetAwaiter();
            return response.Value.ContinueWith(static t => {
                if (t.Status == TaskStatus.Faulted) t.Exception.LogError();
                if (t.Status == TaskStatus.RanToCompletion) return Response.Success;
                return Response.Error;
            }).GetAwaiter();
        }

        // Example using Response Task with T.
        public static TaskAwaiter<Response<T>> GetAwaiter<T>(this Response<Task<T>> response)
        {
            if (!response) return InstanceAsync.Of<Response<T>>().GetAwaiter();
            return response.Value.ContinueWith(static t => {
                if (t.Status == TaskStatus.Faulted) t.Exception.LogError();
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

        #region ResponseTask

        // Example using ResponseTask with no result.
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

        // Example using ResponseTask with T.
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

        #endregion

        // This one is just for the lols.
        public static RestClientAwaiter GetAwaiter(this Uri uri)
        {
            var wc = new WebClient();
            var task = wc.DownloadStringTaskAsync(uri).ContinueWith(t => { wc.Dispose(); return t.Result; });
            return new RestClientAwaiter(task);
        }
    }

    #endregion 

    #region Custom Awaiters

    public readonly struct Sleep
    {
        private readonly Task<int> _sleeper;

        public Sleep(int milliSeconds) => _sleeper = Task.Delay(milliSeconds).ContinueWith(_ => milliSeconds);

        public SleepAwaiter<int> GetAwaiter() => new SleepAwaiter<int>(_sleeper);
    }

    public readonly struct SleepAwaiter<T> : ICriticalNotifyCompletion
    {
        public bool IsCompleted => _task.IsCompleted;
        private readonly Task<T> _task;

        public SleepAwaiter(Task<T> task) => _task = task;

        public void OnCompleted(Action continuation) => _task.ContinueWith(_ => continuation());
        public void UnsafeOnCompleted(Action continuation) => OnCompleted(continuation);
        public T GetResult() => _task.GetAwaiter().GetResult();
    }

    public readonly struct RestClientAwaiter : ICriticalNotifyCompletion
    {
        public bool IsCompleted => _task.IsCompleted;
        private readonly Task<string> _task;

        public RestClientAwaiter(Task<string> task) => _task = task;

        public void OnCompleted(Action continuation) => _task.ContinueWith(_ => continuation());
        public void UnsafeOnCompleted(Action continuation) => OnCompleted(continuation);
        public Response<string> GetResult() => Response.Create(_task.GetAwaiter().GetResult());
    }

    #endregion

    #region Task Type

    class Tester
    {
        public async TaskLike FooAsync()
        {
            await Task.Yield();
            await default(TaskLike);
        }
    }

    public struct TaskLikeMethodBuilder//<T> - make builder a struct with no readonly?
    {
        public TaskLike Task => default(TaskLike);

        public TaskLikeMethodBuilder() => Console.WriteLine(".ctor");

        public static TaskLikeMethodBuilder Create() => new TaskLikeMethodBuilder();

        public void SetResult() => Console.WriteLine("SetResult");
        // Generic version: public void SetResult(T result) => Console.WriteLine("SetResult");

        public void SetException(Exception ex) => Console.WriteLine("Error");

        public void Start<TStateMachine>(ref TStateMachine stateMachine)
            where TStateMachine : IAsyncStateMachine
        {
            Console.WriteLine("Start");
            stateMachine.MoveNext();
        }

        public void AwaitOnCompleted<TAwaiter, TStateMachine>(ref TAwaiter awaiter, ref TStateMachine stateMachine)
            where TAwaiter : INotifyCompletion
            where TStateMachine : IAsyncStateMachine
        {
            Console.WriteLine("AwaitOnCompleted");
            awaiter.OnCompleted(stateMachine.MoveNext);
        }

        public void AwaitUnsafeOnCompleted<TAwaiter, TStateMachine>(ref TAwaiter awaiter, ref TStateMachine stateMachine)
            where TAwaiter : ICriticalNotifyCompletion
            where TStateMachine : IAsyncStateMachine
        {
            Console.WriteLine("AwaitUnsafeOnCompleted");
            AwaitOnCompleted(ref awaiter, ref stateMachine);
        }

        public void SetStateMachine(IAsyncStateMachine stateMachine)
        {
            Console.WriteLine("SetStateMachine");
        }
    }

    [AsyncMethodBuilder(typeof(TaskLikeMethodBuilder))]
    public struct TaskLike//<T>
    {
        public TaskLikeAwaiter GetAwaiter() => default(TaskLikeAwaiter);
    }

    public struct TaskLikeAwaiter : ICriticalNotifyCompletion
    {
        public void GetResult()
        {
            Console.WriteLine("GetResult");
        }

        public bool IsCompleted => true;

        public void OnCompleted(Action continuation)
        {
            Console.WriteLine("OnCompleted");
        }

        public void UnsafeOnCompleted(Action continuation)
        {
            Console.WriteLine("UnsafeOnCompleted");
            OnCompleted(continuation);
        }
    }

    #endregion
}
