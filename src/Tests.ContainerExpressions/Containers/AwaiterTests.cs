using ContainerExpressions.Containers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
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
            await response;
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
            ResponseAsync<int> successResponse = Divide(1, 1);
            Response<int> success = await successResponse;

            ResponseAsync<int> errorResponse = Divide(1, 0);
            Response<int> error = await errorResponse;

            Assert.IsTrue(success);
            Assert.AreEqual(1, success);
            Assert.IsFalse(error);
        }

        private static async Task<int> Divide(int numerator, int denominator)
        {
            var quotient = numerator / denominator;
            await Task.Delay(1); // The method must use async await, otherwise the exception will be thrown at runtime instead of being handled.
            return quotient;
        }
    }

    public static class AwaiterExtensions
    {
        // Example using ValueTask. Will blowup when the response is not valid, or the task is canceled, or faulted.
        public static ValueTaskAwaiter<Response<T>> GetAwaiter<T>(this Response<ValueTask<T>> response)
        {
            return new ValueTask<Response<T>>(Response.Create(response.Value.Result)).GetAwaiter();
        }

        // Example using Task with no result. Will blowup when the response is not valid, or the task is canceled, or faulted.
        public static TaskAwaiter GetAwaiter(this Response<Task> response)
        {
            return response.Value.GetAwaiter();
        }

        // Example using Response with Task.
        public static TaskAwaiter<Response<T>> GetAwaiter<T>(this Response<Task<T>> response)
        {
            if (!response) return InstanceAsync.Of<Response<T>>().GetAwaiter();
            return response.Value.ContinueWith(static t => {
                if (t.Status == TaskStatus.Faulted) t.Exception.LogError();
                if (t.Status == TaskStatus.RanToCompletion) return Response.Create(t.Result);
                return new Response<T>();
            }).GetAwaiter();
        }

        // Example using ResponseAsync with ValueTask.
        public static ValueTaskAwaiter<Response<T>> GetAwaiter<T>(this ResponseAsync<T> response)
        {
            if (response.Task.IsCompleted)
            {
                return response.Task.IsCompletedSuccessfully
                    ? new ValueTask<Response<T>>(Response.Create(response.Task.Result)).GetAwaiter()
                    : new ValueTask<Response<T>>(Response.Create<T>()).GetAwaiter();
            }

            return new ValueTask<Response<T>>(response.Task.AsTask().ContinueWith(static t =>
            {
                if (t.Status == TaskStatus.Faulted) t.Exception.LogError();
                if (t.Status == TaskStatus.RanToCompletion) return Response.Create(t.Result);
                return new Response<T>();
            })).GetAwaiter();
        }

        // Unpack
        public static TaskAwaiter<Response<T>> GetAwaiter<T>(this Response<Task<Response<Response<T>>>> response)
        {
            if (!response) return InstanceAsync.Of<Response<T>>().GetAwaiter();
            return response.Value.UnpackAsync().GetAwaiter();
        }
    }
}
