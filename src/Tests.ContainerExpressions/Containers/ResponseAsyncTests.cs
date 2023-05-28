using ContainerExpressions.Containers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;
using System;
using System.Linq;
using System.Threading;
using System.Runtime.CompilerServices;

namespace Tests.ContainerExpressions.Containers
{
    [TestClass]
    public class ResponseAsyncTests
    {
        private const int _result = 1;
        private static readonly int _numThreads = Environment.ProcessorCount - 1;

        #region Test Functions

        private static ResponseAsync<int> RunAwaiters() => RunAwaitersWithDelay(1);

        private static async ResponseAsync<int> RunAwaitersWithDelay(short delay) // ResponseAsyncAwaiter
        {
            await Task.Yield(); // YieldAwaiter
            await new ValueTask<string>("Hello, World!"); // ValueTaskAwaiter
            return await Task.Delay(delay).ContinueWith(_ => _result); // TaskAwaiter
        }

        private static async ResponseAsync<int> ThrowError(bool throwBeforeAwait = false) { if (throwBeforeAwait) throw new Exception("Error!"); await Task.CompletedTask; throw new Exception("Error!"); }

        [AsyncMethodBuilder(typeof(ResponseAsyncTaskCompletionSource<>))]
        private static async Task<Response<int>> TaskError(bool throwBeforeAwait = false) { if (throwBeforeAwait) throw new Exception("Error!"); await Task.CompletedTask; throw new Exception("Error!"); }

        [AsyncMethodBuilder(typeof(ResponseAsyncValueTaskCompletionSource<>))]
        private static async ValueTask<Response<int>> ValueTaskError(bool throwBeforeAwait = false) { if (throwBeforeAwait) throw new Exception("Error!"); await Task.CompletedTask; throw new Exception("Error!"); }

        [AsyncMethodBuilder(typeof(ResponseAsyncValueTaskSource<>))]
        private static async ValueTask<Response<int>> ValueTaskSourceError(bool throwBeforeAwait = false) { if (throwBeforeAwait) throw new Exception("Error!"); await Task.CompletedTask; throw new Exception("Error!"); }

        [AsyncMethodBuilder(typeof(ResponseAsyncTaskCompletionSource<>))]
        private static async Task<Response<int>> TaskDelay() { await Task.Delay(1); return Response.Create(_result); }

        [AsyncMethodBuilder(typeof(ResponseAsyncValueTaskCompletionSource<>))]
        private static async ValueTask<Response<int>> ValueTaskDelay() { await Task.Delay(1); return Response.Create(_result); }

        [AsyncMethodBuilder(typeof(ResponseAsyncValueTaskSource<>))]
        private static async ValueTask<Response<int>> ValueTaskSourceDelay() { await Task.Delay(1); return Response.Create(_result); }

        private static int TryGetResult() => throw new Exception("Error!");
        private static int GetResult0() => _result;
        private static int GetResult1(int i) => _result;
        private static int GetResult2(int i, string s) => _result;

        private static void TryGetVoid() => throw new Exception("Error!");
        private static void GetVoid0() { }
        private static void GetVoid1(int i) { }
        private static void GetVoid2(int i, string s) { }

        #endregion

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
            var result = ResponseAsync.FromResult(_result).GetAwaiter().GetResult();

            Assert.AreEqual(_result, result);
        }

        [TestMethod]
        public async Task Await_Synchronous_Result()
        {
            var result = await ResponseAsync.FromResult(_result);

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

            var err01 = await ThrowError(throwBeforeAwait: true);
            var err02 = await ThrowError();

            int result = err01 && err02 ? err01 + err02 : error;

            Assert.AreEqual(error, result);
        }

