using ContainerExpressions.Containers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Threading.Tasks;

namespace Tests.ContainerExpressions.Containers
{
    [TestClass]
    public class MaybeTests
    {
        #region Maybe Error

        [TestMethod]
        public void MaybeError_DefaultCtor_IsError()
        {
            var maybe = new Maybe<int, string>();

            Assert.IsFalse(maybe.Match(_ => true, _ => false));
        }

        [TestMethod]
        public void MaybeError_ErrorCtor_IsError()
        {
            var maybe = new Maybe<int, string>();

            Assert.IsFalse(maybe.Match(_ => true, _ => false));
        }

        [TestMethod]
        public void MaybeError_ValueCtor_IsNotError()
        {
            var maybe = new Maybe<int, string>(0);

            Assert.IsTrue(maybe.Match(_ => true, _ => false));
        }

        [TestMethod]
        public void MaybeError_ValueCtor_ToString()
        {
            var answer = 42;

            var maybe = new Maybe<int, string>(answer);

            Assert.AreEqual(answer.ToString(), maybe.ToString());
        }

        [TestMethod]
        public void MaybeError_ErrorCtor_ToString()
        {
            var answer = 42.ToString();

            var maybe = new Maybe<int, string>(answer);

            Assert.AreEqual(answer, maybe.ToString());
        }

        [TestMethod]
        public void MaybeError_ResponseCtor_IsError()
        {
            var error = "error";
            var response = new Response<int>();

            var maybe = new Maybe<int, string>(response, error);

            Assert.IsFalse(maybe.Match(_ => true, _ => false));
            Assert.AreEqual(error, maybe.Match(x => x.ToString(), x => x));
        }

        [TestMethod]
        public void MaybeError_ResponseCtor_IsNotError()
        {
            var error = "error";
            var answer = 42;
            var response = new Response<int>(answer);

            var maybe = new Maybe<int, string>(response, error);

            Assert.IsTrue(maybe.Match(_ => true, _ => false));
            Assert.AreEqual(answer, maybe.Match(x => x, int.Parse));
        }

        [TestMethod]
        public void MaybeError_EitherCtor_IsError()
        {
            var error = "error";
            var either = new Either<int, string>(error);

            var maybe = new Maybe<int, string>(either);

            Assert.IsFalse(maybe.Match(_ => true, _ => false));
            Assert.AreEqual(error, maybe.Match(x => x.ToString(), x => x));
        }

        [TestMethod]
        public void MaybeError_EitherCtor_IsNotError()
        {
            var answer = 42;
            var either = new Either<int, string>(answer);

            var maybe = new Maybe<int, string>(either);

            Assert.IsTrue(maybe.Match(_ => true, _ => false));
            Assert.AreEqual(answer, maybe.Match(x => x, int.Parse));
        }

        [TestMethod]
        public void MaybeError_BindValue()
        {
            var maybe = new Maybe<int, string>(1);
            var bind = maybe.With(2);

            var result = maybe.Bind(bind, (x, y) => maybe.With(x + y));

            Assert.IsTrue(maybe.Match(x => x == 1, _ => false));
            Assert.IsTrue(bind.Match(x => x == 2, _ => false));
            Assert.IsTrue(result.Match(x => x == 3, _ => false));
        }

        [TestMethod]
        public void MaybeError_Bind_FirstOnly()
        {
            var error = "error";
            var maybe = new Maybe<int, string>(error);

            var result = maybe.Bind(maybe.With(1), (x, y) => maybe.With(x + y));

            Assert.IsTrue(result.Match(_ => false, x => x == error));
            Assert.AreEqual(result.GetAggregateErrors().Length, 0);
        }

        [TestMethod]
        public void MaybeError_Bind_SecondOnly()
        {
            var error = "error";
            var maybe = new Maybe<int, string>(1);

            var result = maybe.Bind(maybe.With(error), (x, y) => maybe.With(x + y));

            Assert.IsTrue(maybe.Match(x => x == 1, x => false));
            Assert.IsTrue(result.Match(_ => false, x => x == error));
            Assert.AreEqual(result.GetAggregateErrors().Length, 0);
        }

        [TestMethod]
        public void MaybeError_Bind_FirstAndSecond()
        {
            string error1 = "error1", error2 = "error2";
            var maybe = new Maybe<int, string>(error1);

            var result = maybe.Bind(maybe.With(error2), (x, y) => maybe.With(x + y));

            Assert.IsTrue(maybe.Match(_ => false, x => x == error1));
            Assert.IsTrue(result.Match(_ => false, x => x == error2));
            Assert.AreEqual(maybe.GetAggregateErrors().Length, 0);
            Assert.AreEqual(result.GetAggregateErrors().Length, 1);
            Assert.AreEqual(result.GetAggregateErrors()[0], error1);
        }

