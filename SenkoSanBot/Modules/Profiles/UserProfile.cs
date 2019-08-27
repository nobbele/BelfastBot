using Discord;
using Discord.Commands;
using SenkoSanBot.Services.Database;
using System.Linq;
using System.Threading.Tasks;

namespace SenkoSanBot.Modules.Profiles
{
    [Summary("Commands for use profiles")]
    public class UserProfile : SenkoSanModuleBase
    {
        public JsonDatabaseService Db { get; set; }

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
                 $"► Card Amount: **{userData.Cards.Length}** \n" +
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

            Embed embed = new EmbedBuilder()
                 .WithColor(0x53DF1D)
                 .WithAuthor(author => {
                     author
                         .WithName($"Profile of {target.Username}")
                         .WithIconUrl($"{target.GetAvatarUrl()}");
                 })
                 .AddField("Rolled Characters:", $"{(userData.Cards.Length > 0 ? userData.Cards.Select(card => $"► [{card.Amount}]**[{card.Name}](https://www.animecharactersdatabase.com/characters.php?id={card.Id})**").NewLineSeperatedString() : "**None**")}")
                 .WithFooter(footer => {
                     footer
                         .WithText($"Requested by {Context.User}")
                         .WithIconUrl(Context.User.GetAvatarUrl());
                 })
                 .WithThumbnailUrl($"{target.GetAvatarUrl()}")
                 .Build();

            await ReplyAsync(embed: embed);
        }
    }
}
