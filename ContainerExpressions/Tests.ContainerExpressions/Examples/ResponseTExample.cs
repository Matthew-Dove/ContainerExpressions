using ContainerExpressions.Containers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using System;
using System.IO;

namespace Tests.ContainerExpressions.Examples
{
    [TestClass]
    public class ResponseTExample
    {
        private const string RELATIVE_PATH = "./ExampleUsers";
        private const string CUSTOMER_NAME = "Alice";
        private const int CUSTOMER_ID = 1337;

        [TestInitialize]
        public void Initialize()
        {
            string filePath = $"{RELATIVE_PATH}/{CUSTOMER_ID}.json";

            if (!Directory.Exists(RELATIVE_PATH))
            {
                Directory.CreateDirectory(RELATIVE_PATH);
            }

            if (!File.Exists(filePath))
            {
                File.WriteAllText(filePath, JsonConvert.SerializeObject(new Customer { Name = CUSTOMER_NAME }));
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

        /// <summary>https://github.com/Matthew-Dove/ContainerExpressions#responset</summary>
        [TestMethod]
        public void ResponseT_Example()
        {
            CustomerService service = new CustomerService();
            Response<Customer> customer = service.LoadCustomer(CUSTOMER_ID);

            if (customer.IsValid)
            {
                // Do something with the customer...
            }

            Assert.IsTrue(customer);
            Assert.AreEqual(CUSTOMER_NAME, customer.Value.Name);
        }

        class Customer
        {
            public string Name { get; set; }
        }

        class CustomerService
        {
            public Response<Customer> LoadCustomer(int id)
            {
                var response = new Response<Customer>(); // The response starts off in an invalid state.

                try
                {
                    string json = File.ReadAllText($"{RELATIVE_PATH}/{id}.json");
                    Customer customer = JsonConvert.DeserializeObject<Customer>(json);
                    response = response.WithValue(customer); // The response is in a valid state.
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
