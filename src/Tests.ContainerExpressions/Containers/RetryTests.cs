﻿using ContainerExpressions.Containers;
using ContainerExpressions.Expressions;
using ContainerExpressions.Expressions.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
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
    }
}