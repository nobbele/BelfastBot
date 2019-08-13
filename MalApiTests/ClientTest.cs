using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;
using MalApi;

namespace MalApiTests
{
    [TestClass]
    public class ClientTest
    {
        public TestContext TestContext { get; set; }

        [DataTestMethod]
        [DataRow("Kimetsu no yaiba")]
        [DataRow("Katanagatari")]
        [DataRow("Steins Gate")]
        [DataRow("Hensuki")]
        [DataRow("One Piece")]
        public async Task SearchAnimeAsync(string name)
        {
            TestContext.WriteLine($"Trying to find {name} with limit 5");
            SearchResult[] results = await Client.SearchAnimeAsync(name);
            Assert.IsNotNull(results);
            Assert.IsTrue(results.Length > 0);

            foreach(SearchResult result in results)
            {
                Assert.IsFalse(string.IsNullOrEmpty(result.Title));
                Assert.IsFalse(string.IsNullOrEmpty(result.ImageUrl));
                Assert.IsTrue(result.Id > 0);
            }

            await Task.Delay(100);
        }
    }
}