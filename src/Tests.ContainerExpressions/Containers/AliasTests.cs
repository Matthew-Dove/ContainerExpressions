using ContainerExpressions.Containers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;

namespace Tests.ContainerExpressions.Containers
{
    [TestClass]
    public class AliasTests
    {
        class Trim : Alias<string> { public Trim(string value) : base(string.Concat(value.Where(x=> !Char.IsWhiteSpace(x)))) { } }

        class Id : Alias<Guid> { public Id(Guid value) : base(value) { } }

        class Alias : Alias<string> { public Alias(string value) : base(value) { } }

        class OverrideEquals : Alias<string> {
            public OverrideEquals(string value) : base(value) { }
            private readonly Guid _guid = Guid.NewGuid();
            public override bool Equals(object obj) => obj is OverrideEquals && (((OverrideEquals)obj)._guid == _guid);
            public override int GetHashCode() => _guid.GetHashCode();
        }

        class OverrideHashCode : Alias<string> {
            public OverrideHashCode(string value) : base(value) { }
            private readonly Guid _guid = Guid.NewGuid();
            public override int GetHashCode() => _guid.GetHashCode();
        }

        class OverrideToString : Alias<string> {
            public OverrideToString(string value) : base(value) { }
            public override string ToString() => Value + Value;
        }

        [TestMethod]
        public void AliasCanTransformValues()
        {
            var input = " Hello World ";

            var result = new Trim(input);

            Assert.AreEqual("HelloWorld", result);
        }

        [TestMethod]
        public void DirectlyAccessUnderlyingValue()
        {
            var guid = Guid.NewGuid();

            var result = new Id(guid);

            Assert.AreEqual(guid, result.Value);
        }

        [TestMethod]
        public void ImplicitlyAccessUnderlyingValue()
        {
            var guid = Guid.NewGuid();

            var result = new Id(guid);

            Assert.AreEqual(guid, result);
        }

        [TestMethod]
        public void AliasToString_InvokesTheUnderlyingValuesToString()
        {
            var guid = Guid.NewGuid();

            var result = new Id(guid);

            Assert.AreEqual(guid.ToString(), result.ToString());
        }

        [TestMethod]
        public void AliasImplicitCastToValue()
        {
            string message = "Hello World";

            Alias alias = new Alias(message);
            string result = alias;

            Assert.AreEqual(message, result);
        }

        [TestMethod]
        public void AliasIsEqualToAlias()
        {
            var message = "Hello World";

            var alias1 = new Alias(message);
            var alias2 = new Alias(message);

            Assert.AreEqual(alias1, alias2);
        }

        [TestMethod]
        public void AliasHashCodeAreEqual()
        {
            var guid = Guid.NewGuid();

            var alias1 = new Id(guid);
            var alias2 = new Id(guid);

            Assert.AreEqual(alias1.GetHashCode(), alias2.GetHashCode());
        }

        [TestMethod]
        public void AliasHashCodeAreNotEqual()
        {
            var alias1 = new Id(Guid.NewGuid());
            var alias2 = new Id(Guid.NewGuid());

            Assert.AreNotEqual(alias1.GetHashCode(), alias2.GetHashCode());
        }

        [TestMethod]
        public void CanOverrideAliasHashCode()
        {
            var message = "Hello World";

            var alias1 = new OverrideHashCode(message);
            var alias2 = new OverrideHashCode(message);

            Assert.AreNotEqual(alias1.GetHashCode(), alias2.GetHashCode());
        }

        [TestMethod]
        public void CanOverrideAliasEquals()
        {
            var message = "Hello World";

            var alias1 = new OverrideEquals(message);
            var alias2 = new OverrideEquals(message);

            Assert.AreNotEqual(alias1, alias2);
        }

        [TestMethod]
        public void CanOverrideAliasToString()
        {
            var message = "Hello World";

            var alias = new OverrideToString(message);

            Assert.AreNotEqual(message, alias.ToString());
        }

        [TestMethod]
        public void AliasToStringOverride_DoNotAffectImplicitCasts()
        {
            var message = "Hello World";

            var alias = new OverrideToString(message);

            Assert.AreEqual(message, alias);
        }

        [TestMethod]
        public void AreEqualWhenCastToBase()
        {
            string message = "Hello World";

            Alias alias1 = new Alias(message);
            Alias<string> alias2 = alias1;

            Assert.AreEqual(alias1, alias2);
        }

        [TestMethod]
        public void AreEqualWhenCastToBase_AndDifferentInstances()
        {
            string message = "Hello World";

            Alias alias1 = new Alias(message);
            Alias<string> alias2 = new Alias(message);

            Assert.AreEqual(alias1, alias2);
        }

        [TestMethod]
        public void AliasCompareForEqualsOperator()
        {
            var guid = Guid.NewGuid();

            var alias1 = new Id(guid);
            var alias2 = new Id(guid);
            var result = alias1 == alias2;

            Assert.IsTrue(result);
        }

        [TestMethod]
        public void AliasCompareForNotEqualsOperator()
        {
            var alias1 = new Id(Guid.NewGuid());
            var alias2 = new Id(Guid.NewGuid());
            var result = alias1 != alias2;

            Assert.IsTrue(result);
        }

        [TestMethod]
        public void AliasCompareForEqualsOperatorBaseCast()
        {
            var guid = Guid.NewGuid();

            var alias1 = new Id(guid);
            var alias2 = (Alias<Guid>)new Id(guid);
            var result = alias1 == alias2;

            Assert.IsTrue(result);
        }

        [TestMethod]
        public void AliasCompareForNotEqualsOperatorBaseCast()
        {
            var alias1 = new Id(Guid.NewGuid());
            var alias2 = (Alias<Guid>)new Id(Guid.NewGuid());
            var result = alias1 != alias2;

            Assert.IsTrue(result);
        }

        [TestMethod]
        public void AliasCompareForEqualsOperatorOnValue()
        {
            var value = Guid.NewGuid();
            var alias = new Id(value);
            var result = alias == value;

            Assert.IsTrue(result);
        }

        [TestMethod]
        public void AliasCompareForNotEqualsOperatorOnValue()
        {
            var value = Guid.NewGuid();
            var alias = new Id(Guid.NewGuid());
            var result = alias != value;

            Assert.IsTrue(result);
        }

        [TestMethod]
        public void AliasBaseCompareForEqualsOperatorOnValue()
        {
            var value = Guid.NewGuid();
            var alias = (Alias<Guid>)new Id(value);
            var result = value == alias;

            Assert.IsTrue(result);
        }

        [TestMethod]
        public void AliasBaseCompareForNotEqualsOperatorOnValue()
        {
            var value = Guid.NewGuid();
            var alias = (Alias<Guid>)new Id(Guid.NewGuid());
            var result = value != alias;

            Assert.IsTrue(result);
        }
    }
}
