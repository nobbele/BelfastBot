using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using System.Web;

namespace QuaverApi
{
    public static class Client
    {
        public static readonly string BaseUrl = "https://api.quavergame.com/v1";

        public static async Task<uint?> GetUserIdByNameAsync(string name)
        {
            using (HttpClient httpClient = new HttpClient())
            {
                string json = await httpClient.GetStringAsync($"{BaseUrl}/users/search/{HttpUtility.UrlEncode(name)}");

                JObject obj = JObject.Parse(json);

                JArray users = obj["users"] as JArray;

                if(users.Count <= 0)
                    return null;

                return users[0]["id"].ToObject<uint>();
            }
        }

        public static async Task<User> GetUserAsync(uint id)
        {
            using (HttpClient httpClient = new HttpClient())
            {
                string json = await httpClient.GetStringAsync($"{BaseUrl}/users/full/{id}");

                JObject obj = JObject.Parse(json);
                JToken user = obj["user"];
                JToken userInfo = user["info"];

                User userData = userInfo.ToObject<User>();

                userData.FourKeys = user["keys4"].ToObject<KeyInfo>();
                userData.FourKeys.KeyCount = 4;

                userData.SevenKeys = user["keys7"].ToObject<KeyInfo>();
                userData.SevenKeys.KeyCount = 7;

                return userData;
            }
        }

        public static async Task<Recent> GetUserRecentAsync(uint id, uint mode)
        {
            using (HttpClient httpClient = new HttpClient())
            {
                string json = await httpClient.GetStringAsync($"{BaseUrl}/users/scores/recent?id={id}&mode={mode}");

                JObject obj = JObject.Parse(json);

                JArray scores = obj["scores"] as JArray;

                if(scores.Count <= 0)
                    return null;

                JToken score = scores[0];

                Recent recent = score.ToObject<Recent>();

                recent.Map = await GetMapAsync(score["map"]["id"].ToObject<uint>());

                return recent;
            }
        }

        public static async Task<Map> GetMapAsync(uint id)
        {
            using (HttpClient httpClient = new HttpClient())
            {
                string json = await httpClient.GetStringAsync($"{BaseUrl}/maps/{id}");

                JObject obj = JObject.Parse(json);

                return obj["map"].ToObject<Map>();
            }
        }
    }
}