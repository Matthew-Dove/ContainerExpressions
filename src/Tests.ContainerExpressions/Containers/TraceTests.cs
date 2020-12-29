using ContainerExpressions.Containers;
using ContainerExpressions.Expressions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
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

        #region T

        [TestMethod]
        public void T_LogsMessage()
        {
            var message = "story";

            var response = true.Log(message);

            Assert.AreEqual(true, response);
            Assert.AreEqual(1, _messages.Count);
            Assert.AreEqual(message, _messages[0]);
        }

        [TestMethod]
        public void TFunc_LogsMessage()
        {
            var response = "Hello".Log(x => $"{x} World!");

            Assert.AreEqual("Hello", response);
            Assert.AreEqual(1, _messages.Count);
            Assert.AreEqual("Hello World!", _messages[0]);
        }

        #endregion

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

        #region Maybe

        [TestMethod]
        public void Maybe_ValueError_Func_Valid()
        {
            var input = Guid.NewGuid();
            Func<Guid, string> trace = x => x.ToString();

            var maybe = new Maybe<Guid, string>(input).Log(trace, x => string.Empty);

            Assert.AreEqual(input, Guid.Parse(_messages.Single()));
        }

        [TestMethod]
        public void Maybe_ValueError_Func_NotValid()
        {
            var input = Guid.NewGuid();
            Func<Guid, string> trace = x => x.ToString();

            var maybe = new Maybe<string, Guid>(input).Log(x => string.Empty, trace);

            Assert.AreEqual(input, Guid.Parse(_messages.Single()));
        }

        [TestMethod]
        public void Maybe_ValueError_Const_Valid()
        {
            var maybe = new Maybe<string, int>("Input").Log("Value", "Error");

            Assert.AreEqual("Value", _messages.Single());
        }

        [TestMethod]
        public void Maybe_ValueError_Const_NotValid()
        {
            var maybe = new Maybe<int, string>("Input").Log("Value", "Error");

            Assert.AreEqual("Error", _messages.Single());
        }

        [TestMethod]
        public void Maybe_Value_Func_Valid()
        {
            var input = Guid.NewGuid();
            Func<Guid, string> trace = x => x.ToString();

            var maybe = new Maybe<Guid>(input).Log(trace, x => string.Empty);

            Assert.AreEqual(input, Guid.Parse(_messages.Single()));
        }

        [TestMethod]
        public void Maybe_Value_Func_NotValid()
        {
            var msg = "error";
            var input = new Exception(msg);
            Func<Exception, string> trace = x => x.Message;

            var maybe = new Maybe<Guid>(input).Log(x => string.Empty, trace);

            Assert.AreEqual(msg, _messages.Single());
        }

        [TestMethod]
        public void Maybe_Value_Const_Valid()
        {
            var maybe = new Maybe<string>("Input").Log("Value", "Error");

            Assert.AreEqual("Value", _messages.Single());
        }

        [TestMethod]
        public void Maybe_Value_Const_NotValid()
        {
            var maybe = new Maybe<string>(new Exception("Input")).Log("Value", "Error");

            Assert.AreEqual("Error", _messages.Single());
        }

        #endregion

        #region Async

        [TestMethod]
        public async Task Async_T_LogsMessage()
        {
            var message = "story";

            var response = await Task.FromResult(true).LogAsync(message);

            Assert.AreEqual(true, response);
            Assert.AreEqual(1, _messages.Count);
            Assert.AreEqual(message, _messages[0]);
        }

        [TestMethod]
        public async Task Async_TFunc_LogsMessage()
        {
            var response = await Task.FromResult("Hello").LogAsync(x => $"{x} World!");

            Assert.AreEqual("Hello", response);
            Assert.AreEqual(1, _messages.Count);
            Assert.AreEqual("Hello World!", _messages[0]);
        }

        [TestMethod]
        public async Task Async_ValidResponseTask_SuccesIsLogged()
        {
            var success = "All good";
            var response = await Task.FromResult(new Response(true)).LogAsync(success);

            Assert.IsTrue(response);
            Assert.AreEqual(success, _messages[0]);
        }

        [TestMethod]
        public async Task Async_ChainOfTasksAreLogged()
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

        [TestMethod]
        public async Task Async_Maybe_ValueError_Func_Valid()
        {
            var input = Guid.NewGuid();
            Func<Guid, string> trace = x => x.ToString();

            var maybe = await Task.FromResult(new Maybe<Guid, string>(input)).LogAsync(trace, x => string.Empty);

            Assert.AreEqual(input, Guid.Parse(_messages.Single()));
        }

        [TestMethod]
        public async Task Async_Maybe_ValueError_Func_NotValid()
        {
            var input = Guid.NewGuid();
            Func<Guid, string> trace = x => x.ToString();

            var maybe = await Task.FromResult(new Maybe<string, Guid>(input)).LogAsync(x => string.Empty, trace);

            Assert.AreEqual(input, Guid.Parse(_messages.Single()));
        }

        [TestMethod]
        public async Task Async_Maybe_Value_Func_Valid()
        {
            var input = Guid.NewGuid();
            Func<Guid, string> trace = x => x.ToString();

            var maybe = await Task.FromResult(new Maybe<Guid>(input)).LogAsync(trace, x => string.Empty);

            Assert.AreEqual(input, Guid.Parse(_messages.Single()));
        }

        [TestMethod]
        public async Task Async_Maybe_Value_Func_NotValid()
        {
            var msg = "error";
            var input = new Exception(msg);
            Func<Exception, string> trace = x => x.Message;

            var maybe = await Task.FromResult(new Maybe<Guid>(input)).LogAsync(x => string.Empty, trace);

            Assert.AreEqual(msg, _messages.Single());
        }

        #endregion
    }
}
