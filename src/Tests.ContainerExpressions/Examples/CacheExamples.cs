using ContainerExpressions.Containers;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Tests.ContainerExpressions.Examples
{
    [TestClass]
    public class CacheExamples
    {
        // Example of adding different string references to the cache.
        sealed class Jane : Alias<string> { public Jane() : base(nameof(Jane)) { } }
        sealed class John : Alias<string> { public John() : base(nameof(John)) { } }

        /// <summary>https://github.com/Matthew-Dove/ContainerExpressions#cache</summary>
        [TestMethod]
        public void AliasSameReferenceTypes()
        {
            // This works because we are not adding a string type, we are adding two new types: Jane, and John.
            Cache.Set(new Jane());
            Cache.Set(new John());

            John john = Cache.Get<John>();
            string name = Cache.Get<John>(); // Auto casting provided by Alias<string> works here.
            Jane jane = Cache.Get<Jane>();

            Assert.AreEqual(new John(), john);
            Assert.AreEqual(nameof(John), name);
            Assert.AreEqual(nameof(Jane), jane);
        }

        // Example of adding value types to the cache.
        sealed class One : Alias<int> { public One() : base(1) { } }
        sealed class Two : Alias<int> { public Two() : base(2) { } }

        [TestMethod]
        public void AliasSameValueTypes()
        {
            Cache.Set(new One());
            Cache.Set(new Two());

            One one = Cache.Get<One>();
            int num = Cache.Get<One>();
            Two two = Cache.Get<Two>();

            Assert.AreEqual(new One(), one);
            Assert.AreEqual(1, num);
            Assert.AreEqual(2, two);
        }

        // Example of adding derived types to the cache.
        class Base { }
        class Derived : Base { }

        [TestMethod]
        public void DerivedTypes()
        {
            Cache.Set(new Base());
            Cache.Set(new Derived());

            Derived derived = Cache.Get<Derived>();
            Base @cast = Cache.Get<Derived>();
            Base @base = Cache.Get<Base>();

            Assert.IsNotNull(derived);
            Assert.IsNotNull(@cast);
            Assert.IsNotNull(@base);
        }
    }
}
