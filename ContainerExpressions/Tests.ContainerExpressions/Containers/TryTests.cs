using ContainerExpressions.Containers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace Tests.ContainerExpressions.Containers
{
    [TestClass]
    public class TryTests
    {
        #region BaseTry

        [TestMethod]
        public void Base_ValidCodeRuns_WithNoErrorsLogged()
        {
            var isRan = false;
            var isErrorsLogged = false;
            Action action = () => { isRan = true; };
            Try.SetExceptionLogger((ex) => { isErrorsLogged = true; });

            var result = Try.Run(action);

            Assert.IsTrue(result);
            Assert.IsTrue(isRan);
            Assert.IsFalse(isErrorsLogged);
        }

        [TestMethod]
        public void Base_MethodThrowsError_ErrorIsCaught()
        {
            Action errorMethod = () => { throw new Exception(); };

            var result = Try.Run(errorMethod);

            Assert.IsFalse(result);
        }

        [TestMethod]
        public void Base_MethodThrowsError_LoggerIsCalled()
        {
            var loggerWasCalled = false;
            Try.SetExceptionLogger((ex) => loggerWasCalled = true);
            Action errorMethod = () => { throw new Exception(); };

            var result = Try.Run(errorMethod);

            Assert.IsFalse(result);
            Assert.IsTrue(loggerWasCalled);
        }

        [TestMethod]
        public void Base_WhenLoggerIsRemoved_InstancesStillWorkWithTheLoggerTheyWhereCreatedWith()
        {
            var loggerWasCalled = false;
            Try.SetExceptionLogger((ex) => loggerWasCalled = true);
            Action errorMethod = () => { throw new Exception(); };

            var result = Try.Run(errorMethod);
            Try.SetExceptionLogger((ex) => loggerWasCalled = false);

            Assert.IsFalse(result);
            Assert.IsTrue(loggerWasCalled);
        }

        #endregion

        #region Try

        [TestMethod]
        public void ValidCodeRuns_WithNoErrorsLogged()
        {
            var answer = Guid.NewGuid();
            var isErrorsLogged = false;
            Func<Guid> func = () => answer;
            Try.SetExceptionLogger((ex) => { isErrorsLogged = true; });

            var result = Try.Run(func);

            Assert.IsTrue(result);
            Assert.AreEqual(answer, result);
            Assert.IsFalse(isErrorsLogged);
        }

        [TestMethod]
        public void MethodThrowsError_ErrorIsCaught()
        {
            Func<int> errorMethod = () => { throw new Exception(); };

            var result = Try.Run(errorMethod);

            Assert.IsFalse(result);
        }

        [TestMethod]
        public void MethodThrowsError_LoggerIsCalled()
        {
            var loggerWasCalled = false;
            Try.SetExceptionLogger((ex) => loggerWasCalled = true);
            Func<int> errorMethod = () => { throw new Exception(); };

            var result = Try.Run(errorMethod);

            Assert.IsFalse(result);
            Assert.IsTrue(loggerWasCalled);
        }

        [TestMethod]
        public void WhenLoggerIsRemoved_InstancesStillWorkWithTheLoggerTheyWhereCreatedWith()
        {
            var loggerWasCalled = false;
            Try.SetExceptionLogger((ex) => loggerWasCalled = true);
            Func<int> errorMethod = () => { throw new Exception(); };

            var result = Try.Run(errorMethod);
            Try.SetExceptionLogger((ex) => loggerWasCalled = false);

            Assert.IsFalse(result);
            Assert.IsTrue(loggerWasCalled);
        }

        #endregion
    }
}
