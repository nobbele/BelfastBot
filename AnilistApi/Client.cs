using Newtonsoft.Json.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace MalApi
{
    public static class Client
    {
        public static readonly string BaseUrl = "https://api.jikan.moe/v3/search/anime";

        public static async Task<SearchResult[]> SearchAnimeAsync(string name)
        {
            using (HttpClient httpClient = new HttpClient())
            {
                string json = await httpClient.GetStringAsync($"{BaseUrl}?q={name}&limit=10");

                dynamic obj = JObject.Parse(json);

                dynamic[] jsonResults = (dynamic[])obj.results.ToObject<dynamic[]>();

                SearchResult[] results = new SearchResult[jsonResults.Length];  
                int i = 0;
                foreach (dynamic jsonResult in jsonResults) {

                    results[i] = new SearchResult
                    {
                        Id = jsonResult.mal_id,
                        Airing = jsonResult.airing,
                        Title = jsonResult.title,
                        Synopsis = jsonResult.synopsis,
                        Type = jsonResult.type,
                        Episodes = jsonResult.episodes,
                        Score = jsonResult.score,
                        ImageUrl = jsonResult.image_url,
                        AnimeUrl = jsonResult.url,
                    };
                    i++;
                }   
                return results;
            }
        }
    }
}