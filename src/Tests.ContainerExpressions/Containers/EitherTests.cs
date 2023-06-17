using ContainerExpressions.Containers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Threading.Tasks;

namespace Tests.ContainerExpressions.Containers
{
    [TestClass]
    public class EitherTests
    {
        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void DefaultConstructor_ThrowsError()
        {
            var either = new Either<Guid, int>();

            var result = either.Match(x => x.ToString(), x => x.ToString());
        }

        [TestMethod]
        public void CanAssignToDifferentVars()
        {
            var number = 42;
            Either<Guid, int> either1 = number;
            Either<Guid, int> either2 = either1;

            var result = either2.Match(x => x.ToString(), x => x.ToString());

            Assert.AreEqual(number, int.Parse(result));
        }

        #region T1

        [TestMethod]
        public void T1ValueSet_T1ValueRead()
        {
            var guid = Guid.NewGuid();
            var either = new Either<Guid, int>(guid);

            var result = either.Match(x => x.ToString(), x => x.ToString());

            Assert.AreEqual(guid, Guid.Parse(result));
        }

        [TestMethod]
        public void T1ValueSet_T1ValueRead_WithImplicitOperator()
        {
            var guid = Guid.NewGuid();
            Either<Guid, int> either = guid;

            var result = either.Match(x => x.ToString(), x => x.ToString());

            Assert.AreEqual(guid, Guid.Parse(result));
        }

        #endregion

        #region T2

        [TestMethod]
        public void T2ValueSet_T2ValueRead()
        {
            var number = 42;
            var either = new Either<Guid, int>(number);

            var result = either.Match(x => x.ToString(), x => x.ToString());

            Assert.AreEqual(number, int.Parse(result));
        }

        [TestMethod]
        public void T2ValueSet_T2ValueRead_WithImplicitOperator()
        {
            var number = 42;
            Either<Guid, int> either = number;

            var result = either.Match(x => x.ToString(), x => x.ToString());

            Assert.AreEqual(number, int.Parse(result));
        }

        #endregion

        #region Comparison

        [TestMethod]
        public void Compare_Either_Pass()
        {
            var x = new Either<string, int>(1);
            var y = new Either<string, int>(1);

            var areEqual = x == y;

            Assert.IsTrue(areEqual);
        }

        [TestMethod]
        public void Compare_Either_Fail()
        {
            var x = new Either<string, int>(1);
            var y = new Either<string, int>("1");

            var areEqual = x == y;

            Assert.IsFalse(areEqual);
        }

        [TestMethod]
        public void Compare_T1Value_Pass()
        {
            var either = new Either<string, int>(1);
            var value = 1;

            var areEqual = value == either;

            Assert.IsTrue(areEqual);
        }

        [TestMethod]
        public void Compare_T1Value_FailValue()
        {
            var either = new Either<string, int>(1);
            var value = 2;

            var areEqual = value == either;

            Assert.IsFalse(areEqual);
        }

        [TestMethod]
        public void Compare_T1Value_FailType()
        {
            var either = new Either<string, int>(1);
            var value = "1";

            var areEqual = value == either;

            Assert.IsFalse(areEqual);
        }

        #endregion

        #region TryGet

        [TestMethod]
        public void TryGetT1_Pass()
        {
            var either = new Either<string, int>("1");

            var result = either.TryGetT1(out var value);

            Assert.IsTrue(result);
            Assert.AreEqual("1", value);
        }

        [TestMethod]
        public void TryGetT1_Pass_AutoCast()
        {
            var either = new Either<string, int>("1");

            var result = either.TryGetT1(out var value);

            Assert.IsTrue(result);
            Assert.AreEqual(either, value);
        }

        [TestMethod]
        public void TryGetT1_Fail()
        {
            var either = new Either<string, int>(1);

            var result = either.TryGetT1(out var value);

            Assert.IsFalse(result);
            Assert.AreEqual(default, value);
        }

        [TestMethod]
        public void TryGetT2_Pass()
        {
            var either = new Either<string, int>(1);

            var result = either.TryGetT2(out var value);

            Assert.IsTrue(result);
            Assert.AreEqual(1, value);
        }

        #endregion

        #region When

        private class Check { [ThreadStatic] public static bool IsCalled; }
        private class When
        {
            public static When Self = new When();
            public int Func(string s) => int.Parse(s);
            public void Action(string s) { }
            public Task<int> FuncAsync(string s) => Task.FromResult(int.Parse(s));
            public Task ActionAsync(string s) => Task.CompletedTask;
        }

