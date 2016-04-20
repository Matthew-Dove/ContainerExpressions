using ContainerExpressions.Containers;
using ContainerExpressions.Expressions.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace Tests.ContainerExpressions.Containers
{
    [TestClass]
    public class RetryTests
    {
        [TestMethod]
        public void DoesNotRetry_WhenValid()
        {
            var attempts = 0;
            Func<Response> func = () => { attempts++;  return new Response(true); };

            var response = Retry.Execute(func);

            Assert.IsTrue(response);
            Assert.AreEqual(1, attempts);
        }

        [TestMethod]
        public void RetryThreeTimes_ThenValid()
        {
            var attempts = 0;
            const int MAX_ATTEMPTS = 2;
            Func<Response> func = () => new Response(attempts++ == MAX_ATTEMPTS);

            var response = Retry.Execute(func, RetryOptions.Create(MAX_ATTEMPTS, 0));

            Assert.IsTrue(response);
            Assert.AreEqual(MAX_ATTEMPTS + 1, attempts);
        }
    }
}
