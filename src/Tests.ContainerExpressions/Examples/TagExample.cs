using ContainerExpressions.Containers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace Tests.ContainerExpressions.Examples
{
    [TestClass]
    public class TagExample
    {
        /// <summary>https://github.com/Matthew-Dove/ContainerExpressions#tag</summary>
        [TestMethod]
        public void Tag_Example()
        {
            Guid guid = GenerateGuid(); // 7db88344-5309-46dc-a535-1c3f0e029f69
            DateTime created = guid.Tag().Get<DateTime>(); // 21/07/2024 1:43:04 PM
        }

        private static Guid GenerateGuid()
        {
            // Tag a Guid, with the datetime it was created.
            return Guid.NewGuid().Tag().Set(DateTime.UtcNow);
        }
    }
}
