using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace MalApi
{
    public static class Client
    {
        public static readonly string BaseUrl = "https://api.jikan.moe/v3/search/anime";

        public static async Task<SearchResult> SearchAnimeAsync(string name, int limit)
        {
            using (HttpClient httpClient = new HttpClient())
            {
                string json = await httpClient.GetStringAsync($"{BaseUrl}?q={name}&limit={limit}");

                dynamic obj = JObject.Parse(json);

                if (((dynamic[])obj.results.ToObject<dynamic[]>()).Length < 1)
                    return new SearchResult()
                    {
                        Id = 0,
                        Title = "None",
                    };
                dynamic jsonResult = obj.results[0];

                SearchResult result = new SearchResult
                {
                    Id = jsonResult.mal_id,
                    Title = jsonResult.title,
                };

                return result;
            }
        }
    }
}
