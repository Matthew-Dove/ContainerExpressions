using ContainerExpressions.Containers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace Tests.ContainerExpressions.Containers
{
    [TestClass]
    public class NotNullTests
    {
        private class Model {
            public Guid Id { get; } = Guid.NewGuid();
            public override int GetHashCode() => Id.GetHashCode();
            public override bool Equals(object obj) => obj is Model model && Id.Equals(model.Id);
            public override string ToString() => Id.ToString();
        }

        [TestMethod]
        public void Equatable_Cast()
        {
            Model model = new Model();
            NotNull<Model> notNull = model;

            Model result = notNull;

            Assert.AreEqual(model, result);
        }

        [TestMethod]
        public void Equatable_GetHashCode()
        {
            var model = new Model();
            var notNull = (NotNull<Model>)model;

            Assert.AreEqual(model.GetHashCode(), notNull.GetHashCode());
        }

        [TestMethod]
        public void Equatable_Equals()
        {
            var model = new Model();
            var notNull = (NotNull<Model>)model;

            var result1 = notNull.Equals(model);
            var result2 = notNull == model;
            var result3 = model == notNull;
            var result4 = model.Equals(notNull);

            Assert.IsTrue(result1);
            Assert.IsTrue(result2);
            Assert.IsTrue(result3);
            Assert.IsFalse(result4);
        }

        [TestMethod]
        public void Equatable_Not_Equals()
        {
            var model1 = new Model();
            var model2 = new Model();
            var notNull1 = (NotNull<Model>)model1;
            var notNull2 = (NotNull<Model>)model2;

            var result0 = model1.Equals(model2);

            var result1 = notNull1 != model2;
            var result2 = model2 != notNull1;
            var result3 = notNull1 != notNull2;
            var result4 = notNull2 != notNull1;

            var result5 = notNull1 == model2;
            var result6 = model2 == notNull1;
            var result7 = notNull1 == notNull2;
            var result8 = notNull2 == notNull1;

            var result9 = model1.Equals(model1);

            Assert.IsFalse(result0);

            Assert.IsTrue(result1);
            Assert.IsTrue(result2);
            Assert.IsTrue(result3);
            Assert.IsTrue(result4);

            Assert.IsFalse(result5);
            Assert.IsFalse(result6);
            Assert.IsFalse(result7);
            Assert.IsFalse(result8);

            Assert.IsTrue(result9);
        }

        [TestMethod]
        public void Equatable_ToString()
        {
            var model = new Model();
            var notNull = (NotNull<Model>)model;

            Assert.AreEqual(model.ToString(), notNull.ToString());
        }

        [TestMethod]
        public void Accepts_Reference_ExplicitCast()
        {
            var name = "John Smith";

            var result = (NotNull<string>)name;

            Assert.AreEqual(name, result.Value);
        }

        [TestMethod]
        public void Accepts_Reference_ImplicitCast()
        {
            var name = "John Smith";
            NotNull<string> result;

            result = name;

            Assert.AreEqual(name, result.Value);
        }

        [TestMethod]
        public void Accepts_Reference_FunctionParameter()
        {
            var name = "John Smith";
            string ToUpper(NotNull<string> x) => x.Value.ToUpper();

            var result = ToUpper(name);

            Assert.AreEqual(name.ToUpper(), result);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Rejects_NullReference_ExplicitCast()
        {
            string name = null;

            var result = (NotNull<string>)name;
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Rejects_NullReference_ImplicitCast()
        {
            string name = null;
            NotNull<string> result;

            result = name;
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Rejects_NullReference_FunctionParameter()
        {
            string name = null;
            string ToUpper(NotNull<string> x) => x.Value.ToUpper();

            var result = ToUpper(name);
        }

        [TestMethod]
        public void Equatable_Equals_Object_Null()
        {
            object obj = null;
            string name = "John Smith";
            NotNull<string> notNull = name;

            var result = notNull.Equals(obj);

            Assert.IsFalse(result);
        }

        [TestMethod]
        public void Equatable_Equals_Object_String()
        {
            string name = "John Smith";
            object obj = name;
            NotNull<string> notNull = name;

            var result = notNull.Equals(obj);

            Assert.IsFalse(result);
        }

        [TestMethod]
        public void Equatable_Equals_Object_String_Cast()
        {
            string name = "John Smith";
            object obj = name;
            NotNull<string> notNull = name;

            var result = notNull.Equals((string)obj);

            Assert.IsTrue(result);
        }

        [TestMethod]
        public void Equatable_Equals_String()
        {
            string name = "John Smith";
            NotNull<string> notNull = name;

            var result = notNull.Equals(name);

            Assert.IsTrue(result);
        }

        [TestMethod]
        public void Equatable_Equals_Boxed_Guid()
        {
            Guid guid = Guid.NewGuid();
            NotNull<object> notNull = guid;

            var result = notNull.Equals((object)guid);

            Assert.IsTrue(result);
        }

        [TestMethod]
        public void Equatable_Equals_NotNull()
        {
            string name = "John Smith";
            NotNull<string> x = name;
            NotNull<string> y = "John Smith";

            var result = x.Equals(y);

            Assert.IsTrue(result);
        }

        [TestMethod]
        public void Equatable_Equals_NotNull_OneNull()
        {
            string name = "John Smith";
            NotNull<string> x = name;
            NotNull<string> y = null;

            var result = x.Equals(y);

            Assert.IsFalse(result);
        }

        [TestMethod]
        public void Equatable_Equals_Object_String_EqualsSign()
        {
            string name = "John Smith";
            object obj = name;
            NotNull<string> notNull = name;

            var result = notNull == (NotNull<string>)((string)obj);

            Assert.IsTrue(result);
        }

        [TestMethod]
        public void Equatable_Equals_Object_String_Cast_EqualsSign()
        {
            string name = "John Smith";
            object obj = name;
            NotNull<string> notNull = name;

            var result = notNull == (string)obj;

            Assert.IsTrue(result);
        }

        [TestMethod]
        public void Equatable_Equals_String_EqualsSign()
        {
            string name = "John Smith";
            NotNull<string> notNull = name;

            var result = notNull == name;

            Assert.IsTrue(result);
        }

        [TestMethod]
        public void Equatable_Equals_Boxed_Guid_EqualsSign()
        {
            Guid guid = Guid.NewGuid();
            NotNull<object> notNull = guid;

            var result = notNull == (object)guid;

            Assert.IsTrue(result);
        }

        [TestMethod]
        public void Equatable_Equals_NotNull_EqualsSign()
        {
            string name = "John Smith";
            NotNull<string> x = name;
            NotNull<string> y = "John Smith";

            var result = x == y;

            Assert.IsTrue(result);
        }

        [TestMethod]
        public void Equatable_Equals_NotNull_OneNull_EqualsSign()
        {
            string name = "John Smith";
            NotNull<string> x = name;
            NotNull<string> y = null;

            var result = x  == y;

            Assert.IsFalse(result);
        }
    }
}
