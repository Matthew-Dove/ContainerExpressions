using ContainerExpressions.Containers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace Tests.ContainerExpressions.Examples
{
    [TestClass]
    public class NotNullExample
    {
        private const string USER_NAME = "John Smith";

        private class UserService
        {
            public string GetUsername(string userId) => USER_NAME;
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void NotNull_Null_Example()
        {
            string userId = null;

            var username = GetUserName(userId);
        }

        [TestMethod]
        public void NotNull_Reference_Example()
        {
            string userId = Guid.NewGuid().ToString();

            var username = GetUserName(userId);

            Assert.AreEqual(USER_NAME, username);
        }

        private static string GetUserName(NotNull<string> userId)
        {
            var service = new UserService();

            var username = service.GetUsername(userId);

            return username;
        }
    }
}
