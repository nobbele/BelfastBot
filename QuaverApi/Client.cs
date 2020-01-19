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

                dynamic obj = JObject.Parse(json).ToObject<dynamic>();

                dynamic[] users = obj.users.ToObject<dynamic[]>();

                if(users.Length <= 0)
                    return null;

                return users[0].id.ToObject<uint>();
            }
        }

        public static async Task<User> GetUserAsync(uint id)
        {
            using (HttpClient httpClient = new HttpClient())
            {
                string json = await httpClient.GetStringAsync($"{BaseUrl}/users/full/{id}");

                dynamic obj = JObject.Parse(json).ToObject<dynamic>();
                dynamic user = obj.user;
                dynamic userInfo = user.info;

                User userData = new User
                {
                    Id = userInfo.id.ToObject<uint>(),
                    SteamId = userInfo.steam_id.ToObject<string>(),
                    Username = userInfo.username.ToObject<string>(),
                    Country = userInfo.country.ToObject<string>(),
                    AvatarUrl = userInfo.avatar_url.ToObject<string>(),
                };
                userData.FourKeys = new KeyInfo
                {
                    KeyCount = 4,
                    PerformanceRating = user.keys4.stats.overall_performance_rating.ToObject<float>(),
                    Accuracy = user.keys4.stats.overall_accuracy.ToObject<float>(),
                    PlayCount = user.keys4.stats.play_count.ToObject<uint>(),
                    GlobalRanking = user.keys4.globalRank.ToObject<ulong?>() ?? (ulong)0,
                    CountryRanking = user.keys4.countryRank.ToObject<ulong?>() ?? (ulong)0,
                };
                userData.SevenKeys = new KeyInfo
                {
                    KeyCount = 7,
                    PerformanceRating = user.keys7.stats.overall_performance_rating.ToObject<float>(),
                    Accuracy = user.keys7.stats.overall_accuracy.ToObject<float>(),
                    PlayCount = user.keys7.stats.play_count.ToObject<uint>(),
                    GlobalRanking = user.keys7.globalRank.ToObject<ulong?>() ?? (ulong)0,
                    CountryRanking = user.keys7.countryRank.ToObject<ulong?>() ?? (ulong)0,
                };
                return userData;
            }
        }

        public static async Task<Recent> GetUserRecentAsync(uint id, uint mode)
        {
            using (HttpClient httpClient = new HttpClient())
            {
                string json = await httpClient.GetStringAsync($"{BaseUrl}/users/scores/recent?id={id}&mode={mode}");

                dynamic obj = JObject.Parse(json).ToObject<dynamic>();

                dynamic[] scores = obj.scores.ToObject<dynamic[]>();

                if(scores.Length <= 0)
                    return null;

                dynamic score = scores[0];
                dynamic map = score.map;

                Recent recent = new Recent
                {
                    Id = score.id.ToObject<uint>(),
                    PerformanceRating = score.performance_rating.ToObject<float>(),
                    Accuracy = score.accuracy.ToObject<float>(),
                    Combo = score.max_combo.ToObject<uint>(),
                    Score = score.total_score.ToObject<ulong>(),
                    Grade = score.grade.ToObject<string>(),
                };
                /*recent.Map = new Map
                {
                    Id = map.id.ToObject<uint>(),
                    Artist = map.artist.ToObject<string>(),
                    Title = map.title.ToObject<string>(),
                    DifficultyName = map.difficulty_name.ToObject<string>(),
                    Creator = map.creator_username.ToObject<string>(),
                };*/
                recent.Map = await GetMapAsync(map.id.ToObject<uint>());
                return recent;
            }
        }

        public static async Task<Map> GetMapAsync(uint id)
        {
            using (HttpClient httpClient = new HttpClient())
            {
                string json = await httpClient.GetStringAsync($"{BaseUrl}/maps/{id}");

                dynamic obj = JObject.Parse(json).ToObject<dynamic>();

                dynamic map = obj.map;

                return new Map
                {
                    Id = map.id.ToObject<uint>(),
                    Artist = map.artist.ToObject<string>(),
                    Title = map.title.ToObject<string>(),
                    DifficultyName = map.difficulty_name.ToObject<string>(),
                    Creator = map.creator_username.ToObject<string>(),
                    DifficultyRating = map.difficulty_rating.ToObject<float>(),
                    MapSetId = map.mapset_id.ToObject<uint>(),
                };
            }
        }
    }
}