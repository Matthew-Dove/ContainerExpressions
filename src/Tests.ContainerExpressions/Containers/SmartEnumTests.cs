using ContainerExpressions.Containers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace Tests.ContainerExpressions.Containers
{
    [TestClass]
    public class SmartEnumTests
    {
        [TestMethod]
        public void MyTestMethod()
        {
            try
            {
                new TestMe().Ok();
            }
            catch (Exception ex)
            {

            }
        }
    }
}
