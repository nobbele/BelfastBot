using Common;
using Newtonsoft.Json.Linq;
using System;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;

namespace OsuApi
{
    public static class Client
    {
        public static readonly Uri BaseUrl = new Uri("https://osu.ppy.sh/api");

        public static async Task<UserProfile> GetUserAsync(string token, string user, int mode)
        {
            using (HttpClient httpClient = new HttpClient())
            {
                string json = await httpClient.GetStringAsync($"{BaseUrl.Append("get_user")}?u={HttpUtility.UrlEncode(user)}&k={token}&m={mode}");

                JArray arr = JArray.Parse(json);

                if(arr.Count <= 0)
                    return null;

                dynamic obj = arr[0];

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
                string json = await httpClient.GetStringAsync($"{BaseUrl.Append("get_user_recent")}?u={HttpUtility.UrlEncode(user)}&k={token}&m={mode}");

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
            Console.WriteLine($"Getting beatmap {id} in mode {mode}");
            using (HttpClient httpClient = new HttpClient())
            {
                string json = await httpClient.GetStringAsync($"{BaseUrl.Append("get_beatmaps")}?b={id}&k={token}&m={mode}");

                json = json.TrimStart(new char[] { '[' }).TrimEnd(new char[] { ']' });

                if(string.IsNullOrEmpty(json))
                    return new Beatmap();

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
                    Length = TimeSpan.FromSeconds((int)jsonResult.total_length.ToObject<int>())
                };
            }
        }

        public static async Task<UserBest> GetUserBestAsync(string token, string user, int mode)
        {
            Task<UserProfile> userDataTask = GetUserAsync(token, user, mode);

            using (HttpClient httpClient = new HttpClient())
            {
                string json = await httpClient.GetStringAsync($"{BaseUrl.Append("get_user_best")}?u={HttpUtility.UrlEncode(user)}&k={token}&m={mode}");

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