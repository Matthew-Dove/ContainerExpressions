using ContainerExpressions.Containers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace Tests.ContainerExpressions.Containers
{
    [TestClass]
    public class TagTests
    {
        [TestMethod]
        public void Tag_Set_Get()
        {
            var message = 42;

            var reference = Guid.NewGuid().Tag().Set(message);

            Assert.AreEqual(message, reference.Tag().Get<int>());
        }

        [TestMethod]
        public void Tag_Set_Get_Get()
        {
            var message = 42;

            var reference = Guid.NewGuid().Tag().Set(message);

            Assert.AreEqual(message, reference.Tag().Get<int>());
            Assert.IsFalse(reference.Tag().Get<int>());
        }

        [TestMethod]
        public void Tag_Get()
        {
            var reference = Guid.NewGuid();

            Assert.IsFalse(reference.Tag().Get<int>());
        }

        [TestMethod]
        public void Tag_Set_Keep_Remove()
        {
            var message = 42;

            var reference = Guid.NewGuid().Tag().Set(42);

            Assert.AreEqual(message, reference.Tag().Get<int>(removeTag: false)); // Keep tag.
            Assert.AreEqual(message, reference.Tag().Get<int>(removeTag: true)); // Remove Tag.
            Assert.IsFalse(reference.Tag().Get<int>()); // Tag not found.
        }

        [TestMethod]
        public void TagReference_Set_Keep_Remove()
        {
            var message = 42;

            var reference = "hello world".TagReference().Set(42);

            Assert.AreEqual(message, reference.TagReference().Get<int>(removeTag: false)); // Keep tag.
            Assert.AreEqual(message, reference.TagReference().Get<int>(removeTag: true)); // Remove Tag.
            Assert.IsFalse(reference.TagReference().Get<int>()); // Tag not found.
        }

        [TestMethod]
        public void TagReference_Set_Get_String()
        {
            var message = 42;

            var reference = "hello world".TagReference().Set(message);

            Assert.IsFalse("Hello World!".TagReference().Get<int>());
            Assert.AreEqual(message, reference.TagReference().Get<int>());
        }

        [TestMethod]
        public void TagReference_Set_Get_Object()
        {
            var message = 42;

            var reference = new object().TagReference().Set(message);

            Assert.IsFalse(new object().TagReference().Get<int>());
            Assert.AreEqual(message, reference.TagReference().Get<int>());
        }

        public sealed class MyClass1
        {
            public string MyProperty1 { get; set; }
            public object MyProperty2 { get; set; }
            public MyClass2 MyProperty3 { get; set; }

            public MyClass1()
            {
                MyProperty1 = "hey chum";
                MyProperty2 = 1337;
                MyProperty3 = new MyClass2();
            }
        }

        public class MyClass2
        {
            public Guid MyProperty { get; set; }

            public MyClass2()
            {
                MyProperty = Guid.NewGuid();
            }
        }

        [TestMethod]
        public void TagReference_Set_Get_MyClass()
        {
            var message = new MyClass1();

            var reference = message.TagReference().Set(message);

            Assert.IsFalse(new MyClass1().TagReference().Get<MyClass1>());
            Assert.ReferenceEquals(message.MyProperty2, reference.TagReference().Get<MyClass1>().Value.MyProperty2);
        }
    }
}
