using Newtonsoft.Json.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace OsuApi
{
    public class Client
    {
        public static readonly string BaseUrl = "https://osu.ppy.sh/api/";

        public static async Task<UserResult> GetUserAsync(string token, string user, int mode)
        {
            using (HttpClient httpClient = new HttpClient())
            {
                string json = await httpClient.GetStringAsync($"{BaseUrl}get_user?u={user}&k={token}&m={mode}");

                json = json.TrimStart(new char[] { '[' }).TrimEnd(new char[] { ']' });

                dynamic obj = JObject.Parse(json);

                dynamic jsonResult = (dynamic)obj.ToObject<dynamic>();

                UserResult result = new UserResult
                {
                    UserId = jsonResult.user_id.ToObject<ulong>(),
                    UserName = jsonResult.username.ToObject<string>(),
                    PP = jsonResult.pp_raw.ToObject<float?>() ?? (float)0,
                    Level = jsonResult.level.ToObject<float?>() ?? (float)0,
                    PlayCount = jsonResult.playcount.ToObject<ulong?>() ?? (ulong)0,
                    Accuracy = jsonResult.accuracy.ToObject<float?>() ?? (float)0,
                };
                return result;
            }
        }
        public static async Task<UserRecent[]> GetUserRecentAsync(string token, string user, int mode)
        {
            using (HttpClient httpClient = new HttpClient())
            {
                string json = await httpClient.GetStringAsync($"{BaseUrl}get_user_recent?u={user}&k={token}&m={mode}");

                dynamic obj = JArray.Parse(json);

                dynamic[] jsonResults = (dynamic[])obj.ToObject<dynamic[]>();

                UserRecent[] results = new UserRecent[jsonResults.Length];

                int i = 0;
                foreach (dynamic jsonResult in jsonResults)
                {
                    results[i] = new UserRecent
                    {
                        BeatmapId = jsonResult.beatmap_id.ToObject<ulong>(),
                        Score = jsonResult.score.ToObject<ulong>(),
                        MaxCombo = jsonResult.maxcombo.ToObject<int>(),
                        Rank = jsonResult.rank.ToObject<string>(),
                    };
                    i++;
                }
                return results;
            }
        }
    }
}