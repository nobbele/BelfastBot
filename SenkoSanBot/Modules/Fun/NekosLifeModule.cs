using Discord;
using Discord.Commands;
using Discord.WebSocket;
using System.Linq;
using System.Threading.Tasks;

namespace SenkoSanBot.Modules.Fun
{
    [Summary("Contains commands for nekoslife")]
    public class NekosLifeModule : ModuleBase<SocketCommandContext>
    {
        public DiscordSocketClient Client { get; set; }

        [Command("nekoimg"), Alias("nli")]
        [Summary("Sends a random image from nekoslife with the given tag")]
        public async Task GetImage([Summary("Category to search")]string category)
        {
            string url = await NekosLifeApi.Client.GetSfwImageAsync(category);

            Embed embed = new EmbedBuilder()
                .WithColor(new Color(0xb39df2))
                .WithTitle("Image From Nekos.Life")
                .WithImageUrl(url)
                .WithFooter(footer => {
                    footer
                        .WithText($"Requested by {Context.User}")
                        .WithIconUrl(Context.User.GetAvatarUrl());
                })
                .Build();

            await ReplyAsync(embed: embed);
        }

        [Command("nekogif"), Alias("nlg")]
        [Summary("Sends a random gif from nekoslife with the given tag")]
        public async Task GetGif([Summary("Category to search")]string category, IUser target = null, string verb = "interacted with")
        {
            string url = await NekosLifeApi.Client.GetSfwGifAsync(category);

            target = target ?? Client.CurrentUser;

            Embed embed = new EmbedBuilder()
                .WithColor(new Color(0xb39df2))
                .WithTitle("Gif From Nekos.Life")
                .WithDescription($"{Context.User.Mention} {verb} {target.Mention}")
                .WithImageUrl(url)
                .WithFooter(footer => {
                    footer
                        .WithText($"Requested by {Context.User}")
                        .WithIconUrl(Context.User.GetAvatarUrl());
                })
                .Build();

            await ReplyAsync(embed: embed);
        }

        [Command("hug")]
        public async Task GetHug(IUser target = null) => await GetGif("hug", target, "hugged");
        [Command("kiss")]
        public async Task GetKiss(IUser target = null) => await GetGif("kiss", target, "kissed");
        [Command("poke")]
        public async Task GetPoke(IUser target = null) => await GetGif("poke", target, "poked");
        [Command("slap")]
        public async Task GetSlap(IUser target = null) => await GetGif("slap", target, "slapped");
    }
}
