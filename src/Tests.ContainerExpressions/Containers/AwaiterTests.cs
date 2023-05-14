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
            ResponseValueTask successResponse = Divide(1, 1);
            Response success = await successResponse;

            Response err = await new ResponseValueTask(Divide(1, 0));
            ResponseValueTask errorResponse = Divide(1, 0);
            Response error = await errorResponse;

            Assert.IsTrue(success);
            Assert.IsFalse(error);
            Assert.IsFalse(err);
        }

        [TestMethod]
        public async Task Runtime_Errors_WithT_Are_Handled_Async()
        {
            ResponseValueTask<int> successResponse = Divide(1, 1);
            Response<int> success = await successResponse;

            ResponseValueTask<int> errorResponse = Divide(1, 0);
            Response<int> error = await errorResponse;
            Response<int> err = await new ResponseValueTask<int>(Divide(1, 0));

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

        [TestMethod]
        public async Task TaskResponse_Pass()
        {
            var response = await new ResponseTask(Task.CompletedTask);
            Assert.IsTrue(response);
        }

        [TestMethod]
        public async Task TaskResponse_With_T_Pass()
        {
            var response = await new ResponseTask<int>(Task.FromResult(1));
            Assert.IsTrue(response);
            Assert.AreEqual(1, response);
        }
    }

    #region Awaiter Extensions

    public static class AwaiterExtensions
    {
        public static RestClientAwaiter GetAwaiter(this Uri uri)
        {
            var wc = new WebClient();
            var task = wc.DownloadStringTaskAsync(uri).ContinueWith(t => { wc.Dispose(); return t.Result; }); // This one is just for the lols.
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
