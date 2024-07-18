using ContainerExpressions.Containers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace Tests.ContainerExpressions.Containers
{
    [TestClass]
    public class MessageTests
    {
        [TestMethod]
        public void Message_Set_Get()
        {
            var message = 42;

            var reference = Guid.NewGuid().Message().Set(message);

            Assert.AreEqual(message, reference.Message().Get<int>());
        }

        [TestMethod]
        public void Message_Set_Get_Get()
        {
            var message = 42;

            var reference = Guid.NewGuid().Message().Set(message);

            Assert.AreEqual(message, reference.Message().Get<int>());
            Assert.IsFalse(reference.Message().Get<int>());
        }

        [TestMethod]
        public void Message_Get()
        {
            var reference = Guid.NewGuid();

            Assert.IsFalse(reference.Message().Get<int>());
        }
    }
}
