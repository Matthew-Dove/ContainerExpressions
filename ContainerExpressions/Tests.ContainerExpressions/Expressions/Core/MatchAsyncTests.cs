using ContainerExpressions.Containers;
using ContainerExpressions.Expressions;
using ContainerExpressions.Expressions.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Threading.Tasks;

namespace Tests.ContainerExpressions.Expressions.Core
{
    [TestClass]
    public class MatchAsyncTests
    {
        private readonly static Func<int, bool> _isEven = x => x % 2 == 0;

        private static async Task<T> WaitThenDo<T>(Func<T> func)
        {
            await Task.Delay(0);
            return func();
        }

        [TestMethod]
        public async Task OnePattern_HasMatch_IsValid()
        {
            var guid = Guid.NewGuid();

            var pattern = await Expression.MatchAsync(10,
                Pattern.CreateAsync(_isEven, _ => WaitThenDo(() => Response.Create(guid)))
            );

            Assert.IsTrue(pattern);
            Assert.AreEqual(guid, pattern);
        }

        [TestMethod]
        public async Task OnePattern_HasNoMatch_IsNotValid()
        {
            var pattern = await Expression.MatchAsync(9,
                Pattern.CreateAsync(_isEven, _ => WaitThenDo(() => Response.Create(Guid.NewGuid())))
            );

            Assert.IsFalse(pattern);
        }

        [TestMethod]
        public async Task TwoPatterns_SkipsPatternsUntil_MatchIsFound()
        {
            var guid = Guid.NewGuid();

            var pattern = await Expression.MatchAsync(10,
                Pattern.CreateAsync<int, Guid>((x) => !_isEven(x), _ => WaitThenDo(() => Response.Create(Guid.NewGuid()))), // Skips thi pattern.
                Pattern.CreateAsync<int, Guid>(_isEven, _ => WaitThenDo(() => Response.Create(guid))) // Pattern match.
            );

            Assert.IsTrue(pattern);
            Assert.AreEqual(guid, pattern);
        }
    }
}
