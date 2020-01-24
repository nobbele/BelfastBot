using Discord;
using Discord.Commands;
using System.Linq;
using System.Threading.Tasks;
using System;
using YohaneBot.Services.Database;
using YohaneBot.Services.Giveaway;
using Discord.WebSocket;
using YohaneBot.Modules.Preconditions;

namespace YohaneBot.Modules.Misc
{
    [Summary("Commands for giveaways")]
    public class GiveawayModule : YohaneModuleBase
    {
        public JsonDatabaseService DatabaseService { get; set; }
        public GiveawayService GiveawayService { get; set; }

        [Command("giveaway create")]
        [Summary("Create a giveaway")]
        [RequireGuild]
        public async Task CreateGiveawayAsync([Summary("Time giveaway will last(ex. 2 days)")] string time, [Summary("What to give away")] string content, uint count = 1) 
        {
            if(!DateTimeHelper.TryParseRelative(time, out DateTime end))
                await ReplyAsync("Couldn't parse time");


            Embed embed = new EmbedBuilder()
                 .WithColor(0xF5CD63)
                 .WithAuthor(author => {
                     author
                         .WithName($"{Context.Message.Author.Username} has started a giveaway!")
                         .WithIconUrl($"{Context.Message.Author.GetAvatarUrl()}");
                 })
                 .AddField("Details ▼",
                 $"► Prize: __**{content}**__\n" +
                 $"► Time Limit: **{end.Date}** \n" +
                 $"**React with 🥳 to enter the giveaway!**")
                 .WithFooter(footer => {
                     footer
                         .WithText($"Requested by {Context.User}")
                         .WithIconUrl(Context.User.GetAvatarUrl());
                 })
                 .Build();

            IUserMessage message = await ReplyAsync(embed: embed);

            GiveawayEntry entry = new GiveawayEntry
            {
                End = end,
                ChannelId = Context.Channel.Id,
                Content = content,
                ReactionMessageId = message.Id,
                Count = count,
            };
            ServerEntry server = DatabaseService.GetServerEntry(Context.Guild.Id);
            await message.AddReactionAsync(new Emoji(server.GiveawayReactionEmote));
            GiveawayService.AddGiveaway(entry, Context.Guild.Id);
        }
        [Command("giveaway setemoji")]
        [Summary("Sets giveaway emoji")]
        [RequireGuild]
        public async Task SetEmojiAsync([Summary("Emoji to use")] string emote) 
        {
            DatabaseService.GetServerEntry(Context.Guild.Id).GiveawayReactionEmote = emote;
            DatabaseService.WriteData();
            await ReplyAsync($"Set giveaway emoji to {emote}");
        }
    }
}