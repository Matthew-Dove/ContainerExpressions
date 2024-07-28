using ContainerExpressions.Containers;
using ContainerExpressions.Expressions;
using ContainerExpressions.Expressions.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Tests.ContainerExpressions.Containers
{
    [TestClass]
    public class RetryTests
    {
        [TestMethod]
        public void DoesNotRetry_WhenValid()
        {
            var attempts = 0;
            Func<Response> func = () => { attempts++;  return new Response(true); };

            var response = Retry.Execute(func);

            Assert.IsTrue(response);
            Assert.AreEqual(1, attempts);
        }

        [TestMethod]
        public void RetryThreeTimes_ThenValid()
        {
            var attempts = 0;
            const int MAX_ATTEMPTS = 2;
            Func<Response> func = () => new Response(attempts++ == MAX_ATTEMPTS);

            var response = Retry.Execute(func, RetryOptions.Create(MAX_ATTEMPTS, 0));

            Assert.IsTrue(response);
            Assert.AreEqual(MAX_ATTEMPTS + 1, attempts);
        }

        [TestMethod]
        public void RetryWithComposeExpression()
        {
            int i = 1;
            Func<Response<int>> identity = () => i++ % 2 == 0 ? Response.Create(0) : new Response<int>();
            Func<int, Response<int>> increment = x => i++ % 2 == 0 ? Response.Create(x + 1) : new Response<int>();

            var count = Expression.Compose(identity.Retry(), increment.Retry());

            Assert.AreEqual(5, i);
        }

        [TestMethod]
        public async Task RetryWithComposeExpression_Task()
        {
            int i = 1;
            Func<Task<Response<int>>> identityAsync = () => i++ % 2 == 0 ? Task.FromResult(Response.Create(0)) : Task.FromResult(new Response<int>());
            Func<int, Task<Response<int>>> incrementAsync = x => i++ % 2 == 0 ? Task.FromResult(Response.Create(x + 1)) : Task.FromResult(new Response<int>());

            var count = await Expression.ComposeAsync(identityAsync.RetryAsync(), incrementAsync.RetryAsync());

            Assert.AreEqual(5, i);
        }

        [TestMethod]
        public void ExponentialRetry_RetryAttempt_IsCorrect()
        {
            var attempts = 0;
            const int MAX_ATTEMPTS = 5;
            var retries = new List<int>();
            Func<int, int> exponential = retryAttempt => { retries.Add(retryAttempt); return retryAttempt; };
            Func<Response> func = () => new Response(attempts++ == MAX_ATTEMPTS);

            var response = Retry.Execute(func, RetryOptions.CreateExponential(MAX_ATTEMPTS, exponential));

            Assert.IsTrue(response);
            for (int i = 0; i < MAX_ATTEMPTS; i++)
            {
                Assert.AreEqual(i + 1, retries[i]);
            }
        }

        [TestMethod]
        public void ExponentialRetry_DefaultImplementation()
        {
            var attempts = 0;
            Func<Response> func = () => { attempts++; return new Response(false); };

            var response = Retry.Execute(func, RetryOptions.CreateExponential());

            Assert.IsFalse(response);
            Assert.AreEqual(RetryOptions.DEFAULT_EXPONENTIAL_RETRIES + 1, attempts);
        }

        [TestMethod]
        public void JitterIsEnabled_JitterIsApplied_RoundToZero_1()
        {
            var delay = 1;
            var options = RetryOptions.Create(1, delay, true);

            var result = options.GetMillisecondsDelay(1);

            Assert.AreEqual(delay, result); // Jitter rounds to zero, so we always get the raw delay (lower egde case).
        }

        [TestMethod]
        public void JitterIsEnabled_JitterIsApplied_RoundToZero_9()
        {
            var delay = 9;
            var options = RetryOptions.Create(1, delay, true);

            var result = options.GetMillisecondsDelay(1);

            Assert.AreEqual(delay, result); // Jitter rounds to zero, so we always get the raw delay (upper edge case).
        }

        [TestMethod]
        public void JitterIsEnabled_JitterIsApplied_10()
        {
            var delay = 10;
            var options = RetryOptions.Create(1, delay, true);

            var result = options.GetMillisecondsDelay(1);

            Assert.IsTrue(result >= (delay - 1) && result <= (delay + 1)); // From 10 and above, we start getting non-zero jitter; in this case -1, 0, or +1 (to the original delay).
        }

        [TestMethod]
        public void JitterIsEnabled_JitterIsApplied_1000()
        {
            int delay = 1000, jitterRange = (int)(delay * 0.1D);
            var options = RetryOptions.Create(1, delay, true);

            var result = options.GetMillisecondsDelay(1);

            Assert.IsTrue(result >= (delay - jitterRange) && result <= (delay + jitterRange));
        }

        [TestMethod]
        public void JitterIsNotEnabled_JitterIsNotApplied()
        {
            int delay = 999;
            var options = RetryOptions.Create(1, delay, false);

            var result = options.GetMillisecondsDelay(1);

            Assert.AreEqual(delay, result);
        }

        [TestMethod]
        public void Response_Retry()
        {
            var retries = 0;
            Func<Response> response = () => { retries++;  return new Response(); };

            var result = response.Retry()();

            Assert.IsFalse(result);
            Assert.AreEqual(2, retries);
        }

        [TestMethod]
        public void Response_RetryExponential()
        {
            const int max = 3;
            var retries = 0;
            Func<Response> response = () => { retries++; return new Response(retries == max); };

            var result = response.RetryExponential()();

            Assert.IsTrue(result);
            Assert.AreEqual(max, retries);
        }

        [TestMethod]
        public void Response_RetryCustom()
        {
            const int max = 5;
            var retries = 0;
            Func<Response> response = () => { retries++; return new Response(); };

            var result = response.Retry(RetryOptions.Create(max, 0))();

            Assert.IsFalse(result);
            Assert.AreEqual(max + 1, retries);
        }

        [TestMethod]
        public void Response_Retry_TArg()
        {
            var retries = 0;
            Func<int, Response> response = x => { retries++; return new Response(); };

            var result = response.Retry()(42);

            Assert.IsFalse(result);
            Assert.AreEqual(2, retries);
        }

        [TestMethod]
        public async Task Response_RetryAsync()
        {
            var retries = 0;
            Func<Task<Response>> response = () => { retries++; return Task.FromResult(new Response()); };

            var result = await response.RetryAsync()();

            Assert.IsFalse(result);
            Assert.AreEqual(2, retries);
        }

        [TestMethod]
        public async Task Response_RetryAsync_TArg()
        {
            var retries = 0;
            Func<int, Task<Response>> response = x => { retries++; return Task.FromResult(new Response()); };

            var result = await response.RetryAsync()(42);

            Assert.IsFalse(result);
            Assert.AreEqual(2, retries);
        }

        [TestMethod]
        public void ResponseT_Retry()
        {
            var retries = 0;
            Func<Response<int>> response = () => { retries++; return new Response<int>(); };

            var result = response.Retry()();

            Assert.IsFalse(result);
            Assert.AreEqual(2, retries);
        }

        [TestMethod]
        public void ResponseT_Retry_TArg()
        {
            var retries = 0;
            Func<int, Response<string>> response = x => { retries++; return new Response<string>(); };

            var result = response.Retry()(42);

            Assert.IsFalse(result);
            Assert.AreEqual(2, retries);
        }

        [TestMethod]
        public async Task ResponseT_RetryAsync()
        {
            var retries = 0;
            Func<Task<Response<int>>> response = () => { retries++; return Task.FromResult(new Response<int>()); };

            var result = await response.RetryAsync()();

            Assert.IsFalse(result);
            Assert.AreEqual(2, retries);
        }

        [TestMethod]
        public async Task ResponseT_RetryAsync_TArg()
        {
            var retries = 0;
            Func<int, Task<Response<int>>> response = x => { retries++; return Task.FromResult(new Response<int>()); };

            var result = await response.RetryAsync()(42);

            Assert.IsFalse(result);
            Assert.AreEqual(2, retries);
        }

        [TestMethod]
        public async Task ResponseAsync_Retry()
        {
            var retries = 0;
            Func<ResponseAsync<int>> response = () => { retries++; return ResponseAsync.FromException<int>(new Exception("Error!")); };

            var result = await response.RetryAsync();

            Assert.IsFalse(result);
            Assert.AreEqual(2, retries);
        }

        [TestMethod]
        public async Task ResponseAsync_Retry_TArg()
        {
            var retries = 0;
            Func<int, ResponseAsync<int>> response = _ => { retries++; return ResponseAsync.FromException<int>(new Exception("Error!")); };

            var result = await response.RetryAsync()(42);

            Assert.IsFalse(result);
            Assert.AreEqual(2, retries);
        }
    }
}
