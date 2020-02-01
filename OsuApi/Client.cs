using Common;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using osu.Game.Beatmaps;
using osu.Game.Rulesets.Osu.Difficulty;
using osu.Game.Rulesets.Osu;
using osu.Game.Scoring;
using Moq;
using System.IO;
using osu.Game.Rulesets.Scoring;
using osu.Game.Rulesets.Taiko;
using osu.Game.Rulesets.Catch;
using osu.Game.Rulesets.Mania;
using osu.Game.Rulesets;
using osu.Game.Rulesets.Difficulty;
using System.Collections.Concurrent;

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

        public static async Task<PlayResult[]> GetUserRecentAsync(string token, string user, uint mode, uint count = 10)
        {
            Task<UserProfile> userDataTask = GetUserAsync(token, user, mode);

            using (HttpClient httpClient = new HttpClient())
            {
                string json = await httpClient.GetStringAsync($"{BaseUrl.Append("get_user_recent")}?u={HttpUtility.UrlEncode(user)}&k={token}&m={mode}");

                PlayResult[] results = await GetPlayResultAsync(token, mode, json, userDataTask, count);

                return results;
            }
        }

        public static float GetPP(Stream beatmapStream, PlayResult result, bool ifSs)
        {
            Ruleset ruleset = result.Mode switch
            {
                0 => new OsuRuleset(),
                1 => new TaikoRuleset(),
                2 => new CatchRuleset(),
                3 => new ManiaRuleset(),
                _ => throw new Exception("Invalid mode"),
            };
            BeatmapInfo beatmapInfo = new BeatmapInfo()
            {
                BaseDifficulty = new BeatmapDifficulty()
                {
                    DrainRate = result.BeatmapData.DrainRate,
                    ApproachRate = result.BeatmapData.ApproachRate,
                    CircleSize = result.BeatmapData.CircleSize,
                    OverallDifficulty = result.BeatmapData.OverallDifficulty,
                },
                Ruleset = ruleset.RulesetInfo,
            };
            FakeWorkingBeatmap beatmap = new FakeWorkingBeatmap(beatmapStream, beatmapInfo);
            ScoreInfo score = new ScoreInfo()
            {
                MaxCombo = (int)result.Combo,
                Mods = ruleset.GetAllMods().Where(mod => result.Mods.GetFromBitflag().Select(m => m.ToShortString()).Contains(mod.Acronym)).ToArray(),
                Accuracy = (float)result.Accuracy.Accuracy,
                Statistics = result.Accuracy.Statistics,
            };
            if(ifSs)
            {
                score.Statistics = new Dictionary<HitResult, int>()
                {
                    { HitResult.Miss, 0 },
                    { HitResult.Ok, 0 },
                    { HitResult.Meh, 0 },
                    { HitResult.Good, 0 },
                    { HitResult.Perfect, 0 },
                    { HitResult.Great, result.Accuracy.Statistics.Sum(k => k.Value) }
                };
            }
            PerformanceCalculator calc = ruleset.CreatePerformanceCalculator(beatmap, score);
            
            return (float)calc.Calculate();
        }

        public static async Task<PlayResult[]> GetUserBestAsync(string token, string user, uint mode, uint count = 10)
        {
            Task<UserProfile> userDataTask = GetUserAsync(token, user, mode);

            using (HttpClient httpClient = new HttpClient())
            {
                string json = await httpClient.GetStringAsync($"{BaseUrl.Append("get_user_best")}?u={HttpUtility.UrlEncode(user)}&k={token}&m={mode}");

                PlayResult[] results = await GetPlayResultAsync(token, mode, json, userDataTask, count);

                return results;
            }
        }

        private static async Task<PlayResult[]> GetPlayResultAsync(string token, uint mode, string json, Task<UserProfile> userDataTask, uint count)
        {
            JArray arr = JArray.Parse(json);

            List<PlayResult> results = new List<PlayResult>(arr.Count);

            using(HttpClient httpClient = new HttpClient())
            {
                foreach(JToken jsonResult in arr)
                {
                    Task<Beatmap> beatmapDataTask = GetBeatmapAsync(token, jsonResult["beatmap_id"].ToObject<ulong>(), mode);

                    PlayResult result = jsonResult.ToObject<PlayResult>();

                    result.Mode = mode;

                    result.Accuracy = JsonConvert.DeserializeObject<OsuAccuracy>(jsonResult.ToString(), new OsuAccuracyConverter(mode));

                    await Task.WhenAll(userDataTask, beatmapDataTask);

                    result.PlayerData = userDataTask.Result;
                    result.BeatmapData = beatmapDataTask.Result;

                    if(result.BeatmapData == null || result.PlayerData == null)
                        continue;

                    Stream beatmap = await httpClient.GetStreamAsync($"https://osu.ppy.sh/osu/{result.BeatmapData.Id}");
                    if(float.IsNaN(result.PP))
                        result.PP = GetPP(beatmap, result, false);
                    if(float.IsNaN(result.SSPP))
                        result.SSPP = GetPP(beatmap, result, true);

                    results.Add(result);

                    if(results.Count >= count)
                        break;
                };
            }

            return results.ToArray();
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
    }
}