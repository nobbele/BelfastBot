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
            SearchResult result = await Client.SearchAnimeAsync(name, 5);
            Assert.IsNotNull(result);

            await Task.Delay(100);
        }
    }
}