        [TestMethod]
        public void MaybeError_Bind_OneAggregate()
        {
            string error1 = "error1", error2 = "error2", error3 = "error3";
            var maybe = new Maybe<int, string>(error1);

            var result1 = maybe.Bind(maybe.With(error2), (x, y) => maybe.With(x + y));
            var result2 = result1.Bind(maybe.With(error3), (x, y) => maybe.With(x + y));

            Assert.IsTrue(maybe.Match(_ => false, x => x == error1));
            Assert.IsTrue(result1.Match(_ => false, x => x == error2));
            Assert.IsTrue(result2.Match(_ => false, x => x == error3));
            Assert.AreEqual(maybe.GetAggregateErrors().Length, 0);
            Assert.AreEqual(result1.GetAggregateErrors().Length, 1);
            Assert.AreEqual(result1.GetAggregateErrors()[0], error1);
            Assert.AreEqual(result2.GetAggregateErrors().Length, 2);
            Assert.AreEqual(result2.GetAggregateErrors()[0], error1);
            Assert.AreEqual(result2.GetAggregateErrors()[1], error2);
        }

        [TestMethod]
        public void MaybeError_Bind_BothAggregate()
        {
            string error1 = "error1", error2 = "error2", error3 = "error3", error4 = "error4";

            var maybe1 = new Maybe<int, string>(error1);
            var result1 = maybe1.Bind(maybe1.With(error2), (x, y) => maybe1.With(x + y));

            var maybe2 = new Maybe<int, string>(error3);
            var result2 = maybe2.Bind(maybe2.With(error4), (x, y) => maybe2.With(x + y));

            var aggregate = result1.Bind(result2, (x, y) => new Maybe<int, string>(x + y));

            Assert.IsTrue(aggregate.Match(_ => false, x => x == error4));
            Assert.AreEqual(aggregate.GetAggregateErrors()[0], error1);
            Assert.AreEqual(aggregate.GetAggregateErrors()[1], error2);
            Assert.AreEqual(aggregate.GetAggregateErrors()[2], error3);
        }

        [TestMethod]
        public async Task MaybeError_BindAsync_BothNotValid()
        {
            var first = new Maybe<int, string>("error1");
            var second = new Maybe<int, string>("error2");

            var result = await first.BindAsync(second, (x, y) => Task.FromResult(new Maybe<int, string>(x + y)));

            Assert.AreEqual("error2", result.Match(_ => string.Empty, x => x));
            Assert.AreEqual("error1", result.GetAggregateErrors()[0]);
        }

        [TestMethod]
        public async Task MaybeError_BindAsync_FirstNotValid()
        {
            var first = new Maybe<int, string>("error1");
            var second = new Maybe<int, string>(1);

            var result = await first.BindAsync(second, (x, y) => Task.FromResult(new Maybe<int, string>(x + y)));

            Assert.AreEqual("error1", result.Match(_ => string.Empty, x => x));
            Assert.AreEqual(1, second.Match(x => x, _ => 0));
        }

        [TestMethod]
        public async Task MaybeError_BindAsync_SecondNotValid()
        {
            var first = new Maybe<int, string>(1);
            var second = new Maybe<int, string>("error1");

            var result = await first.BindAsync(second, (x, y) => Task.FromResult(new Maybe<int, string>(x + y)));

            Assert.AreEqual("error1", result.Match(_ => string.Empty, x => x));
            Assert.AreEqual(1, first.Match(x => x, _ => 0));
        }

        [TestMethod]
        public void MaybeError_BindAsync_FirstNotValid_DifferentErrorModels()
        {
            var first = new Maybe<int, (int, string)>((99, "error1"));
            var second = new Maybe<int, string>(1);

            var result = first.Bind(second, x => $"Code: {x.Item1}, Message: {x.Item2}", (x, y) => new Maybe<int, string>(x + y));

            Assert.AreEqual("Code: 99, Message: error1", result.Match(_ => string.Empty, x => x));
        }

        [TestMethod]
        public void MaybeError_Bind_FirstNotValid_DifferentErrorMModels_TupleItem1()
        {
            var error = (99, "error1");
            var first = new Maybe<int, (int, string)>(error);
            var second = new Maybe<int, string>(1);

            var result = first.Bind(second, (x, y) => new Maybe<int, ((int, string), string)>(x + y));

            Assert.AreEqual(error, result.Match(_ => default, x => x.Item1));
        }

        [TestMethod]
        public void MaybeError_Bind_SecondNotValid_DifferentErrorMModels_TupleItem2()
        {
            var error = "error1";
            var first = new Maybe<int, (int, string)>(1);
            var second = new Maybe<int, string>(error);

            var result = first.Bind(second, (x, y) => new Maybe<int, ((int, string), string)>(x + y));

            Assert.AreEqual(error, result.Match(_ => default, x => x.Item2));
        }

        [TestMethod]
        public async Task MaybeTaskError_BindAsync_BothNotValid()
        {
            var first = new Maybe<int, string>("error1");
            var second = Task.FromResult(new Maybe<int, string>("error2"));

            var result = await first.BindAsync(second, (x, y) => Task.FromResult(new Maybe<int, string>(x + y)));

            Assert.AreEqual("error2", result.Match(_ => string.Empty, x => x));
            Assert.AreEqual("error1", result.GetAggregateErrors()[0]);
        }

