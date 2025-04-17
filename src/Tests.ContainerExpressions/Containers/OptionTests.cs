using ContainerExpressions.Containers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Tests.ContainerExpressions.Containers
{
    [TestClass]
    public class OptionTests
    {
        public sealed class Card : Option, IEquatable<Card>
        {
            public static readonly Card Visa = new("VI");
            public static readonly Card Mastercard = new("MC");
            public static readonly Card Diners = new("DI");
            public static readonly Card Amex = new("AX");

            public static Card Jcb { get; } = new("JB");

            private Card(string value) : base(value) { }

            public bool Equals(Card other) => other is not null && ReferenceEquals(this, other);

            public static Card Parse(string value) => Option.Parse<Card>(value);
            public static bool TryParse(string value, out Card card) => Option.TryParse<Card>(value, out card);
            public static IEnumerable<string> GetValues() => Option.GetValues<Card>();
        }

        [TestMethod]
        public void Option_ImplicitConversionToString()
        {
            var scheme = Card.Visa;
            string value = scheme;

            Assert.AreEqual("VI", value);
        }

        [TestMethod]
        public void Option_TryParse()
        {
            var result = Card.TryParse("mc", out var card);

            Assert.IsTrue(result);
            Assert.AreEqual(Card.Mastercard, card);
        }

        [TestMethod]
        public void Option_Equals()
        {
#pragma warning disable CS1718
            var areEqual = Card.Visa == Card.Visa; // Comparison made to same variable (CS1718).
#pragma warning restore CS1718
            var alsoEqual = Card.Parse("vi") == Card.Visa;
            var areNotEqual = Card.Visa == Card.Mastercard;

            Assert.IsTrue(areEqual);
            Assert.IsTrue(alsoEqual);
            Assert.IsFalse(areNotEqual);
        }

        [TestMethod]
        public void Option_ReferenceEquals()
        {
            var areEqual = Card.Visa.Equals(Card.Visa);
            var alsoEqual = Card.Parse("vi").Equals(Card.Visa);
            var areNotEqual = Card.Visa.Equals(Card.Mastercard);

            Assert.IsTrue(areEqual);
            Assert.IsTrue(alsoEqual);
            Assert.IsFalse(areNotEqual);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void Option_ParseError()
        {
            var card = Card.Parse("invalid");
        }

        [TestMethod]
        public void Option_GetValues()
        {
            var values = Card.GetValues();

            Assert.IsTrue(values.Any(x => x == Card.Visa.Value));
            Assert.IsTrue(values.Any(x => x == Card.Jcb));
        }

        [TestMethod]
        public void Option_EqualsOperatorOverloads()
        {
            var visa1 = Card.Visa;
            var visa2 = Card.Parse("VI");

            Assert.AreSame(visa1, visa2);
            Assert.IsTrue(visa1 == "VI");
            Assert.IsTrue("vi" == visa1);
            Assert.IsTrue(visa1 == Card.Visa);
            Assert.IsTrue(visa1 != Card.Mastercard);
            Assert.IsFalse(visa1 == "MC");
            Assert.IsTrue(visa1 != "MC");

            Assert.IsFalse(Card.Visa.Equals((object)null));
            Assert.IsFalse(Card.Visa.Equals((Card)null));
            Assert.IsFalse(Card.Visa == null);
            Assert.IsFalse(null == Card.Visa);
        }
    }
}
