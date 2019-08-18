using Discord.Commands;
using System.Threading.Tasks;
using OsuApi;
using Discord;
using System.Linq;
using SenkoSanBot.Services.Database;
using SenkoSanBot.Services.Pagination;
using Discord.WebSocket;
using System;

namespace SenkoSanBot.Modules.Osu
{
    [Summary("Commands for osu")]
    public class OsuModule : SenkoSanModuleBase
    {
        public JsonDatabaseService Db { get; set; }
        public PaginatedMessageService PaginatedMessageService { get; set; }
        public DiscordSocketClient IClient { get; set; }


        [Command("osuset")]
        [Summary("Set osu name")]
        public async Task SetUserAsync([Summary("Name to set")] string name = null)
        {
            Db.GetUserEntry(0, Context.User.Id).OsuName = name;
            Db.WriteData();
            await ReplyAsync($"> Your osu name is now set to **{name}**");
        }

        #region Mode Index
        private string GetNameForModeIndex(int mode)
        {
            switch (mode)
            {
                case 0:
                    return "standard";
                case 1:
                    return "taiko";
                case 2:
                    return "ctb";
                case 3:
                    return "mania";
            }
            return "Unknown";
        }

        private string GetLinkSuffixForModeIndex(int mode)
        {
            switch (mode)
            {
                case 0:
                    return "osu";
                case 1:
                    return "taiko";
                case 2:
                    return "fruits";
                case 3:
                    return "mania";
            }
            return "";
        }
        #endregion

        #region Embed
        private Embed GetUserProfileEmbed(UserProfile user, int index, EmbedFooterBuilder footer) => new EmbedBuilder()
            .WithColor(0xE664A0)
            .WithAuthor(author => {
                author
                    .WithName($"{user.UserName}'s osu!{GetNameForModeIndex(user.Mode)} Data")
                    .WithUrl($"https://osu.ppy.sh/users/{user.UserId}/{GetLinkSuffixForModeIndex(user.Mode)}")
                    .WithIconUrl($"https://a.ppy.sh/{user.UserId}");
            })
            .AddField("<:rankingS:462313719762911233> Accuracy", user.Accuracy.ToString("0.00"), true)
            .AddField("PP", user.PP.ToString("0.00"), true)
            .AddField("Play Count", user.PlayCount, true)
            .AddField("Level", user.Level.ToString("0"), true)
            .WithThumbnailUrl($"https://a.ppy.sh/{user.UserId}")
            .WithFooter(footer)
            .Build();

        private Embed GetBeatmapResultEmbed(PlayResult result, int index, EmbedFooterBuilder footer) => new EmbedBuilder()
            .WithColor(0xE664A0)
            .WithAuthor(author => {
                author
                    .WithName($"{result.PlayerData.UserName}'s Recent osu!{GetNameForModeIndex(result.PlayerData.Mode)} Play")
                    .WithUrl($"https://osu.ppy.sh/users/{result.PlayerData.UserId}/{GetLinkSuffixForModeIndex(result.PlayerData.Mode)}")
                    .WithIconUrl($"https://a.ppy.sh/{result.PlayerData.UserId}");
            })
            .AddField("Details", $"s **Rank: {result.Rank.Replace("X", "SS")}\n► Score: {result.Score} | Combo: {result.Combo}**")
            .AddField("Beatmap", $"**[{result.BeatmapData.Name}](https://osu.ppy.sh/b/{result.BeatmapData.Id}) " +
            $"[{result.BeatmapData.StarRating.ToString("0.00")}☆] {result.BeatmapData.Bpm} Bpm** Length: **{result.BeatmapData.Lenght.ToShortForm()}**" +
            $"\n **Made By: [{result.BeatmapData.CreatorName}](https://osu.ppy.sh/users/{result.BeatmapData.CreatorId})**")
            .WithImageUrl($"https://assets.ppy.sh/beatmaps/{result.BeatmapData.SetId}/covers/cover.jpg")
            .WithFooter(footer)
            .Build();

        private Embed GetUserBestEmbed(UserBest result, int index, EmbedFooterBuilder footer) => new EmbedBuilder()
            .WithColor(0xE664A0)
            .WithFooter(footer)
            .WithAuthor(author => {
                author
                    .WithName($"{result.PlayerData.UserName}'s Best Plays on osu!{GetNameForModeIndex(result.PlayerData.Mode)}")
                    .WithUrl($"https://osu.ppy.sh/users/{result.PlayerData.UserId}/{GetLinkSuffixForModeIndex(result.PlayerData.Mode)}")
                    .WithIconUrl($"https://a.ppy.sh/{result.PlayerData.UserId}");
            })
            .AddField("Details", $"**Rank: {result.Rank} ► PP: {result.PP.ToString("0.00")} | Score: {result.Score} | Combo: {result.Combo}**")
            .AddField("Beatmap", $"**[{result.BeatmapData.Name}](https://osu.ppy.sh/b/{result.BeatmapData.Id}) " +
            $"[{result.BeatmapData.StarRating.ToString("0.00")}☆] {result.BeatmapData.Bpm} Bpm** " +
            $"\n **Made By: [{result.BeatmapData.CreatorName}](https://osu.ppy.sh/users/{result.BeatmapData.CreatorId})**")
            .WithImageUrl($"https://assets.ppy.sh/beatmaps/{result.BeatmapData.SetId}/covers/cover.jpg")
            .Build();
        #endregion

