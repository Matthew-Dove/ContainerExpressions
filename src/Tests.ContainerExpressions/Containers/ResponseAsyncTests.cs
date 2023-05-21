using ContainerExpressions.Containers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;
using System;
using System.Linq;

namespace Tests.ContainerExpressions.Containers
{
    [TestClass]
    public class ResponseAsyncTests
    {
        private const int _result = 1;

        private static async ResponseAsync<int> RunAwaiters()
        {
            await Task.Yield(); // YieldAwaiter
            await new ValueTask<string>("Hello, World!"); // ValueTaskAwaiter
            return await Task.Delay(1).ContinueWith(_ => _result); // TaskAwaiter
        }

        private static async ResponseAsync<int> ThrowError() { await Task.CompletedTask; throw new Exception("Error!"); }

        [TestMethod]
        public async Task Happy_Path()
        {
            var result = await RunAwaiters();

            Assert.IsTrue(result);
            Assert.AreEqual(_result, result);
        }

        [TestMethod]
        public void Synchronous_Result()
        {
            var result = ResponseAsync<int>.FromResult(_result).GetAwaiter().GetResult();

            Assert.AreEqual(_result, result);
        }

        [TestMethod]
        public async Task Await_Synchronous_Result()
        {
            var result = await ResponseAsync<int>.FromResult(_result);

            Assert.AreEqual(_result, result);
        }

        [TestMethod]
        public async Task Many_Tasks()
        {
            var results = await Task.WhenAll(RunAwaiters().AsTask(), RunAwaiters());

            var sum = results.Where(x => x).Sum(x => x);

            Assert.AreEqual(_result + _result, sum);
        }

        [TestMethod]
        public async Task Error_Handling()
        {
            const int error = -1;

            var err01 = await ThrowError();
            var err02 = await ThrowError();

            int result = err01 && err02 ? err01 + err02 : error;

            Assert.AreEqual(error, result);
        }

        [TestMethod]
        public async Task Error_Logging()
        {
            var msg = string.Empty;
            Try.SetExceptionLogger(ex => msg = ex.Message);

            var result = await ThrowError();

            Assert.IsFalse(result);
            Assert.AreNotEqual(string.Empty, msg);
        }

        [TestMethod]
        public async Task Many_Awaits()
        {
            var response = RunAwaiters();

            var result01 = await response;

            await response;
            await response;
            await response;
            await response;
            await response;

            var result02 = await response;

            Assert.AreEqual(_result, result01);
            Assert.AreEqual(result01, result02);
        }
    }
}
