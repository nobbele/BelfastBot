using Discord;
using Discord.Commands;
using System.Threading.Tasks;

namespace BelfastBot.Modules.Fun
{
    [Summary("Commands for booru websites")]
    public class SafeBooruModule : BelfastModuleBase
    {
        [Command("booru"), Alias("sbi")]
        [Summary("Gets images from safebooru with the given tag")]
        public async Task GetImage([Summary("Tag for search")] [Remainder] string tag = "azur_lane")
        {
            Logger.LogInfo($"{Context.User} requested a gif from safebooru");

            tag = tag.Replace(' ', '_');
            string url = await SafeBooruApi.Client.GetRandomPostAsync(tag);

            if(url == null)
            {
                await ReplyAsync("The requested tag is blacklisted, doesn't exist or all recent posts contain blacklisted tags");
                return;
            }

            Embed embed = new EmbedBuilder()
                .WithColor(0xb39df2)
                .WithTitle("Image From Safebooru.org")
                .WithImageUrl(url)
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