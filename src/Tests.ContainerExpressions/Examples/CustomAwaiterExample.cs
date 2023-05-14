using ContainerExpressions.Containers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;

namespace Tests.ContainerExpressions.Examples
{
    [TestClass]
    public class CustomAwaiterExample
    {
        /// <summary>https://github.com/Matthew-Dove/ContainerExpressions#custom-awaiters</summary>
        [TestMethod]
        public async Task Response_Awaiter_Example()
        {
            int number = 100;
            Response<int> input = Response.Create(number);

            Task<Response<int>> task = Task.FromResult(input); // Normal: Task<Response<T>>
            Response<Task<Response<int>>> response = Response.Create(task); // Target: Response<Task<Response<T>>>

            Response<int> result = await response; // This is a "safe" Response, it will be invalid when the Task is canceled, or faulted - instead of throwing exceptions.

            Assert.IsTrue(result);
            Assert.AreEqual(number, result);
        }
    }
}
