using ContainerExpressions.Containers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Threading.Tasks;

namespace Tests.ContainerExpressions.Examples
{
    [TestClass]
    public class ResponseAsyncTExample
    {
        private const int _theAnswer = 42;
        private const string _theQuestion = "What is the question?";

        /// <summary>https://github.com/Matthew-Dove/ContainerExpressions#responseasynct</summary>
        [TestMethod]
        public async Task ResponseAsync_Captures_Exceptions()
        {
            var errorMessage = string.Empty;
            Try.SetExceptionLogger(ex => errorMessage = ex.Message);

            var value = await GetValue();
            var error = await GetError();

            Assert.IsTrue(value);
            Assert.AreEqual(_theAnswer, value);
            Assert.IsFalse(error);
            Assert.AreEqual(_theQuestion, errorMessage);
        }

        private static async ResponseAsync<int> GetValue()
        {
            return await Task.FromResult(_theAnswer);
        }

        private static async ResponseAsync<int> GetError()
        {
            await Task.CompletedTask;
            throw new Exception(_theQuestion);
        }
    }
}
