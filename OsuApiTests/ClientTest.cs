using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;
using OsuApi;
using System;

namespace OsuApiTests
{
    [TestClass]
    public class ClientTest
    {
        public TestContext TestContext { get; set; }

        [DataTestMethod]
        [DataRow("TheJayDuck", 0)]
        [DataRow("TheJayDuck", 1)]
        [DataRow("TheJayDuck", 2)]
        [DataRow("TheJayDuck", 3)]
        [DataRow("Nobbele", 0)]
        [DataRow("Nobbele", 1)]
        [DataRow("Nobbele", 2)]
        [DataRow("Nobbele", 3)]
        public async Task SearchUser(string name, int mode)
        {
            TestContext.WriteLine($"Trying to find {name} on mode {mode}");
            string token = Environment.GetEnvironmentVariable("SenkoSanOsuTestToken");
            UserProfile result = await Client.GetUserAsync(token, name, mode);
            Assert.IsNotNull(result);

            Assert.IsFalse(string.IsNullOrEmpty(result.UserName));

            await Task.Delay(100);
        }

        [DataTestMethod]
        [DataRow("Nobbele", 0)]
        public async Task GetUserRecent(string name, int mode)
        {
            TestContext.WriteLine($"Trying to find {name} on mode {mode}");
            string token = Environment.GetEnvironmentVariable("SenkoSanOsuTestToken");
            PlayResult results = await Client.GetUserRecentAsync(token, name, mode);

            await Task.Delay(100);
        }
    }
}
