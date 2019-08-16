using Discord.Commands;
using System.Threading.Tasks;
using OsuApi;
using Discord;
using SenkoSanBot.Services.Configuration;
using System.Linq;
using SenkoSanBot.Services.Database;
using SenkoSanBot.Services.Pagination;

namespace SenkoSanBot.Modules.Osu
{
    [Summary("Commands for osu")]
    public class OsuModule : SenkoSanModuleBase
    {
        public JsonDatabaseService Db { get; set; }

        [Command("osuset")]
        [Summary("Set osu name")]
        public async Task SetUserAsync([Summary("Name to set")] string name = null)
        {
            Db.GetUserEntry(Context.Guild.Id, Context.User.Id).OsuName = name;
            Db.WriteData();
            await ReplyAsync($"Set name to {name}");
        }

        private async Task SearchUserAsync(IUser user, int modeIndex)
        {
            user = user ?? Context.User;
            string name = Db.GetUserEntry(Context.Guild.Id, user.Id).OsuName;
            if (string.IsNullOrEmpty(name))
            {
                await ReplyAsync("Please set your osu name");
                return;
            }
            await SearchUserAsync(name, modeIndex);
        }

        private async Task SearchUserAsync(string name, int modeIndex)
        {
            UserResult result = await Client.GetUserAsync(Config.Configuration.OsuApiToken, name, modeIndex);

            Embed embed = new EmbedBuilder()
                .WithColor(0xE664A0)
                .WithTitle("User Profile")
                .AddField("Username", $"**[{result.UserName}](https://osu.ppy.sh/users/{result.UserId})**")
                .AddField("Accuracy", result.Accuracy.ToString("00.00"), true)
                .AddField("PP", result.PP.ToString("0000"), true)
                .AddField("Play Count", result.PlayCount, true)
                .AddField("Level", result.Level.ToString("00"), true)
                .WithThumbnailUrl($"https://a.ppy.sh/{result.UserId}")
                .WithFooter(footer => {
                    footer
                        .WithText($"Requested by {Context.User}")
                        .WithIconUrl(Context.User.GetAvatarUrl());
                })
                .Build();

            await ReplyAsync(embed: embed);
        }

        public async Task GetRecentPlays(string name, int modeIndex)
        {
            UserRecent[] results = await Client.GetUserRecentAsync(Config.Configuration.OsuApiToken, name, modeIndex);

            await PaginatedMessageService.SendPaginatedDataMessage(
                Context.Channel,
                results,
                (result, index, footer) => GenerateEmbedFor(result, name, footer)
            );



        }

        private Embed GenerateEmbedFor(UserRecent result, string searchWord, EmbedFooterBuilder footer)
        {
            EmbedFieldBuilder fieldBuilder = new EmbedFieldBuilder()
                .WithName("Recent Plays");

            string value = string.Empty;

            int i = 1;

            value = "Nothing found".IfTargetNullOrEmpty(value);

            fieldBuilder.WithValue(value);

            return new EmbedBuilder()
                .WithColor(0x53DF1D)
                .WithTitle($"Search Result For **{searchWord}**")
                .AddField("Beatmap", $"**[{searchWord}](https://jisho.org/search/{url})**")
                .AddField(fieldBuilder)
                .WithFooter(footer)
                .WithThumbnailUrl("")
                .Build();
        }

        [Command("std"), Alias("osu")]
        [Summary("Get std profile details from an user")]
        public async Task OsuSearchUserAsync([Summary("Name to search")] IUser user = null)
        {
            await SearchUserAsync(user, 0);
        }

        [Command("std"), Alias("osu")]
        [Summary("Get std profile details from an user")]
        public async Task OsuSearchUserAsync([Summary("Name to search")] string name)
        {
            await SearchUserAsync(name, 0);
        }

        [Command("taiko")]
        [Summary("Get taiko profile details from an user")]
        public async Task TaikoSearchUserAsync([Summary("Name to search")] IUser user = null)
        {
            await SearchUserAsync(user, 1);
        }

        [Command("taiko")]
        [Summary("Get taiko profile details from an user")]
        public async Task TaikoSearchUserAsync([Summary("Name to search")] string name)
        {
            await SearchUserAsync(name, 1);
        }

        [Command("ctb")]
        [Summary("Get ctb profile details from an user")]
        public async Task CtbSearchUserAsync([Summary("Name to search")] IUser user = null)
        {
            await SearchUserAsync(user, 2);
        }

        [Command("ctb")]
        [Summary("Get ctb profile details from an user")]
        public async Task CtbSearchUserAsync([Summary("Name to search")] string name)
        {
            await SearchUserAsync(name, 2);
        }

        [Command("mania")]
        [Summary("Get mania profile details from an user")]
        public async Task ManiaSearchUserAsync([Summary("Name to search")] IUser user = null)
        {
            await SearchUserAsync(user, 3);
        }

        [Command("mania")]
        [Summary("Get mania profile details from an user")]
        public async Task ManiaSearchUserAsync([Summary("Name to search")] string name)
        {
            await SearchUserAsync(name, 3);
        }
    }
}