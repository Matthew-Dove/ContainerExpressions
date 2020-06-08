using Microsoft.VisualStudio.TestTools.UnitTesting;
using ContainerExpressions.Containers;
using System;
using System.Threading.Tasks;
using ContainerExpressions.Expressions;

namespace Tests.ContainerExpressions.Containters
{
    [TestClass]
    public class ResponseTests
    {
        #region Response

        [TestMethod]
        public void Response_CreateFromTValue_IsValid()
        {
            var value = 42;

            var response = value.ToResponse();

            Assert.IsTrue(response);
        }

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
        public void Response_ToString_InvalidState_EmptyString()
        {
            var response = new Response<int>();

            Assert.AreEqual(string.Empty, response.ToString());
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
        public void ResponseT_Convert_ToResponse()
        {
            var response = new Response<int>(10);

            Response value = response;

            Assert.AreEqual(value.IsValid, response.IsValid);
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
        public void Response_IsValid_BindPropagates_Lift()
        {
            var answer = 42;
            Func<int, int> @double = x => x * 2;

            var response = Response.Create(answer).Bind(@double.Lift());

            Assert.IsTrue(response);
            Assert.AreEqual(@double(answer), response);
        }

        [TestMethod]
        public void LiftWillWorkInCompose()
        {
            Func<int> answer = () => 42;

            var stringAnswer = Expression.Compose(answer.Lift(), x => Response.Create(x.ToString()));

            Assert.AreEqual(answer().ToString(), stringAnswer);
        }

        [TestMethod]
        public async Task Response_IsValid_BindPropagates_LiftAsync()
        {
            var answer = 42;
            Func<int, Task<int>> @double = x => Task.FromResult(x * 2);

            var response = await Response.Create(answer).BindAsync(@double.LiftAsync());

            Assert.IsTrue(response);
            Assert.AreEqual(await @double(answer), response);
        }

        [TestMethod]
        public async Task Response_IsValid_CallsBindAsync_OnTask()
        {
            var answer = Task.FromResult(Response.Create(42));

            var doubleTheAnswer = await answer.BindAsync(x => Task.FromResult(Response.Create(x * 2)));

            Assert.AreEqual(answer.Result * 2, doubleTheAnswer);
        }

        [TestMethod]
        public void Response_IsNotValid_BindDoesNotPropagates()
        {
            var isBindRan = false;
            Func<int, Response<string>> binder = x => { isBindRan = true; return new Response<string>(); };

            var response = new Response<int>().Bind(binder);

            Assert.IsFalse(response);
            Assert.IsFalse(isBindRan);
        }

        [TestMethod]
        public void Bind_From_TValue()
        {
            var userId = 1337;
            var username = "John Smith";
            Func<int, Response<string>> getUsername = id => Response.Create(username);

            var result = userId.BindValue(getUsername);

            Assert.AreEqual(username, result);
        }

        [TestMethod]
        public async Task Bind_From_AsyncTValue()
        {
            var userId = Task.FromResult(1337);
            var username = "John Smith";
            Func<int, Response<string>> getUsername = id => Response.Create(username);

            var result = await userId.BindValueAsync(getUsername);

            Assert.AreEqual(username, result);
        }

        [TestMethod]
        public async Task Bind_From_TValue_Async()
        {
            var userId = 1337;
            var username = "John Smith";
            Func<int, Task<Response<string>>> getUsername = id => Task.FromResult(Response.Create(username));

            var result = await userId.BindValueAsync(getUsername);

            Assert.AreEqual(username, result);
        }

        [TestMethod]
        public async Task Bind_From_AsyncTValue_Async()
        {
            var userId = Task.FromResult(1337);
            var username = "John Smith";
            Func<int, Task<Response<string>>> getUsername = id => Task.FromResult(Response.Create(username));

            var result = await userId.BindValueAsync(getUsername);

            Assert.AreEqual(username, result);
        }

        [TestMethod]
        public void Response_IsValid_TransformPropagates()
        {
            var answer = 42;
            var isInvoked = false;
            Func<string, int> intParse = x => { isInvoked = true; return int.Parse(x); };

            var response = Response.Create(answer.ToString()).Transform(intParse);

            Assert.IsTrue(response);
            Assert.AreEqual(answer, response);
            Assert.IsTrue(isInvoked);
        }

        [TestMethod]
        public void Response_IsNotValid_TransformDoesNotPropagates()
        {
            var isInvoked = false;
            Func<string, int> intParse = x => { isInvoked = true; return int.Parse(x); };

            var response = new Response<string>().Transform(intParse);

            Assert.IsFalse(response);
            Assert.IsFalse(isInvoked);
        }

        [TestMethod]
        public void Response_IsNotValid_PivotNotValid()
        {
            var input = new Response<string>();
            Func<string, Response<int>> trueFunc = x => { throw new InvalidOperationException(); };
            Func<string, Response<int>> falseFunc = x => { throw new InvalidOperationException(); };

            var response = input.Pivot(true, trueFunc, falseFunc);

            Assert.IsFalse(response);
        }

        [TestMethod]
        public void Response_IsValid_ConditionIsTrue_Func1Invoked()
        {
            var isInvoked = false;
            var answer = 42;
            var input = Response.Create(answer.ToString());
            Func<string, Response<int>> trueFunc = x => { isInvoked = true; return Response.Create(int.Parse(x)); };
            Func<string, Response<int>> falseFunc = x => { throw new InvalidOperationException(); };

            var response = input.Pivot(true, trueFunc, falseFunc);

            Assert.IsTrue(isInvoked);
            Assert.IsTrue(response);
            Assert.AreEqual(answer, response);
        }

        [TestMethod]
        public void Response_IsValid_ConditionIsTrue_Func2Invoked()
        {
            var isInvoked = false;
            var answer = 42;
            var input = Response.Create(answer.ToString());
            Func<string, Response<int>> trueFunc = x => { throw new InvalidOperationException(); };
            Func<string, Response<int>> falseFunc = x => { isInvoked = true; return Response.Create(int.Parse(x)); };

            var response = input.Pivot(false, trueFunc, falseFunc);

            Assert.IsTrue(isInvoked);
            Assert.IsTrue(response);
            Assert.AreEqual(answer, response);
        }

        [TestMethod]
        public void Response_IsValid_FuncIsInvoked()
        {
            var answer = 42;
            var response = new Response<int>(answer);

            var result = response.IsTrue(x => x == answer);

            Assert.IsTrue(result);
        }

        [TestMethod]
        public void Response_IsNotValid_FuncIsNotInvoked()
        {
            var answer = 42;
            var response = new Response<int>();

            var result = response.IsTrue(x => x == answer);

            Assert.IsFalse(result);
        }

        [TestMethod]
        public void BothResponses_AreValid_FunnelFunc_IsInvoked()
        {
            var response1 = new Response<bool>(true);
            var response2 = new Response<bool>(false);

            var response = Expression.Funnel(response1, response2, (x, y) => false);

            Assert.IsTrue(response.IsValid);
            Assert.IsFalse(response.Value);
        }

        [TestMethod]
        public void OneResponse_IsNotValid_FunnelFunc_IsNotInvoked()
        {
            var isInvoked = false;
            var response1 = new Response<bool>(true);
            var response2 = new Response<bool>();

            var response = Expression.Funnel(response1, response2, (x, y) => { isInvoked = true; return Response.Create(false); });

            Assert.IsFalse(response.IsValid);
            Assert.IsFalse(isInvoked);
        }

        [TestMethod]
        public void Response_Lift_Action()
        {
            var response = new Response();
            var isCalled = false;
            Action func = () => { isCalled = true; };

            var lift = func.Lift();
            Assert.IsFalse(isCalled);

            var result = response.Bind(lift);
            Assert.IsFalse(isCalled);

            response = response.AsValid();
            result = response.Bind(lift);
            Assert.IsTrue(isCalled);
        }

        [TestMethod]
        public void Response_Lift_Action_WithArg()
        {
            var isCalled = false;
            Action<int> func = _ => { isCalled = true; };

            var lift = func.Lift();
            Assert.IsFalse(isCalled);

            var result = lift(42);

            Assert.IsTrue(result);
            Assert.IsTrue(isCalled);
        }

        [TestMethod]
        public void Response_Lift_Func()
        {
            var answer = 42;
            var isCalled = false;
            Func<int> func = () => { isCalled = true; return answer; };

            var lift = func.Lift();
            Assert.IsFalse(isCalled);

            var result = lift();

            Assert.IsTrue(result);
            Assert.AreEqual(answer, result);
            Assert.IsTrue(isCalled);
        }

        [TestMethod]
        public void Response_Lift_Func_WithArg()
        {
            var answer = 42;
            var isCalled = false;
            Func<int, int> func = x => { isCalled = true; return x; };

            var lift = func.Lift();
            Assert.IsFalse(isCalled);

            var result = lift(answer);

            Assert.IsTrue(result);
            Assert.AreEqual(answer, result);
            Assert.IsTrue(isCalled);
        }

        [TestMethod]
        public async Task Response_Lift_Func_Async()
        {
            var answer = 42;
            var isCalled = false;
            Func<Task<int>> func = () => { isCalled = true; return Task.FromResult(answer); };

            var lift = func.LiftAsync();
            Assert.IsFalse(isCalled);

            var result = await lift();

            Assert.IsTrue(result);
            Assert.AreEqual(answer, result);
            Assert.IsTrue(isCalled);
        }

        [TestMethod]
        public async Task Response_Lift_Func_Async_WithArg()
        {
            var answer = 42;
            var isCalled = false;
            Func<int, Task<int>> func = x => { isCalled = true; return Task.FromResult(x); };

            var lift = func.LiftAsync();
            Assert.IsFalse(isCalled);

            var result = await lift(answer);

            Assert.IsTrue(result);
            Assert.AreEqual(answer, result);
            Assert.IsTrue(isCalled);
        }

        [TestMethod]
        public void Response_Bind_ResponseT_ResponseTResult()
        {
            var answer = 42;
            var response = answer.ToString().ToResponse();
            Func<string, Response<int>> func = x => int.Parse(x).ToResponse();

            var result = response.Bind(func);

            Assert.AreEqual(answer, result);
        }

        [TestMethod]
        public async Task Response_Bind_ResponseTAsync_ResponseTResult()
        {
            var answer = 42;
            var response = Task.FromResult(answer.ToString().ToResponse());
            Func<string, Response<int>> func = x => int.Parse(x).ToResponse();

            var result = await response.BindAsync(func);

            Assert.AreEqual(answer, result);
        }

        [TestMethod]
        public async Task Response_Bind_ResponseT_ResponseTResultAsync()
        {
            var answer = 42;
            var response = answer.ToString().ToResponse();
            Func<string, Task<Response<int>>> func = x => Task.FromResult(int.Parse(x).ToResponse());

            var result = await response.BindAsync(func);

            Assert.AreEqual(answer, result);
        }

        [TestMethod]
        public async Task Response_Bind_ResponseTAsync_ResponseTResultAsync()
        {
            var answer = 42;
            var response = Task.FromResult(answer.ToString().ToResponse());
            Func<string, Task<Response<int>>> func = x => Task.FromResult(int.Parse(x).ToResponse());

            var result = await response.BindAsync(func);

            Assert.AreEqual(answer, result);
        }

        [TestMethod]
        public void Response_Bind_ResponseT_Response()
        {
            var isCalled = false;
            var response = 42.ToResponse();
            Func<int, Response> func = _ => { isCalled = true; return new Response(true); };

            var result = response.Bind(func);

            Assert.IsTrue(result);
            Assert.IsTrue(isCalled);
        }

        [TestMethod]
        public async Task Response_Bind_ResponseTAsync_Response()
        {
            var isCalled = false;
            var response = Task.FromResult(42.ToResponse());
            Func<int, Response> func = _ => { isCalled = true; return new Response(true); };

            var result = await response.BindAsync(func);

            Assert.IsTrue(result);
            Assert.IsTrue(isCalled);
        }

        [TestMethod]
        public async Task Response_Bind_ResponseT_ResponseAsync()
        {
            var isCalled = false;
            var response = 42.ToResponse();
            Func<int, Task<Response>> func = _ => { isCalled = true; return Task.FromResult(new Response(true)); };

            var result = await response.BindAsync(func);

            Assert.IsTrue(result);
            Assert.IsTrue(isCalled);
        }

        [TestMethod]
        public async Task Response_Bind_ResponseTAsync_ResponseTAsync()
        {
            var isCalled = false;
            var response = Task.FromResult(42.ToResponse());
            Func<int, Task<Response>> func = _ => { isCalled = true; return Task.FromResult(new Response(true)); };

            var result = await response.BindAsync(func);

            Assert.IsTrue(result);
            Assert.IsTrue(isCalled);
        }

        #endregion

        #region Response IEquatable

        private sealed class Model
        {
            public bool EqualsCalled { get; private set; } = false;
            private readonly Guid _guid = Guid.NewGuid();

            public override int GetHashCode() => _guid.GetHashCode();
            public override bool Equals(object obj) => Equals(obj as Model);
            public bool Equals(Model model)
            {
                EqualsCalled = true;
                return model != null && _guid.Equals(model._guid);
            }
        }

        [TestMethod]
        public void Equatable_GetHashCode_Invalid()
        {
            var response = new Response<string>();

            Assert.IsFalse(response);
            Assert.IsFalse(response.IsValid);
            Assert.AreEqual(false.GetHashCode(), response.GetHashCode());
        }

        [TestMethod]
        public void Equatable_GetHashCode_Valid_Value()
        {
            var value = 1337;

            var response = new Response<int>(value);

            Assert.AreEqual(value.GetHashCode(), response.GetHashCode());
        }

        [TestMethod]
        public void Equatable_GetHashCode_Valid_Reference()
        {
            var reference = new Model();

            var response = Response.Create(reference);

            Assert.AreEqual(reference.GetHashCode(), response.GetHashCode());
        }

        [TestMethod]
        public void Equatable_Equals_Sign_SameValue()
        {
            var value = 10;
            var response = Response.Create(value);

            var result1 = response == value;
            var result2 = value == response;

            Assert.IsTrue(result1);
            Assert.IsTrue(result2);
        }

        [TestMethod]
        public void Equatable_Equals_Sign_NotSameValue()
        {
            var value = 10;
            var response = Response.Create(value + 1);

            var result1 = response == value;
            var result2 = value == response;

            Assert.IsFalse(result1);
            Assert.IsFalse(result2);
        }

        [TestMethod]
        public void Equatable_NotEquals_Sign_SameValue()
        {
            var value = 10;
            var response = Response.Create(value );

            var result1 = response != value;
            var result2 = value != response;

            Assert.IsFalse(result1);
            Assert.IsFalse(result2);
        }

        [TestMethod]
        public void Equatable_NotEquals_Sign_NotSameValue()
        {
            var value = 10;
            var response = Response.Create(value + 1);

            var result1 = response != value;
            var result2 = value != response;

            Assert.IsTrue(result1);
            Assert.IsTrue(result2);
        }

        [TestMethod]
        public void Equatable_Equals_Sign_Response_SameValue()
        {
            var value = 10;
            var response1 = Response.Create(value);
            var response2 = Response.Create(value);

            var result1 = response1 == response2;
            var result2 = response2 == response1;

            Assert.IsTrue(result1);
            Assert.IsTrue(result2);
        }

        [TestMethod]
        public void Equatable_Equals_Sign_Response_NotSameValue()
        {
            var value = 10;
            var response1 = Response.Create(value);
            var response2 = Response.Create(value + 1);

            var result1 = response1 == response2;
            var result2 = response2 == response1;

            Assert.IsFalse(result1);
            Assert.IsFalse(result2);
        }

        [TestMethod]
        public void Equatable_NotEquals_Sign_Response_SameValue()
        {
            var value = 10;
            var response1 = Response.Create(value);
            var response2 = Response.Create(value);

            var result1 = response1 != response2;
            var result2 = response2 != response1;

            Assert.IsFalse(result1);
            Assert.IsFalse(result2);
        }

        [TestMethod]
        public void Equatable_NotEquals_Sign_Response_NotSameValue()
        {
            var value = 10;
            var response1 = Response.Create(value);
            var response2 = Response.Create(value + 1);

            var result1 = response1 != response2;
            var result2 = response2 != response1;

            Assert.IsTrue(result1);
            Assert.IsTrue(result2);
        }

        [TestMethod]
        public void Equatable_Equals_Sign_SameValue_ReferenceComparison()
        {
            var value = new object();
            var response = Response.Create(value);

            var result1 = response == value;
            var result2 = value == response;

            Assert.IsTrue(result1);
            Assert.IsTrue(result2);
        }

        [TestMethod]
        public void Equatable_Equals_Sign_NotSameValue_ReferenceComparison()
        {
            var value = new object();
            var response = Response.Create(new object());

            var result1 = response == value;
            var result2 = value == response;

            Assert.IsFalse(result1);
            Assert.IsFalse(result2);
        }

        [TestMethod]
        public void Equatable_Equals_Sign_Response_SameValue_ReferenceComparison()
        {
            var value = new object();
            var response1 = Response.Create(value);
            var response2 = Response.Create(value);

            var result1 = response1 == response2;
            var result2 = response2 == response1;

            Assert.IsTrue(result1);
            Assert.IsTrue(result2);
        }

        [TestMethod]
        public void Equatable_Equals_Sign__Response_NotSameValue_ReferenceComparison()
        {
            var response1 = Response.Create(new object());
            var response2 = Response.Create(new object());

            var result1 = response1 == response2;
            var result2 = response2 == response1;

            Assert.IsFalse(result1);
            Assert.IsFalse(result2);
        }

        [TestMethod]
        public void Equatable_Equals_Sign_SameValue_ReferenceComparison_OverrideEquals()
        {
            var value = new Model();
            var response = Response.Create(value);

            var result1 = response == value;
            var result2 = value == response;

            Assert.IsTrue(result1);
            Assert.IsTrue(result2);
            Assert.IsTrue(value.EqualsCalled);
        }

        [TestMethod]
        public void Equatable_Equals_Sign_NotSameValue_ReferenceComparison_OverrideEquals()
        {
            var value = new Model();
            var response = Response.Create(new Model());

            var result1 = response == value;
            var result2 = value == response;

            Assert.IsFalse(result1);
            Assert.IsFalse(result2);
            Assert.IsTrue(response.Value.EqualsCalled);
        }

        [TestMethod]
        public void Equatable_Equals_Sign_Response_SameValue_ReferenceComparison_OverrideEquals()
        {
            var value = new Model();
            var response1 = Response.Create(value);
            var response2 = Response.Create(value);

            var result1 = response1 == response2;
            var result2 = response2 == response1;

            Assert.IsTrue(result1);
            Assert.IsTrue(result2);
            Assert.IsTrue(response1.Value.EqualsCalled);
        }

        [TestMethod]
        public void Equatable_Equals_Sign__Response_NotSameValue_ReferenceComparison_OverrideEquals()
        {
            var response1 = Response.Create(new Model());
            var response2 = Response.Create(new Model());

            var result1 = response1 == response2;
            var result2 = response2 == response1;

            Assert.IsFalse(result1);
            Assert.IsFalse(result2);
            Assert.IsTrue(response1.Value.EqualsCalled);
            Assert.IsTrue(response2.Value.EqualsCalled);
        }

        [TestMethod]
        public void Equatable_Object_Cast_T_Equals()
        {
            var value = 10;
            var response = Response.Create(value);

            var result = response.Equals((object)value);

            Assert.IsTrue(result);
        }

        [TestMethod]
        public void Equatable_Object_T_Equals()
        {
            var value = 10;
            var response = Response.Create(value);

            var result = response.Equals(value);

            Assert.IsTrue(result);
        }

        [TestMethod]
        public void Equatable_Object_Cast_ResponseT_Equals()
        {
            var value = 10;
            var response1 = Response.Create(value);
            var response2 = Response.Create(value);

            var result = response1.Equals((object)response2);

            Assert.IsTrue(result);
        }

        [TestMethod]
        public void Equatable_Object_ResponseT_Equals()
        {
            var value = 10;
            var response1 = Response.Create(value);
            var response2 = Response.Create(value);

            var result = response1.Equals(response2);

            Assert.IsTrue(result);
        }

        [TestMethod]
        public void Equatable_BothResponseAreInvalid_DifferentT_AreNotEqual()
        {
            var response1 = new Response<int>();
            var response2 = new Response<string>();

            var result = response1.Equals(response2);

            Assert.IsFalse(result);
        }

        [TestMethod]
        public void Equatable_BothResponseAreInvalid_AreEqual()
        {
            var response1 = new Response<int>();
            var response2 = new Response<int>();

            var result = response1.Equals(response2);

            Assert.IsTrue(result);
        }

        [TestMethod]
        public void Equatable_TwoResponses_OneIsNull_AreNotEqual()
        {
            var response1 = new Response<string>(null);
            var response2 = new Response<string>("Hi!");

            var result1 = response1.Equals(response2);
            var result2 = response2.Equals(response1);

            Assert.IsFalse(result1);
            Assert.IsFalse(result2);
        }

        [TestMethod]
        public void Equatable_TwoResponses_BothAreNull_AreEqual()
        {
            var response1 = new Response<string>(null);
            var response2 = new Response<string>(null);

            var result1 = response1.Equals(response2);
            var result2 = response2.Equals(response1);

            Assert.IsTrue(result1);
            Assert.IsTrue(result2);
        }

        [TestMethod]
        public void Equatable_TwoResponses_BothAreNull_DifferentT_AreNotEqual()
        {
            var response1 = new Response<string>(null);
            var response2 = new Response<Model>(null);

            var result1 = response1.Equals(response2);
            var result2 = response2.Equals(response1);

            Assert.IsFalse(result1);
            Assert.IsFalse(result2);
        }

        #endregion

        #region BaseResponse

        [TestMethod]
        public void BaseResponse_HasValue_IValid()
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
        public void BaseResponse_Create_InvalidResponseT()
        {
            var response = new Response<int>();

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

        [TestMethod]
        public void BaseResponse_Bind_From_TValue()
        {
            var username = "John Smith";
            Response updateUser(string name) => new Response(name.Length > 0);

            var result = username.BindValue(updateUser);

            Assert.IsTrue(result);
        }

        [TestMethod]
        public async Task BaseResponse_Bind_From_TaskValue()
        {
            var username = Task.FromResult("John Smith");
            Response updateUser(string name) => new Response(name.Length > 0);

            var result = await username.BindValueAsync(updateUser);

            Assert.IsTrue(result);
        }

        [TestMethod]
        public async Task BaseResponse_BindAsync_From_Value()
        {
            var username = "John Smith";
            Task<Response> updateUser(string name) => Task.FromResult(new Response(name.Length > 0));

            var result = await username.BindValueAsync(updateUser);

            Assert.IsTrue(result);
        }

        [TestMethod]
        public async Task BaseResponse_BindAsync_From_TaskValue()
        {
            var username = Task.FromResult("John Smith");
            Task<Response> updateUser(string name) => Task.FromResult(new Response(name.Length > 0));

            var result = await username.BindValueAsync(updateUser);

            Assert.IsTrue(result);
        }

        [TestMethod]
        public void BaseResponse_Bind_ResponseT()
        {
            var answer = 42;
            var response = new Response();
            Func<Response<int>> func = () => answer.ToResponse();

            var result = response.Bind(func);
            Assert.IsFalse(result);

            response = response.AsValid();
            result = response.Bind(func);

            Assert.IsTrue(result);
            Assert.AreEqual(answer, result);
        }

        [TestMethod]
        public async Task BaseResponse_BindAsync_ResponseT()
        {
            var answer = 42;
            var response = Task.FromResult(new Response(true));
            Func<Response<int>> func = () => answer.ToResponse();

            var result = await response.BindAsync(func);

            Assert.IsTrue(result);
            Assert.AreEqual(answer, result);
        }

        [TestMethod]
        public async Task BaseResponse_Bind_ResponseTAsync()
        {
            var answer = 42;
            var response = new Response(true);
            Func<Task<Response<int>>> func = () => Task.FromResult(answer.ToResponse());

            var result = await response.BindAsync(func);

            Assert.IsTrue(result);
            Assert.AreEqual(answer, result);
        }

        [TestMethod]
        public async Task BaseResponse_BindAsync_ResponseTAsync()
        {
            var answer = 42;
            var response = Task.FromResult(new Response(true));
            Func<Task<Response<int>>> func = () => Task.FromResult(answer.ToResponse());

            var result = await response.BindAsync(func);

            Assert.IsTrue(result);
            Assert.AreEqual(answer, result);
        }

        [TestMethod]
        public void BaseResponse_Bind_Response_Response()
        {
            var response = new Response(true);
            var isCalled = false;
            Func<Response> func = () => { isCalled = true; return new Response(true); };

            var result = response.Bind(func);

            Assert.IsTrue(isCalled);
            Assert.IsTrue(result);
        }

        [TestMethod]
        public async Task BaseResponse_Bind_ResponseAsync_Response()
        {
            var response = Task.FromResult(new Response(true));
            var isCalled = false;
            Func<Response> func = () => { isCalled = true; return new Response(true); };

            var result = await response.BindAsync(func);

            Assert.IsTrue(isCalled);
            Assert.IsTrue(result);
        }

        [TestMethod]
        public async Task BaseResponse_Bind_Response_ResponseAsync()
        {
            var response = new Response(true);
            var isCalled = false;
            Func<Task<Response>> func = () => { isCalled = true; return Task.FromResult(new Response(true)); };

            var result = await response.BindAsync(func);

            Assert.IsTrue(isCalled);
            Assert.IsTrue(result);
        }

        [TestMethod]
        public async Task BaseResponse_Bind_ResponseAsync_ResponseAsync()
        {
            var response = Task.FromResult(new Response(true));
            var isCalled = false;
            Func<Task<Response>> func = () => { isCalled = true; return Task.FromResult(new Response(true)); };

            var result = await response.BindAsync(func);

            Assert.IsTrue(isCalled);
            Assert.IsTrue(result);
        }

        #endregion

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
            Assert.AreEqual(result.AggregateErrors.Length, 0);
        }

        [TestMethod]
        public void MaybeError_Bind_SecondOnly()
        {
            var error = "error";
            var maybe = new Maybe<int, string>(1);

            var result = maybe.Bind(maybe.With(error), (x, y) => maybe.With(x + y));

            Assert.IsTrue(maybe.Match(x => x == 1, x => false));
            Assert.IsTrue(result.Match(_ => false, x => x == error));
            Assert.AreEqual(result.AggregateErrors.Length, 0);
        }

        [TestMethod]
        public void MaybeError_Bind_FirstAndSecond()
        {
            string error1 = "error1", error2 = "error2";
            var maybe = new Maybe<int, string>(error1);

            var result = maybe.Bind(maybe.With(error2), (x, y) => maybe.With(x + y));

            Assert.IsTrue(maybe.Match(_ => false, x => x == error1));
            Assert.IsTrue(result.Match(_ => false, x => x == error2));
            Assert.AreEqual(maybe.AggregateErrors.Length, 0);
            Assert.AreEqual(result.AggregateErrors.Length, 1);
            Assert.AreEqual(result.AggregateErrors[0], error1);
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
            Assert.AreEqual(maybe.AggregateErrors.Length, 0);
            Assert.AreEqual(result1.AggregateErrors.Length, 1);
            Assert.AreEqual(result1.AggregateErrors[0], error1);
            Assert.AreEqual(result2.AggregateErrors.Length, 2);
            Assert.AreEqual(result2.AggregateErrors[0], error1);
            Assert.AreEqual(result2.AggregateErrors[1], error2);
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
            Assert.AreEqual(aggregate.AggregateErrors[0], error1);
            Assert.AreEqual(aggregate.AggregateErrors[1], error2);
            Assert.AreEqual(aggregate.AggregateErrors[2], error3);
        }

        [TestMethod]
        public async Task MaybeError_BindAsync_BothNotValid()
        {
            var first = new Maybe<int, string>("error1");
            var second = new Maybe<int, string>("error2");

            var result = await first.BindAsync(second, (x, y) => Task.FromResult(new Maybe<int, string>(x + y)));

            Assert.AreEqual("error2", result.Match(_ => string.Empty, x => x));
            Assert.AreEqual("error1", result.AggregateErrors[0]);
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
        public async Task MaybeTaskError_BindAsync_BothNotValid()
        {
            var first = new Maybe<int, string>("error1");
            var second = Task.FromResult(new Maybe<int, string>("error2"));

            var result = await first.BindAsync(second, (x, y) => Task.FromResult(new Maybe<int, string>(x + y)));

            Assert.AreEqual("error2", result.Match(_ => string.Empty, x => x));
            Assert.AreEqual("error1", result.AggregateErrors[0]);
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
        public void MaybeError_Transform()
        {
            var wasCalled = false;
            var maybe = new Maybe<int, string>("error");

            var result = maybe.Transform(x => { wasCalled = true; return x + 1; });

            Assert.IsFalse(wasCalled);
            Assert.IsTrue(maybe.Match(_ => false, x => x == "error"));
            Assert.IsTrue(result.Match(_ => false, x => x == "error"));
        }

        #endregion

        #region Maybe Value

        [TestMethod]
        public void MaybeValue_Transform()
        {
            var wasCalled = false;
            var maybe = new Maybe<int, string>(1);

            var result = maybe.Transform(x => { wasCalled = true; return x + 1; });

            Assert.IsTrue(wasCalled);
            Assert.IsTrue(maybe.Match(x => x == 1, _ => false));
            Assert.IsTrue(result.Match(x => x == 2, _ => false));
        }

        [TestMethod]
        public async Task MaybeValue_BindAsync_BothAreValid()
        {
            var first = new Maybe<int, string>(1);
            var second = new Maybe<int, string>(2);

            var result = await first.BindAsync(second, (x, y) => Task.FromResult(new Maybe<int, string>(x + y)));

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

        #endregion
    }
}
