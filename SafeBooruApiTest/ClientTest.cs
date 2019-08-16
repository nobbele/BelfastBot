using Microsoft.VisualStudio.TestTools.UnitTesting;
using SafeBooruApi;
using System.Linq;
using System.Threading.Tasks;

namespace SafeBooruApiTests
{
    [TestClass]
    public class ClientTest
    {
        public TestContext TestContext { get; set; }

        [DataTestMethod]
        [DataRow("fate/grand_order")]
        [DataRow("black_legwear")]
        [DataRow("white_hair")]
        [DataRow("bangs")]
        [DataRow("panties")]
        [DataRow("flat_chest")]
        public async Task GetRandomPostAsync(string tag)
        {
            TestContext.WriteLine($"Trying tag {tag}");
            string url = await Client.GetRandomPostAsync(tag);
            Assert.IsFalse(string.IsNullOrEmpty(url) && !Client.BlacklistedTags.Contains(tag));
            await Task.Delay(100);
        }
    }
}