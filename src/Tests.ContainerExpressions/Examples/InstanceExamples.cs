using ContainerExpressions.Containers;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Tests.ContainerExpressions.Examples
{
    [TestClass]
    public class InstanceExamples
    {
        // Example of adding different string references.
        sealed class Jane : Alias<string> { public Jane() : base(nameof(Jane)) { } }
        sealed class John : Alias<string> { public John() : base(nameof(John)) { } }

        /// <summary>https://github.com/Matthew-Dove/ContainerExpressions#instance</summary>
        [TestMethod]
        public void AliasSameReferenceTypes()
        {
            // This works because we are not adding a string type, we are adding two new types: Jane, and John.
            Instance.Create(new Jane());
            Instance.Create(new John());

            John john = Instance.Of<John>();
            string name = Instance.Of<John>(); // Auto casting provided by Alias<string> works here.
            Jane jane = Instance.Of<Jane>();

            Assert.AreEqual(new John(), john);
            Assert.AreEqual(nameof(John), name);
            Assert.AreEqual(nameof(Jane), jane);
        }

        // Example of adding value types.
        sealed class One : Alias<int> { public One() : base(1) { } }
        sealed class Two : Alias<int> { public Two() : base(2) { } }

        [TestMethod]
        public void AliasSameValueTypes()
        {
            Instance.Create(new One());
            Instance.Create(new Two());

            One one = Instance.Of<One>();
            int num = Instance.Of<One>();
            Two two = Instance.Of<Two>();

            Assert.AreEqual(new One(), one);
            Assert.AreEqual(1, num);
            Assert.AreEqual(2, two);
        }

        // Example of adding derived types.
        class Base { }
        class Derived : Base { }

        [TestMethod]
        public void DerivedTypes()
        {
            Instance.Create(new Base());
            Instance.Create(new Derived());

            Derived derived = Instance.Of<Derived>();
            Base @cast = Instance.Of<Derived>();
            Base @base = Instance.Of<Base>();

            Assert.IsNotNull(derived);
            Assert.IsNotNull(@cast);
            Assert.IsNotNull(@base);
        }
    }
}
