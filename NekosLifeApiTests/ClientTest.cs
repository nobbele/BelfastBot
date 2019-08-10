using Microsoft.VisualStudio.TestTools.UnitTesting;
using NekosLifeApi;
using System.Diagnostics;
using System.Threading.Tasks;

namespace NekosLifeApiTests
{
    [TestClass]
    public class ClientTest
    {
        public TestContext TestContext { get; set; }

        [TestMethod]
        public async Task GetSfwImageCategoriesAsync()
        {
            string[] categories = await Client.GetSfwImageCategoriesAsync();
            Assert.IsNotNull(categories);

            TestContext.WriteLine($"Got {categories.Length} categories");
            Assert.IsTrue(categories.Length > 0);
        }

        [TestMethod]
        public async Task GetSfwGifCategoriesAsync()
        {
            string[] categories = await Client.GetSfwGifCategoriesAsync();
            Assert.IsNotNull(categories);

            TestContext.WriteLine($"Got {categories.Length} categories");
            Assert.IsTrue(categories.Length > 0);
        }

        [TestMethod]
        public async Task GetSfwImageAsync()
        {
            foreach (string category in await Client.GetSfwImageCategoriesAsync())
            {
                TestContext.WriteLine($"Trying category {category}");
                string url = await Client.GetSfwImageAsync(category);
                Assert.IsFalse(string.IsNullOrEmpty(url));
                await Task.Delay(100);
            }
        }

        [TestMethod]
        public async Task GetSfwGifAsync()
        {
            foreach (string category in await Client.GetSfwGifCategoriesAsync())
            {
                TestContext.WriteLine($"Trying category {category}");
                string url = await Client.GetSfwGifAsync(category);
                Assert.IsFalse(string.IsNullOrEmpty(url));
                await Task.Delay(100);
            }
        }
    }
}
