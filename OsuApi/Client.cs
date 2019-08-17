using Newtonsoft.Json.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace OsuApi
{
    public static class Client
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

                return new UserProfile
                {
                    UserId = jsonResult.user_id.ToObject<ulong>(),
                    UserName = jsonResult.username.ToObject<string>(),
                    Mode = mode,
                    PP = jsonResult.pp_raw.ToObject<float?>() ?? (float)0,
                    Level = jsonResult.level.ToObject<float?>() ?? (float)0,
                    PlayCount = jsonResult.playcount.ToObject<ulong?>() ?? (ulong)0,
                    Accuracy = jsonResult.accuracy.ToObject<float?>() ?? (float)0,
                };
            }
        }
        public static async Task<PlayResult> GetUserRecentAsync(string token, string user, int mode)
        {
            Task<UserProfile> userDataTask = GetUserAsync(token, user, mode);

            using (HttpClient httpClient = new HttpClient())
            {
                string json = await httpClient.GetStringAsync($"{BaseUrl}/get_user_recent?u={user}&k={token}&m={mode}");

                dynamic obj = JArray.Parse(json);

                dynamic[] jsonResults = (dynamic[])obj.ToObject<dynamic[]>();

                if (jsonResults.Length <= 0)
                    return new PlayResult();
                dynamic jsonResult = jsonResults[0];

                Task<Beatmap> beatmapDataTask = GetBeatmapAsync(token, jsonResult.beatmap_id.ToObject<ulong>(), mode);

                await Task.WhenAll(userDataTask, beatmapDataTask);

                return new PlayResult
                {
                    PlayerData = userDataTask.Result,
                    BeatmapData = beatmapDataTask.Result,
                    Score = jsonResult.score.ToObject<ulong>(),
                    Combo = jsonResult.maxcombo.ToObject<int>(),
                    Rank = jsonResult.rank.ToObject<string>(),
                };
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

                return new Beatmap
                {
                    Name = jsonResult.title.ToObject<string>(),
                    CreatorName = jsonResult.creator.ToObject<string>(),
                    CreatorId = jsonResult.creator_id.ToObject<ulong>(),
                    Id = jsonResult.beatmap_id.ToObject<ulong>(),
                    SetId = jsonResult.beatmapset_id.ToObject<ulong>(),
                    StarRating = jsonResult.difficultyrating.ToObject<float>(),
                    Bpm = jsonResult.bpm.ToObject<int>(),
                };
            }
        }

        public static async Task<UserBest> GetUserBestAsync(string token, string user, int mode)
        {
            Task<UserProfile> userDataTask = GetUserAsync(token, user, mode);

            using (HttpClient httpClient = new HttpClient())
            {
                string json = await httpClient.GetStringAsync($"{BaseUrl}/get_user_best?u={user}&k={token}&m={mode}");

                dynamic obj = JArray.Parse(json);

                dynamic[] jsonResults = (dynamic[])obj.ToObject<dynamic[]>();

                if (jsonResults.Length <= 0)
                    return new UserBest();
                dynamic jsonResult = jsonResults[0];

                Task<Beatmap> beatmapDataTask = GetBeatmapAsync(token, jsonResult.beatmap_id.ToObject<ulong>(), mode);

                await Task.WhenAll(userDataTask, beatmapDataTask);

                return new UserBest
                {
                    PlayerData = userDataTask.Result,
                    BeatmapData = beatmapDataTask.Result,
                    Score = jsonResult.score.ToObject<ulong>(),
                    Combo = jsonResult.maxcombo.ToObject<int>(),
                    Rank = jsonResult.rank.ToObject<string>(),
                    PP = jsonResult.pp.ToObject<float>(),
                };
            }
        }
    }
}