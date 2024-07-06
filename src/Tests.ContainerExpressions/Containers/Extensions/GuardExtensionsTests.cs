﻿using ContainerExpressions.Containers.Extensions;
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
        [TestMethod]
        [ExpectedException(typeof(Exception))]
        public void ThrowError_Exception()
        {
            var ex = new Exception("error");

            ex.ThrowError();
        }

        [TestMethod]
        [ExpectedException(typeof(Exception))]
        public void ThrowError_ExceptionDispatchInfo()
        {
            var ex = new Exception("error");
            var di = ExceptionDispatchInfo.Capture(ex);

            di.ThrowDispatchError();
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ThrowIfNull()
        {
            object obj = null;

            obj.ThrowIfNull();
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void ThrowIfDefault()
        {
            0.ThrowIfDefault();
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void ThrowIfNullOrEmpty_String()
        {
            var @string = string.Empty;

            @string.ThrowIfNullOrEmpty();
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void ThrowIfNullOrEmpty_Array()
        {
            var array = new int[] { };

            array.ThrowIfNullOrEmpty();
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void ThrowIfNullOrEmpty_IEnumerable()
        {
            var enumerable = Enumerable.Empty<int>();

            enumerable.ThrowIfNullOrEmpty();
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void ThrowIfNullOrEmpty_List()
        {
            var list = new List<int>();

            list.ThrowIfNullOrEmpty();
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void ThrowIfLessThan()
        {
            int age = 1, adult = 18;

            age.ThrowIfLessThan(adult);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void ThrowIfGreaterThan()
        {
            int age = 18, child = 17;

            age.ThrowIfGreaterThan(child);
        }

        [TestMethod]
        [ExpectedException(typeof(Exception))]
        public void ThrowIfFaultedOrCanceled_Faulted()
        {
            var task = Task.FromException<int>(new Exception("error"));

            task.ThrowIfFaultedOrCanceled();
        }

        [TestMethod]
        [ExpectedException(typeof(AggregateException))]
        public void ThrowIfFaultedOrCanceled_ManyFaulted()
        {
            var task1 = Task.FromException<int>(new Exception("error1"));
            var task2 = Task.FromException<int>(new Exception("error2"));

            var task = Task.WhenAll(task1, task2);

            task.ThrowIfFaultedOrCanceled();
        }

        [TestMethod]
        [ExpectedException(typeof(TaskCanceledException))]
        public void ThrowIfFaultedOrCanceled_Canceled()
        {
            var task = Task.FromCanceled(new CancellationToken(true));

            task.ThrowIfFaultedOrCanceled();
        }
    }
}
