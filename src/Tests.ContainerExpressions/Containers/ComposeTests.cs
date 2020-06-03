using ContainerExpressions.Containers;
using ContainerExpressions.Expressions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Threading.Tasks;

namespace Tests.ContainerExpressions.Containers
{
    [TestClass]
    public class ComposeTests
    {
        [TestMethod]
        public void Compose_Synchronous_ResponseT_2Funcs()
        {
            var answer = 42;
            Func<string, int> parse = int.Parse;

            var result = Expression.Compose(answer.ToString().Lift(), parse.Lift());

            Assert.AreEqual(answer, result);
        }

        [TestMethod]
        public void Compose_Synchronous_ResponseT_MinFuncs()
        {
            var answer = 42;
            Func<string, int> parse = int.Parse;

            var result = Expression.Compose(answer.ToString().Lift(), parse.Lift());

            Assert.AreEqual(answer, result);
        }

        [TestMethod]
        public void Compose_Synchronous_ResponseT_MaxFuncs()
        {
            var answer = 42.ToResponse();
            Func<string, int> parse = int.Parse;
            Func<int, Response<int>> pass = x => x.ToResponse();

            var result = Expression.Compose(answer.Lift(), pass, pass, pass, pass, pass, pass, pass.Transform(x => x.ToString()), parse.Lift());

            Assert.AreEqual(answer, result);
        }

        [TestMethod]
        public void Compose_Synchronous_Response_MinFuncs()
        {
            var answer = 42;
            Func<int, Response> compare = x => new Response(x == answer);

            var result = Expression.Compose(42.Lift().Transform(x => x), compare);

            Assert.IsTrue(result);
        }

        [TestMethod]
        public void Compose_Synchronous_Response_MaxFuncs()
        {
            var answer = 42;
            Func<int, Response> compare = x => new Response(x == answer);
            Func<int, Response<int>> pass = x => x.ToResponse();

            var result = Expression.Compose(answer.Lift(), pass, pass, pass, pass, pass, pass, pass, compare);

            Assert.IsTrue(result);
        }

        [TestMethod]
        public async Task Compose_Asynchronous_ResponseT_MinFuncs()
        {
            var answer = 42;
            Func<string, Task<int>> parse = x => Task.FromResult(int.Parse(x));

            var result = await Expression.ComposeAsync(answer.LiftAsync().TransformAsync(x => x.ToString()), parse.LiftAsync());

            Assert.AreEqual(answer, result);
        }

        [TestMethod]
        public async Task Compose_Asynchronous_ResponseT_MaxFuncs()
        {
            var answer = 42;
            Func<string, Task<int>> parse = x => Task.FromResult(int.Parse(x));
            Func<int, Task<Response<int>>> pass = x => Task.FromResult(x.ToResponse());

            var result = await Expression.ComposeAsync(answer.LiftAsync(), pass, pass, pass, pass, pass, pass, pass.TransformAsync(x => x.ToString()), parse.LiftAsync());

            Assert.AreEqual(answer, result);
        }

        [TestMethod]
        public async Task Compose_Asynchronous_Response_MinFuncs()
        {
            var answer = 42;
            Func<int, Task<Response>> compare = x => Task.FromResult(new Response(x == answer));

            var result = await Expression.ComposeAsync(42.LiftAsync().TransformAsync(x => x), compare);

            Assert.IsTrue(result);
        }

        [TestMethod]
        public async Task Compose_Asynchronous_Response_MaxFuncs()
        {
            var answer = 42;
            Func<int, Task<Response>> compare = x => Task.FromResult(new Response(x == answer));
            Func<int, Task<Response<int>>> pass = x => Task.FromResult(x.ToResponse());

            var result = await Expression.ComposeAsync(answer.LiftAsync(), pass, pass, pass, pass, pass, pass, pass, compare);

            Assert.IsTrue(result);
        }
    }
}
