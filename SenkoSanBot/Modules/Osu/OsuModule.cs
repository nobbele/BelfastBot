using Discord.Commands;
using System.Threading.Tasks;
using OsuApi;
using Discord;
using SenkoSanBot.Services.Configuration;

namespace SenkoSanBot.Modules.Osu
{
    [Summary("Commands for osu")]
    public class OsuModule : SenkoSanModuleBase
    {
        [Command("osu")]
        [Summary("Get profile details from an user")]
        public async Task SearchUserAsync(string mode, [Summary("Name to search")] [Remainder] string name = "")
        {
            int modeIndex = 0;
            switch (mode)
            {
                case "0":
                case "std":
                    modeIndex = 0;
                    break;
                case "1":
                case "taiko":
                    modeIndex = 1;
                    break;
                case "2":
                case "ctb":
                    modeIndex = 2;
                    break;
                case "3":
                case "mania":
                    modeIndex = 3;
                    break;
                default:
                    if (!string.IsNullOrEmpty(name))
                        name = $"{mode} {name}";
                    else
                        name = mode;
                    break;
            }

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
    }
}