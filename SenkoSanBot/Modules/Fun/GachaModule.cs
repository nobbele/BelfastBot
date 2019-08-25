using Discord;
using Discord.Commands;
using SenkoSanBot.Services.Pagination;
using System.Linq;
using System.Threading.Tasks;

namespace SenkoSanBot.Modules.Fun
{
    [Summary("Commands for Gacha")]
    public class GachaModule : SenkoSanModuleBase
    {
        public PaginatedMessageService PaginatedMessageService { get; set; }

        [Command("roll")]
        [Summary("Rolls random character from a card pack")]
        public async Task GetGacha([Summary("Card pack index")][Remainder]int cardPack = 1)
        {
            if (!Config.Configuration.Packs.TryGetValue(cardPack, out string content))
            {
                await ReplyAsync("> Invalid Card Pack");
                return;
            }

            AnimeCharacterDatabaseApi.AnimeResult anime = await AnimeCharacterDatabaseApi.Client.SearchAnimeAsync(content);
            AnimeCharacterDatabaseApi.CharacterResult result = await AnimeCharacterDatabaseApi.Client.GetCharactersAsync(anime.Id);

            Embed embed = new EmbedBuilder()
                .WithColor(0xb39df2)
                .WithAuthor(author => {
                    author
                        .WithName($"You rolled {result.Name}! 🎉")
                        .WithUrl($"https://www.animecharactersdatabase.com/characters.php?id={result.Id}");
                })
                .WithThumbnailUrl(anime.Image)
                .AddField("Details", $"► From: **{anime.Name}**\n" +
                $"► Name: **{result.Name}**\n" +
                $"► Gender: **{result.Gender}**")
                .WithImageUrl(result.ImageUrl)
                .WithFooter(footer => {
                    footer
                        .WithText($"Requested by {Context.User}")
                        .WithIconUrl(Context.User.GetAvatarUrl());
                })
                .Build();

            await ReplyAsync(embed: embed);
        }

        [Command("card packs"), Alias("cards")]
        [Summary("Shows list of available card packs")]
        public async Task CardPacksAsync()
        {
            if (Config.Configuration.Packs.Count <= 0)
            {
                await ReplyAsync("> No card packs found");
                return;
            }

            Embed embed = new EmbedBuilder()
                .WithColor(0xb39df2)
                .WithTitle($"List of available card packs")
                .AddField("Card Packs", $"{(Config.Configuration.Packs.Select(packs => $"► [**{packs.Key}** : **{packs.Value}**]").NewLineSeperatedString())}")
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