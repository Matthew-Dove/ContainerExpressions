using ContainerExpressions.Containers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;

namespace Tests.ContainerExpressions.Examples
{
    [TestClass]
    public class FireAndForgetExample
    {
        /// <summary>https://github.com/Matthew-Dove/ContainerExpressions#fire-and-forget</summary>
        [TestMethod]
        public async Task FireAndForget_Example()
        {
            // Push several tasks to "FireAndForget".
            FireAndForget.Push(Task.FromResult(42));
            FireAndForget.Push(Task.Delay(0));
            FireAndForget.Push(Task.CompletedTask);

            // If the tasks are very quick, they will finish; and remove themselves before you can wait for them.
            // Longer running tasks have a higher chance of still existing, and then needing to be waited on.
            var completedTasks = await FireAndForget.WhenAll();

            Assert.IsTrue(completedTasks >= 0 && completedTasks <= 3);
        }
    }
}
