using ContainerExpressions.Containers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Tests.ContainerExpressions.Containers
{
    [TestClass]
    public class CacheTests
    {
        [TestMethod]
        public void CanGetBaseTask()
        {
            var a = Cache.Get<Task>();
            var b = Cache.Get<Task[]>();
            var c = Cache.Get<IEnumerable<Task>>();
            var d = Cache.Get<List<Task>>();

            Assert.IsTrue(a.IsCompletedSuccessfully);
            Assert.AreEqual(0, b.Length);
            Assert.IsFalse(c.Any());
            Assert.AreEqual(0, d.Count);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void BaseTaskTValueMustBeSetFirst()
        {
            var hashset = Cache.Get<HashSet<Task>>();
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void CannotSetNull()
        {
            Cache.Set<HashSet<Task>>(null);
        }

        [TestMethod]
        public void CanSetTValueOnce()
        {
            Cache.Set(new HashSet<Task>());

            var hashset = Cache.Get<HashSet<Task>>();

            Assert.IsNotNull(hashset);
            Assert.AreEqual(0, hashset.Count);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void CannotSetTValueTwice()
        {
            Cache.Set(new HashSet<Task>());
            Cache.Set(new HashSet<Task>());
        }
    }
}
