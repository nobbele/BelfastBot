using Discord;
using Discord.Commands;
using System.Threading.Tasks;

namespace BelfastBot.Modules.Fun
{
    [Summary("Contains commands for nekoslife")]
    public class NekosLifeModule : BelfastModuleBase
    {
        public IDiscordClient Client { get; set; }

        [Command("nekoimg"), Alias("nimg")]
        [RateLimit(typeof(NekosLifeModule), perMinute: 45)]
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

        [Command("nekogif"), Alias("ngif")]
        [RateLimit(typeof(NekosLifeModule), perMinute: 45)]
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
        [RateLimit(typeof(NekosLifeModule), perMinute: 45)]
        [Summary("Hugs Belfast or user specified")]
        public async Task GetHug([Summary("User to hug")] IUser target = null) => await GetGif("hug", target, "hugged");
        [Command("kiss")]
        [RateLimit(typeof(NekosLifeModule), perMinute: 45)]
        [Summary("kisses Belfast or user specified")]
        public async Task GetKiss([Summary("User to kiss")] IUser target = null) => await GetGif("kiss", target, "kissed");
        [Command("poke")]
        [RateLimit(typeof(NekosLifeModule), perMinute: 45)]
        [Summary("pokes Belfast or user specified")]
        public async Task GetPoke([Summary("User to poke")] IUser target = null) => await GetGif("poke", target, "poked");
        [Command("slap")]
        [RateLimit(typeof(NekosLifeModule), perMinute: 45)]
        [Summary("slaps Belfast or user specified")]
        public async Task GetSlap([Summary("User to slap")] IUser target = null) => await GetGif("slap", target, "slapped");        
        [Command("pat")]
        [RateLimit(typeof(NekosLifeModule), perMinute: 45)]
        [Summary("Pats Belfast or user specified")] 
        public async Task GetPat([Summary("User to pat")] IUser target = null) => await GetGif("pat", target, "patted");
    }
}