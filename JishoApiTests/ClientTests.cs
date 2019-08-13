using JishoApi;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace JishoApiTests
{
    [TestClass]
    public class ClientTests
    {
        public TestContext TestContext { get; set; }

        [DataTestMethod]
        [DataRow("food")]
        [DataRow("to wait")]
        [DataRow("eat")]
        [DataRow("sleepy")]
        [DataRow("well")]
        [DataRow("おとな")]
        [DataRow("大丈夫")]
        [DataRow("パソコン")]
        [DataRow("メシ")]
        public async Task SearchWordAsync(string word)
        {
            TestContext.WriteLine($"Trying word {word}");
            SearchResult[] results = await Client.GetWordAsync(word);
            Assert.IsNotNull(results);
            Assert.IsTrue(results.Length > 0);

            await Task.Delay(100);
        }
    }
}