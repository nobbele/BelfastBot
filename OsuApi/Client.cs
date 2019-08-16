using Newtonsoft.Json.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace OsuApi
{
    public partial class Client
    {
        public static readonly string BaseUrl = "https://osu.ppy.sh/api";

        public static async Task<UserProfile> GetUserAsync(string token, string user, int mode)
        {
            using (HttpClient httpClient = new HttpClient())
            {
                string json = await httpClient.GetStringAsync($"{BaseUrl}/get_user?u={user}&k={token}&m={mode}");

                json = json.TrimStart(new char[] { '[' }).TrimEnd(new char[] { ']' });

                dynamic obj = JObject.Parse(json);

                dynamic jsonResult = (dynamic)obj.ToObject<dynamic>();

                UserProfile result = new UserProfile
                {
                    UserId = jsonResult.user_id.ToObject<ulong>(),
                    UserName = jsonResult.username.ToObject<string>(),
                    Mode = mode,
                    PP = jsonResult.pp_raw.ToObject<float?>() ?? (float)0,
                    Level = jsonResult.level.ToObject<float?>() ?? (float)0,
                    PlayCount = jsonResult.playcount.ToObject<ulong?>() ?? (ulong)0,
                    Accuracy = jsonResult.accuracy.ToObject<float?>() ?? (float)0,
                };
                return result;
            }
        }
        public static async Task<PlayResult[]> GetUserRecentAsync(string token, string user, int mode)
        {
            using (HttpClient httpClient = new HttpClient())
            {
                string json = await httpClient.GetStringAsync($"{BaseUrl}/get_user_recent?u={user}&k={token}&m={mode}");

                dynamic obj = JArray.Parse(json);

                dynamic[] jsonResults = (dynamic[])obj.ToObject<dynamic[]>();

                PlayResult[] results = new PlayResult[jsonResults.Length];

                int i = 0;
                foreach (dynamic jsonResult in jsonResults)
                {
                    results[i] = new PlayResult
                    {
                        PlayerData = await GetUserAsync(token, user, mode),
                        BeatmapData = await GetBeatmapAsync(token, jsonResult.beatmap_id.ToObject<ulong>(), mode),
                        Score = jsonResult.score.ToObject<ulong>(),
                        Combo = jsonResult.maxcombo.ToObject<int>(),
                        Rank = jsonResult.rank.ToObject<string>(),
                    };
                    i++;
                }
                return results;
            }
        }

        public static async Task<Beatmap> GetBeatmapAsync(string token, ulong id, int mode)
        {
            using (HttpClient httpClient = new HttpClient())
            {
                string json = await httpClient.GetStringAsync($"{BaseUrl}/get_beatmaps?b={id}&k={token}&m={mode}");

                json = json.TrimStart(new char[] { '[' }).TrimEnd(new char[] { ']' });

                dynamic obj = JObject.Parse(json);

                dynamic jsonResult = (dynamic)obj.ToObject<dynamic>();

                Beatmap result = new Beatmap
                {
                    Name = jsonResult.title.ToObject<string>(),
                    Id = jsonResult.beatmap_id.ToObject<ulong>(),
                    SetId = jsonResult.beatmapset_id.ToObject<ulong>(),
                    StarRating = jsonResult.difficultyrating.ToObject<float>(),
                    Bpm = jsonResult.bpm.ToObject<int>(),
                };
                return result;
            }
        }
    }
}