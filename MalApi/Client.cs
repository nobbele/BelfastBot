using Newtonsoft.Json.Linq;
using System;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace MalApi
{
    public static class Client
    {
        public static readonly Uri BaseUri = new Uri("https://api.jikan.moe/v3");

        public static async Task<AnimeResult> GetDetailedAnimeResultsAsync(ulong id)
        {
            using (HttpClient httpClient = new HttpClient())
            {
                string json = await httpClient.GetStringAsync($"{BaseUri}/anime/{id}");

                dynamic obj = JObject.Parse(json);

                dynamic jsonResult = (dynamic)obj.ToObject<dynamic>();

                dynamic[] studios = (dynamic[])jsonResult.studios.ToObject<dynamic[]>();
                dynamic studio = studios.ElementAtOrDefault(0);

                return new AnimeResult
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
                    //Detailed
                    Source = jsonResult.source,
                    Duration = jsonResult.duration,
                    Broadcast = jsonResult.broadcast,
                    TrailerUrl = jsonResult.trailer_url,
                    Studio = studio?.name,
                    StudioUrl = studio?.url,
                };
            }
        }

        public static async Task<ulong[]> GetAnimeIdAsync(string name, int limit)
        {
            using (HttpClient httpClient = new HttpClient())
            {
                string json = await httpClient.GetStringAsync($"{BaseUri}/search/anime?q={name}&limit={limit}");

                dynamic obj = JObject.Parse(json);

                dynamic[] jsonResults = (dynamic[])obj.results.ToObject<dynamic[]>();

                ulong[] results = new ulong[jsonResults.Length];
                int i = 0;
                foreach (dynamic jsonResult in jsonResults)
                {

                    results[i] = jsonResult.mal_id;
                    i++;
                }
                return results;
            }
        }
        public static async Task<StudioResult> GetStudioResultsAsync(int id)
        {
            using (HttpClient httpClient = new HttpClient())
            {
                string json = await httpClient.GetStringAsync($"{BaseUri}/producer/{id}");

                dynamic obj = JObject.Parse(json);

                dynamic jsonResult = (dynamic)obj.ToObject<dynamic>();

                return new StudioResult
                {
                    Name = jsonResult.meta.name.ToObject<string>(),
                    StudioUrl = jsonResult.meta.url.ToObject<string>(),
                };
            }

        }
    }
}