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

        #region Combining Alias & Either

        /**
         * You can create custom reusable friendly named types by combining `Alias`, and `Either`.
         * This can save you from needing to write out the entire `Either` type each time you use it; and you can give the type a descriptive name.
        **/

        // Accepts either a: `string`, or an `int`. Does not do any extra processing on the input.
        class StringOrInt : Alias<Either<int, string>> {
            public StringOrInt(Either<int, string> value) : base(value) { }
        }

        [TestMethod]
        public void AliasT_EitherT_StringOrInt_Example()
        {
            StringOrInt stringOrInt = new StringOrInt("1"); // String value
            int number = stringOrInt.Value.Match(x => x, int.Parse); // Output: 1
            bool isString = stringOrInt == "1"; // Output: true
            bool isInt = stringOrInt == 1; // Output: false

            Assert.AreEqual(1, number);
            Assert.IsTrue(isString);
            Assert.IsFalse(isInt);

            stringOrInt = new StringOrInt(1); // Int value
            number = stringOrInt.Value.Match(x => x, int.Parse); // Output: 1
            isString = stringOrInt == "1"; // Output: false
            isInt = stringOrInt == 1; // Output: true

            Assert.AreEqual(1, number);
            Assert.IsFalse(isString);
            Assert.IsTrue(isInt);
        }

        // Accepts either a: `string`, `short`, `int`, or `long`. Converts the input to a `long`.
        class ConvertToLong : Alias<long> {
            public ConvertToLong(Either<string, short, int, long> value) : base(value.Match(long.Parse, Convert.ToInt64, Convert.ToInt64, x => x)) { }
        }

        [TestMethod]
        public void AliasT_EitherT_ConvertToLong_Example()
        {
            ConvertToLong convertToLong = new ConvertToLong("1"); // String value
            long number = convertToLong; // Output: 1

            Assert.AreEqual(1L, number);

            convertToLong = new ConvertToLong((short)1); // Short value
            number = convertToLong; // Output: 1

            Assert.AreEqual(1L, number);

            convertToLong = new ConvertToLong((int)1); // Int value
            number = convertToLong; // Output: 1

            Assert.AreEqual(1L, number);

            convertToLong = new ConvertToLong((long)1); // Long value
            number = convertToLong; // Output: 1

            Assert.AreEqual(1L, number);
        }

        #endregion
    }
}
