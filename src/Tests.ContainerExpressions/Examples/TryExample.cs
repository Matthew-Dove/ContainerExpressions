using ContainerExpressions.Containers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using System;
using System.IO;

namespace Tests.ContainerExpressions.Examples
{
    [TestClass]
    public class TryExample
    {
        private const string RELATIVE_PATH = "./Tryxample";

        [TestInitialize]
        public void Initialize()
        {
            if (!Directory.Exists(RELATIVE_PATH))
            {
                Directory.CreateDirectory(RELATIVE_PATH);
            }
        }

        [TestCleanup]
        public void Cleanup()
        {
            if (Directory.Exists(RELATIVE_PATH))
            {
                Directory.Delete(RELATIVE_PATH, true);
            }
        }

        /// <summary>https://github.com/Matthew-Dove/ContainerExpressions#tryt</summary>
        [TestMethod]
        public void Try_Example()
        {
            var errorsLogged = 0;
            Try.SetExceptionLogger((_) => errorsLogged++);

            var widget = new Widget();
            var result = Try.Run(() => Persist(widget));

            Assert.IsTrue(result);
            Assert.AreEqual(0, errorsLogged);
        }

        private static void Persist(Widget widget)
        {
            var contents = JsonConvert.SerializeObject(widget);
            var path = $"{RELATIVE_PATH}/{Path.GetRandomFileName()}.json";
            File.WriteAllText(path, contents);
        }

        private class Widget
        {
            public Guid Id { get; } = Guid.NewGuid();
        }
    }
}