        [TestMethod]
        public async Task MaybeTaskError_BindAsync_FirstNotValid()
        {
            var first = new Maybe<int, string>("error1");
            var second = Task.FromResult(new Maybe<int, string>(1));

            var result = await first.BindAsync(second, (x, y) => Task.FromResult(new Maybe<int, string>(x + y)));

            Assert.AreEqual("error1", result.Match(_ => string.Empty, x => x));
        }

        [TestMethod]
        public async Task MaybeTaskError_BindAsync_SecondNotValid()
        {
            var first = new Maybe<int, string>(1);
            var second = Task.FromResult(new Maybe<int, string>("error1"));

            var result = await first.BindAsync(second, (x, y) => Task.FromResult(new Maybe<int, string>(x + y)));

            Assert.AreEqual("error1", result.Match(_ => string.Empty, x => x));
            Assert.AreEqual(1, first.Match(x => x, _ => 0));
        }

        [TestMethod]
        public async Task MaybeTaskError_BindAsync_FirstNotValid_DifferentErrorModels()
        {
            var first = new Maybe<int, (int, string)>((99, "error1"));
            var second = Task.FromResult(new Maybe<int, string>(1));

            var result = await first.BindAsync(second, x => $"Code: {x.Item1}, Message: {x.Item2}", (x, y) => Task.FromResult(new Maybe<int, string>(x + y)));

            Assert.AreEqual("Code: 99, Message: error1", result.Match(_ => string.Empty, x => x));
        }

        [TestMethod]
        public async Task MaybeError_Bind_SecondIsTask_ErrorModelConversion()
        {
            var first = new Maybe<int, string>("128");
            var second = Task.FromResult(new Maybe<decimal, byte>(3.0M));
            Func<string, byte> convert = x => byte.Parse(x);
            Func<int, decimal, Maybe<double, byte>> func = (x, y) => new Maybe<double, byte>((double)(x / y));

            var result = await first.BindAsync(second, convert, func);

            Assert.IsTrue(result.Match(_ => false, x => x == 128));
        }

        [TestMethod]
        public async Task MaybeError_Bind_SecondIsTask_TupleErrorModel()
        {
            var first = new Maybe<int, string>("error");
            var second = Task.FromResult(new Maybe<decimal, byte>(3.0M));
            Func<int, decimal, Maybe<double, (string, byte)>> func = (x, y) => new Maybe<double, (string, byte)>((double)(x / y));

            var result = await first.BindAsync(second, func);

            Assert.IsTrue(result.Match(_ => false, x => x.Item1 == "error"));
        }

        [TestMethod]
        public async Task MaybeError_Bind_SecondIsTask_WithValue_ErrorModelConversion()
        {
            var first = new Maybe<int, string>("128");
            var second = Task.FromResult(new Maybe<decimal, byte>(3.0M));
            Func<string, byte> convert = x => byte.Parse(x);
            Func<int, decimal, Task<Maybe<double, byte>>> func = (x, y) => Task.FromResult(new Maybe<double, byte>((double)(x / y)));

            var result = await first.BindAsync(second, convert, func);

            Assert.IsTrue(result.Match(_ => false, x => x == 128));
        }

        [TestMethod]
        public async Task MaybeError_Bind_SecondIsTask_WithValue_TupleErrorModel()
        {
            var first = new Maybe<int, string>("error");
            var second = Task.FromResult(new Maybe<decimal, byte>(3.0M));
            Func<int, decimal, Task<Maybe<double, (string, byte)>>> func = (x, y) => Task.FromResult(new Maybe<double, (string, byte)>((double)(x / y)));

            var result = await first.BindAsync(second, func);

            Assert.IsTrue(result.Match(_ => false, x => x.Item1 == "error"));
        }

        [TestMethod]
        public async Task MaybeError_Bind_FirstIsTask_ErrorModelConversion()
        {
            var first = Task.FromResult(new Maybe<int, string>("128"));
            var second = new Maybe<decimal, byte>(3.0M);
            Func<string, byte> convert = x => byte.Parse(x);
            Func<int, decimal, Maybe<double, byte>> func = (x, y) => new Maybe<double, byte>((double)(x / y));

            var result = await first.BindAsync(second, convert, func);

            Assert.IsTrue(result.Match(_ => false, x => x == 128));
        }

        [TestMethod]
        public async Task MaybeError_Bind_FirstIsTask_TupleErrorModel()
        {
            var first = Task.FromResult(new Maybe<int, string>("error"));
            var second = new Maybe<decimal, byte>(3.0M);
            Func<int, decimal, Maybe<double, (string, byte)>> func = (x, y) => new Maybe<double, (string, byte)>((double)(x / y));

            var result = await first.BindAsync(second, func);

            Assert.IsTrue(result.Match(_ => false, x => x.Item1 == "error"));
        }

        [TestMethod]
        public async Task MaybeError_Bind_FirstIsTask_WithValue_ErrorModelConversion()
        {
            var first = Task.FromResult(new Maybe<int, string>("128"));
            var second = new Maybe<decimal, byte>(3.0M);
            Func<string, byte> convert = x => byte.Parse(x);
            Func<int, decimal, Task<Maybe<double, byte>>> func = (x, y) => Task.FromResult(new Maybe<double, byte>((double)(x / y)));

            var result = await first.BindAsync(second, convert, func);

            Assert.IsTrue(result.Match(_ => false, x => x == 128));
        }

