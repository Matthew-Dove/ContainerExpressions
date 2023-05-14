using ContainerExpressions.Containers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;

namespace Tests.ContainerExpressions.Examples
{
    [TestClass]
    public class ResponseValueTaskExample
    {
        /// <summary>https://github.com/Matthew-Dove/ContainerExpressions#responsetaskt</summary>
        [TestMethod]
        public async Task Task_To_ResponseTask_To_Response()
        {
            ResponseValueTask<int> successResponse = Divide(1, 1);
            Response<int> success = await successResponse; // Response<int> success = Divide(1, 1); is not possible to cast; so we use ResponseTask<int>.

            ResponseValueTask<int> errorResponse = Divide(1, 0); // This would normally be a runtime exception.
            Response<int> error = await errorResponse;

            Assert.IsTrue(success);
            Assert.AreEqual(1, success);
            Assert.IsFalse(error);
        }

        private static async Task<int> Divide(int numerator, int denominator)
        {
            var quotient = numerator / denominator;
            await Task.Delay(1);
            return quotient;
        }
    }
}
