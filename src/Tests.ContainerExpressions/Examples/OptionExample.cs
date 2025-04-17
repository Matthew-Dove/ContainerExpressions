using ContainerExpressions.Containers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;

namespace Tests.ContainerExpressions.Examples
{
    [TestClass]
    public class OptionExample
    {
        public sealed class HttpMethod : Option, IEquatable<HttpMethod>
        {
            // Fields.
            public static readonly HttpMethod Get = new("GET");
            public static readonly HttpMethod Put = new("PUT");
            public static readonly HttpMethod Post = new("POST");
            public static readonly HttpMethod Delete = new("DELETE");

            // Properties.
            public static HttpMethod Patch { get; } = new("PATCH");

            // Private constructor controls allowed instances.
            private HttpMethod(string value) : base(value) { }

            // Typed equals.
            public bool Equals(HttpMethod other) => other is not null && ReferenceEquals(this, other);

            // Shareable utility methods, implemented by the base Option class.
            public static HttpMethod Parse(string value) => Option.Parse<HttpMethod>(value);
            public static bool TryParse(string value, out HttpMethod method) => Option.TryParse<HttpMethod>(value, out method);
            public static IEnumerable<string> GetValues() => Option.GetValues<HttpMethod>();
        }

        /// <summary>https://github.com/Matthew-Dove/ContainerExpressions#option</summary>
        [TestMethod]
        public void Option_Example()
        {
            var method = HttpMethod.Post;

            if (method == HttpMethod.Get) Console.WriteLine("Retrieving resource.");
            if (method == HttpMethod.Put) Console.WriteLine("Upserting resource.");
            if (method == HttpMethod.Patch) Console.WriteLine("Updating resource.");
            if (method == HttpMethod.Post) Console.WriteLine("Creating resource.");
            if (method == HttpMethod.Delete) Console.WriteLine("Removing resource.");
        }
    }
}
