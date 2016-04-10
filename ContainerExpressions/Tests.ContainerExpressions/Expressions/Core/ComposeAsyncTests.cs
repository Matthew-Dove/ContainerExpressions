using ContainerExpressions.Containers;
using ContainerExpressions.Expressions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Threading.Tasks;

namespace Tests.ContainerExpressions.Expressions.Core
{
    [TestClass]
    public class ComposeAsyncTests
    {
        private static async Task<T> WaitThenDo<T>(Func<T> func)
        {
            await Task.Delay(0);
            return func();
        }

        [TestMethod]
        public async Task ManyValidFunctions_ResultsInAValidContainer()
        {
            Func<Task<Response<float>>> funcAsync1 = () => WaitThenDo(() => Response.Create(3.14f));
            Func<float, Task<Response<string>>> funcAsync2 = (input) => WaitThenDo(() => Response.Create(input.ToString()));
            Func<string, Task<Response<string[]>>> funcAsync3 = (input) => WaitThenDo(() => Response.Create(input.Split('.')));

            var result = await Expression.ComposeAsync(funcAsync1, funcAsync2, funcAsync3);

            Assert.IsTrue(result);
        }

        [TestMethod]
        public async Task OneInValidFunction_ResultsInAnInvalidContainer()
        {
            Func<Task<Response<float>>> funcAsync1 = () => WaitThenDo(() => Response.Create(3.14f));
            Func<float, Task<Response<string>>> funcAsync2 = (input) => WaitThenDo(() => Response.Create<string>()); // Invalid.
            Func<string, Task<Response<string[]>>> funcAsync3 = (input) => WaitThenDo(() => Response.Create(input.Split('.')));

            var result = await Expression.ComposeAsync(funcAsync1, funcAsync2, funcAsync3);

            Assert.IsFalse(result);
        }

        [TestMethod]
        public async Task AfterAnInValidFunction_AllPostFunctionsDoNotExecute()
        {
            var id1 = Guid.NewGuid();
            var id2 = id1;

            Func<Task<Response<float>>> funcAsync1 = () => WaitThenDo(() => Response.Create(3.14f));
            Func<float, Task<Response<string>>> funcAsync2 = (input) => WaitThenDo(() => Response.Create<string>()); // Invalid.
            Func<string, Task<Response<string[]>>> funcAsync3 = (input) => WaitThenDo(() => {
                id2 = Guid.NewGuid();
                return Response.Create(input.Split('.'));
            });

            var result = await Expression.ComposeAsync(funcAsync1, funcAsync2, funcAsync3);

            Assert.AreEqual(id1, id2);
        }

        [TestMethod]
        public async Task WhenAllFunctionsAreValid_TheLastFunctionWillExecute() // All functions will execute, but the last one is the logical test, since it can only run once all the others before have.
        {
            var id1 = Guid.NewGuid();
            var id2 = id1;

            Func<Task<Response<float>>> funcAsync1 = () => WaitThenDo(() => Response.Create(3.14f));
            Func<float, Task<Response<string>>> funcAsync2 = (input) => WaitThenDo(() => Response.Create(input.ToString()));
            Func<string, Task<Response<string[]>>> funcAsync3 = (input) => WaitThenDo(() => {
                id2 = Guid.NewGuid();
                return Response.Create(input.Split('.'));
            });

            var result = await Expression.ComposeAsync(funcAsync1, funcAsync2, funcAsync3);

            Assert.AreNotEqual(id1, id2);
        }
    }
}
