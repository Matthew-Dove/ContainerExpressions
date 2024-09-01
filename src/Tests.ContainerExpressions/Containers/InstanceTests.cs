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
        #region Instance

        [TestMethod]
        public void Instance_CanGetBaseTask()
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

        class HS : Alias<HashSet<Task>> { public HS() : base(default) { } }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void Instance_BaseTaskTValueMustBeSetFirst()
        {
            var hashset = Instance.Of<HS>();
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Instance_CannotSetNull()
        {
            Instance.Create<HashSet<Task>>(null);
        }

        [TestMethod]
        public void Instance_CanSetTValueOnce()
        {
            Instance.Create(new HashSet<Task>());

            var hashset = Instance.Of<HashSet<Task>>();

            Assert.IsNotNull(hashset);
            Assert.AreEqual(0, hashset.Count);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void Instance_CannotSetTValueTwice()
        {
            Instance.Create(new HashSet<Task>());
            Instance.Create(new HashSet<Task>());
        }

        #endregion

        #region InstanceAsync

        #region Task

        [TestMethod]
        public async Task InstanceAsync_Task_Value()
        {
            var result = await InstanceAsync.Of<int>();
            Assert.AreEqual(default(int), result);
        }

        [TestMethod]
        public async Task InstanceAsync_Task_Reference()
        {
            var result = await InstanceAsync.Of<string>();
            Assert.IsTrue(result == null || result == string.Empty);
        }

        class S : Alias<string> { public S() : base(string.Empty) { } }

        [TestMethod]
        public async Task InstanceAsync_Task_CustomReference()
        {
            InstanceAsync.Create(new S());
            var result = await InstanceAsync.Of<S>();
            Assert.AreEqual(string.Empty, result);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void InstanceAsync_Task_CustomReference_CannotSetDefault()
        {
            InstanceAsync.Create(default(string));
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void InstanceAsync_Task_CustomReference_CannotSetTwice()
        {
            InstanceAsync.Create(string.Empty);
            InstanceAsync.Create(string.Empty);
        }

        #endregion

        #region ValueTask

        [TestMethod]
        public async Task InstanceAsync_ValueTask_Value()
        {
            var result = await InstanceAsync.ValueOf<int>();
            Assert.AreEqual(default(int), result);
        }

        [TestMethod]
        public async Task InstanceAsync_ValueTask_Reference()
        {
            var result = await InstanceAsync.ValueOf<string>();
            Assert.IsTrue(result == null || result == string.Empty);
        }

        [TestMethod]
        public async Task InstanceAsync_ValueTask_CustomReference()
        {
            InstanceAsync.CreateValue(string.Empty);
            var result = await InstanceAsync.ValueOf<string>();
            Assert.AreEqual(string.Empty, result);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void InstanceAsync_ValueTask_CustomReference_CannotSetDefault()
        {
            InstanceAsync.CreateValue(default(string));
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void InstanceAsync_ValueTask_CustomReference_CannotSetTwice()
        {
            InstanceAsync.CreateValue(string.Empty);
            InstanceAsync.CreateValue(string.Empty);
        }

        #endregion

        #region ResponseAsync

        [TestMethod]
        public async Task InstanceAsync_ResponseAsync_Value()
        {
            var result = await InstanceAsync.ResponseOf<int>();
            Assert.AreEqual(default(int), result);
        }

        [TestMethod]
        public async Task InstanceAsync_ResponseAsync_Reference()
        {
            var result = await InstanceAsync.ResponseOf<string>();
            Assert.IsTrue(result.Value == null || result.Value == string.Empty);
        }

        [TestMethod]
        public async Task InstanceAsync_ResponseAsync_CustomReference()
        {
            InstanceAsync.CreateResponse(string.Empty);
            var result = await InstanceAsync.ResponseOf<string>();
            Assert.AreEqual(string.Empty, result);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void InstanceAsync_ResponseAsync_CustomReference_CannotSetDefault()
        {
            InstanceAsync.CreateResponse(default(string));
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void InstanceAsync_ResponseAsync_CustomReference_CannotSetTwice()
        {
            InstanceAsync.CreateResponse(string.Empty);
            InstanceAsync.CreateResponse(string.Empty);
        }

        #endregion

        #endregion
    }
}
