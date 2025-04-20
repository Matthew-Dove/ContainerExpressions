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
        #region Option

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

        #endregion

        #region Generic Option

        public sealed class StatusCode : Option<int>
        {
            public static readonly StatusCode Ok = new(200);
            public static readonly StatusCode NotFound = new(404);
            public static readonly StatusCode InternalServerError = new(500);
            private StatusCode(int value) : base(value) { }

            public static IEnumerable<int> GetValues() => GetValues<StatusCode>();
            public static StatusCode Parse(int value) => Parse<StatusCode>(value);
        }

        public sealed class OptionArray : Option<int[]>
        {
            public static readonly OptionArray Field = new([24, 42]);
            private OptionArray(int[] value) : base(value) { }
            public static OptionArray Parse(int[] value) => Parse<OptionArray>(value);
        }

        public sealed class OptionArrayEmpty : Option<int[]>
        {
            public static readonly OptionArrayEmpty Field = new([]);
            private OptionArrayEmpty(int[] value) : base(value) { }
            public static OptionArrayEmpty Parse(int[] value) => Parse<OptionArrayEmpty>(value);
        }

        public sealed class OptionString : Option<string>
        {
            public static readonly OptionString Field = new("hi");
            private OptionString(string value) : base(value) { }
            public static OptionString Parse(string value) => Parse<OptionString>(value);
        }

        public sealed class OptionEnumerable : Option<IEnumerable<int>>
        {
            public static readonly OptionEnumerable Field = new(Enumerable.Range(1, 2));
            private OptionEnumerable(IEnumerable<int> value) : base(value) { }
            public static OptionEnumerable Parse(IEnumerable<int> value) => Parse<OptionEnumerable>(value);
        }

        public sealed class OptionTuple : Option<Tuple<int, int>>
        {
            public static readonly OptionTuple Field = new(Tuple.Create(24, 42));
            private OptionTuple(Tuple<int, int> value) : base(value) { }
            public static OptionTuple Parse(Tuple<int, int> value) => Parse<OptionTuple>(value);
        }

        public sealed class OptionValueTuple : Option<(int, int)>
        {
            public static readonly OptionValueTuple Field = new((24, 42));
            private OptionValueTuple((int, int) value) : base(value) { }
            public static OptionValueTuple Parse((int, int) value) => Parse<OptionValueTuple>(value);
        }

        public sealed class OptionList : Option<List<int>>
        {
            public static readonly OptionList Field = new([24, 42]);
            private OptionList(List<int> value) : base(value) { }
            public static OptionList Parse(List<int> value) => Parse<OptionList>(value);
        }

        public sealed class OptionDictionary : Option<Dictionary<int, int>>
        {
            public static readonly OptionDictionary Field = new(new Dictionary<int, int> { { 24, 42 }, { 42, 24 } });
            private OptionDictionary(Dictionary<int, int> value) : base(value) { }
            public static OptionDictionary Parse(Dictionary<int, int> value) => Parse<OptionDictionary>(value);
        }

        public class MyClass : IEquatable<MyClass>
        {
            public int Property { get; set; }
            public bool Equals(MyClass other) => other is not null && Property.Equals(other.Property);
        }

        public class MyClassValueEquals
        {
            public int Property { get; set; }
        }

        public sealed class OptionClass : Option<MyClass>
        {
            public static readonly OptionClass Field = new(new MyClass { Property = 42 });
            private OptionClass(MyClass value) : base(value) { }
            public static OptionClass Parse(MyClass value) => Parse<OptionClass>(value);
        }

        public sealed class OptionClassValueEquals : Option<MyClassValueEquals>
        {
            public static readonly OptionClassValueEquals Field = new(new MyClassValueEquals { Property = 42 });
            private OptionClassValueEquals(MyClassValueEquals value) : base(value) { }
            public static OptionClassValueEquals Parse(MyClassValueEquals value) => Parse<OptionClassValueEquals>(value, static (x, y) => x.Property == y.Property);
        }

        public readonly struct MyStruct
        {
            public int Property { get; init; }
        }

        public sealed class OptionStruct : Option<MyStruct>
        {
            public static readonly OptionStruct Field = new(new MyStruct { Property = 42 });
            private OptionStruct(MyStruct value) : base(value) { }
            public static OptionStruct Parse(MyStruct value) => Parse<OptionStruct>(value);
        }

        [TestMethod]
        public void GenericOption_StatusCode()
        {
            var areEqual = 200 == StatusCode.Ok;
            var areNotEqual = 201 == StatusCode.Ok;
            var areAlsoNotEqual = StatusCode.NotFound == StatusCode.Ok;

            Assert.IsTrue(areEqual);
            Assert.IsFalse(areNotEqual);
            Assert.IsFalse(areAlsoNotEqual);

            var codes = StatusCode.GetValues().ToArray();
            CollectionAssert.AreEqual(new[] { 200, 404, 500 }, codes);
        }

        [TestMethod]
        public void GenericOption_Parse_Array()
        {
            var target = new[] { 24, 42 };
            var option = OptionArray.Parse(target);
            Assert.IsTrue(target.SequenceEqual(option.Value));
        }

        [TestMethod]
        public void GenericOption_Parse_Array_Empty()
        {
            var target = new int[] { };
            var option = OptionArrayEmpty.Parse(target);
            Assert.IsTrue(target.SequenceEqual(option.Value));
        }

        [TestMethod]
        public void GenericOption_Parse_Int()
        {
            var target = 200;
            var option = StatusCode.Parse(target);
            Assert.AreEqual(target, option);
        }

        [TestMethod]
        public void GenericOption_Parse_String()
        {
            var target = "hi";
            var option = OptionString.Parse(target);
            Assert.AreEqual(target, option);
        }

        [TestMethod]
        public void GenericOption_Parse_Enumerable()
        {
            IEnumerable<int> target = Enumerable.Range(1, 2);
            var option = OptionEnumerable.Parse(target);
            Assert.IsTrue(target.SequenceEqual(option.Value));
        }

        [TestMethod]
        public void GenericOption_Parse_Tuple()
        {
            var target = Tuple.Create(24, 42);
            var option = OptionTuple.Parse(target);
            Assert.AreEqual(target, option);
        }

        [TestMethod]
        public void GenericOption_Parse_ValueTuple()
        {
            var target = (24, 42);
            var option = OptionValueTuple.Parse(target);
            Assert.AreEqual(target, option);
        }

        [TestMethod]
        public void GenericOption_Parse_List()
        {
            var target = new List<int> { 24, 42 };
            var option = OptionList.Parse(target);
            Assert.IsTrue(target.SequenceEqual(option.Value));
        }

        [TestMethod]
        public void GenericOption_Parse_Dictionary()
        {
            var target = new Dictionary<int, int> { { 24, 42 }, { 42, 24 } };
            var option = OptionDictionary.Parse(target);
            Assert.IsTrue(target.SequenceEqual(option.Value));
        }

        [TestMethod]
        public void GenericOption_Parse_Class()
        {
            var target = new MyClass { Property = 42 };
            var option = OptionClass.Parse(target);
            Assert.AreEqual(target.Property, option.Value.Property);
        }

        [TestMethod]
        public void GenericOption_Parse_ClassValueEquals()
        {
            var target = new MyClassValueEquals { Property = 42 };
            var option = OptionClassValueEquals.Parse(target);
            Assert.AreEqual(target.Property, option.Value.Property);
        }

        [TestMethod]
        public void GenericOption_Parse_Struct()
        {
            var target = new MyStruct { Property = 42 };
            var option = OptionStruct.Parse(target);
            Assert.AreEqual(target.Property, option.Value.Property);
        }

        #endregion
    }
}
