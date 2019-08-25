using Newtonsoft.Json.Linq;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace AnimeCharacterDatabaseApi
{
    public class Client
    {
        public static readonly string BaseUrl = "http://www.animecharactersdatabase.com/api_series_characters.php";

        public static async Task<AnimeResult> SearchAnimeAsync(string name)
        {
            using (HttpClient httpClient = new HttpClient())
            {
                string json = await httpClient.GetStringAsync($"{BaseUrl}?anime_q={name}");

                dynamic obj = JObject.Parse(json);

                dynamic[] jsonResult = (dynamic[])obj.search_results.ToObject<dynamic[]>();

                Random rand = new Random();
                dynamic randomResult = jsonResult[rand.Next(0, jsonResult.Length)];

                return new AnimeResult
                {
                    Name = randomResult.anime_name,
                    Image = randomResult.anime_image,
                    Id = randomResult.anime_id.ToObject<ulong>(),                
                };
            }
        }

        public static async Task<CharacterResult> GetCharactersAsync(ulong id)
        {
            using (HttpClient httpClient = new HttpClient())
            {
                string json = await httpClient.GetStringAsync($"{BaseUrl}?anime_id={id}");

                dynamic obj = JObject.Parse(json);

                dynamic[] jsonResult = (dynamic[])obj.characters.ToObject<dynamic[]>();

                Random rand = new Random();
                dynamic randomResult = jsonResult[rand.Next(0, jsonResult.Length)];

                return new CharacterResult
                {
                    Id = randomResult.id.ToObject<ulong>(),
                    Name = randomResult.name,
                    Gender = randomResult.gender,
                    ImageUrl = randomResult.character_image,
                };
            }
        }
    }
}