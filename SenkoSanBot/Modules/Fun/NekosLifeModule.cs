using Discord;
using Discord.Commands;
using Discord.WebSocket;
using SenkoSanBot.Services;
using System.Threading.Tasks;

namespace SenkoSanBot.Modules.Fun
{
    [Summary("Contains commands for nekoslife")]
    public class NekosLifeModule : SenkoSanModuleBase
    {
        public DiscordSocketClient Client { get; set; }

        [Command("nekoimg"), Alias("nli")]
        [Summary("Sends a random image from nekoslife with the given tag")]
        public async Task GetImage([Summary("Category to search")] string category)
        {
            Logger.LogInfo($"{Context.User} requested an image from nekoslife");

            string url = await NekosLifeApi.Client.GetSfwImageAsync(category);

            Embed embed = new EmbedBuilder()
                .WithColor(0xb39df2)
                .WithTitle("Image From Nekos.Life")
                .WithImageUrl(url)
                .WithFooter(footer => {
                    footer
                        .WithText($"Requested by {Context.User}")
                        .WithIconUrl(Context.User.GetAvatarUrl());
                })
                .Build();

            await ReplyAsync(embed: embed);

            Logger.LogInfo($"Successfully sent nekoslife image");
        }

        [Command("nekogif"), Alias("nlg")]
        [Summary("Sends a random gif from nekoslife with the given tag")]
        public async Task GetGif([Summary("Category to search")] string category, [Summary("The user to do [verb] with")] IUser target = null, [Summary("The verb to display")] [Remainder] string verb = "interacted with")
        {
            Logger.LogInfo($"{Context.User} requested a gif from nekoslife");

            string url = await NekosLifeApi.Client.GetSfwGifAsync(category);

            target = target ?? Client.CurrentUser;

            Embed embed = new EmbedBuilder()
                .WithColor(0xb39df2)
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

            Logger.LogInfo($"Successfully sent nekoslife gif");
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