        [TestMethod]
        public async Task MaybeError_Bind_FirstIsTask_WithValue_TupleErrorModel()
        {
            var first = Task.FromResult(new Maybe<int, string>("error"));
            var second = new Maybe<decimal, byte>(3.0M);
            Func<int, decimal, Task<Maybe<double, (string, byte)>>> func = (x, y) => Task.FromResult(new Maybe<double, (string, byte)>((double)(x / y)));

            var result = await first.BindAsync(second, func);

            Assert.IsTrue(result.Match(_ => false, x => x.Item1 == "error"));
        }

        [TestMethod]
        public async Task MaybeError_Bind_FirstAndSecondMaybe_TupleErrorModel()
        {
            var first = Task.FromResult(new Maybe<int, string>("error"));
            var second = Task.FromResult(new Maybe<decimal, byte>(3.0M));
            Func<int, decimal, Maybe<double, (string, byte)>> func = (x, y) => new Maybe<double, (string, byte)>((double)(x / y));

            var result = await first.BindAsync(second, func);

            Assert.IsTrue(result.Match(_ => false, x => x.Item1 == "error"));
        }

        [TestMethod]
        public async Task MaybeError_Bind_FirstAndSecondMaybe_ErrorModelConversion()
        {
            var first = Task.FromResult(new Maybe<int, string>("128"));
            var second = Task.FromResult(new Maybe<decimal, byte>(3.0M));
            Func<string, byte> convert = x => byte.Parse(x);
            Func<int, decimal, Maybe<double, byte>> func = (x, y) => new Maybe<double, byte>((double)(x / y));

            var result = await first.BindAsync(second, convert, func);

            Assert.IsTrue(result.Match(_ => false, x => x == 128));
        }

        [TestMethod]
        public async Task MaybeError_Bind_FirstAndSecondMaybeIsTask_ErrorModelConversion()
        {
            var first = Task.FromResult(new Maybe<int, string>("128"));
            var second = Task.FromResult(new Maybe<decimal, byte>(3.0M));
            Func<string, byte> convert = x => byte.Parse(x);
            Func<int, decimal, Task<Maybe<double, byte>>> func = (x, y) => Task.FromResult(new Maybe<double, byte>((double)(x / y)));

            var result = await first.BindAsync(second, convert, func);

            Assert.IsTrue(result.Match(_ => false, x => x == 128));
        }

        [TestMethod]
        public async Task MaybeError_Bind_FirstAndSecondMaybeIsTask_TupleErrorModel()
        {
            var first = Task.FromResult(new Maybe<int, string>("error"));
            var second = Task.FromResult(new Maybe<decimal, byte>(3.0M));
            Func<int, decimal, Task<Maybe<double, (string, byte)>>> func = (x, y) => Task.FromResult(new Maybe<double, (string, byte)>((double)(x / y)));

            var result = await first.BindAsync(second, func);

            Assert.IsTrue(result.Match(_ => false, x => x.Item1 == "error"));
        }

        #endregion

        #region Maybe Value

        [TestMethod]
        public async Task MaybeValue_BindAsync_BothAreValid()
        {
            var first = new Maybe<int, string>(1);
            var second = new Maybe<int, string>(2);

            var result = await first.BindAsync(second, (x, y) => Task.FromResult(new Maybe<int, string>(x + y)));

            Assert.AreEqual(3, result.Match(x => x, _ => 0));
        }

        [TestMethod]
        public async Task MaybeValue_BindAsyn_BothAreValid()
        {
            var first = new Maybe<int, string>(1);
            var second = Task.FromResult(new Maybe<int, string>(2));

            var result = await first.BindAsync(second, (x, y) => new Maybe<int, string>(x + y));

            Assert.AreEqual(3, result.Match(x => x, _ => 0));
        }

        [TestMethod]
        public async Task MaybeTaskValue_BindAsync_BothAreValid()
        {
            var first = new Maybe<int, string>(1);
            var second = Task.FromResult(new Maybe<int, string>(2));

            var result = await first.BindAsync(second, (x, y) => Task.FromResult(new Maybe<int, string>(x + y)));

            Assert.AreEqual(3, result.Match(x => x, _ => 0));
        }

        [TestMethod]
        public void MaybeValue_Bind_FirstAndSecond()
        {
            var maybe = new Maybe<int, string>(1);

            var result = maybe.Bind(maybe.With(1), (x, y) => maybe.With(x + y));

            Assert.IsTrue(result.Match(x => x == 2, _ => false));
        }

        [TestMethod]
        public void MaybeValue_Bind_OneAggregate()
        {
            var maybe = new Maybe<int, string>(1);

            var result1 = maybe.Bind(maybe.With(1), (x, y) => maybe.With(x + y));
            var result2 = result1.Bind(maybe.With(1), (x, y) => maybe.With(x + y));

            Assert.IsTrue(result2.Match(x => x == 3, _ => false));
        }

