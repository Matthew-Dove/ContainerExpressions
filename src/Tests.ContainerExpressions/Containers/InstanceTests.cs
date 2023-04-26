using ContainerExpressions.Containers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Tests.ContainerExpressions.Containers
{
    [TestClass]
    public class InstanceTests
    {
        [TestMethod]
        public void CanGetBaseTask()
        {
            var a = Instance.Of<Task>();
            var b = Instance.Of<Task[]>();
            var c = Instance.Of<IEnumerable<Task>>();
            var d = Instance.Of<List<Task>>();

            Assert.IsTrue(a.IsCompletedSuccessfully);
            Assert.AreEqual(0, b.Length);
            Assert.IsFalse(c.Any());
            Assert.AreEqual(0, d.Count);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void BaseTaskTValueMustBeSetFirst()
        {
            var hashset = Instance.Of<HashSet<Task>>();
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void CannotSetNull()
        {
            Instance.Create<HashSet<Task>>(null);
        }

        [TestMethod]
        public void CanSetTValueOnce()
        {
            Instance.Create(new HashSet<Task>());

            var hashset = Instance.Of<HashSet<Task>>();

            Assert.IsNotNull(hashset);
            Assert.AreEqual(0, hashset.Count);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void CannotSetTValueTwice()
        {
            Instance.Create(new HashSet<Task>());
            Instance.Create(new HashSet<Task>());
        }
    }
}
