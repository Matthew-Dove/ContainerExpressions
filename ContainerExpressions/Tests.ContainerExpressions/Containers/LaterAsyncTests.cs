using ContainerExpressions.Containers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Tests.ContainerExpressions.Containers
{
    [TestClass]
    public class LaterAsyncTests
    {
        [TestMethod]
        public async Task ThereIsNoRaceCondition_WhenGeneratingAValue_OnMoreThanOneThreadAtATime()
        {
            var timesInvoked = 0;
            var padLock = new object(); 
            var later = Later.CreateAsync(async () => {
                await Task.Delay(100); // Give the threads a little bit of time to get into a possible race condition.
                lock (padLock)
                {
                    timesInvoked++;
                }
                return 42;
            });

            var threads = new Thread[Environment.ProcessorCount];
            for (int i = 0; i < threads.Length; i++)
            {
                threads[i] = new Thread(() => { var value = later.Value; });
                threads[i].Start();
            }
            var result = later.Value;
            for (int j = 0; j < threads.Length; j++)
            {
                threads[j].Join();
            }
            await result;

            Assert.AreEqual(1, timesInvoked);
        }

        [TestMethod]
        public async Task GeneratingFunction_Invoked_OnlyOnce()
        {
            var expectedTimesInvoked = 1;
            var timesGeneratingFunctionInvoked = 0;
            var later = Later.CreateAsync(() => { timesGeneratingFunctionInvoked++; return Task.FromResult(42); });

            var attemptOne = await later.Value; // Value is initialized here.
            var attemptTwo = await later.Value; // Value is simply returned here (without calling the generating function again).

            Assert.AreEqual(expectedTimesInvoked, timesGeneratingFunctionInvoked);
            Assert.AreEqual(attemptOne, attemptTwo); // Not a necessary condition to test the numner of times invoked, but I don't want the second attempt optimised away, as that would force the test to always pass.
        }

        [TestMethod]
        public async Task SameValue_Is_AlwaysReturned()
        {
            var later = Later.CreateAsync(() => Task.FromResult(Guid.NewGuid()));

            var attemptOne = await later.Value;
            var attemptTwo = await later.Value;

            Assert.AreEqual(attemptOne, attemptTwo); // If guids are different, we have a problem.
        }

        [TestMethod]
        public async Task Later_Converts_ToT()
        {
            var returnValue = 42;
            var later = Later.CreateAsync(() => Task.FromResult(returnValue));

            Task<int> value = later; // Implicit conversion to T.
            var result = await value;

            Assert.AreEqual(returnValue, result);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void NullGeneratingFunction_ThrowsError()
        {
            Later.CreateAsync<object>(null);
        }
    }
}
