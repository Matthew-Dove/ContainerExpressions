using ContainerExpressions.Containers;
using ContainerExpressions.Expressions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace Tests.ContainerExpressions.Examples
{
    [TestClass]
    public class ReduceTExample
    {
        /// <summary>https://github.com/Matthew-Dove/ContainerExpressions#reducet</summary>
        public void ReduceT_Example()
        {
            Func<string, string, string> combine = (x, y) => string.Concat(x, " ", y);

            var words = new Response<string>[] { Response.Create("world") };
            var arg1 = "hello";

            string sentence = Expression.Reduce(combine, arg1, words);

            Assert.AreEqual("hello world", sentence);
        }
    }
}
