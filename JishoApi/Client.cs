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

        public static async Task<SearchResult> GetWordAsync(string word)
        {
            using (HttpClient httpClient = new HttpClient())
            {
                string json = await httpClient.GetStringAsync($"{BaseUrl}?keyword={word}");

                dynamic obj = JObject.Parse(json);

                if (((dynamic[])obj.data.ToObject<dynamic[]>()).Length < 1)
                    return new SearchResult()
                    {
                        Word = "None",
                        Japanese = new KeyValuePair<string, string>[0],
                        English = new EnglishDefinition[0],
                    };
                dynamic jsonResult = obj.data[0];

                SearchResult result = new SearchResult
                {
                    Word = jsonResult.slug,
                    Japanese = ((dynamic[])jsonResult.japanese.ToObject<dynamic[]>()).Select(o => new KeyValuePair<string, string>((string)o.word, (string)o.reading)).ToArray(),
                    English = ((dynamic[])jsonResult.senses.ToObject<dynamic[]>()).Select(o => new EnglishDefinition((string[])o.english_definitions.ToObject<string[]>(), (string[])o.info.ToObject<string[]>())).ToArray()
                };

                return result;
            }
        }
    }
}