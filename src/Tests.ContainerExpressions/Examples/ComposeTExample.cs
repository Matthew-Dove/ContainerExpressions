using ContainerExpressions.Containers;
using ContainerExpressions.Expressions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using System.Net;

namespace Tests.ContainerExpressions.Examples
{
    [TestClass]
    public class ComposeTExample
    {
        private const string RELATIVE_PATH = "./ComposeHtmlExample";

        [TestInitialize]
        public void Initialize()
        {
            if (!Directory.Exists(RELATIVE_PATH))
            {
                Directory.CreateDirectory(RELATIVE_PATH);
            }
        }

        [TestCleanup]
        public void Cleanup()
        {
            if (Directory.Exists(RELATIVE_PATH))
            {
                Directory.Delete(RELATIVE_PATH, true);
            }
        }

        /// <summary>https://github.com/Matthew-Dove/ContainerExpressions#composet</summary>
        [TestMethod]
        public void ComposeT_Example()
        {
            var fileExists = false;
            var filepath = Expression.Compose(DownloadHtml, PersistHtml);

            if (filepath)
            {
                fileExists = File.Exists(filepath);
            }

            Assert.IsTrue(filepath);
            Assert.IsTrue(fileExists);
        }

        private static Response<string> DownloadHtml()
        {
            var response = new Response<string>();

            try
            {
                using (var wc = new WebClient())
                {
                    var html = wc.DownloadString("https://example.com/");
                    response = response.With(html);
                }
            }
            catch
            {
                // Log error here...
            }

            return response;
        }

        private static Response<string> PersistHtml(string html)
        {
            var response = new Response<string>();

            try
            {
                var path = $"{RELATIVE_PATH}/{Path.GetRandomFileName()}.html";
                File.WriteAllText(path, html);
                response = response.With(path);
            }
            catch
            {
                // Log error here...
            }

            return response;
        }
    }
}
