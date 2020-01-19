using Common;
using Newtonsoft.Json;
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

        public static async Task<UserProfile> GetUserAsync(string token, string user, uint mode)
        {
            using (HttpClient httpClient = new HttpClient())
            {
                string json = await httpClient.GetStringAsync($"{BaseUrl.Append("get_user")}?u={HttpUtility.UrlEncode(user)}&k={token}&m={mode}");

                JArray arr = JArray.Parse(json);

                if(arr.Count <= 0)
                    return null;

                UserProfile profile = arr[0].ToObject<UserProfile>();
                profile.Mode = mode;
                return profile;
            }
        }
        public static async Task<PlayResult> GetUserRecentAsync(string token, string user, uint mode)
        {
            Task<UserProfile> userDataTask = GetUserAsync(token, user, mode);

            using (HttpClient httpClient = new HttpClient())
            {
                string json = await httpClient.GetStringAsync($"{BaseUrl.Append("get_user_recent")}?u={HttpUtility.UrlEncode(user)}&k={token}&m={mode}");

                JArray arr = JArray.Parse(json);

                if (arr.Count <= 0)
                    return null;

                JToken jsonResult = arr[0];

                Task<Beatmap> beatmapDataTask = GetBeatmapAsync(token, jsonResult["beatmap_id"].ToObject<ulong>(), mode);

                PlayResult result = jsonResult.ToObject<PlayResult>();

                result.Accuracy = JsonConvert.DeserializeObject<OsuAccuracy>(jsonResult.ToString(), new OsuAccuracyConverter(mode));

                await Task.WhenAll(userDataTask, beatmapDataTask);

                result.PlayerData = userDataTask.Result;
                result.BeatmapData = beatmapDataTask.Result;

                return result;
            }
        }

        public static async Task<Beatmap> GetBeatmapAsync(string token, ulong id, uint mode)
        {
            Console.WriteLine($"Getting beatmap {id} in mode {mode}");
            using (HttpClient httpClient = new HttpClient())
            {
                string json = await httpClient.GetStringAsync($"{BaseUrl.Append("get_beatmaps")}?b={id}&k={token}&m={mode}");

                JArray arr = JArray.Parse(json);

                if(arr.Count <= 0)
                    return null;

                return arr[0].ToObject<Beatmap>();
            }
        }

        public static async Task<UserBest> GetUserBestAsync(string token, string user, uint mode)
        {
            Task<UserProfile> userDataTask = GetUserAsync(token, user, mode);

            using (HttpClient httpClient = new HttpClient())
            {
                string json = await httpClient.GetStringAsync($"{BaseUrl.Append("get_user_best")}?u={HttpUtility.UrlEncode(user)}&k={token}&m={mode}");

                JArray arr = JArray.Parse(json);

                if (arr.Count <= 0)
                    return null;

                JToken jsonResult = arr[0];

                Task<Beatmap> beatmapDataTask = GetBeatmapAsync(token, jsonResult["beatmap_id"].ToObject<ulong>(), mode);

                UserBest result = jsonResult.ToObject<UserBest>();

                result.Accuracy = JsonConvert.DeserializeObject<OsuAccuracy>(jsonResult.ToString(), new OsuAccuracyConverter(mode));

                await Task.WhenAll(userDataTask, beatmapDataTask);

                result.PlayerData = userDataTask.Result;
                result.BeatmapData = beatmapDataTask.Result;

                return result;
            }
        }
    }
}