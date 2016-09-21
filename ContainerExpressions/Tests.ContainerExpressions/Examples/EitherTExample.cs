using ContainerExpressions.Containers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace Tests.ContainerExpressions.Examples
{
    [TestClass]
    public class EitherTExample
    {
        private struct Ok { }
        private struct Error { }

        /// <summary>https://github.com/Matthew-Dove/ContainerExpressions#eithert</summary>
        [TestMethod]
        public void EitherT_Example()
        {
            Either<Ok, Error> either = new Ok();

            if (new Random().Next() % 2 == 0)
            {
                either = new Error();
            }

            string message = either.Match(
                ok => "Operation was successful.", // When Either's type is Ok, this string is returned.
                error => "Internal error - try again later." // When Either's type is Error, this string is returned.
            );

            Assert.IsNotNull(message);
        }
    }
}
