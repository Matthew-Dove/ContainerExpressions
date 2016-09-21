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

        [TestMethod]
        public void T1ValueSet_T1ValueRead_WithHelperClass()
        {
            var guid = Guid.NewGuid();
            var either = Either.Create<Guid, int>(guid);

            var result = either.Match(x => x.ToString(), x => x.ToString());

            Assert.AreEqual(guid, Guid.Parse(result));
        }

        [TestMethod]
        public void T1ValueSet_T1ValueRead_WithChainHelperClass()
        {
            var guid = Guid.NewGuid();
            var either = Either.Create(guid).WithType<int>();

            var result = either.Match(x => x.ToString(), x => x.ToString());

            Assert.AreEqual(guid, Guid.Parse(result));
        }

        [TestMethod]
        public void T1ValueSet_T1ValueRead_CopyConstructor()
        {
            var guid = Guid.NewGuid();
            var either1 = new Either<Guid, int>(guid);
            var either2 = new Either<Guid, int>(either1);

            var result = either2.Match(x => x.ToString(), x => x.ToString());

            Assert.AreEqual(guid, Guid.Parse(result));
        }

        [TestMethod]
        public void T1ValueSet_T1ValueRead_CopyConstructorHelperClass()
        {
            var guid = Guid.NewGuid();
            var either1 = Either.Create(guid).WithType<int>();
            var either2 = Either.Create(either1);

            var result = either2.Match(x => x.ToString(), x => x.ToString());

            Assert.AreEqual(guid, Guid.Parse(result));
        }

        [TestMethod]
        public void T1ValueSet_T1ValueIs_ToStringed()
        {
            var guid = Guid.NewGuid();
            var either = Either.Create<Guid, int>(guid);

            var result = either.ToString();

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

        [TestMethod]
        public void T2ValueSet_T2ValueRead_WithHelperClass()
        {
            var number = 42;
            var either = Either.Create<Guid, int>(number);

            var result = either.Match(x => x.ToString(), x => x.ToString());

            Assert.AreEqual(number, int.Parse(result));
        }

        [TestMethod]
        public void T2ValueSet_T2ValueRead_WithChainHelperClass()
        {
            var number = 42;
            var either = Either.Create(number).WithType<int>();

            var result = either.Match(x => x.ToString(), x => x.ToString());

            Assert.AreEqual(number, int.Parse(result));
        }

        [TestMethod]
        public void T2ValueSet_T2ValueRead_CopyConstructor()
        {
            var number = 42;
            var either1 = new Either<Guid, int>(number);
            var either2 = new Either<Guid, int>(either1);

            var result = either2.Match(x => x.ToString(), x => x.ToString());

            Assert.AreEqual(number, int.Parse(result));
        }

        [TestMethod]
        public void T2ValueSet_T2ValueRead_CopyConstructorHelperClass()
        {
            var number = 42;
            var either1 = Either.Create(number).WithType<int>();
            var either2 = Either.Create(either1);

            var result = either2.Match(x => x.ToString(), x => x.ToString());

            Assert.AreEqual(number, int.Parse(result));
        }

        [TestMethod]
        public void T2ValueSet_T2ValueIs_ToStringed()
        {
            var number = 42;
            var either = Either.Create<Guid, int>(number);

            var result = either.ToString();

            Assert.AreEqual(number, int.Parse(result));
        }

        #endregion
    }
}
