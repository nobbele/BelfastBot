using Discord.Commands;
using System.Threading.Tasks;
using OsuApi;
using Discord;
using System.Linq;
using SenkoSanBot.Services.Database;
using SenkoSanBot.Services.Pagination;
using Discord.WebSocket;

namespace SenkoSanBot.Modules.Osu
{
    [Summary("Commands for osu")]
    public class OsuModule : SenkoSanModuleBase
    {
        public JsonDatabaseService Db { get; set; }
        public PaginatedMessageService PaginatedMessageService { get; set; }
        public DiscordSocketClient IClient { get; set; }

        private string GetNameForModeIndex(uint mode)
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

        private string GetLinkSuffixForModeIndex(uint mode)
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
            return "Unknown";
        }

        private IEmote GetEmoteForRank(string rank)
        {
            switch(rank)
            {
                case "SS":
                    return Emotes.OsuSS;
                case "XH":
                    return Emotes.OsuXH;
                case "S":
                    return Emotes.OsuS;
                case "SH":
                    return Emotes.OsuSH;
                case "A":
                    return Emotes.OsuA;
                case "B":
                    return Emotes.OsuB;
                case "C":
                    return Emotes.OsuC;
                case "D":
                    return Emotes.OsuD;
                case "F":
                    return Emotes.OsuF;
            }
            return Emotes.SenkoShock;
        }

        private Embed GetUserProfileEmbed(UserProfile user, int index, EmbedFooterBuilder footer) => new EmbedBuilder()
            .WithColor(0xE664A0)
            .WithAuthor(author => {
                author
                    .WithName($"{user.UserName}'s osu!{GetNameForModeIndex(user.Mode)} Data")
                    .WithUrl($"https://osu.ppy.sh/users/{user.UserId}/{GetLinkSuffixForModeIndex(user.Mode)}")
                    .WithIconUrl($"https://osu.ppy.sh/images/flags/{user.Country}.png");
            })
            .AddField("Details ▼", $"" +
            $"__**Main Details**__\n" +
            $"► Accuracy: **{user.Accuracy:F2}%**\n" +
            $"► PP: **{user.PP:F2}**\n" +
            $"► Play Count: **{user.PlayCount}**\n" +
            $"► Level: **{user.Level:F0}**\n" +
            $"__**Ranking**__\n" +
            $"► Global Rank: **{user.GlobalRanking}**\n" +
            $"► Country Rank: **{user.CountryRanking} [{user.Country}]**")
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
            .AddField($"Details ▼", $"" +
            $"__**Main Details**__\n" +
            $"► Rank: **{GetEmoteForRank(result.Rank)}**\n" +
            $"► Accuracy: **{(result.Accuracy?.Accuracy ?? 0) * 100:F2}%**\n" +
            $"► PP: **{result.PP:F2}**\n" +
            $"► Mods: **{"None".IfTargetIsNullOrEmpty(result.Mods.ToShortString())}**\n" +
            $"► Score: **{result.Score}**\n" +
            $"► Combo: **{result.Combo}**\n" +
            $"__**Beatmap**__\n" +
            $"**[{result.BeatmapData.Name}](https://osu.ppy.sh/b/{result.BeatmapData.Id})**\n" +
            $"► **[{result.BeatmapData.StarRating:F2}☆] {result.BeatmapData.Bpm}** Bpm\n" +
            $"► Lenght **{result.BeatmapData.Length.ToShortForm()}**\n" +
            $"► Made By: **[{result.BeatmapData.CreatorName}](https://osu.ppy.sh/users/{result.BeatmapData.CreatorId})**")
            .WithImageUrl($"https://assets.ppy.sh/beatmaps/{result.BeatmapData.SetId}/covers/cover.jpg")
            .WithFooter(footer)
            .Build();

        private Embed GetUserBestEmbed(UserBest result, int index, EmbedFooterBuilder footer) => 
            GetBeatmapResultEmbed(result.PlayResult, index, footer);

        private string GetOsuUsername(IUser user)
        {
            string name = Db.GetUserEntry(0, user.Id).OsuName;
            if (string.IsNullOrEmpty(name))
            {
                return null;
            }
            return name;
        }

        [Command("osuset")]
        [Summary("Set osu name")]
        public async Task SetUserAsync([Summary("Name to set")] string name)
        {
            Db.GetUserEntry(0, Context.User.Id).OsuName = name;
            Db.WriteData();
            await ReplyAsync($"> Your osu name is now set to **{name}**");
        }

        [Command("osu")]
        [Summary("Get std profile details from an user")]
        public async Task OsuGetUserAsync([Summary("Name to search")] [Remainder] string target_name = "")
        {
            string username = null;

            IUser target = Context.Message.MentionedUsers.FirstOrDefault();

            if (target != null && target.IsBot)
                return;

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
            var taskList = Enumerable.Range(0, modeCount).Select(i => Client.GetUserAsync(Config.Configuration.OsuApiToken, username, (uint)i));
            UserProfile[] results = await Task.WhenAll(taskList);

            results = results.Where(res => res != null).ToArray();

            if(results.Length <= 0)
                await ReplyAsync($"No user **{username}** found");
            else
                await PaginatedMessageService.SendPaginatedDataMessageAsync(Context.Channel, results, GetUserProfileEmbed);
        }

        [Command("osurecent"), Alias("rs")]
        [Summary("Get recent play")]
        public async Task GetRecentPlay([Summary("Name to search")] [Remainder] string target_name = "")
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
            for(uint i = 0; i < modeCount; i++)
                taskList[i] = Client.GetUserRecentAsync(Config.Configuration.OsuApiToken, username, i);
            PlayResult[] results = await Task.WhenAll(taskList);

            PlayResult[] validResults = results.Where(result => (result?.BeatmapData?.Id ?? 0) != 0).ToArray();

            if(validResults.Length > 0)
                await PaginatedMessageService.SendPaginatedDataMessageAsync(Context.Channel, validResults, GetBeatmapResultEmbed);
            else
                await ReplyAsync($"No recent plays found for {username}");
        }

        [Command("osubest"), Alias("bs")]
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
            for (uint i = 0; i < modeCount; i++)
                taskList[i] = Client.GetUserBestAsync(Config.Configuration.OsuApiToken, username, i);
            UserBest[] results = await Task.WhenAll(taskList);

            UserBest[] validResults = results.Where(result => (result?.PlayResult.BeatmapData.Id ?? 0) != 0).ToArray();

            if(validResults.Length > 0)
                await PaginatedMessageService.SendPaginatedDataMessageAsync(Context.Channel, validResults, GetUserBestEmbed);
            else
                await ReplyAsync($"No best plays found for {username}");
        }
    }
}