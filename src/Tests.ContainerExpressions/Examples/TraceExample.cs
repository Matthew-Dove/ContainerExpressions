using ContainerExpressions.Containers;
using ContainerExpressions.Expressions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;

namespace Tests.ContainerExpressions.Examples
{
    [TestClass]
    public class TraceExample
    {
        /// <summary>https://github.com/Matthew-Dove/ContainerExpressions#trace</summary>
        [TestMethod]
        public void Trace_Example()
        {
            // Initialize the Trace with a logger.
            var logs = new List<string>();
            Trace.SetLogger(log => logs.Add(log));

            // Create a function to trace the incrementing.
            Func<int, string> trace = x => string.Format("The value of the int is {0}.", x);

            // Some functions that keep incrementing their input.
            Func<Response<int>> identity = () => Response.Create(0);
            Func<int, Response<int>> increment = x => Response.Create(x + 1);

            var count = Expression.Compose(identity.Log(trace), increment.Log(trace), increment.Log(trace));

            // The follow is logged to Trace:
            // The value of the int is 0.
            // The value of the int is 1.
            // The value of the int is 2.
        }
    }
}
