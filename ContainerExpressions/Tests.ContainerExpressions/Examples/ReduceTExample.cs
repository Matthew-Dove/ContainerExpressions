using ContainerExpressions.Expressions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace Tests.ContainerExpressions.Examples
{
    [TestClass]
    public class ReduceTExample
    {
        /// <summary>ttps://github.com/Matthew-Dove/ContainerExpressions#reducet</summary>
        public void ReduceT_Example()
        {
            Func<string, string, string> combine = (x, y) => string.Concat(x, " ", y);

            var words = new string[] { "world" };
            var arg1 = "hello";

            var sentence = Expression.Reduce(arg1, words, combine);

            Assert.AreEqual("hello world", sentence);
        }
    }
}