        [TestMethod]
        public void Error_Handling_Blocking()
        {
            const int error = -1;

            var err01 = ThrowError(throwBeforeAwait: true).GetAwaiter().GetResult();
            var err02 = ThrowError().GetAwaiter().GetResult();

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

        [TestMethod]
        public void Many_Threads()
        {
            var isError = false;
            var func = RunAwaiters();

            Parallel.For(0, _numThreads, async _ => {
                var result = await func;
                if (!result.IsTrue(x => x == _result)) Volatile.Write(ref isError, true);
            });

            Assert.IsFalse(Volatile.Read(ref isError));
        }

        [TestMethod]
        public void Many_Threads_Task()
        {
            var isError = false;
            var func = RunAwaiters().AsTask();

            Parallel.For(0, _numThreads, async _ => {
                var result = await func;
                if (!result.IsTrue(x => x == _result)) Volatile.Write(ref isError, true);
            });

            Assert.IsFalse(Volatile.Read(ref isError));
        }

        [TestMethod]
        public void Many_Threads_ValueTask()
        {
            var isError = false;
            var func = RunAwaiters();

            Parallel.For(0, _numThreads, async _ => {
                var result = await func.AsValueTask();
                if (!result.IsTrue(x => x == _result)) Volatile.Write(ref isError, true);
            });

            Assert.IsFalse(Volatile.Read(ref isError));
        }

        [TestMethod]
        public async Task Func_Awaiters()
        {
            // Creating anonymous delegates around functions with N args.
            var funcLift0 = await Lambda.ToFunc(GetResult0);
            var funcLift1 = await Lambda.ToFunc(() => GetResult1(1));
            var funcLift2 = await Lambda.ToFunc(() => GetResult2(1, ""));

            Assert.AreEqual(_result, funcLift0);
            Assert.AreEqual(_result, funcLift1);
            Assert.AreEqual(_result, funcLift2);

            // Gathering a function's args first, then supplying them to the function at the end.
            var funcCast0 = await Lambda.Args().ToFunc(GetResult0);
            var funcCast1 = await Lambda.Args(1).ToFunc(GetResult1);
            var funcCast2 = await Lambda.Args(1, "").ToFunc(GetResult2);

            Assert.AreEqual(_result, funcCast0);
            Assert.AreEqual(_result, funcCast1);
            Assert.AreEqual(_result, funcCast2);

            // Creating anonymous delegates around actions with N args.
            var actionLift0 = await Lambda.ToAction(GetVoid0);
            var actionLift1 = await Lambda.ToAction(() => GetVoid1(1));
            var actionLift2 = await Lambda.ToAction(() => GetVoid2(1, ""));

            Assert.AreEqual(Unit.Instance, actionLift0);
            Assert.AreEqual(Unit.Instance, actionLift1);
            Assert.AreEqual(Unit.Instance, actionLift2);

            // Gathering a actions's args first, then supplying them to the action at the end.
            var actionCast0 = await Lambda.Args().ToAction(GetVoid0);
            var actionCast1 = await Lambda.Args(1).ToAction(GetVoid1);
            var actionCast2 = await Lambda.Args(1, "").ToAction(GetVoid2);

            Assert.AreEqual(Unit.Instance, actionCast0);
            Assert.AreEqual(Unit.Instance, actionCast1);
            Assert.AreEqual(Unit.Instance, actionCast2);

            // Functions that throw errors are invalid Response{T} types, and not runtime exception.
            var func1 = TryGetResult;
            var funcResult1 = await func1;

            Assert.IsFalse(funcResult1);

            // Void functions that throw errors are invalid Response types, and not runtime exception.
            var action1 = TryGetVoid;
            var actionResult1 = await action1;

            Assert.IsFalse(actionResult1);
        }

        [TestMethod]
        public async Task AsyncMethodBuilder_TaskError()
        {
            var blocking1 = TaskError().GetAwaiter().GetResult();
            var blocking2 = TaskError(throwBeforeAwait: true).GetAwaiter().GetResult();

            var result1 = await TaskError();
            var result2 = await TaskError(throwBeforeAwait: true);

            Assert.IsFalse(blocking1);
            Assert.IsFalse(blocking2);

            Assert.IsFalse(result1);
            Assert.IsFalse(result2);
        }

        [TestMethod]
        public async Task AsyncMethodBuilder_ValueTaskError()
        {
            var blocking1 = ValueTaskError().GetAwaiter().GetResult();
            var blocking2 = ValueTaskError(throwBeforeAwait: true).GetAwaiter().GetResult();

            var result1 = await ValueTaskError();
            var result2 = await ValueTaskError(throwBeforeAwait: true);

            Assert.IsFalse(blocking1);
            Assert.IsFalse(blocking2);

            Assert.IsFalse(result1);
            Assert.IsFalse(result2);
        }

        [TestMethod]
        public async Task AsyncMethodBuilder_ValueTaskSourceError()
        {
            var blocking1 = ValueTaskSourceError().GetAwaiter().GetResult();
            var blocking2 = ValueTaskSourceError(throwBeforeAwait: true).GetAwaiter().GetResult();

            var result1 = await ValueTaskSourceError();
            var result2 = await ValueTaskSourceError(throwBeforeAwait: true);

            Assert.IsFalse(blocking1);
            Assert.IsFalse(blocking2);

            Assert.IsFalse(result1);
            Assert.IsFalse(result2);
        }

        [TestMethod]
        public void Many_Threads_TaskDelay()
        {
            var isError = false;

            Parallel.For(0, _numThreads, async _ => {
                var result = await TaskDelay();
                if (!result.IsTrue(x => x == _result)) Volatile.Write(ref isError, true);
            });

            Assert.IsFalse(Volatile.Read(ref isError));
        }

        [TestMethod]
        public void Many_Threads_ValueTaskDelay()
        {
            var isError = false;

            Parallel.For(0, _numThreads, async _ => {
                var result = await ValueTaskDelay();
                if (!result.IsTrue(x => x == _result)) Volatile.Write(ref isError, true);
            });

            Assert.IsFalse(Volatile.Read(ref isError));
        }

        [TestMethod]
        public void Many_Threads_ValueTaskSourceDelay()
        {
            var isError = false;

            Parallel.For(0, _numThreads, async _ =>
            {
                var result = await ValueTaskSourceDelay();
                if (!result.IsTrue(x => x == _result)) Volatile.Write(ref isError, true);
            });

            Assert.IsFalse(Volatile.Read(ref isError));
        }
    }
}
