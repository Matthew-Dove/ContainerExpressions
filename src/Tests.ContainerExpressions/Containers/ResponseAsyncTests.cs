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
        private static readonly int _numThreads = Environment.ProcessorCount;

        #region Test Functions

        private static async ResponseAsync<int> RunAwaiters()
        {
            await Task.Yield(); // YieldAwaiter
            await ResponseAsync.FromResult("Hello"); // ResponseAsyncAwaiter
            await new ValueTask<string>("World"); // ValueTaskAwaiter
            return await Task.Delay(1).ContinueWith(_ => _result); // TaskAwaiter
        }

        private static async Task<int> RunAwaitersWithTask()
        {
            await Task.Yield();
            await ResponseAsync.FromResult("Hello");
            await new ValueTask<string>("World");
            return await Task.Delay(1).ContinueWith(_ => _result);
        }

        private static async ValueTask<int> RunAwaitersWithValueTask()
        {
            await Task.Yield();
            await ResponseAsync.FromResult("Hello");
            await new ValueTask<string>("World");
            return await Task.Delay(1).ContinueWith(_ => _result);
        }

        private static async ResponseAsync<int> ThrowError(bool throwBeforeAwait = false) { if (throwBeforeAwait) throw new Exception("Error!"); await Task.CompletedTask; throw new Exception("Error!"); }

        [AsyncMethodBuilder(typeof(ResponseAsyncTaskCompletionSource<>))]
        private static async Task<Response<int>> TaskError(bool throwBeforeAwait = false) { if (throwBeforeAwait) throw new Exception("Error!"); await Task.CompletedTask; throw new Exception("Error!"); }

        [AsyncMethodBuilder(typeof(ResponseAsyncTaskCompletionSource<>))]
        private static async Task<Response<int>> TaskDelay() { await Task.Delay(1); return Response.Create(_result); }

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
        public async Task Happy_Path_WithTask()
        {
            var result = await RunAwaitersWithTask().AsResponse();

            Assert.IsTrue(result);
            Assert.AreEqual(_result, result);
        }

        [TestMethod]
        public async Task Happy_Path_WithValueTask()
        {
            var result = await RunAwaitersWithValueTask().AsResponse();

            Assert.IsTrue(result);
            Assert.AreEqual(_result, result);
        }

        [TestMethod]
        public void Happy_Path_Blocking()
        {
            var result = RunAwaiters().GetAwaiter().GetResult();

            Assert.IsTrue(result);
            Assert.AreEqual(_result, result);
        }

        [TestMethod]
        public void Happy_Path_WithTask_Blocking()
        {
            var result = RunAwaitersWithTask().AsResponse().GetAwaiter().GetResult();

            Assert.IsTrue(result);
            Assert.AreEqual(_result, result);
        }

        [TestMethod]
        public void Happy_Path_WithValueTask_Blocking()
        {
            var result = RunAwaitersWithValueTask().AsResponse().GetAwaiter().GetResult();

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
            var results = await Task.WhenAll(RunAwaiters().ToTask(), RunAwaiters());

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
            Try.SetFormattedExceptionLogger((ex, _, _) => msg = ex.Message);

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
        public async Task Many_Threads()
        {
            var isError = false;

            await Parallel.ForEachAsync(Enumerable.Repeat(0, _numThreads), async (_, _) => {
                var result = await RunAwaiters();
                if (!result.IsTrue(x => x == _result)) Volatile.Write(ref isError, true);
            });

            Assert.IsFalse(Volatile.Read(ref isError));
        }

        [TestMethod]
        public async Task Many_Threads_Task()
        {
            var isError = false;

            await Parallel.ForEachAsync(Enumerable.Repeat(0, _numThreads), async (_, _) => {
                var result = await RunAwaiters().ToTask();
                if (!result.IsTrue(x => x == _result)) Volatile.Write(ref isError, true);
            });

            Assert.IsFalse(Volatile.Read(ref isError));
        }

        [TestMethod]
        public async Task Many_Threads_ValueTask()
        {
            var isError = false;

            await Parallel.ForEachAsync(Enumerable.Repeat(0, _numThreads), async (_, _) => {
                var result = await RunAwaiters().ToValueTask();
                if (!result.IsTrue(x => x == _result)) Volatile.Write(ref isError, true);
            });

            Assert.IsFalse(Volatile.Read(ref isError));
        }

        [TestMethod]
        public async Task Many_Threads_WhenAll()
        {
            var tasks = new Task<Response<int>>[_numThreads];
            for (int i = 0; i < _numThreads; i++) tasks[i] = RunAwaiters();

            var results = await Task.WhenAll(tasks);

            var count = results.Count(x => x.IsTrue(y => y == _result));
            Assert.AreEqual(_numThreads, count);
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

        private static async Task<int> Add(int a, int b)
        {
            await Task.Delay(1);
            return a + b;
        }

        [TestMethod]
        public async Task Func_Awaiters_Task_Custom_Awaiter()
        {
            var adder = Lambda.Args(1, 1).ToFunc(Add);

            var result = await adder; // Func is handled by a custom awaiter in ResponseAsync.

            Assert.AreEqual(2, result);
        }

        [TestMethod]
        public async Task Func_Awaiters_Task_Standard_Awaiter()
        {
            var adder = Lambda.Args(1, 1).ToFunc(Add);

            var result = await adder(); // Func is invoked, so the normal task awaiter handles this one.

            Assert.AreEqual(2, result);
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
        public async Task Many_Threads_TaskDelay()
        {
            var isError = false;

            await Parallel.ForEachAsync(Enumerable.Repeat(0, _numThreads), async (_, _) => {
                var result = await TaskDelay();
                if (!result.IsTrue(x => x == _result)) Volatile.Write(ref isError, true);
            });

            Assert.IsFalse(Volatile.Read(ref isError));
        }
    }
}
