using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;
using OsuApi;

namespace OsuApiTests
{
    [TestClass]
    public class ClientTest
    {
        public TestContext TestContext { get; set; }

        [DataTestMethod]
        [DataRow("TheJayDuck")]
        [DataRow("Nobbele")]
        public async Task SearchUser(string name)
        {
            TestContext.WriteLine($"Trying to find {name}");
            UserResult result = await Client.GetUser(name);
            Assert.IsNotNull(result);

            Assert.IsFalse(string.IsNullOrEmpty(result.UserName));

            await Task.Delay(100);
        }
    }
}
