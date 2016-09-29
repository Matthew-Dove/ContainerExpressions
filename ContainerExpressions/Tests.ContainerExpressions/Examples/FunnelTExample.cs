using ContainerExpressions.Containers;
using ContainerExpressions.Expressions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace Tests.ContainerExpressions.Examples
{
    [TestClass]
    public class FunnelTExample
    {
        /// <summary>https://github.com/Matthew-Dove/ContainerExpressions#funnelt</summary>
        [TestMethod]
        public void FunnelT_Example()
        {
            Func<double, double, Response<double>> divide = (dividend, divisor) => divisor == 0 ? new Response<double>() : Response.Create(dividend / divisor);

            var e = divide(150D, 55D);
            var pi = divide(22D, 7D);

            var answer = Expression.Funnel(e, pi, Math.Pow);

            Assert.IsTrue(answer);
            Assert.AreEqual(Math.Pow(e, pi), answer);
        }
    }
}
