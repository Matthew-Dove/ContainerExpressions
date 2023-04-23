using ContainerExpressions.Containers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

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

        #endregion
    }
}
