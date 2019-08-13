using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace JishoApi
{
    public class Client
    {
        public static readonly string BaseUrl = "https://jisho.org/api/v1/search/words";

        public static async Task<SearchResult[]> GetWordAsync(string word)
        {
            using (HttpClient httpClient = new HttpClient())
            {
                string json = await httpClient.GetStringAsync($"{BaseUrl}?keyword={word}");

                dynamic obj = JObject.Parse(json);

                dynamic[] jsonResults = (dynamic[])obj.data.ToObject<dynamic[]>();

                SearchResult[] results = new SearchResult[jsonResults.Length];

                int i = 0;
                foreach (dynamic jsonResult in jsonResults)
                {
                    results[i] = new SearchResult
                    {
                        Word = jsonResult.slug,
                        Japanese = ((dynamic[])jsonResult.japanese.ToObject<dynamic[]>()).Select(o => new KeyValuePair<string, string>((string)o.word, (string)o.reading)).ToArray(),
                        English = ((dynamic[])jsonResult.senses.ToObject<dynamic[]>()).Select(o => new EnglishDefinition((string[])o.english_definitions.ToObject<string[]>(), (string[])o.info.ToObject<string[]>())).ToArray()
                    };
                    i++;
                }

                return results;
            }
        }
    }
}