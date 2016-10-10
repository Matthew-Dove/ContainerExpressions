using ContainerExpressions.Containers;
using ContainerExpressions.Expressions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

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

        [TestMethod]
        public void TraceWillComposeWithOtherFunctions()
        {
            var traceSteps = 0;
            Func<Response<int>> identity = () => Response.Create(0);
            Func<int, Response<int>> increment = x => Response.Create(x + 1);
            Func<int, string> counter = x => { traceSteps++; return string.Format("The value of the int is {0}.", x); };

            var count = Expression.Compose(identity.Log(counter), increment.Log(counter), increment.Log(counter));

            Assert.IsTrue(count);
            Assert.AreEqual(traceSteps, _messages.Count);
            for (var i = 0; i < _messages.Count; i++)
            {
                Assert.AreEqual(counter(i), _messages[i]);
            }
        }

        [TestMethod]
        public void TraceWillComposeWithStringMessages()
        {
            string success = "All good", fail = "Woops";
            Func<Response<int>> identity = () => Response.Create(0);
            Func<int, Response<int>> increment = x => Response.Create(x + 1);
            Func<int, Response<int>> stop = x => new Response<int>();

            var count = Expression.Compose(identity.Log(success, fail), increment.Log(success, fail), stop.Log(success, fail), increment.Log(success, fail)); // The last increment won't be invoked.

            Assert.IsFalse(count);
            Assert.AreEqual(2, _messages.FindAll(x => x == success).Count);
            Assert.AreEqual(1, _messages.FindAll(x => x == fail).Count);
        }

        [TestMethod]
        public void TraceWithInputAndOutput()
        {
            Func<int, int, string> trace = (input, output) => $"Input: {input}, Output: {output}.";
            Func<Response<int>> identity = () => Response.Create(0);
            Func<int, Response<int>> increment = x => Response.Create(x + 1);

            var count = Expression.Compose(identity, increment.Log(trace), increment.Log(trace), increment.Log(trace));

            Assert.AreEqual(3, _messages.Count);
            Assert.AreEqual(trace(0, 1), _messages[0]);
        }

        #endregion

        #region Async

        [TestMethod]
        public async Task ValidResponseTask_SuccesIsLogged()
        {
            var success = "All good";
            var response = await Task.FromResult(new Response(true)).LogAsync(success);

            Assert.IsTrue(response);
            Assert.AreEqual(success, _messages[0]);
        }

        [TestMethod]
        public async Task ChainOfTasksAreLogged()
        {
            string success = "All good";
            Func<Task<Response<int>>> identityAsync = () => Task.FromResult(Response.Create(0));
            Func<int, Task<Response<int>>> incrementAsync = x => Task.FromResult(Response.Create(x + 1));

            var count = await Expression.ComposeAsync(identityAsync.LogAsync(success), incrementAsync.LogAsync(success), incrementAsync.LogAsync(success));

            Assert.IsTrue(count);
            Assert.AreEqual(2, count);
            Assert.AreEqual(3, _messages.Count);
            Assert.AreEqual(3, _messages.FindAll(x => x == success).Count);
        }

        #endregion
    }
}
