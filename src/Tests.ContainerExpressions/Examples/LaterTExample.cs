using ContainerExpressions.Containers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Security.Principal;
using System.Threading;

namespace Tests.ContainerExpressions.Examples
{
    [TestClass]
    public class LaterTExample
    {
        private const string USER_NAME = "Bob";

        /// <summary>https://github.com/Matthew-Dove/ContainerExpressions#latert</summary>
        [TestMethod]
        public void LaterT_Example()
        {
            IUserService userService = new UserService();
            Thread.CurrentPrincipal = new GenericPrincipal(new GenericIdentity(USER_NAME), null); // Thread Principal is set after the service is created.

            var name = userService.Name;

            Assert.AreEqual(USER_NAME, name);
        }

        interface IUserService
        {
            string Name { get; }
        }

        class UserService : IUserService
        {
            public string Name { get { return _username; } }
            private readonly Later<string> _username;

            public UserService()
            {
                _username = Later.Create(() => Thread.CurrentPrincipal.Identity.Name);
            }
        }
    }
}
