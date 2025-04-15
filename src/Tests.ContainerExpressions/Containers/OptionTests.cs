using ContainerExpressions.Containers;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Tests.ContainerExpressions.Containers
{
    [TestClass]
    public class OptionTests
    {
        public class Card
        {
            public static readonly Option Visa = new Option("VI");
            public static readonly Option Mastercard = new Option("MC");
            public static readonly Option Diners = new Option { Value = "DI" };
            public static readonly Option Amex = new Option { Value = "AX" };
        }

        [TestMethod]
        public void Option_ImplicitConversionToString()
        {
            var option = Card.Visa;
            string value = option;
            Assert.AreEqual("VI", value);
        }
    }
}
