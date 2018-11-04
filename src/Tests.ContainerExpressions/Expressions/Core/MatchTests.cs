using ContainerExpressions.Containers;
using ContainerExpressions.Expressions;
using ContainerExpressions.Expressions.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace Tests.ContainerExpressions.Expressions.Core
{
    [TestClass]
    public class MatchTests
    {
        private readonly static Func<int, bool> _isEven = x => x % 2 == 0;

        [TestMethod]
        public void OnePattern_HasMatch_IsValid()
        {
            var guid = Guid.NewGuid();

            var pattern = Expression.Match(10,
                Pattern.Create(_isEven, _ => Response.Create(guid))
            );

            Assert.IsTrue(pattern);
            Assert.AreEqual(guid, pattern);
        }

        [TestMethod]
        public void OnePattern_HasNoMatch_IsNotValid()
        {
            var pattern = Expression.Match(9,
                Pattern.Create(_isEven, _ => Response.Create(Guid.NewGuid()))
            );

            Assert.IsFalse(pattern);
        }

        [TestMethod]
        public void TwoPatterns_SkipsPatternsUntil_MatchIsFound()
        {
            var guid = Guid.NewGuid();

            var pattern = Expression.Match(10,
                Pattern.Create<int, Guid>(x => !_isEven(x), _ => Response.Create(Guid.NewGuid())), // Skips this pattern.
                Pattern.Create<int, Guid>(_isEven, _ => Response.Create(guid)) // Pattern match.
            );

            Assert.IsTrue(pattern);
            Assert.AreEqual(guid, pattern);
        }

        [TestMethod]
        public void PivotMatch_ExecutesCorrectPattern()
        {
            var pivot = 1;
            var guid = new Guid();

            var pattern = Expression.Match(pivot, guid,
                Pattern.Create<int, Guid, Guid>(x => x == 0, _ => Response.Create(Guid.NewGuid())),
                Pattern.Create<int, Guid, Guid>(x => x == 1, id => Response.Create(id))
            );

            Assert.IsTrue(pattern);
            Assert.AreEqual(guid, pattern);
        }
    }
}
