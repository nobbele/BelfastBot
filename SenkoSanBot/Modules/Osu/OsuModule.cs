using Discord.Commands;
using System.Threading.Tasks;
using OsuApi;
using Discord;
using SenkoSanBot.Services.Configuration;
using System.Linq;
using SenkoSanBot.Services.Database;
using SenkoSanBot.Services.Pagination;
using Discord.WebSocket;
using System.Collections.Generic;

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
            Db.GetUserEntry(Context.Guild.Id, Context.User.Id).OsuName = name;
            Db.WriteData();
            await ReplyAsync($"Set name to {name}");
        }

        private string GetNameForModeIndex(int mode)
        {
            switch(mode)
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

        private Embed GetUserProfileEmbed(UserProfile user, int index, EmbedFooterBuilder footer) => new EmbedBuilder()
                .WithColor(0xE664A0)
                .WithTitle($"User Profile for mode osu!{GetNameForModeIndex(user.Mode)}")
                .AddField("Username", $"**[{user.UserName}](https://osu.ppy.sh/users/{user.UserId}/{GetLinkSuffixForModeIndex(user.Mode)})**")
                .AddField("Accuracy", user.Accuracy.ToString("00.00"), true)
                .AddField("PP", user.PP.ToString("00.00"), true)
                .AddField("Play Count", user.PlayCount, true)
                .AddField("Level", user.Level.ToString("00.00"), true)
                .WithThumbnailUrl($"https://a.ppy.sh/{user.UserId}")
                .WithFooter(footer)
                .Build();

        private string GetOsuUsername(IUser user)
        {
            string name = Db.GetUserEntry(Context.Guild.Id, user.Id).OsuName;
            if (string.IsNullOrEmpty(name))
            {
                return null;
            }
            return name;
        }

        [Command("std"), Alias("osu")]
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

            const int modeCount = 4;
            var taskList = Enumerable.Range(0, modeCount).Select(i => Client.GetUserAsync(Config.Configuration.OsuApiToken, username, i));
            IEnumerable<UserProfile> results = await Task.WhenAll(taskList);

            await PaginatedMessageService.SendPaginatedDataMessage(Context.Channel, results, GetUserProfileEmbed);
        }

        private Embed GetBeatmapResultEmbed(PlayResult result, int index, EmbedFooterBuilder footer) => new EmbedBuilder()
                .WithColor(0x53DF1D)
                .WithTitle($"Recent plays by playername")
                .AddField("Rank", result.Rank)
                .AddField("Beatmap", $"**[beatmapName](https://osu.ppy.sh/beatmapsets/{result.BeatmapId})**")
                .WithFooter(footer)
                .WithThumbnailUrl("")
                .Build();

        [Command("recent"), Alias("rs")]
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

            const int modeCount = 4;
            var taskList = Enumerable.Range(0, modeCount).Select(i => Client.GetUserRecentAsync(Config.Configuration.OsuApiToken, username, i));
            IEnumerable<PlayResult> results = (await Task.WhenAll(taskList)).Select(recents => recents.FirstOrDefault());

            IEnumerable<PlayResult> validResults = results.Where(result => result.BeatmapId != 0);

            await PaginatedMessageService.SendPaginatedDataMessage(Context.Channel, validResults, GetBeatmapResultEmbed);
        }
    }
}