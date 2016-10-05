using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ContainerExpressions.Containers;
using ContainerExpressions.Expressions;

namespace Tests.ContainerExpressions.Expressions.Core
{
    [TestClass]
    public class ComposeTests
    {
        [TestMethod]
        public void ManyValidFunctions_ResultsInAValidContainer()
        {
            Func<Response<float>> func1 = () => Response.Create(3.14f);
            Func<float, Response<string>> func2 = (input) => Response.Create(input.ToString());
            Func<string, Response<string[]>> func3 = (input) => Response.Create(input.Split('.'));

            var result = Expression.Compose(func1, func2, func3);

            Assert.IsTrue(result);
        }

        [TestMethod]
        public void OneInValidFunction_ResultsInAnInvalidContainer()
        {
            Func<Response<float>> func1 = () => Response.Create(3.14f);
            Func<float, Response<string>> func2 = (input) => new Response<string>(); // Invalid.
            Func<string, Response<string[]>> func3 = (input) => Response.Create(input.Split('.'));

            var result = Expression.Compose(func1, func2, func3);

            Assert.IsFalse(result);
        } 

        [TestMethod]
        public void AfterAnInValidFunction_AllPostFunctionsDoNotExecute()
        {
            var id1 = Guid.NewGuid();
            var id2 = id1;

            Func<Response<float>> func1 = () => Response.Create(3.14f);
            Func<float, Response<string>> func2 = (input) => new Response<string>(); // Invalid.
            Func<string, Response<string[]>> func3 = (input) => {
                id2 = Guid.NewGuid();
                return Response.Create(input.Split('.'));
            };

            var result = Expression.Compose(func1, func2, func3);

            Assert.AreEqual(id1, id2);
        }

        [TestMethod]
        public void WhenAllFunctionsAreValid_TheLastFunctionWillExecute() // All functions will execute, but the last one is the logical test, since it can only run once all the others before have.
        {
            var id1 = Guid.NewGuid();
            var id2 = id1;

            Func<Response<float>> func1 = () => Response.Create(3.14f);
            Func<float, Response<string>> func2 = (input) => Response.Create(input.ToString());
            Func<string, Response<string[]>> func3 = (input) => {
                id2 = Guid.NewGuid();
                return Response.Create(input.Split('.'));
            };

            var result = Expression.Compose(func1, func2, func3);

            Assert.AreNotEqual(id1, id2);
        }
    }
}
