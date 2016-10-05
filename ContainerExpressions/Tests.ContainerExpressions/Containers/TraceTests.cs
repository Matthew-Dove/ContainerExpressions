using ContainerExpressions.Containers;
using ContainerExpressions.Expressions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;

namespace Tests.ContainerExpressions.Containers
{
    [TestClass]
    public class TraceTests
    {
        private List<string> _messages = null;

        [TestInitialize]
        public void Initialize()
        {
            _messages = new List<string>();
            Trace.SetLogger(message => _messages.Add(message));
        }

        #region Response

        [TestMethod]
        public void ValidResponse_LogsMessage()
        {
            var success = "Hello World!";

            var response = new Response(true).Log(success);

            Assert.AreEqual(1, _messages.Count);
            Assert.AreEqual(success, _messages[0]);
        }

        [TestMethod]
        public void InValidResponse_DoesNotLogMessage()
        {
            var success = "Hello World!";

            var response = new Response(false).Log(success);

            Assert.AreEqual(0, _messages.Count);
        }

        [TestMethod]
        public void ValidResponse_LogsSuccessMessage()
        {
            string success = "Much Success", fail = "No Dice";

            var response = new Response(true).Log(success, fail);

            Assert.AreEqual(1, _messages.Count);
            Assert.AreEqual(success, _messages[0]);
        }

        [TestMethod]
        public void InValidResponse_LogsFailMessage()
        {
            string success = "Much Success", fail = "No Dice";

            var response = new Response(false).Log(success, fail);

            Assert.AreEqual(1, _messages.Count);
            Assert.AreEqual(fail, _messages[0]);
        }

        #endregion

        #region ResponseT

        [TestMethod]
        public void ValidResponseT_LogsMessage()
        {
            var success = "Hello World!";

            var response = new Response<int>(42).Log(success);

            Assert.AreEqual(1, _messages.Count);
            Assert.AreEqual(success, _messages[0]);
        }

        [TestMethod]
        public void InValidResponseT_DoesNotLogMessage()
        {
            var success = "Hello World!";

            var response = new Response<int>().Log(success);

            Assert.AreEqual(0, _messages.Count);
        }

        [TestMethod]
        public void ValidResponseT_LogsSuccessMessage()
        {
            string success = "Much Success", fail = "No Dice";

            var response = new Response<int>(42).Log(success, fail);

            Assert.AreEqual(1, _messages.Count);
            Assert.AreEqual(success, _messages[0]);
        }

        [TestMethod]
        public void InValidResponseT_LogsFailMessage()
        {
            string success = "Much Success", fail = "No Dice";

            var response = new Response<int>().Log(success, fail);

            Assert.AreEqual(1, _messages.Count);
            Assert.AreEqual(fail, _messages[0]);
        }

        [TestMethod]
        public void ValidResponseT_LogsSuccessFunc()
        {
            var answer = 42;
            Func<int, string> success = x => string.Format("Hello World! * {0}", x);

            var response = new Response<int>(answer).Log(success);

            Assert.AreEqual(1, _messages.Count);
            Assert.AreEqual(success(answer), _messages[0]);
        }

        [TestMethod]
        public void InValidResponseT_DoesNotLogSuccessFunc()
        {
            Func<int, string> success = x => string.Format("Hello World! * {0}", x);

            var response = new Response<int>().Log(success);

            Assert.AreEqual(0, _messages.Count);
        }

        [TestMethod]
        public void ValidResponseT_LogsSuccessFuncNotFailMessage()
        {
            var answer = 42;
            Func<int, string> success = x => string.Format("Hello World! * {0}", x);
            var fail = "Goodbye Cruel World!";

            var response = new Response<int>(answer).Log(success, fail);

            Assert.AreEqual(1, _messages.Count);
            Assert.AreEqual(success(answer), _messages[0]);
        }

        [TestMethod]
        public void InValidResponseT_LogsFailMessageNotSuccessFunc()
        {
            Func<int, string> success = x => string.Format("Hello World! * {0}", x);
            var fail = "Goodbye Cruel World!";

            var response = new Response<int>().Log(success, fail);

            Assert.AreEqual(1, _messages.Count);
            Assert.AreEqual(fail, _messages[0]);
        }

        #endregion
    }
}
