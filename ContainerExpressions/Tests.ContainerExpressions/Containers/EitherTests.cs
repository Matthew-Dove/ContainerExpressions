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
    }
}
