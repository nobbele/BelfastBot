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

        [Command("profile"), Alias("prf")]
        [Summary("Shows details of an users profile")]
        public async Task CheckProfileAsync([Summary("(optional) The user profile to get")] IUser target = null)
        {
            target = target ?? Context.Message.Author;

            if (target.IsBot)
                return;

            DatabaseUserEntry userData = Db.GetUserEntry(0, target.Id);

            Embed embed = new EmbedBuilder()
                 .WithColor(0x53DF1D)
                 .WithAuthor(author => {
                     author
                         .WithName($"Profile of {target.Username}")
                         .WithIconUrl($"{target.GetAvatarUrl()}");
                 })
                 .AddField("Details:",
                 $"__**Gacha Details**__\n" +
                 $"► Coins: **{userData.Coin}**\n" +
                 $"► Gacha Rolls: **{userData.GachaRolls}**\n" +
                 $"► Card Amount: **{userData.Cards.Count}** \n" +
                 $"__**Status In Current Server**__\n" +
                 $"► Warn Amount: **{userData.Warns.Count}**/{Config.Configuration.MaxWarnAmount}\n")
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
                ? userData.Cards.Select(card => $"► **[{card.Name}](https://www.animecharactersdatabase.com/characters.php?id={card.Id})** x{card.Amount}").NewLineSeperatedString()
                : "**None**";

            string[] strings = characterString.NCharLimitToClosestDelimeter(512, "\n");

            await PaginatedMessageService.SendPaginatedDataMessageAsync(
                Context.Channel,
                strings,
                (result, index, footer) => GetUserGachaEmbed(target, result, footer)
            );
        }

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

        [Command("card sell")]
        public async Task SellCardAsync([Remainder]string cardName)
        {
            DatabaseUserEntry userData = Db.GetUserEntry(0, Context.Message.Author.Id);
            GachaCard exits = userData.Cards.SingleOrDefault(card => string.Equals(card.Name, cardName, StringComparison.OrdinalIgnoreCase));
            if(exits != null)
            {
                int refundCoin = Config.Configuration.GachaPrice / 2;
                await ReplyAsync($"> Sold **{exits.Name}** for **{refundCoin}** coins");
                if (exits.Amount > 1)
                    exits.Amount--;
                else
                    userData.Cards.Remove(exits);
                userData.Coin += refundCoin;
                Db.WriteData();
                return;
            }
            await ReplyAsync("> Couldn't find the specified card index");
        }
    }
}