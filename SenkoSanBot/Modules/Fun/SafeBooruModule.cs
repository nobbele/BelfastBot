using Discord;
using Discord.Commands;
using System.Threading.Tasks;

namespace SenkoSanBot.Modules.Fun
{
    public class SafeBooruModule : ModuleBase<SocketCommandContext>
    {
        [Command("sbi")]
        [Summary("Gets images from safebooru with the given tag")]
        public async Task GetImage([Summary("Tag for search")] [Remainder] string tag)
        {
            tag = tag.Replace(' ', '_');
            string url = await SafeBooruApi.Client.GetRandomPostAsync(tag);

            Embed embed = new EmbedBuilder()
                .WithColor(new Color(0xb39df2))
                .WithTitle("Image From Safebooru.org")
                .WithImageUrl(url)
                .Build();

            await ReplyAsync(embed: embed);
        }

    }
}
