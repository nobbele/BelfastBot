using Discord;
using Discord.Commands;
using System.Threading.Tasks;

namespace SenkoSanBot.Modules.Fun
{
    [Summary("Commands for booru websites")]
    public class SafeBooruModule : ModuleBase<SocketCommandContext>
    {
        [Command("booru"), Alias("sbi")]
        [Summary("Gets images from safebooru with the given tag")]
        public async Task GetImage([Summary("Tag for search")] [Remainder] string tag = "sewayaki_kitsune_no_senko-san")
        {
            tag = tag.Replace(' ', '_');
            string url = await SafeBooruApi.Client.GetRandomPostAsync(tag);

            Embed embed = new EmbedBuilder()
                .WithColor(new Color(0xb39df2))
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
