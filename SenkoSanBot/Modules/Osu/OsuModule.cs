using Discord.Commands;
using System.Threading.Tasks;
using OsuApi;
using Discord;

namespace SenkoSanBot.Modules.Osu
{
    [Summary("Commands for osu")]
    public class OsuModule : SenkoSanModuleBase
    {
        [Command("osu")]
        [Summary("Get profile details from an user")]
        public async Task SearchUserAsync([Summary("Name to search")] [Remainder] string name, string mode = "std")
        {
            int modeIndex = 0;
            switch (mode)
            {
                case "std":
                    modeIndex = 0;
                    break;
                case "taiko":
                    modeIndex = 1;
                    break;
                case "ctb":
                    modeIndex = 2;
                    break;
                case "mania":
                    modeIndex = 3;
                    break;
            }

            UserResult result = await Client.GetUser(name, modeIndex);

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
    }
}