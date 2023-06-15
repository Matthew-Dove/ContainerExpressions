using ContainerExpressions.Containers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Net;
using System.Runtime.CompilerServices;
using System.Threading;
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

        [TestMethod]
        public async Task Func_ResponseAsync_Awaiter_T_Ok()
        {
            Func<ResponseAsync<int>> func = () => ResponseAsync.FromResult(1);

            var result = await func;

            Assert.IsTrue(result);
            Assert.AreEqual(1, result);
        }

        [TestMethod]
        public async Task Func_ResponseAsync_Awaiter_T_Error()
        {
            Func<ResponseAsync<int>> func = () => ResponseAsync.FromException<int>(new Exception("await error"));

            var result = await func;

            Assert.IsFalse(result);
        }

        [TestMethod]
        public async Task Func_Awaiter_T_Ok()
        {
            Func<int> func = () => 1;

            var result = await func;

            Assert.IsTrue(result);
            Assert.AreEqual(1, result);
        }

        [TestMethod]
        public async Task Func_Awaiter_T_Error()
        {
            Func<int> func = () => throw new Exception("await error");

            var result = await func;

            Assert.IsFalse(result);
        }

        [TestMethod]
        public async Task Action_Awaiter_Ok()
        {
            Action action = () => { };

            var result = await action;

            Assert.IsTrue(result);
        }

        [TestMethod]
        public async Task Action_Awaiter_Error()
        {
            Action action = () => throw new Exception("await error");

            var result = await action;

            Assert.IsFalse(result);
        }

        [TestMethod]
        public async Task Func_Task_Awaiter_Void_Ok()
        {
            Func<Task> func = () => Task.CompletedTask;

            await func;
        }

        [TestMethod]
        public async Task Func_Task_Awaiter_Void_Exception()
        {
            Func<Task> func = () => Task.FromException(new FormatException());

            var result = await func;

            Assert.IsFalse(result);
        }

        [TestMethod]
        public async Task Func_Task_Awaiter_Void_Canceled()
        {
            Func<Task> func = () => Task.FromCanceled(new CancellationToken(true));

            var result = await func;

            Assert.IsFalse(result);
        }

        [TestMethod]
        public async Task Func_Task_Awaiter_T_Ok()
        {
            Func<Task<int>> func = () => Task.FromResult(1);

            var result = await func;

            Assert.AreEqual(1, result);
        }

        [TestMethod]
        public async Task Func_Task_Awaiter_T_Exception()
        {
            Func<Task<int>> func = () => Task.FromException<int>(new FormatException());

            var result = await func;

            Assert.IsFalse(result);
        }

        [TestMethod]
        public async Task Func_Task_Awaiter_T_Canceled()
        {
            Func<Task<int>> func = () => Task.FromCanceled<int>(new CancellationToken(true));

            var result = await func;

            Assert.IsFalse(result);
        }

        [TestMethod]
        public async Task Func_Task_Awaiter_Response_Ok()
        {
            Func<Task<Response>> func = () => Task.FromResult(Response.Success);

            var result = await func;

            Assert.IsTrue(result);
        }

        [TestMethod]
        public async Task Func_Task_Awaiter_Response_Exception()
        {
            Func<Task<Response>> func = () => Task.FromException<Response>(new FormatException());

            var result = await func;

            Assert.IsFalse(result);
        }

        [TestMethod]
        public async Task Func_Task_Awaiter_Response_Canceled()
        {
            Func<Task<Response>> func = () => Task.FromCanceled<Response>(new CancellationToken(true));

            var result = await func;

            Assert.IsFalse(result);
        }

        [TestMethod]
        public async Task Func_Task_Awaiter_ResponseT_Ok()
        {
            Func<Task<Response<int>>> func = () => Task.FromResult(Response.Create(1));

            var result = await func;

            Assert.AreEqual(1, result);
        }

        [TestMethod]
        public async Task Func_Task_Awaiter_ResponseT_Exception()
        {
            Func<Task<Response<int>>> func = () => Task.FromException<Response<int>>(new FormatException());

            var result = await func;

            Assert.IsFalse(result);
        }

        [TestMethod]
        public async Task Func_Task_Awaiter_ResponseT_Canceled()
        {
            Func<Task<Response<int>>> func = () => Task.FromCanceled<Response<int>>(new CancellationToken(true));

            var result = await func;

            Assert.IsFalse(result);
        }

        [TestMethod]
        public async Task Func_ValueTask_Awaiter_ValueResponse_Ok()
        {
            Func<ValueTask> func = () => new ValueTask();

            var result = await func;

            Assert.IsTrue(result);
        }

        [TestMethod]
        public async Task Func_ValueTask_Awaiter_TaskResponse_Ok()
        {
            Func<ValueTask> func = () => new ValueTask(Task.CompletedTask);

            var result = await func;

            Assert.IsTrue(result);
        }

        [TestMethod]
        public async Task Func_ValueTask_Awaiter_TaskResponse_Delay()
        {
            Func<ValueTask> func = () => new ValueTask(Task.Delay(1));

            var result = await func;

            Assert.IsTrue(result);
        }

        [TestMethod]
        public async Task Func_ValueTask_Awaiter_ValueResponse_FuncIsNull()
        {
            Func<ValueTask> func = null;

            var result = await func;

            Assert.IsFalse(result);
        }

        [TestMethod]
        public async Task Func_ValueTask_Awaiter_TaskResponse_Error()
        {
            Func<ValueTask> func = () => new ValueTask(Task.FromException(new FormatException()));

            var result = await func;

            Assert.IsFalse(result);
        }

        [TestMethod]
        public async Task Func_ValueTask_Awaiter_TaskResponse_DelayedError()
        {
            Func<ValueTask> func = () => new ValueTask(Task.Delay(1).ContinueWith(_ => throw new FormatException()));

            var result = await func;

            Assert.IsFalse(result);
        }

        [TestMethod]
        public async Task Func_ValueTask_Awaiter_TaskResponse_Cancel()
        {
            Func<ValueTask> func = () => new ValueTask(Task.FromCanceled(new CancellationToken(true)));

            var result = await func;

            Assert.IsFalse(result);
        }

        [TestMethod]
        public async Task Func_ValueTask_Awaiter_TaskResponse_DelayedCancel()
        {
            Func<ValueTask> func = () => new ValueTask(Task.Delay(1).ContinueWith(_ => { }, TaskContinuationOptions.OnlyOnFaulted));

            var result = await func;

            Assert.IsFalse(result);
        }

        [TestMethod]
        public async Task Func_ValueTask_Awaiter_ValueResponseT_Ok()
        {
            Func<ValueTask<int>> func = () => new ValueTask<int>(1);

            var result = await func;

            Assert.AreEqual(1, result);
        }

        [TestMethod]
        public async Task Func_ValueTask_Awaiter_Response_Ok()
        {
            Func<ValueTask<Response>> func = () => new ValueTask<Response>(Response.Success);

            var result = await func;

            Assert.IsTrue(result);
        }

        [TestMethod]
        public async Task Func_ValueTask_Awaiter_ResponseT_Ok()
        {
            Func<ValueTask<Response<int>>> func = () => new ValueTask<Response<int>>(Response<int>.Success(1));

            var result = await func;

            Assert.AreEqual(result, 1);
        }
    }

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
}