        [TestMethod]
        public void MaybeValue_Bind_BothAggregate()
        {
            var maybe1 = new Maybe<int, string>(1);
            var result1 = maybe1.Bind(maybe1.With(1), (x, y) => maybe1.With(x + y));

            var maybe2 = new Maybe<int, string>(1);
            var result2 = maybe2.Bind(maybe2.With(1), (x, y) => maybe2.With(x + y));

            var aggregate = result1.Bind(result2, (x, y) => new Maybe<int, string>(x + y));

            Assert.IsTrue(aggregate.Match(x => x == 4, _ => false));
        }

        [TestMethod]
        public async Task MaybeValue_Bind_FirstMaybe()
        {
            var first = Task.FromResult(new Maybe<int, string>(2));
            var second = new Maybe<int, string>(3);
            Func<int, int, Maybe<int, string>> func = (x, y) => new Maybe<int, string>(x + y);

            var result = await first.BindAsync(second, func);

            Assert.IsTrue(result.Match(x => x == 5, _ => false));
        }

        [TestMethod]
        public async Task MaybeValue_Bind_FirstMaybeIsTask()
        {
            var first = Task.FromResult(new Maybe<int, string>(2));
            var second = new Maybe<int, string>(3);
            Func<int, int, Task<Maybe<int, string>>> func = (x, y) => Task.FromResult(new Maybe<int, string>(x + y));

            var result = await first.BindAsync(second, func);

            Assert.IsTrue(result.Match(x => x == 5, _ => false));
        }

        [TestMethod]
        public async Task MaybeValue_Bind_SecondMaybeIsTask()
        {
            var first = new Maybe<int, string>(2);
            var second = Task.FromResult(new Maybe<int, string>(3));
            Func<int, int, Task<Maybe<int, string>>> func = (x, y) => Task.FromResult(new Maybe<int, string>(x + y));

            var result = await first.BindAsync(second, func);

            Assert.IsTrue(result.Match(x => x == 5, _ => false));
        }

        [TestMethod]
        public async Task MaybeValue_Bind_FirstAndSecondMaybeIsTask()
        {
            var first = Task.FromResult(new Maybe<int, string>(2));
            var second = Task.FromResult(new Maybe<decimal, string>(3.0M));
            Func<int, decimal, Task<Maybe<double, string>>> func = (x, y) => Task.FromResult(new Maybe<double, string>((double)(x / y)));

            var result = await first.BindAsync(second, func);

            Assert.AreEqual((double)(2 / 3M), result.Match(x => x, _ => 0D));
        }

        [TestMethod]
        public async Task Maybe_MatchAsync_Valid_sync_async()
        {
            var maybe = Maybe.Value(1).Error<string>();

            var result = await maybe.MatchAsync(x => Task.FromResult(x + 1), _ => 0);

            Assert.AreEqual(2, result);
        }

        [TestMethod]
        public async Task Maybe_MatchAsync_Invalid_sync_async()
        {
            var maybe = Maybe.Value<int>().Error("error");

            var result = await maybe.MatchAsync(_ => Task.FromResult(string.Empty), x => x);

            Assert.AreEqual("error", result);
        }

        [TestMethod]
        public async Task Maybe_MatchAsync_Aggregate_sync_async()
        {
            var maybe = Maybe.Value<int>().Error("error1");

            var result = await maybe.Bind(maybe.With("error2"), (x, y) => maybe.With(x + y)).MatchAsync(_ => Task.FromResult(default((string, string[]))), (x, y) => (x, y));

            Assert.AreEqual("error2", result.Item1);
            Assert.AreEqual("error1", result.Item2[0]);
            Assert.AreEqual(1, result.Item2.Length);
        }

        #endregion

        #region Maybe Miscellaneous

        [TestMethod]
        public void Maybe_Create_Value()
        {
            var value = 42;

            var result = Maybe.Value(value).Error<string>();

            Assert.IsTrue(result.Match(x => x == value, _ => false));
        }

        [TestMethod]
        public void Maybe_Create_Error()
        {
            var error = "error";

            var result = Maybe.Value<int>().Error(error);

            Assert.IsTrue(result.Match(_ => false, x => x == error));
        }

        [TestMethod]
        public void Maybe_With_Response()
        {
            var response = new Response<int>(42);
            var maybe = Maybe.Create<int, string>();

            var result = maybe.With(response, "error");

            Assert.AreEqual(response, result.Match(x => x, _ => new Response<int>()));
        }

        [TestMethod]
        public void Maybe_With_Response_Invalid()
        {
            var response = new Response<int>();
            var maybe = Maybe.Create<int, string>();

            var result = maybe.With(response, "error");

            Assert.AreEqual("error", result.Match(_ => string.Empty, x => x));
        }

        [TestMethod]
        public void Maybe_With_Either()
        {
            var either = new Either<int, string>(42);
            var maybe = Maybe.Create<int, string>();

            var result = maybe.With(either);

            Assert.AreEqual(either.Match(x => x, _ => -1), result.Match(x => x, _ => 1));
        }

