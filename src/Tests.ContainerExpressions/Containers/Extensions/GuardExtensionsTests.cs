using ContainerExpressions.Containers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.ExceptionServices;
using System.Threading;
using System.Threading.Tasks;

namespace Tests.ContainerExpressions.Containers.Extensions
{
    [TestClass]
    public class GuardExtensionsTests
    {
        record class A(string Name);
        private readonly A _a = new A(default);
        private readonly A[] _aa = [default];
        private readonly A[] _aaa = [new A(default)];

        record struct B(int Age);
        private readonly B _b = new B(default);
        private readonly B[] _bb = [default];
        private readonly B[] _bbb = [new B(default)];

        [TestMethod]
        public void ThrowError_Exception()
        {
            var ex = new Exception("error");

            Assert.ThrowsExactly<Exception>(() => ex.ThrowError());
        }

        [TestMethod]
        public void ThrowError_ExceptionDispatchInfo()
        {
            var ex = new Exception("error");
            var di = ExceptionDispatchInfo.Capture(ex);

            Assert.ThrowsExactly<Exception>(() => di.ThrowDispatchError());
        }

        [TestMethod]
        public void ThrowIfNull()
        {
            object obj = null;

            Assert.ThrowsExactly<ArgumentNullException>(() => obj.ThrowIfNull());
        }

        [TestMethod]
        public void ThrowIfDefault()
        {
            Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => 0.ThrowIfDefault());
        }

        [TestMethod]
        public void ThrowIfNullOrEmpty_String()
        {
            var @string = string.Empty;

            Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => @string.ThrowIfNullOrEmpty());
        }

        [TestMethod]
        public void ThrowIfNullOrEmpty_Array()
        {
            var array = new int[] { };

            Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => array.ThrowIfNullOrEmpty());
        }

        [TestMethod]
        public void ThrowIfNullOrEmpty_IEnumerable()
        {
            var enumerable = Enumerable.Empty<int>();

            Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => enumerable.ThrowIfNullOrEmpty());
        }

        [TestMethod]
        public void ThrowIfNullOrEmpty_List()
        {
            var list = new List<int>();

            Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => list.ThrowIfNullOrEmpty());
        }

        [TestMethod]
        public void ThrowIfLessThan()
        {
            int age = 1, adult = 18;

            Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => age.ThrowIfLessThan(adult));
        }

        [TestMethod]
        public void ThrowIfGreaterThan()
        {
            int age = 18, child = 17;

            Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => age.ThrowIfGreaterThan(child));
        }

        [TestMethod]
        public void ThrowIfFaultedOrCanceled_Faulted()
        {
            var task = Task.FromException<int>(new Exception("error"));

            Assert.ThrowsExactly<Exception>(() => task.ThrowIfFaultedOrCanceled());
        }

        [TestMethod]
        public void ThrowIfFaultedOrCanceled_ManyFaulted()
        {
            var task1 = Task.FromException<int>(new Exception("error1"));
            var task2 = Task.FromException<int>(new Exception("error2"));

            var task = Task.WhenAll(task1, task2);

            Assert.ThrowsExactly<AggregateException>(() => task.ThrowIfFaultedOrCanceled());
        }

        [TestMethod]
        public void ThrowIfFaultedOrCanceled_Canceled()
        {
            var task = Task.FromCanceled(new CancellationToken(true));

            Assert.ThrowsExactly<TaskCanceledException>(() => task.ThrowIfFaultedOrCanceled());
        }

        [TestMethod]
        public void ThrowIfNull_Guard()
        {
            Assert.ThrowsExactly<ArgumentNullException>(() => _a.ThrowIfNull(x => x.Name));
        }

        [TestMethod]
        public void ThrowIfNull_Sequence()
        {
            Assert.ThrowsExactly<ArgumentNullException>(() => _aa.ThrowIfSequenceIsNull());
        }

        [TestMethod]
        public void ThrowIfNull_Sequence_Guard()
        {
            Assert.ThrowsExactly<ArgumentNullException>(() => _aaa.ThrowIfSequenceIsNull(x => x.Name));
        }

        [TestMethod]
        public void ThrowIfDefault_Guard()
        {
            Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => _b.ThrowIfDefault(x => x.Age));
        }

        [TestMethod]
        public void ThrowIfDefault_Sequence()
        {
            Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => _bb.ThrowIfSequenceIsDefault());
        }

        [TestMethod]
        public void ThrowIfDefault_Sequence_Guard()
        {
            Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => _bbb.ThrowIfSequenceIsDefault(x => x.Age));
        }

        [TestMethod]
        public void ThrowIfNullOrEmpty_Sequence()
        {
            var ss = new string[] { string.Empty };
            Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => ss.ThrowIfSequenceIsNullOrEmpty());
        }

        [TestMethod]
        public void ThrowIfNullOrEmpty_Sequence_Guard()
        {
            Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => _aaa.ThrowIfSequenceIsNullOrEmpty(x => x.Name));
        }                                                                                       

        [TestMethod]
        public void ThrowIf()
        {
            Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => _b.ThrowIf(x => x.Age <= 0));
        }

        [TestMethod]
        public async Task ThrowIfAsync1()
        {
            await Assert.ThrowsExactlyAsync<ArgumentOutOfRangeException>(() =>
                Task.FromResult(_b).ThrowIfAsync(x => x.Age <= 0)
            );
        }

        [TestMethod]
        public async Task ThrowIfAsync2()
        {
            await Assert.ThrowsExactlyAsync<ArgumentOutOfRangeException>(() =>
                Task.FromResult(_b).ThrowIfAsync(x => Task.FromResult(x.Age <= 0))
            );
        }

        [TestMethod]
        public async Task ThrowIfAsync3()
        {
            await Assert.ThrowsExactlyAsync<ArgumentOutOfRangeException>(() =>
                _b.ThrowIfAsync(x => Task.FromResult(x.Age <= 0))
            );
        }

        [TestMethod]
        public void ThrowIf_Sequence()
        {
            Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => _aaa.ThrowIfSequence(x => x.Name is null));
        }
    }
}
