using Discord;
using Discord.Commands;
using Discord.WebSocket;
using System.Threading.Tasks;

namespace SenkoSanBot.Modules.Fun
{
    public class NekosLifeModule : ModuleBase<SocketCommandContext>
    {
        public DiscordSocketClient Client { get; set; }

        [Command("nli")]
        [Summary("Gets images from nekoslife with the given tag")]
        public async Task GetImage([Summary("Category to search")]string category)
        {
            string url = await NekosLifeApi.Client.GetSfwImageAsync(category);

            Embed embed = new EmbedBuilder()
                .WithColor(new Color(0xb39df2))
                .WithTitle("Image From Nekos.Life")
                .WithImageUrl(url)
                .Build();

            await ReplyAsync("", embed: embed);
        }

        [Command("nlg")]
        [Summary("Gets gifs from nekoslife with the given tag")]
        public async Task GetGif([Summary("Category to search")]string category)
        {
            string url = await NekosLifeApi.Client.GetSfwGifAsync(category);

            Embed embed = new EmbedBuilder()
                .WithColor(new Color(0xb39df2))
                .WithTitle("Gif From Nekos.Life")
                .WithImageUrl(url)
                .Build();

            await ReplyAsync("", embed: embed);
        }
    }
}