        [TestMethod]
        public void Maybe_With_Value()
        {
            var value = 42;
            var maybe = Maybe.Create<int, string>();

            var result = maybe.With(value);

            Assert.AreEqual(value, result.Match(x => x, _ => 0));
        }

        [TestMethod]
        public void Maybe_With_Error()
        {
            var error = "error";
            var maybe = Maybe.Create<int, string>();

            var result = maybe.With(error);

            Assert.AreEqual(error, result.Match(_ => string.Empty, x => x));
        }

        [TestMethod]
        public void Maybe_GetValueOrDefault_Valid()
        {
            var answer = 42;
            var maybe = Maybe.Value(answer).Error<string>();

            var result = maybe.GetValueOrDefault(1);

            Assert.AreEqual(answer, result);
        }

        [TestMethod]
        public void Maybe_GetValueOrDefault_Invalid()
        {
            var @default = 1;
            var maybe = Maybe.Value<int>().Error("error");

            var result = maybe.GetValueOrDefault(@default);

            Assert.AreEqual(@default, result);
        }

        [TestMethod]
        public void Maybe_ToEither_Valid()
        {
            var maybe = Maybe.Value(42).Error<string>();

            var result = maybe.ToEither();

            Assert.IsTrue(result.Match(x => x == 42, _ => false));
        }

        [TestMethod]
        public void Maybe_ToEither_Invalid()
        {
            var maybe = Maybe.Value<int>().Error("error");

            var result = maybe.ToEither();

            Assert.IsTrue(result.Match(_ => false, x => x == "error"));
        }

        [TestMethod]
        public void Maybe_ToResponse_Valid()
        {
            var maybe = Maybe.Value(42).Error<string>();

            var result = maybe.ToResponse();

            Assert.IsTrue(result);
            Assert.AreEqual(42, result);
        }

        [TestMethod]
        public void Maybe_ToResponse_Invalid()
        {
            var maybe = Maybe.Value<int>().Error("error");

            var result = maybe.ToResponse();

            Assert.IsFalse(result);
        }

        [TestMethod]
        public async Task Maybe_GetValueOrDefaultAsync_Valid()
        {
            var answer = 42;
            var maybe = Task.FromResult(Maybe.Value(answer).Error<string>());

            var result = await maybe.GetValueOrDefaultAsync(1);

            Assert.AreEqual(answer, result);
        }

        [TestMethod]
        public async Task Maybe_GetValueOrDefaultAsync_Invalid()
        {
            var @default = 1;
            var maybe = Task.FromResult(Maybe.Value<int>().Error("error"));

            var result = await maybe.GetValueOrDefaultAsync(@default);

            Assert.AreEqual(@default, result);
        }

        [TestMethod]
        public async Task Maybe_ToEitherAsync_Valid()
        {
            var maybe = Task.FromResult(Maybe.Value(42).Error<string>());

            var result = await maybe.ToEitherAsync();

            Assert.IsTrue(result.Match(x => x == 42, _ => false));
        }

        [TestMethod]
        public async Task Maybe_ToEitherAsync_Invalid()
        {
            var maybe = Task.FromResult(Maybe.Value<int>().Error("error"));

            var result = await maybe.ToEitherAsync();

            Assert.IsTrue(result.Match(_ => false, x => x == "error"));
        }

        [TestMethod]
        public async Task Maybe_ToResponseAsync_Valid()
        {
            var maybe = Task.FromResult(Maybe.Value(42).Error<string>());

            var result = await maybe.ToResponseAsync();

            Assert.IsTrue(result);
            Assert.AreEqual(42, result);
        }

        [TestMethod]
        public async Task Maybe_ToResponseAsync_Invalid()
        {
            var maybe = Task.FromResult(Maybe.Value<int>().Error("error"));

            var result = await maybe.ToResponseAsync();

            Assert.IsFalse(result);
        }

        [TestMethod]
        public void Maybe_ToString_Valid()
        {
            var maybe = Maybe.Value(42).Error<string>();

            var result = maybe.ToString();

            Assert.AreEqual("42", result);
        }

        [TestMethod]
        public void Maybe_ToString_InValid()
        {
            var maybe = Maybe.Value<int>().Error("error");

            var result = maybe.ToString();

            Assert.AreEqual("error", result);
        }

        [TestMethod]
        public void Maybe_Implicit_Response()
        {
            var maybe = Maybe.Value(42).Error<string>();

            Response<int> response = maybe;

            Assert.AreEqual(42, response);
        }

        [TestMethod]
        public void Maybe_Implicit_Response_Error()
        {
            var maybe = Maybe.Value<int>().Error("error");

            Response<int> response = maybe;

            Assert.IsFalse(response);
        }

        [TestMethod]
        public void Maybe_Implicit_Either()
        {
            var maybe = Maybe.Value(42).Error<string>();

            Either<int, string> either = maybe;

            Assert.AreEqual(42, either.Match(x => x, _ => 0));
        }

        [TestMethod]
        public void Maybe_Implicit_Either_Error()
        {
            var maybe = Maybe.Value<int>().Error("error");

            Either<int, string> either = maybe;

            Assert.AreEqual("error", either.Match(_ => string.Empty, x => x));
        }