        [TestMethod]
        public void When_T1()
        {
            Check.IsCalled = false;

            var either = new Either<string, int>("1");
            var number = either.WhenT1(int.Parse, out int num);
            var response = either.WhenT1(static x => { Check.IsCalled = true; });

            Assert.IsTrue(number);
            Assert.AreEqual(1, num);
            Assert.IsTrue(response);
            Assert.IsTrue(Check.IsCalled);
        }

        [TestMethod]
        public void When_Not_T1()
        {
            Check.IsCalled = false;

            var either = new Either<string, int>(1);
            var number = either.WhenT1(int.Parse, out int num);
            var response = either.WhenT1(static x => { Check.IsCalled = true; });

            Assert.IsFalse(number);
            Assert.IsFalse(response);
            Assert.IsFalse(Check.IsCalled);
        }

        [TestMethod]
        public void When_T1_Instance_Method()
        {
            var either = new Either<string, int>("1");
            var number = either.WhenT1(When.Self.Func, out int num);
            var response = either.WhenT1(When.Self.Action);

            Assert.IsTrue(number);
            Assert.AreEqual(1, num);
            Assert.IsTrue(response);
        }

        [TestMethod]
        public void When_T2()
        {
            Check.IsCalled = false;

            var either = new Either<string, int>(1);
            var number = either.WhenT2(Lambda.Identity, out int num);
            var response = either.WhenT2(static x => { Check.IsCalled = true; });

            Assert.IsTrue(number);
            Assert.AreEqual(1, num);
            Assert.IsTrue(response);
            Assert.IsTrue(Check.IsCalled);
        }

        [TestMethod]
        public async Task WhenAsyncFunc_T1()
        {
            var either = new Either<string, int>("1");
            var number = either.WhenAsyncT1(When.Self.FuncAsync, out Task<int> num);

            var n = await num;

            Assert.IsTrue(number);
            Assert.AreEqual(1, n);
        }

        [TestMethod]
        public async Task WhenAsyncFunc_T2()
        {
            var either = new Either<string, int>(1);
            var number = either.WhenAsyncT2(Lambda.IdentityAsync, out Task<int> num);

            var n = await num;

            Assert.IsTrue(number);
            Assert.AreEqual(1, n);
        }

        [TestMethod]
        public async Task WhenAsyncAction_T1()
        {
            Check.IsCalled = false;
            var either = new Either<string, int>("1");
            var number = either.WhenAsyncT1(When.Self.ActionAsync, out Task task1);
            var response = either.WhenAsyncT1(static x => { Check.IsCalled = true; return Task.CompletedTask; }, out Task task2);

            await Task.WhenAll(task1, task2);

            Assert.IsTrue(number);
            Assert.IsTrue(response);
            Assert.IsTrue(Check.IsCalled);
        }

        [TestMethod]
        public async Task WhenAsyncAction_T2()
        {
            Check.IsCalled = false;
            var either = new Either<string, int>(1);
            var number = either.WhenAsyncT2(static x => When.Self.ActionAsync(x.ToString()), out Task task1);
            var response = either.WhenAsyncT2(static x => { Check.IsCalled = true; return Task.CompletedTask; }, out Task task2);

            await Task.WhenAll(task1, task2);

            Assert.IsTrue(number);
            Assert.IsTrue(response);
            Assert.IsTrue(Check.IsCalled);
        }

        #endregion

        #region Match

        [TestMethod]
        public void MatchFunc_T1()
        {
            var either = new Either<string, int>("1");

            var result = either.Match(Lambda.Identity, _ => "0");

            Assert.AreEqual("1", result);
        }

        [TestMethod]
        public async Task MatchFuncAsync_Implicit_T1()
        {
            var either = new Either<string, int>("1");

            var result = await either.MatchAsync(Lambda.IdentityAsync, Lambda.DefaultAsync<int, string>("0"));

            Assert.AreEqual("1", result);
        }

        [TestMethod]
        public async Task MatchFuncAsync_Explicit_T1()
        {
            var either = new Either<string, int>("1");

            var result = await either.Match(Lambda.IdentityAsync, Lambda.DefaultAsync<int, string>("0"));

            Assert.AreEqual("1", result);
        }

        [TestMethod]
        public void MatchAction_T1()
        {
            Check.IsCalled = false;
            var either = new Either<string, int>("1");

            either.Match(x => { Check.IsCalled = true; }, x => { });

            Assert.IsTrue(Check.IsCalled);
        }

        [TestMethod]
        public async Task MatchActionAsync_T1()
        {
            Check.IsCalled = false;
            var either = new Either<string, int>("1");

            await either.MatchAsync(x => { Check.IsCalled = true; return Task.CompletedTask; }, x => Task.CompletedTask);

            Assert.IsTrue(Check.IsCalled);
        }

        #endregion
    }
}
