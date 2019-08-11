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
            SearchResult result = await Client.GetWordAsync(word);
            Assert.IsNotNull(result);
            Assert.IsFalse(string.IsNullOrEmpty(word));
            Assert.IsTrue(result.Japanese.Length > 0);
            Assert.IsTrue(result.English.Length > 0);

            foreach(KeyValuePair<string, string> japWord in result.Japanese)
            {
                Assert.IsFalse(string.IsNullOrEmpty(japWord.Value));
            }
            foreach (EnglishDefinition engDef in result.English)
            {
                Assert.IsTrue(engDef.English.Length > 0);
                foreach (string engWord in engDef.English)
                {
                    Assert.IsFalse(string.IsNullOrEmpty(engWord));
                }
            }

            await Task.Delay(100);
        }
    }
}