        [TestMethod]
        public void Maybe_Implicit_Maybe()
        {
            var either = new Either<int, string>(42);

            Maybe<int, string> maybe = either;

            Assert.AreEqual(either.Match(x => x, _ => -1), maybe.Match(x => x, _ => 1));
        }

        [TestMethod]
        public void Maybe_Implicit_Maybe_Error()
        {
            var either = new Either<int, string>("error");

            Maybe<int, string> maybe = either;

            Assert.AreEqual(either.Match(_ => "X", x => x), maybe.Match(_ => "Y", y => y));
        }

        [TestMethod]
        public void Maybe_Implicit_Value()
        {
            var value = 42;

            Maybe<int, string> maybe = value;

            Assert.AreEqual(value, maybe.Match(x => x, _ => 0));
        }

        [TestMethod]
        public void Maybe_Implicit_Error()
        {
            var error = "error";

            Maybe<int, string> maybe = error;

            Assert.AreEqual(error, maybe.Match(_ => string.Empty, x => x));
        }

        #endregion

        #region Maybe Match

        [TestMethod]
        public void Maybe_Match_Value_Sync_Sync()
        {
            var answer = 42;
            var maybe = Maybe.Value(answer).Error<string>();

            var result = maybe.Match(x => x, _ => 0);

            Assert.AreEqual(answer, result);
        }

        [TestMethod]
        public void Maybe_Match_Error_Sync_Sync()
        {
            var error = "error";
            var maybe = Maybe.Value<int>().Error(error);

            var result = maybe.Match(_ => string.Empty, x => x);

            Assert.AreEqual(error, result);
        }

        [TestMethod]
        public void Maybe_Match_Error_Aggregate_Sync_Sync()
        {
            string error1 = "error1", error2 = "error2", error = null;
            var maybe = Maybe.Value<int>().Error(error1);

            var aggregate = maybe.Bind(maybe.With(error2), (x, y) => maybe.With(x + y));
            var result = aggregate.Match(_ => string.Empty, (x, y) => { error = y[0]; return x; });

            Assert.AreEqual(error1, error);
            Assert.AreEqual(error2, result);
        }

        [TestMethod]
        public async Task Maybe_Match_Value_Sync_Async()
        {
            var answer = 42;
            var maybe = Maybe.Value(answer).Error<string>();

            var result = await maybe.MatchAsync(x => Task.FromResult(x), _ => 0);

            Assert.AreEqual(answer, result);
        }

        [TestMethod]
        public async Task Maybe_Match_Error_Sync_Async()
        {
            var error = "error";
            var maybe = Maybe.Value<int>().Error(error);

            var result = await maybe.MatchAsync(_ => Task.FromResult(string.Empty), x => x);

            Assert.AreEqual(error, result);
        }

        [TestMethod]
        public async Task Maybe_Match_Error_Aggregate_Sync_Async()
        {
            string error1 = "error1", error2 = "error2", error = null;
            var maybe = Maybe.Value<int>().Error(error1);

            var aggregate = maybe.Bind(maybe.With(error2), (x, y) => maybe.With(x + y));
            var result = await aggregate.MatchAsync(_ => Task.FromResult(string.Empty), (x, y) => { error = y[0]; return x; });

            Assert.AreEqual(error1, error);
            Assert.AreEqual(error2, result);
        }

        [TestMethod]
        public async Task Maybe_Match_Value_Async_Sync()
        {
            var answer = 42;
            var maybe = Task.FromResult(Maybe.Value(answer).Error<string>());

            var result = await maybe.MatchAsync(x => x, _ => 0);

            Assert.AreEqual(answer, result);
        }

        [TestMethod]
        public async Task Maybe_Match_Error_Async_Sync()
        {
            var error = "error";
            var maybe = Task.FromResult(Maybe.Value<int>().Error(error));

            var result = await maybe.MatchAsync(_ => string.Empty, x => x);

            Assert.AreEqual(error, result);
        }

        [TestMethod]
        public async Task Maybe_Match_Error_Aggregate_Async_Sync()
        {
            string error1 = "error1", error2 = "error2", error = null;
            var maybe = Task.FromResult(Maybe.Value<int>().Error(error1));

            var aggregate = maybe.BindAsync(maybe.With(error2), (x, y) => maybe.With(x + y));
            var result = await aggregate.MatchAsync(_ => string.Empty, (x, y) => { error = y[0]; return x; });

            Assert.AreEqual(error1, error);
            Assert.AreEqual(error2, result);
        }

        [TestMethod]
        public async Task Maybe_Match_Value_Async_Async()
        {
            var answer = 42;
            var maybe = Task.FromResult(Maybe.Value(answer).Error<string>());

            var result = await maybe.MatchAsync(x => Task.FromResult(x), _ => 0);

            Assert.AreEqual(answer, result);
        }

        [TestMethod]
        public async Task Maybe_Match_Error_Async_Async()
        {
            var error = "error";
            var maybe = Task.FromResult(Maybe.Value<int>().Error(error));

            var result = await maybe.MatchAsync(_ => Task.FromResult(string.Empty), x => x);

            Assert.AreEqual(error, result);
        }

