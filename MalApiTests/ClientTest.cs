using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;
using MalApi;
using System.Linq;

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
            ulong[] ids = await Client.GetAnimeIdAsync(name, 10);
            AnimeResult[] results = await Task.WhenAll(ids.Select(id => Client.GetDetailedAnimeResultsAsync(id)));

            Assert.IsNotNull(results);
            Assert.IsTrue(results.Length > 0);

            foreach(AnimeResult result in results)
            {
                Assert.IsFalse(string.IsNullOrEmpty(result.Title));
                Assert.IsFalse(string.IsNullOrEmpty(result.ImageUrl));
                Assert.IsTrue(result.Id > 0);
            }

            await Task.Delay(100);
        }
    }
}