using Common;
using Discord;
using Discord.Commands;
using SenkoSanBot.Services.Database;
using SenkoSanBot.Services.Pagination;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace SenkoSanBot.Modules.Profiles
{
    [Summary("Commands for use profiles")]
    public class UserProfile : SenkoSanModuleBase
    {
        public JsonDatabaseService Db { get; set; }
        public PaginatedMessageService PaginatedMessageService { get; set; }
        private Embed GetUserGachaEmbed(IUser target, string gachaString, EmbedFooterBuilder footer) => new EmbedBuilder()
                 .WithColor(0x53DF1D)
                 .WithAuthor(author => {
                     author
                         .WithName($"Profile of {target.Username}")
                         .WithIconUrl($"{target.GetAvatarUrl()}");
                 })
                 .AddField("Rolled Characters:", $"{gachaString}")
                 .WithFooter(footer)
                 .WithThumbnailUrl($"{target.GetAvatarUrl()}")
                 .Build();

        [Command("profile"), Alias("prf")]
        [Summary("Shows details of an users profile")]
        public async Task CheckProfileAsync([Summary("(optional) The user profile to get")] IUser target = null)
        {
            target = target ?? Context.Message.Author;

            if (target.IsBot)
                return;

            DatabaseUserEntry userData = Db.GetUserEntry(0, target.Id);

            Embed embed = new EmbedBuilder()
                 .WithColor(0xF5CD63)
                 .WithAuthor(author => {
                     author
                         .WithName($"Profile of {target.Username}")
                         .WithIconUrl($"{target.GetAvatarUrl()}");
                 })
                 .AddField("Details:",
                 $"__**Status In Current Server**__\n" +
                 $"► Warn Amount: **{userData.Warns.Count}**/{Config.Configuration.MaxWarnAmount}\n" +
                 $"__**Profile**__\n" +
                 $"► Level: **{userData.Level}** \n" +
                 $"► Coins: **{userData.Coins}** {Emotes.DiscordCoin}\n" +
                 $"► Gacha Rolls: **{userData.GachaRolls}**\n" +
                 $"► Card Amount: **{userData.Cards.Count}**\n" +
                 $"► Favorite Card: **{(userData.FavoriteCard != null ? $"[{userData.FavoriteCard.Name}](https://www.animecharactersdatabase.com/characters.php?id={userData.FavoriteCard.Id})" : "None")}**")
                 .WithFooter(footer => {
                     footer
                         .WithText($"Requested by {Context.User}")
                         .WithIconUrl(Context.User.GetAvatarUrl());
                 })
                 .WithThumbnailUrl($"{target.GetAvatarUrl()}")
                 .Build();

            await ReplyAsync(embed: embed);
        }

        [Command("cards")]
        [Summary("Shows cards that an user has")]
        public async Task GetUserGacha([Summary("(optional) The user profile to get")] IUser target = null)
        {
            target = target ?? Context.Message.Author;

            if (target.IsBot)
                return;

            DatabaseUserEntry userData = Db.GetUserEntry(0, target.Id);

            string characterString = userData.Cards.Count > 0 
                ? userData.Cards.Select(card => $"► **[{card.Name}](https://www.animecharactersdatabase.com/characters.php?id={card.Id})** [{card.Rarity}] x{card.Amount}").NewLineSeperatedString()
                : "**None**";

            if (userData.Cards.Count <= 0)
                return;

            string[] strings = characterString.NCharLimitToClosestDelimeter(512, "\n");

            await PaginatedMessageService.SendPaginatedDataMessageAsync(
                Context.Channel,
                strings,
                (result, index, footer) => GetUserGachaEmbed(target, result, footer)
            );
        }

        [Command("card favorite"), Alias("cfav")]
        [Summary("Favorite one of the cards you own")]
        public async Task FavoriteCardAsync([Summary("Card name to favorite")][Remainder]string cardName)
        {
            DatabaseUserEntry userData = Db.GetUserEntry(0, Context.Message.Author.Id);
            GachaCard exits = userData.Cards.SingleOrDefault(card => string.Equals(card.Name, cardName, StringComparison.OrdinalIgnoreCase));
            if (exits != null)
            {
                userData.FavoriteCard = exits;
                await ReplyAsync($"> Set **{exits.Name}** as favorite card");
                Db.WriteData();
                return;
            }
            await ReplyAsync("> Couldn't find the specified card with given name");
        }

        [Command("card sell"), Alias("csell")]
        [Summary("Sell your cards")]
        public async Task SellCardAsync([Summary("Card rarity to sell")]string rarity, [Summary("Card name to sell")][Remainder]string cardName)
        {
            DatabaseUserEntry userData = Db.GetUserEntry(0, Context.Message.Author.Id);
            GachaCard exits = userData.Cards.FirstOrDefault(card => string.Equals(card.Name, cardName, StringComparison.OrdinalIgnoreCase) && string.Equals(card.Rarity.ToString(), rarity, StringComparison.OrdinalIgnoreCase));
            if(exits != null)
            {
                int refundCoin = GetPriceForCard(exits.Rarity);

                await ReplyAsync($"> Sold **{exits.Name} [{exits.Rarity.ToString()}]** for **{refundCoin}** coins {Emotes.DiscordCoin}");
                if (exits.Amount > 1)
                    exits.Amount--;
                else
                {
                    if (userData.FavoriteCard == exits)
                        userData.FavoriteCard = null;
                    userData.Cards.Remove(exits);
                }
                userData.Coins += refundCoin;
                Db.WriteData();
                return;
            }
            await ReplyAsync("> Couldn't find the specified card with given name");
        }
        private int GetPriceForCard(CardRarity rarity)
        {
            switch (rarity)
            {
                case CardRarity.Bronze:
                    return Config.Configuration.BronzeCardPrice;
                case CardRarity.Silver:
                    return Config.Configuration.SilverCardPrice;
                case CardRarity.Gold:
                    return Config.Configuration.GoldCardPrice;
            }
            return 0;
        }
    }
}