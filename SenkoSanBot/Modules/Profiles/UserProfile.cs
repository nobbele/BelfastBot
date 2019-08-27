using Discord;
using Discord.Commands;
using SenkoSanBot.Services.Database;
using SenkoSanBot.Services.Pagination;
using System.Collections.Generic;
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
                 $"__**Server Status**__\n" +
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
        public async Task SellCardAsync(int index)
        {
            DatabaseUserEntry userData = Db.GetUserEntry(0, Context.Message.Author.Id);
            GachaCard exits = userData.Cards[index];
            //Also check amount
            if(exits != null)
            {
                await ReplyAsync($"> Removed {exits.Name} and gained x coins"); //Fix
                userData.Cards.Remove(userData.Cards[index]);
                userData.Coin += Config.Configuration.GachaPrice / 2; //Change this
                Db.WriteData();
                return;
            }
            await ReplyAsync("> Couldn't find the specified card index");
        }
    }
    public static class StringExtensionMethods
    {
        /// <summary>
        /// Assumes no string between 2 delimeters is longer than n
        /// </summary>
        public static string[] NCharLimitToClosestDelimeter(this string str, int n, string delim)
        {
            List<string> list = new List<string>(str.Length / n);

            while (str.Length > n)
            {
                int lastIndexToClosestDelim = 0;
                int indexToClosestDelim = 0;
                while ((indexToClosestDelim = str.IndexOf(delim, indexToClosestDelim + delim.Length)) < n)
                {
                    lastIndexToClosestDelim = indexToClosestDelim;
                }

                list.Add(str.Substring(0, lastIndexToClosestDelim));
                str = str.Remove(0, lastIndexToClosestDelim + delim.Length);
            }

            list.Add(str);

            return list.ToArray();
        }
    }
}