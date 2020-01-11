using Discord.Commands;
using System.Threading.Tasks;
using QuaverApi;
using Discord;
using System.Linq;
using SenkoSanBot.Services.Database;
using SenkoSanBot.Services.Pagination;
using Discord.WebSocket;

namespace SenkoSanBot.Modules.Quaver
{
    [Summary("Commands for quaver")]
    public class QuaverModule : SenkoSanModuleBase
    {
        public JsonDatabaseService Db { get; set; }
        public PaginatedMessageService PaginatedMessageService { get; set; }

        private Embed GetUserEmbed((User user, KeyInfo key) data, int index, EmbedFooterBuilder footer) => new EmbedBuilder()
            .WithColor(0xE664A0)
            .WithAuthor(author => {
                author
                    .WithName($"{data.user.Username}'s Quaver {data.key.KeyCount}K Info")
                    .WithUrl($"https://quavergame.com/profile/{data.user.Id}")
                    .WithIconUrl(data.user.AvatarUrl);
            })
            .AddField("Performance Rating", data.key.PerformanceRating.ToString("0.00"), true)
            .WithThumbnailUrl(data.user.AvatarUrl)
            .WithFooter(footer)
            .Build();

        private Embed GetRecentEmbed(User user, Map map, Recent recent) => new EmbedBuilder()
            .WithColor(0xE664A0)
            .WithAuthor(author => {
                author
                    .WithName($"{user.Username}'s Recent 4K Play")
                    .WithUrl($"https://quavergame.com/profile/{user.Id}")
                    .WithIconUrl(user.AvatarUrl);
            })
            .AddField("Map", $"**{map.Artist} - {map.Title} [{map.DifficultyName}] ({map.DifficultyRating})**")
            .AddField("Accuracy", $"{recent.Accuracy.ToString("0.00")}%", true)
            .AddField("Performance Rating", recent.PerformanceRating.ToString("0.00"), true)
            .WithThumbnailUrl($"https://quaver.blob.core.windows.net/banners/{map.Id}_banner.jpg")
            .Build();

        [Command("quaver set")]
        [Summary("Sets the user name for quaver commands")]
        public async Task SetUserAsync([Summary("Name to set")] string name)
        {
            uint id = (Db.GetUserEntry(0, Context.User.Id).QuaverId = await Client.GetUserIdByNameAsync(name)).Value;
            await ReplyAsync($"Set user id to {id}");
        }

        [Command("quaver user")]
        [Summary("Get info about user")]
        public async Task GetUserAsync([Summary("Name to get info about")] string name = null)
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

        [Command("quaver recent 4k"), Alias("qr4k")]
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

        [Command("quaver recent 7k"), Alias("qr7k")]
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