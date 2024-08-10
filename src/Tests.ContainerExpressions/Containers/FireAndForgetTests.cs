using ContainerExpressions.Containers;
using ContainerExpressions.Expressions.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Tests.ContainerExpressions.Containers
{
    [TestClass]
    public class FireAndForgetTests
    {
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Push_TaskIsNull()
        {
            Task task = null;

            FireAndForget.Push(task);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Push_ManyTasksIsNull()
        {
            Task[] tasks = null;

            FireAndForget.Push(tasks);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Push_ManyTasksOneIsNull()
        {
            Task[] tasks = new Task[] { Task.CompletedTask, null, Task.CompletedTask };

            FireAndForget.Push(tasks);
        }

        [TestMethod]
        public async Task Push_TaskFromResult()
        {
            var task = Task.FromResult(42);

            FireAndForget.Push(task);
            var completed = await FireAndForget.WhenAll();

            Assert.IsTrue(completed.Between(0, 1));
        }

        [TestMethod]
        public async Task Push_TaskFromException()
        {
            var task = Task.FromException(new Exception("Error!"));

            FireAndForget.Push(task);
            var completed = await FireAndForget.WhenAll();

            Assert.IsTrue(completed.Between(0, 1));
        }

        [TestMethod]
        public async Task Push_TaskFromCanceled()
        {
            var task = Task.FromCanceled(new CancellationToken(true));

            FireAndForget.Push(task);
            var completed = await FireAndForget.WhenAll();

            Assert.IsTrue(completed.Between(0, 1));
        }

        [TestMethod]
        public async Task Push_CancellationToken()
        {
            using var cts1 = new CancellationTokenSource(0); // Expected to not be picked up by WhenAll(), but "could" depending on timing.
            using var cts2 = new CancellationTokenSource(1); // Likey to still be there for the WhenAll() call, but has a pretty high chance of already being processed.
            var task1 = new Task(() => { }, cts1.Token);
            var task2 = new Task(() => { }, cts2.Token);

            FireAndForget.Push(task1);
            FireAndForget.Push(task2);
            var completed = await FireAndForget.WhenAll();

            Assert.IsTrue(completed.Between(0, 2)); // Most likey to equal 1 (closer to 50%, than 100% of the time).
        }

        [TestMethod]
        public async Task Push_TaskFromResult_TaskFromException_TaskFromCanceled()
        {
            var fromResult = Task.FromResult(42); // Likey to still be there for the WhenAll().
            var fromException = Task.FromException(new Exception("Error!")); // Likey to still be there for the WhenAll().
            var fromCanceled = Task.FromCanceled(new CancellationToken(true)); // Expected to not be picked up by WhenAll().

            FireAndForget.Push(fromResult);
            FireAndForget.Push(fromException);
            FireAndForget.Push(fromCanceled);
            var completed = await FireAndForget.WhenAll();

            Assert.IsTrue(completed.Between(0, 3)); // Most likey to equal 2 (close to 100% of the time).
        }

        [TestMethod]
        public async Task Push_AwaitManyTasks()
        {
            Task task1 = Task.Delay(10), task2 = Task.Delay(11), task3 = Task.Delay(12);

            FireAndForget.Push(task1);
            FireAndForget.Push(task2);
            FireAndForget.Push(task3);
            var completed = await FireAndForget.WhenAll();

            // Going a bit crazy on the types of asserts, to test out the different kinds of equality checkers.
            Assert.IsTrue(3.Equals(completed));
            Assert.IsTrue(completed.Equals(3));
            Assert.IsTrue(3 == completed);
            Assert.AreEqual(3, completed);
            Assert.AreEqual<int>(3, completed);
            Assert.AreEqual<CompletedTasks>(3, completed);
        }

        [TestMethod]
        public async Task Push_AwaitZeroTasks()
        {
            var completed = await FireAndForget.WhenAll();

            Assert.IsTrue(completed == 0);
        }

        [TestMethod]
        public async Task Push_ContinueAddingBackgroundTasks()
        {
            using var cts = new CancellationTokenSource();
            var token = cts.Token;

            // This keeps adding new background jobs, while a slice is being awaited.
            var task = Task.Run(async () => {
                while (!token.IsCancellationRequested)
                {
                    FireAndForget.Push(Task.Delay(111));
                    await Task.Delay(1);
                }
            }, token);

            FireAndForget.Push(Task.Delay(100));
            var completed = await FireAndForget.WhenAll(); // A "slice" of the current pushed tasks are awaited here (more are being added in the background).
            cts.Cancel();

            // There are now more jobs here, as new ones were being added while waiting for the first WhenAll() to finish.
            var completedBackgroundJobs = await FireAndForget.WhenAll();

            // All the background jobs that were added while the first task ran, should all be cleaned up by the second WhenAll() call above.
            var zeroNewJobs = await FireAndForget.WhenAll(); // Expecting no more jobs to be found.

            Assert.AreEqual(0, zeroNewJobs);
        }

        [TestMethod]
        public async Task Push_ManyTasks()
        {
            var tasks = new Task[]
            {
                Task.CompletedTask, Task.CompletedTask, Task.CompletedTask
            };

            FireAndForget.Push(tasks);
            var completed = await FireAndForget.WhenAll();

            Assert.IsTrue(completed.Between(0, 3));
        }
    }

    file static class FireAndForgetTestsExtensions
    {
        public static bool Between(this CompletedTasks completed, int min, int max) => completed >= min && completed <= max;
    }
}
