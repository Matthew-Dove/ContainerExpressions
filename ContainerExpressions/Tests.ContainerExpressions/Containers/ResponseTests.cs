using Microsoft.VisualStudio.TestTools.UnitTesting;
using ContainerExpressions.Containers;
using System;

namespace Tests.ContainerExpressions.Containters
{
    [TestClass]
    public class ResponseTests
    {
        #region Response

        [TestMethod]
        public void Response_HasValue_IsValid()
        {
            var response = Response.Create(10);

            Assert.IsTrue(response);
        }

        [TestMethod]
        public void Response_HasNoValue_IsNotValid()
        {
            var response = new Response<int>();

            Assert.IsFalse(response);
        }

        [TestMethod]
        public void Response_ToString_ReturnsUnderlyingValueToString()
        {
            var value = 10;
            var response = new Response<int>(value);

            Assert.AreEqual(value.ToString(), response.ToString());
        }

        [TestMethod]
        public void Response_Convert_ToT()
        {
            var value = 10;
            var response = new Response<int>(value);

            int result = response;

            Assert.AreEqual(value, result);
        }

        [TestMethod]
        public void Response_Convert_ToBool()
        {
            var response = new Response<int>(10);

            bool value = response;

            Assert.AreEqual(value, response.IsValid);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void Response_GetValueWithInValidState_Error()
        {
            var response = new Response<int>();

            int value = response;
        }

        [TestMethod]
        public void Response_GetValueOrDefaultWithInValidState_ReturnsDefault()
        {
            var defaultValue = 42;
            var response = new Response<int>();

            int value = response.GetValueOrDefault(defaultValue);

            Assert.AreEqual(defaultValue, value);
        }

        [TestMethod]
        public void Response_GetValueOrDefaultWithValidState_ReturnsUnderlyingValue()
        {
            var result = 42;
            var response = new Response<int>(result);

            int value = response.GetValueOrDefault(result * -1);

            Assert.AreEqual(result, value);
        }

        [TestMethod]
        public void Response_WithValue_IsValid()
        {
            var responseWithNoValue = new Response<int>();
            var responseWithValue = new Response<int>(42);

            responseWithNoValue = responseWithNoValue.WithValue(42);
            responseWithValue = responseWithValue.WithValue(42);

            Assert.IsTrue(responseWithNoValue);
            Assert.IsTrue(responseWithValue);
        }

        [TestMethod]
        public void Response_WithNoValue_IsNotValid()
        {
            var responseWithValue = new Response<int>(42);
            var responseWithNoValue = new Response<int>();

            responseWithValue = responseWithValue.WithNoValue();
            responseWithNoValue = responseWithNoValue.WithNoValue();

            Assert.IsFalse(responseWithValue);
            Assert.IsFalse(responseWithNoValue);
        }

        #endregion

        #region BaseResponse

        [TestMethod]
        public void Response_HasValue_IValid()
        {
            var response = new Response(true);

            Assert.IsTrue(response);
        }

        [TestMethod]
        public void BaseResponse_False_IsNotValid()
        {
            var response = new Response(false);

            Assert.IsFalse(response);
        }

        [TestMethod]
        public void BaseResponse_ToString_ReturnsStringValueOfIsValid()
        {
            var isValid = true;
            var response = new Response(isValid);

            Assert.AreEqual(isValid.ToString(), response.ToString());
        }

        [TestMethod]
        public void BaseResponse_AsValid_IsValid()
        {
            var responseFalse = new Response();
            var responseTrue = new Response(true);

            responseFalse = responseFalse.AsValid();
            responseTrue = responseTrue.AsValid();

            Assert.IsTrue(responseFalse);
            Assert.IsTrue(responseTrue);
        }

        [TestMethod]
        public void BaseResponse_AsInvalid_IsNotValid()
        {
            var responseTrue = new Response(true);
            var responseFalse = new Response();

            responseTrue = responseTrue.AsInvalid();
            responseFalse = responseFalse.AsInvalid();

            Assert.IsFalse(responseTrue);
            Assert.IsFalse(responseFalse);
        }

        [TestMethod]
        public void BaseResponse_Create_InvalidResponseT()
        {
            var response = Response.Create<int>();

            Assert.IsFalse(response);
        }

        [TestMethod]
        public void BaseResponse_Create_ValidResponseT()
        {
            var response = Response.Create(42);

            Assert.IsTrue(response);
        }

        [TestMethod]
        public void BaseResponse_Convert_ToBool()
        {
            var response = new Response();

            bool value = response;

            Assert.AreEqual(value, response.IsValid);
        }

        #endregion
    }
}
