using ContainerExpressions.Containers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace Tests.ContainerExpressions.Examples
{
    [TestClass]
    public class MaybeExample
    {
        /// <summary>https://github.com/Matthew-Dove/ContainerExpressions#maybetvalue-terror</summary>
        [TestMethod]
        public void Maybe_ValueError_Example()
        {
            var parse = new ParseService();

            var number = parse.Integer("1234");
            var nan = parse.Integer("hello world");

            var result1 = number.Match(value => $"{value} is a integer!", error => $"Error: {error}.");
            var result2 = nan.Match(value => $"{value} is a integer!", error => $"Error: {error}.");

            Assert.AreEqual("1234 is a integer!", result1);
            Assert.AreEqual("Error: InputNotInteger.", result2);
        }

        [TestMethod]
        public void Maybe_Value_Example()
        {
            var parse = new ParseService();

            var number = parse.Int("1234");
            var nan = parse.Int("hello world");

            var result1 = number.Match(value => $"{value} is a integer!", error => $"Error: {error}.");
            var result2 = nan.Match(value => $"{value} is a integer!", error => $"Error: {error}.");

            Assert.AreEqual("1234 is a integer!", result1);
            Assert.IsTrue(result2.Contains("System.FormatException"));
        }

        private class ParseService
        {
            public enum ParseError { InputNull, InputNotInteger }

            private static readonly Maybe<int, ParseError> _maybe = default;

            public Maybe<int, ParseError> Integer(string input)
            {
                if (input == null) return _maybe.With(ParseError.InputNull);

                if (!int.TryParse(input, out int number)) return _maybe.With(ParseError.InputNotInteger);

                return _maybe.With(number);
            }

            public Maybe<int> Int(string input)
            {
                var maybe = new Maybe<int>();

                try
                {
                    var result = int.Parse(input);
                    maybe = maybe.With(result);
                }
                catch (Exception ex)
                {
                    maybe = maybe.With(ex);
                }

                return maybe;
            }
        }
    }
}