        private string GetOsuUsername(IUser user)
        {
            string name = Db.GetUserEntry(0, user.Id).OsuName;
            if (string.IsNullOrEmpty(name))
            {
                return null;
            }
            return name;
        }

        [Command("osu"), Alias("prf")]
        [Summary("Get std profile details from an user")]
        public async Task OsuGetUserAsync([Summary("Name to search")] string target_name = "")
        {
            string username = null;

            IUser target = Context.Message.MentionedUsers.FirstOrDefault();
            if (target != null)
                username = GetOsuUsername(target);
            else if (!string.IsNullOrEmpty(target_name))
                username = target_name;
            else
                username = GetOsuUsername(Context.User);

            if (username == null)
            {
                await ReplyAsync("Couldn't find a valid user, have you set your username using osuset?");
                return;
            }

            Logger.LogInfo($"Searching for user {username} on Osu");

            const int modeCount = 4;
            var taskList = Enumerable.Range(0, modeCount).Select(i => Client.GetUserAsync(Config.Configuration.OsuApiToken, username, i));
            UserProfile[] results = await Task.WhenAll(taskList);

            await PaginatedMessageService.SendPaginatedDataMessageAsync(Context.Channel, results, GetUserProfileEmbed);
        }

        [Command("osu recent"), Alias("rs")]
        [Summary("Get recent play")]
        public async Task GetRecentPlay([Summary("Name to search")] string target_name = "")
        {

            string username = null;

            IUser target = Context.Message.MentionedUsers.FirstOrDefault();
            if (target != null)
                username = GetOsuUsername(target);
            else if (!string.IsNullOrEmpty(target_name))
                username = target_name;
            else
                username = GetOsuUsername(Context.User);

            if (username == null)
            {
                await ReplyAsync("Couldn't find a valid user, have you set your username using osuset?");
                return;
            }

            Logger.LogInfo($"Searching for user {username}'s recent plays on Osu");

            const int modeCount = 4;
            Task<PlayResult>[] taskList = new Task<PlayResult>[modeCount];
            for(int i = 0; i < modeCount; i++)
                taskList[i] = Client.GetUserRecentAsync(Config.Configuration.OsuApiToken, username, i);
            PlayResult[] results = await Task.WhenAll(taskList);

            PlayResult[] validResults = results.Where(result => result.BeatmapData.Id != 0).ToArray();

            await PaginatedMessageService.SendPaginatedDataMessageAsync(Context.Channel, validResults, GetBeatmapResultEmbed);
        }

        [Command("osu best"), Alias("bs")]
        [Summary("Get recent play")]
        public async Task GetBestPlays([Summary("Name to search")] string target_name = "")
        {
            string username = null;

            IUser target = Context.Message.MentionedUsers.FirstOrDefault();
            if (target != null)
                username = GetOsuUsername(target);
            else if (!string.IsNullOrEmpty(target_name))
                username = target_name;
            else
                username = GetOsuUsername(Context.User);

            if (username == null)
            {
                await ReplyAsync("Couldn't find a valid user, have you set your username using **.osuset**?");
                return;
            }

            Logger.LogInfo($"Searching for user {username}'s best plays on Osu");

            const int modeCount = 4;
            Task<UserBest>[] taskList = new Task<UserBest>[modeCount];
            for (int i = 0; i < modeCount; i++)
                taskList[i] = Client.GetUserBestAsync(Config.Configuration.OsuApiToken, username, i);
            UserBest[] results = await Task.WhenAll(taskList);

            UserBest[] validResults = results.Where(result => result.BeatmapData.Id != 0).ToArray();

            await PaginatedMessageService.SendPaginatedDataMessageAsync(Context.Channel, validResults, GetUserBestEmbed);
        }
    }

    public static class TimeSpanExtensionMethods
    {
        public static string ToShortForm(this TimeSpan t)
        {
            string shortForm = "";
            if (t.Hours > 0)
            {
                shortForm += string.Format("{0}h", t.Hours.ToString());
            }
            if (t.Minutes > 0)
            {
                shortForm += string.Format("{0}m", t.Minutes.ToString());
            }
            if (t.Seconds > 0)
            {
                shortForm += string.Format("{0}s", t.Seconds.ToString());
            }
            return shortForm;
        }
    }
}