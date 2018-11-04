using ContainerExpressions.Containers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using System;
using System.IO;

namespace Tests.ContainerExpressions.Examples
{
    [TestClass]
    public class BaseResponseExample
    {
        private const string RELATIVE_PATH = "./ExampleUser";

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

        /// <summary>https://github.com/Matthew-Dove/ContainerExpressions#response</summary>
        [TestMethod]
        public void BaseResponse_Example()
        {
            var customerId = 1337;
            CustomerService service = new CustomerService();

            Response isSaved = service.SaveCustomer(customerId, new Customer { Name = "Alice" });

            Assert.IsTrue(isSaved);
            Assert.IsTrue(File.Exists($"{RELATIVE_PATH}/{customerId}.json"));
        }

        class Customer
        {
            public string Name { get; set; }
        }

        class CustomerService
        {
            public Response SaveCustomer(int id, Customer customer)
            {
                var response = new Response(); // The response starts off in an invalid state.

                try
                {
                    string json = JsonConvert.SerializeObject(customer);
                    File.WriteAllText($"{RELATIVE_PATH}/{id}.json", json);
                    response = response.AsValid(); // The response is in a valid state.
                }
                catch (Exception ex)
                {
                    Log.Error(ex);
                }

                return response;
            }
        }

        class Log
        {
            public static void Error(Exception ex)
            {
                // Log error here...
                Assert.Fail(ex.Message);
            }
        }
    }
}
