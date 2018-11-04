using ContainerExpressions.Containers;
using ContainerExpressions.Expressions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;

namespace Tests.ContainerExpressions.Expressions.Core
{
    [TestClass]
    public class ReduceTests
    {
        [TestMethod]
        public void CanReduceInts_JustLikeSum()
        {
            Func<int, int, int> sum = (x, y) => x + y;

            var numbers = new Response<int>[] { Response.Create(1), Response.Create(3), Response.Create(3), Response.Create(7) };
            var firstNumber = 0;

            var total = Expression.Reduce(sum, firstNumber, numbers);

            Assert.AreEqual(numbers.Select(x => x.Value).Sum(), total);
        }
    }
}
