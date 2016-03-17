using Microsoft.VisualStudio.TestTools.UnitTesting;
using ContainerExpressions.Containers;
using System;

namespace Tests.ContainerExpressions.Containters
{
    [TestClass]
    public class LaterTests
    {
        [TestMethod]
        public void GeneratingFunction_Invoked_OnlyOnce()
        {
            var expectedTimesInvoked = 1;
            var timesGeneratingFunctionInvoked = 0;
            var later = Later.Create(() => { timesGeneratingFunctionInvoked++; return 42; });

            var attemptOne = later.Value; // Value is initialized here.
            var attemptTwo = later.Value; // Value is simply returned here (without calling the generating function again).

            Assert.AreEqual(expectedTimesInvoked, timesGeneratingFunctionInvoked);
            Assert.AreEqual(attemptOne, attemptTwo); // Not a necessary condition to test the numner of times invoked, but I don't want the second attempt optimised away, as that would force the test to always pass.
        }

        [TestMethod]
        public void SameValue_Is_AlwaysReturned()
        {
            var later = Later.Create(() => Guid.NewGuid());

            var attemptOne = later.Value;
            var attemptTwo = later.Value;

            Assert.AreEqual(attemptOne, attemptTwo); // If guids are different, we have a problem.
        }

        [TestMethod]
        public void Later_Converts_ToT()
        {
            var returnValue = 42;
            var later = Later.Create(() => returnValue);

            int value = later; // Implicit conversion to T.

            Assert.AreEqual(returnValue, value);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void NullGeneratingFunction_ThrowsError()
        {
            Later.Create<object>(null);
        }
    }
}
