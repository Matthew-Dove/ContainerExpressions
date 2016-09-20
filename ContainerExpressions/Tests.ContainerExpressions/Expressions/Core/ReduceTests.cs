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

            var numbers = new int[] { 1, 3, 3, 7 };
            var firstNumber = 0;

            var total = Expression.Reduce(firstNumber, numbers, sum);

            Assert.AreEqual(numbers.Sum(), total);
        }
    }
}
