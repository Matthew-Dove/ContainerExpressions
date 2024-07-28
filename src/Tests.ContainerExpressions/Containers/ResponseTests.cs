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

            responseWithNoValue = responseWithNoValue.With(42);
            responseWithValue = responseWithValue.With(42);

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
        public void Response_IsValid_ConditionIsFalse_Func2Invoked()
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

        class PivotOverloadTest
        {
            public static Response<int> TrueFunc() => Response<int>.Success(0);
            public static Response<int> FalseFunc() => Response<int>.Error;

            public static Response<int> TrueFunc(string x) => Response<int>.Success(int.Parse(x));
            public static Response<int> FalseFunc(string _) => Response<int>.Error;

            public static Task<Response<int>> TrueFuncAsync() => Task.FromResult(TrueFunc());
            public static Task<Response<int>> FalseFuncAsync() => Task.FromResult(FalseFunc());

            public static Task<Response<int>> TrueFuncAsync(string x) => Task.FromResult(TrueFunc(x));
            public static Task<Response<int>> FalseFuncAsync(string x) => Task.FromResult(FalseFunc(x));
        }

        [TestMethod]
        public async Task Response_Pivot_AsyncToSync_AsyncToAsync_OverloadIsOk()
        {
            var input = Task.FromResult(Response.Success);

            // AsyncToSync

            Func<Response<int>> trueFunc = () => Response<int>.Success(0);
            Func<Response<int>> falseFunc = () => Response<int>.Error;

            var response1 = await input.PivotAsync(true, trueFunc, falseFunc);
            var response2 = await input.PivotAsync(true, PivotOverloadTest.TrueFunc, PivotOverloadTest.FalseFunc);

            Assert.IsTrue(response1);
            Assert.IsTrue(response2);

            // AsyncToAsync

            Func<Task<Response<int>>> trueFuncAsync = () => Task.FromResult(trueFunc());
            Func<Task<Response<int>>> falseFuncAsync = () => Task.FromResult(falseFunc());

            response1 = await input.PivotAsync(true, trueFuncAsync, falseFuncAsync);
            response2 = await input.PivotAsync(true, PivotOverloadTest.TrueFuncAsync, PivotOverloadTest.FalseFuncAsync);

            Assert.IsTrue(response1);
            Assert.IsTrue(response2);
        }

        [TestMethod]
        public async Task Response_PivotWithInput_AsyncToSync_AsyncToAsync_OverloadIsOk()
        {
            var input = Task.FromResult(Response.Create(0.ToString()));

            // AsyncToSync

            Func<string, Response<int>> trueFunc = (x) => Response<int>.Success(int.Parse(x));
            Func<string, Response<int>> falseFunc = (_) => Response<int>.Error;

            var response1 = await input.PivotAsync(true, trueFunc, falseFunc);
            var response2 = await input.PivotAsync(true, PivotOverloadTest.TrueFunc, PivotOverloadTest.FalseFunc);

            Assert.IsTrue(response1);
            Assert.IsTrue(response2);

            // AsyncToAsync

            Func<string, Task<Response<int>>> trueFuncAsync = (x) => Task.FromResult(trueFunc(x));
            Func<string, Task<Response<int>>> falseFuncAsync = (x) => Task.FromResult(falseFunc(x));

            response1 = await input.PivotAsync(true, trueFuncAsync, falseFuncAsync);
            response2 = await input.PivotAsync(true, PivotOverloadTest.TrueFuncAsync, PivotOverloadTest.FalseFuncAsync);

            Assert.IsTrue(response1);
            Assert.IsTrue(response2);
        }

        [TestMethod]
        public async Task Response_Pivot_WhenFuncConditionIsTrue_ResponseIsValid()
        {
            var answer = 42;
            var input = Task.FromResult(Response.Create(answer.ToString()));
            Func<string, bool> condition = x => int.TryParse(x, out int i) && i == answer;

            Func<string, Task<Response<int>>> trueFuncAsync = x => Task.FromResult(Response<int>.Success(int.Parse(x)));
            Func<string, Task<Response<int>>> falseFuncAsync = _ => Task.FromResult(Response<int>.Error);

            var response1 = await input.PivotAsync(condition, trueFuncAsync, falseFuncAsync);
            var response2 = await input.PivotAsync(condition, PivotOverloadTest.TrueFuncAsync, PivotOverloadTest.FalseFuncAsync);

            Assert.IsTrue(response1);
            Assert.IsTrue(response2);
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
        public async Task BothResponses_AreValid_FunnelFunc_IsInvoked_Async()
        {
            var response1 = Task.FromResult(new Response<bool>(true));
            var response2 = Task.FromResult(new Response<bool>(false));

            var response = await Expression.FunnelAsync(response1, response2, (x, y) => false);

            Assert.IsTrue(response.IsValid);
            Assert.IsFalse(response.Value);
        }

        [TestMethod]
        public async Task BothResponses_AreValid_FunnelFunc_IsInvoked_AsyncTask()
        {
            var response1 = Task.FromResult(new Response<bool>(true));
            var response2 = Task.FromResult(new Response<bool>(false));

            var response = await Expression.FunnelAsync(response1, response2, (x, y) => Task.FromResult(false));

            Assert.IsTrue(response.IsValid);
            Assert.IsFalse(response.Value);
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

        #region Unpack BaseResponse

        [TestMethod]
        public void Unpack_BaseResponse_Pass()
        {
            var input = new Response<Response>(new Response(true));

            var result = input.Unpack();

            Assert.IsTrue(result);
        }

        [TestMethod]
        public void Unpack_BaseResponse_FailValue()
        {
            var input = new Response<Response>(new Response());

            var result = input.Unpack();

            Assert.IsFalse(result);
        }

        [TestMethod]
        public void Unpack_BaseResponse_FailContainer()
        {
            var input = new Response<Response>();

            var result = input.Unpack();

            Assert.IsFalse(result);
        }

        [TestMethod]
        public async Task Unpack_BaseResponse_Task_Pass()
        {
            var input = Task.FromResult(new Response<Response>(new Response(true)));

            var result = await input.UnpackAsync();

            Assert.IsTrue(result);
        }

        [TestMethod]
        public async Task Unpack_BaseResponse_Task_FailValue()
        {
            var input = Task.FromResult(new Response<Response>(new Response()));

            var result = await input.UnpackAsync();

            Assert.IsFalse(result);
        }

        [TestMethod]
        public async Task Unpack_BaseResponse_Task_FailContainer()
        {
            var input = Task.FromResult(new Response<Response>());

            var result = await input.UnpackAsync();

            Assert.IsFalse(result);
        }

        [TestMethod]
        public async Task Unpack_BaseResponse_Task_FailTask()
        {
            var input = Task.FromException<Response<Response>>(new Exception());

            var result = await input.UnpackAsync();

            Assert.IsFalse(result);
        }

        [TestMethod]
        public async Task Unpack_BaseResponse_Task_Async_Pass()
        {
            var input = Task.FromResult(new Response<Task<Response>>(Task.FromResult(new Response(true))));

            var result = await input.UnpackAsync();

            Assert.IsTrue(result);
        }

        [TestMethod]
        public async Task Unpack_BaseResponse_Task_Async_FailValue()
        {
            var input = Task.FromResult(new Response<Task<Response>>(Task.FromResult(new Response())));

            var result = await input.UnpackAsync();

            Assert.IsFalse(result);
        }

        [TestMethod]
        public async Task Unpack_BaseResponse_Task_Async_FailContainer()
        {
            var input = Task.FromResult(new Response<Task<Response>>());

            var result = await input.UnpackAsync();

            Assert.IsFalse(result);
        }

        [TestMethod]
        public async Task Unpack_BaseResponse_Task_Async_FailTask_FirstContinuation()
        {
            var input = Task.FromException<Response<Task<Response>>>(new Exception());

            var result = await input.UnpackAsync();

            Assert.IsFalse(result);
        }

        [TestMethod]
        public async Task Unpack_BaseResponse_Task_Async_FailTask_SecondContinuation()
        {
            var input = Task.FromResult(new Response<Task<Response>>(Task.FromException<Response>(new Exception())));

            var result = await input.UnpackAsync();

            Assert.IsFalse(result);
        }

        #endregion

        #region Unpack Response

        [TestMethod]
        public void Unpack_Response_Pass()
        {
            var input = new Response<Response<int>>(new Response<int>(0));

            var result = input.Unpack();

            Assert.IsTrue(result);
            Assert.AreEqual(0, result);
        }

        [TestMethod]
        public void Unpack_Response_FailValue()
        {
            var input = new Response<Response<int>>(new Response<int>());

            var result = input.Unpack();

            Assert.IsFalse(result);
        }

        [TestMethod]
        public void Unpack_Response_FailContainer()
        {
            var input = new Response<Response<int>>();

            var result = input.Unpack();

            Assert.IsFalse(result);
        }

        [TestMethod]
        public async Task Unpack_Response_Task_Pass()
        {
            var input = Task.FromResult(new Response<Response<int>>(new Response<int>(0)));

            var result = await input.UnpackAsync();

            Assert.IsTrue(result);
            Assert.AreEqual(0, result);
        }

        [TestMethod]
        public async Task Unpack_Response_Task_FailValue()
        {
            var input = Task.FromResult(new Response<Response<int>>(new Response<int>()));

            var result = await input.UnpackAsync();

            Assert.IsFalse(result);
        }

        [TestMethod]
        public async Task Unpack_Response_Task_FailContainer()
        {
            var input = Task.FromResult(new Response<Response<int>>());

            var result = await input.UnpackAsync();

            Assert.IsFalse(result);
        }

        [TestMethod]
        public async Task Unpack_Response_Task_FailTask()
        {
            var input = Task.FromException<Response<Response<int>>>(new Exception());

            var result = await input.UnpackAsync();

            Assert.IsFalse(result);
        }

        [TestMethod]
        public async Task Unpack_Response_Task_Async_Pass()
        {
            var input = Task.FromResult(new Response<Task<Response<int>>>(Task.FromResult(new Response<int>(0))));

            var result = await input.UnpackAsync();

            Assert.IsTrue(result);
            Assert.AreEqual(0, result);
        }

        [TestMethod]
        public async Task Unpack_Response_Task_Async_FailValue()
        {
            var input = Task.FromResult(new Response<Task<Response<int>>>(Task.FromResult(new Response<int>())));

            var result = await input.UnpackAsync();

            Assert.IsFalse(result);
        }

        [TestMethod]
        public async Task Unpack_Response_Task_Async_FailContainer()
        {
            var input = Task.FromResult(new Response<Task<Response<int>>>());

            var result = await input.UnpackAsync();

            Assert.IsFalse(result);
        }

        [TestMethod]
        public async Task Unpack_Response_Task_Async_FailTask_FirstContinuation()
        {
            var input = Task.FromException<Response<Task<Response<int>>>>(new Exception());

            var result = await input.UnpackAsync();

            Assert.IsFalse(result);
        }

        [TestMethod]
        public async Task Unpack_Response_Task_Async_FailTask_SecondContinuation()
        {
            var input = Task.FromResult(new Response<Task<Response<int>>>(Task.FromException<Response<int>>(new Exception())));

            var result = await input.UnpackAsync();

            Assert.IsFalse(result);
        }

        #endregion

        #region Validate

        [TestMethod]
        public void Validate_Sync_ResponseIsValid()
        {
            var target = Response.Create(1);

            var response = target.Validate(x => x > 0);

            Assert.IsTrue(response);
        }

        [TestMethod]
        public void Validate_Sync_ResponseIsNotValid()
        {
            var target = Response.Create(0);

            var response = target.Validate(x => x > 0);

            Assert.IsFalse(response);
        }

        [TestMethod]
        public async Task Validate_Async_ResponseIsValid()
        {
            var target = Task.FromResult(Response.Create(1));

            var response = await target.ValidateAsync(x => x > 0);

            Assert.IsTrue(response);
        }

        [TestMethod]
        public async Task Validate_Async_ResponseIsNotValid()
        {
            var target = Task.FromResult(Response.Create(0));

            var response = await target.ValidateAsync(x => x > 0);

            Assert.IsFalse(response);
        }

        [TestMethod]
        public async Task Validate_Async_TaskIsError_ResponseIsNotValid()
        {
            var target = Task.FromException<Response<int>>(new Exception("Error!"));

            var response = await target.ValidateAsync(x => x > 0);

            Assert.IsFalse(response);
        }

        #endregion

        #region BindIf

        [TestMethod]
        public async Task BindIf_ResponseIsValid_PredicateIsTrue_BindIsExecuted()
        {
            var answer = 42;
            var response = Task.FromResult(Response.Create(answer));
            Func<int, bool> predicate = x => x > 0;
            Func<int, Task<Response<string>>> func = x => Task.FromResult(Response.Create(x.ToString()));

            var bind = await response.BindIfAsync(predicate, func);

            Assert.IsTrue(bind);
            Assert.AreEqual(answer.ToString(), bind);
        }

        [TestMethod]
        public async Task BindIf_ResponseIsNotValid_PredicateIsTrue_BindIsNoExecuted()
        {
            var response = Task.FromResult(Response<int>.Error);
            Func<int, bool> predicate = x => x > 0;
            Func<int, Task<Response<string>>> func = x => Task.FromResult(Response.Create(x.ToString()));

            var bind = await response.BindIfAsync(predicate, func);

            Assert.IsFalse(bind);
        }

        [TestMethod]
        public async Task BindIf_ResponseIsValid_PredicateIsNotTrue_BindIsNotExecuted()
        {
            var answer = -1;
            var response = Task.FromResult(Response.Create(answer));
            Func<int, bool> predicate = x => x > 0;
            Func<int, Task<Response<string>>> func = x => Task.FromResult(Response.Create(x.ToString()));

            var bind = await response.BindIfAsync(predicate, func);

            Assert.IsFalse(bind);
        }

        [TestMethod]
        public async Task BindIf_TaskHasError_BindIsNotExecuted()
        {
            var response = Task.FromException<Response<int>>(new Exception("Error!"));
            Func<int, bool> predicate = x => x > 0;
            Func<int, Task<Response<string>>> func = x => Task.FromResult(Response.Create(x.ToString()));

            var bind = await response.BindIfAsync(predicate, func);

            Assert.IsFalse(bind);
        }

        #endregion
    }
}