        [TestMethod]
        public async Task Maybe_Match_Error_Aggregate_Async_Async()
        {
            string error1 = "error1", error2 = "error2", error = null;
            var maybe = Task.FromResult(Maybe.Value<int>().Error(error1));

            var aggregate = maybe.BindAsync(maybe.With(error2), (x, y) => maybe.With(x + y));
            var result = await aggregate.MatchAsync(_ => Task.FromResult(string.Empty), (x, y) => { error = y[0]; return x; });

            Assert.AreEqual(error1, error);
            Assert.AreEqual(error2, result);
        }

        #endregion

        #region Transform

        [TestMethod]
        public void Maybe_Transform_Value()
        {
            var wasCalled = false;
            var maybe = new Maybe<int, string>(1);

            var result = maybe.Transform(x => { wasCalled = true; return x + 1; });

            Assert.IsTrue(wasCalled);
            Assert.IsTrue(maybe.Match(x => x == 1, _ => false));
            Assert.IsTrue(result.Match(x => x == 2, _ => false));
        }

        [TestMethod]
        public void Maybe_Transform_Error()
        {
            var wasCalled = false;
            var maybe = new Maybe<int, string>("error");

            var result = maybe.Transform(x => { wasCalled = true; return x + 1; });

            Assert.IsFalse(wasCalled);
            Assert.IsTrue(maybe.Match(_ => false, x => x == "error"));
            Assert.IsTrue(result.Match(_ => false, x => x == "error"));
        }

        [TestMethod]
        public async Task Maybe_TransformAsync_Func_Value()
        {
            var wasCalled = false;
            var maybe = new Maybe<int, string>(1);

            var result = await maybe.TransformAsync(x => { wasCalled = true; return Task.FromResult(x + 1); });

            Assert.IsTrue(wasCalled);
            Assert.IsTrue(maybe.Match(x => x == 1, _ => false));
            Assert.IsTrue(result.Match(x => x == 2, _ => false));
        }

        [TestMethod]
        public async Task Maybe_TransformAsync__Func_Error()
        {
            var wasCalled = false;
            var maybe = new Maybe<int, string>("error");

            var result = await maybe.TransformAsync(x => { wasCalled = true; return Task.FromResult(x + 1); });

            Assert.IsFalse(wasCalled);
            Assert.IsTrue(maybe.Match(_ => false, x => x == "error"));
            Assert.IsTrue(result.Match(_ => false, x => x == "error"));
        }

        [TestMethod]
        public async Task Maybe_Transform_Maybe_Value()
        {
            var wasCalled = false;
            var maybe = Task.FromResult(new Maybe<int, string>(1));

            var result = await maybe.TransformAsync(x => { wasCalled = true; return x + 1; });

            Assert.IsTrue(wasCalled);
            Assert.IsTrue(await maybe.MatchAsync(x => x == 1, _ => false));
            Assert.IsTrue(result.Match(x => x == 2, _ => false));
        }

        [TestMethod]
        public async Task Maybe_Transform_Maybe_Error()
        {
            var wasCalled = false;
            var maybe = Task.FromResult(new Maybe<int, string>("error"));

            var result = await maybe.TransformAsync(x => { wasCalled = true; return x + 1; });

            Assert.IsFalse(wasCalled);
            Assert.IsTrue(await maybe.MatchAsync(_ => false, x => x == "error"));
            Assert.IsTrue(result.Match(_ => false, x => x == "error"));
        }

        [TestMethod]
        public async Task Maybe_Transform_MaybeFunc_Value()
        {
            var wasCalled = false;
            var maybe = Task.FromResult(new Maybe<int, string>(1));

            var result = await maybe.TransformAsync(x => { wasCalled = true; return Task.FromResult(x + 1); });

            Assert.IsTrue(wasCalled);
            Assert.IsTrue(await maybe.MatchAsync(x => x == 1, _ => false));
            Assert.IsTrue(result.Match(x => x == 2, _ => false));
        }

        [TestMethod]
        public async Task Maybe_Transform_MaybeFunc_Error()
        {
            var wasCalled = false;
            var maybe = Task.FromResult(new Maybe<int, string>("error"));

            var result = await maybe.TransformAsync(x => { wasCalled = true; return Task.FromResult(x + 1); });

            Assert.IsFalse(wasCalled);
            Assert.IsTrue(await maybe.MatchAsync(_ => false, x => x == "error"));
            Assert.IsTrue(result.Match(_ => false, x => x == "error"));
        }

        #endregion
    }

    public static class MaybeTestsExtensions
    {
        private static class Cache<TValue, TError>
        {
            private static readonly TError[] _errors = new TError[0];
            public static TError[] WhenValue(TValue _) => _errors;
            public static TError[] WhenError(TError _, TError[] aggregateErrors) => aggregateErrors;
        }

        public static TError[] GetAggregateErrors<TValue, TError>(this Maybe<TValue, TError> maybe)
        {
            return maybe.Match(Cache<TValue, TError>.WhenValue, Cache<TValue, TError>.WhenError);
        }
    }
}
