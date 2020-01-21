using Common;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using SenkoSanBot.Services.Database;
using SenkoSanBot.Services.Pagination;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using SenkoSanBot.Modules.Preconditions;

namespace SenkoSanBot.Modules.Profiles
{
    [Summary("Commands for use profiles")]
    public class UserProfile : SenkoSanModuleBase
    {
        public JsonDatabaseService Db { get; set; }
        public PaginatedMessageService PaginatedMessageService { get; set; }
        public DiscordSocketClient Client { get; set; }

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

        private Embed GetLeaderBoardPageEmbed(SocketGuild guild, string lbString, EmbedFooterBuilder footer) => new EmbedBuilder()
                 .WithColor(0x53DF1D)
                 .WithAuthor(author => {
                     author
                         .WithName($"Leaderboard of {guild.Name}");
                 })
                 .AddField("Leaderboard:", $"{lbString}")
                 .WithFooter(footer)
                 .WithThumbnailUrl($"{guild.IconUrl}")
                 .Build();

        [Command("profile"), Alias("prf")]
        [Summary("Shows details of an users profile")]
        public async Task CheckProfileAsync([Summary("(optional) The user profile to get")] IUser target = null)
        {
            target = target ?? Context.Message.Author;

            if (target.IsBot)
                return;

            DatabaseUserEntry userData = Db.GetUserEntry(0, target.Id);

            string warns = "NaN";
            if (Context.Guild != null)
            {
                DatabaseUserEntry serverUserData = Db.GetUserEntry(Context.Guild.Id, target.Id);
                warns = serverUserData.Warns.Count.ToString();
            }

            Embed embed = new EmbedBuilder()
                 .WithColor(0xF5CD63)
                 .WithAuthor(author => {
                     author
                         .WithName($"Profile of {target.Username}")
                         .WithIconUrl($"{target.GetAvatarUrl()}");
                 })
                 .AddField("Details ▼",
                 $"__**Status In Current Server**__\n" +
                 $"► Warn Amount: **{warns}**/{Config.Configuration.MaxWarnAmount}\n" +
                 $"__**Profile**__\n" +
                 $"► Level: **{userData.Level}** [**{userData.Xp}**] \n" +
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

        [Command("leaderboard"), Alias("lb")]
        [Summary("Shows server leaderboard")]
        [RequireGuild]
        public async Task LeaderboardAsync(bool global = false) 
        {
            ServerEntry server = Db.GetServerEntry(0);
            IEnumerable<DatabaseUserEntry> users = server.Users;
            if(!global)
                users = users.Where(user => Client.GetUser(user.Id).MutualGuilds.Select(guild => guild.Id).Contains(Context.Guild.Id));
            IEnumerable<DatabaseUserEntry> sortedUser = users.OrderByDescending(user => user.Xp);

            string lbString = sortedUser.Select(user => $"`{Context.Guild.GetUser(user.Id)}` - lvl {user.Level}({user.Xp} xp)").NewLineSeperatedString();

            if(string.IsNullOrEmpty(lbString))
            {
                await ReplyAsync("Empty leaderboard");
                return;
            }

            string[] strings = lbString.NCharLimitToClosestDelimeter(512, "\n");

            await PaginatedMessageService.SendPaginatedDataMessageAsync(
                Context.Channel,
                strings,
                (result, index, footer) => GetLeaderBoardPageEmbed(Context.Guild, result, footer)
            );
        }

        [Command("cards")]
        [Summary("Shows cards that an user has")]
        public async Task GetUserGacha([Summary("(optional) The user profile to get")] IUser target = null)
        {
            target = target ?? Context.Message.Author;

            if (target.IsBot)
                return;

            DatabaseUserEntry userData = Db.GetUserEntry(0, target.Id);

            if (userData.Cards.Count <= 0)
            {
                await Context.Channel.SendMessageAsync($"{Emotes.SenkoShock} You don't seem to have any cards.\nTry .gao to open cards.");
                return;
            }

            string characterString = userData.Cards.Count > 0 
                ? userData.Cards.Select(card => $"► **[{card.Name}](https://www.animecharactersdatabase.com/characters.php?id={card.Id})** [{card.Rarity}] x{card.Amount}").NewLineSeperatedString()
                : "**None**";

            string[] strings = characterString.NCharLimitToClosestDelimeter(512, "\n");

            await PaginatedMessageService.SendPaginatedDataMessageAsync(
                Context.Channel,
                strings,
                (result, index, footer) => GetUserGachaEmbed(target, result, footer)
            );
        }

        [Command("cardfavorite"), Alias("cfav")]
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

        [Command("cardsell"), Alias("csell")]
        [Summary("Sell your cards")]
        public async Task SellCardAsync([Summary("Card rarity to sell")] string rarity, [Summary("Card name to sell")] [Remainder] string cardName)
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
                case CardRarity.Common:
                    return Config.Configuration.CommonCardPrice;
                case CardRarity.Rare:
                    return Config.Configuration.RareCardPrice;
                case CardRarity.SR:
                    return Config.Configuration.SRCardPrice;                
                case CardRarity.SSR:
                    return Config.Configuration.SSRCardPrice;
            }
            return 0;
        }
    }
}