using Discord;
using Discord.Commands;
using SenkoSanBot.Services.Database;
using SenkoSanBot.Services.Pagination;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace SenkoSanBot.Modules.Gacha
{
    [Summary("Commands for Gacha")]
    public class GachaModule : SenkoSanModuleBase
    {
        public JsonDatabaseService Db { get; set; }
        public PaginatedMessageService PaginatedMessageService { get; set; }
        private Random m_random;

        [Command("open")]
        [Summary("Opens random character from a card pack")]
        public async Task GetGacha([Summary("Card pack index")][Remainder]int cardPack = 1)
        {
            if (!Config.Configuration.Packs.TryGetValue(cardPack, out string content))
            {
                await ReplyAsync("> Invalid Card Pack");
                return;
            }

            DatabaseUserEntry userData = Db.GetUserEntry(0, Context.Message.Author.Id);

            if (userData.Coins < Config.Configuration.GachaPrice)
            {
                await ReplyAsync($"> Insufficient Coins {Emotes.DiscordCoin}");
                return;
            }

            AnimeCharacterDatabaseApi.AnimeResult anime = await AnimeCharacterDatabaseApi.Client.SearchAnimeAsync(content);
            AnimeCharacterDatabaseApi.CharacterResult result = await AnimeCharacterDatabaseApi.Client.GetCharactersAsync(anime.Id);
            
            userData.Coins -= Config.Configuration.GachaPrice;
            userData.GachaRolls++;

            m_random = new Random(DateTime.Now.Millisecond);
            float rarityLevel = (float)m_random.NextDouble();

            GachaCard newCard = new GachaCard { Name = result.Name, Id = result.Id, Amount = 1};
            newCard.Rarity = newCard.Rarity.ToPercent(rarityLevel);

            GachaCard existingCard = userData.Cards.SingleOrDefault(card => card.Id == result.Id && card.Rarity == newCard.Rarity);
            if (existingCard != null)
                existingCard.Amount++;
            else
                userData.Cards.Add(newCard);

            Db.WriteData();

            Embed embed = new EmbedBuilder()
                .WithColor(GetColorForCard(newCard.Rarity))
                .WithAuthor(author => {
                    author
                        .WithName($"Opened {anime.Name} Card Pack! 🎉")
                        .WithUrl($"https://www.animecharactersdatabase.com/characters.php?id={result.Id}");
                })
                .WithThumbnailUrl(anime.Image)
                .AddField($"Details", 
                $"► Rarity: **{newCard.Rarity}**\n" +
                $"► Name: **{result.Name}**\n" +
                $"► Gender: **{result.Gender}**\n" +
                $"► From: **{anime.Name}**\n" +
                $"{result.Name} has been added to you cards list!")
                .WithImageUrl(result.ImageUrl)
                .WithFooter(footer => {
                    footer
                        .WithText($"Paid {Config.Configuration.GachaPrice} coins for a roll")
                        .WithIconUrl(Context.User.GetAvatarUrl());
                })
                .Build();

            await ReplyAsync(embed: embed);
        }

        [Command("gacha packs"), Alias("gap")]
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

        private Color GetColorForCard(CardRarity rarity)
        {
            switch (rarity)
            {
                case CardRarity.Common:
                    return new Color(0xCD7f32);
                case CardRarity.Rare:
                    return new Color(0xC0C0C0);
                case CardRarity.SR:
                    return new Color(0xffd700);
                case CardRarity.SSR:
                    return new Color(0xde6585);
            }
            return new Color(0x000000);
        }
    }
}