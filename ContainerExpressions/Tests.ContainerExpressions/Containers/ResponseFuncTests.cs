using ContainerExpressions.Containers;
using ContainerExpressions.Expressions.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Tests.ContainerExpressions.Containers
{
    [TestClass]
    public class ResponseFuncTests
    {
        private const int RETRY_ATTEMPS = 2;
        private int _retryAttemps = 0;

        [TestMethod]
        public void Retry()
        {
            var response = Response.Create(Divide).Retry(RetryOptions.Create(RETRY_ATTEMPS, 0));

            Assert.IsFalse(response);
            Assert.AreEqual(RETRY_ATTEMPS + 1, _retryAttemps);
        }

        private Response<int> Divide()
        {
            _retryAttemps++;
            return Try.Run(() => Divide(1, 0));
        }

        private static int Divide(int dividend, int divisor) => dividend / divisor;
    }
}
