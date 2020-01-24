using Discord;
using Discord.Commands;
using Discord.WebSocket;
using System.Threading.Tasks;

namespace YohaneBot.Modules.Fun
{
    [Summary("Contains commands for nekoslife")]
    public class NekosLifeModule : YohaneModuleBase
    {
        public DiscordSocketClient Client { get; set; }

        [Command("nekoimg"), Alias("nli")]
        [Summary("Sends a random image from nekoslife with the given tag")]
        public async Task GetImage([Summary("Category to search")] string category = "neko")
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
        }

        [Command("hug")]
        [Summary("Hugs senko-san or user specified")]
        public async Task GetHug([Summary("The user to hug")] IUser target = null) => await GetGif("hug", target, "hugged");
        [Command("kiss")]
        [Summary("kisses senko-san or user specified")]
        public async Task GetKiss([Summary("The user to kiss")] IUser target = null) => await GetGif("kiss", target, "kissed");
        [Command("poke")]
        [Summary("pokes senko-san or user specified")]
        public async Task GetPoke([Summary("The user to poke")] IUser target = null) => await GetGif("poke", target, "poked");
        [Command("slap")]
        [Summary("slaps senko-san or user specified")]
        public async Task GetSlap([Summary("The user to slap")] IUser target = null) => await GetGif("slap", target, "slapped");
    }
}