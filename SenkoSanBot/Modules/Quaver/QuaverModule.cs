using Discord.Commands;
using System.Threading.Tasks;
using QuaverApi;
using Discord;
using SenkoSanBot.Services.Database;
using SenkoSanBot.Services.Pagination;

namespace SenkoSanBot.Modules.Quaver
{
    [Summary("Commands for quaver")]
    public class QuaverModule : SenkoSanModuleBase
    {
        public JsonDatabaseService Db { get; set; }
        public PaginatedMessageService PaginatedMessageService { get; set; }

        private Embed GetUserEmbed((User user, KeyInfo key) data, int index, EmbedFooterBuilder footer) => new EmbedBuilder()
            .WithColor(0x43EBFB)
            .WithAuthor(author => {
                author
                    .WithName($"{data.user.Username}'s Quaver {data.key.KeyCount}K Info")
                    .WithUrl($"https://quavergame.com/profile/{data.user.Id}")
                    .WithIconUrl($"https://quavergame.com/static/flags/4x3/{data.user.Country}.png");
            })
            .AddField("Details ▼", $"" +
            $"__**Main Details**__\n" +
            $"► Accuracy: **{data.key.Accuracy.ToString("0.00")}**\n" +
            $"► Performance: **{data.key.PerformanceRating.ToString("0.00")}**\n" +
            $"► Play Count: **{data.key.PlayCount}**\n" +
            $"__**Ranking**__\n" +
            $"► Global Rank: **{data.key.GlobalRanking}**\n" +
            $"► Country Rank: **{data.key.CountryRanking} [{data.user.Country}]**")
            .WithThumbnailUrl(data.user.AvatarUrl)
            .WithFooter(footer)
            .Build();

        private Embed GetRecentEmbed(User user, Map map, Recent recent) => new EmbedBuilder()
            .WithColor(0x43EBFB)
            .WithAuthor(author => {
                author
                    .WithName($"{user.Username}'s Recent 4K Play")
                    .WithUrl($"https://quavergame.com/profile/{user.Id}")
                    .WithIconUrl(user.AvatarUrl);
            })
            .AddField("Details ▼", $"" +
            $"__**Main Details**__\n" +
            $"► Performance: **{recent.PerformanceRating:F2}**\n" +
            $"► Grade: **{recent.Grade}**\n" +
            $"► Accuracy: **{recent.Accuracy:F2}**\n" +
            $"► Combo: **{recent.Combo}**\n" +
            $"__**Map**__\n" +
            $"**[{map.Title}](https://quavergame.com/mapsets/map/{map.Id})**\n" +
            $"► **{map.DifficultyName} [{map.DifficultyRating:F2}☆]**\n" +
            $"► Made By: **[{map.Creator}]**")
            .WithImageUrl($"https://quaver.blob.core.windows.net/banners/{map.MapSetId}_banner.jpg")
            .Build();

        [Command("quaver set")]
        [Summary("Sets the user name for quaver commands")]
        public async Task SetUserAsync([Summary("Name to set")] string name)
        {
            uint id = (Db.GetUserEntry(0, Context.User.Id).QuaverId = await Client.GetUserIdByNameAsync(name)).Value;
            await ReplyAsync($"Set user id to {id}");
        }

        [Command("quaver")]
        [Summary("Get info about user")]
        public async Task GetUserAsync([Summary("Name to get info about")] [Remainder] string name = null)
        {
            uint? id = null;
            if(string.IsNullOrEmpty(name))
            {
                id = Db.GetUserEntry(0, Context.User.Id).QuaverId;
            }
            else
            {
                id = await Client.GetUserIdByNameAsync(name);
            }

            if(id == null)
            {
                await ReplyAsync("Couldn't find any user, please set one or specify in arguments");
                return;
            }

            uint userId = id.Value;

            User user = await Client.GetUserAsync(userId);
            (User, KeyInfo)[] keys = { (user, user.FourKeys), (user, user.SevenKeys) };

            await PaginatedMessageService.SendPaginatedDataMessageAsync(Context.Channel, keys, GetUserEmbed);
        }

        [Command("quaverrecent 4k"), Alias("qr4k")]
        [Summary("Get recent 4k play info by user")]
        public async Task GetRecent4KAsync([Summary("User to get recent play from")] string name = null)
        {
            uint? id = null;
            if(string.IsNullOrEmpty(name))
            {
                id = Db.GetUserEntry(0, Context.User.Id).QuaverId;
            }
            else
            {
                id = await Client.GetUserIdByNameAsync(name);
            }

            if(id == null)
            {
                await ReplyAsync("Couldn't find any user, please set one or specify in arguments");
                return;
            }

            uint userId = id.Value;

            Recent recent = await Client.GetUserRecentAsync(userId, 1);
            Map map = recent.Map;
            User user = await Client.GetUserAsync(userId);

            await ReplyAsync(embed: GetRecentEmbed(user, map, recent));
        }

        [Command("quaverrecent 7k"), Alias("qr7k")]
        [Summary("Get recent 7k play info by user")]
        public async Task GetRecent7KAsync([Summary("User to get recent play from")] string name = null)
        {
            uint? id = null;
            if(string.IsNullOrEmpty(name))
            {
                id = Db.GetUserEntry(0, Context.User.Id).QuaverId;
            }
            else
            {
                id = await Client.GetUserIdByNameAsync(name);
            }

            if(id == null)
            {
                await ReplyAsync("Couldn't find any user, please set one or specify in arguments");
                return;
            }

            uint userId = id.Value;

            Recent recent = await Client.GetUserRecentAsync(userId, 2);
            Map map = recent.Map;
            User user = await Client.GetUserAsync(userId);

            await ReplyAsync(embed: GetRecentEmbed(user, map, recent));
        }
    }
}