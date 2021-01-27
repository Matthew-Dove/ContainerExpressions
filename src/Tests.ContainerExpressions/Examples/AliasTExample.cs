using ContainerExpressions.Containers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace Tests.ContainerExpressions.Examples
{
    [TestClass]
    public class AliasTExample
    {
        /// <summary>https://github.com/Matthew-Dove/ContainerExpressions#aliast</summary>
        [TestMethod]
        public void AliasT_Example()
        {
            var acme = new AcmeService();

            var search = acme.SearchFor("john.smith@example.com");
            var detail = acme.DetailFor(search);
            
            Assert.AreEqual(search, detail.Id); // Note the type AcmeRequestId has been implicitly cast to string.
        }

        private class AcmeRequestId : Alias<string> { public AcmeRequestId(string value) : base(value) { } }

        private class AcmeService
        {
            public AcmeRequestId SearchFor(string email) => new AcmeRequestId(Guid.NewGuid().ToString()); // Mock network calls to the ACME API.

            public Customer DetailFor(AcmeRequestId acmeRequestId) => new Customer(acmeRequestId, "John Smith");
        }

        private class Customer
        {
            public string Id { get; }

            public string Name { get; }

            public Customer(string id, string name)
            {
                Id = id;
                Name = name;
            }
        }

        [TestMethod]
        public void AliasT_ToUpperCase()
        {
            var message = "hello world";

            var upper = new UpperCase(message); // Example of using Alias to perform a simple value transformation on the underlying type.

            Assert.AreEqual(message.ToUpper(), upper);
        }

        private class UpperCase : Alias<string> { public UpperCase(string value) : base(value?.ToUpper()) { } }
    }
}
