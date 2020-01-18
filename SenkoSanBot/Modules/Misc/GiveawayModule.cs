using Discord;
using Discord.Commands;
using System.Linq;
using System.Threading.Tasks;
using System;
using SenkoSanBot.Services.Database;
using SenkoSanBot.Services.Giveaway;

namespace SenkoSanBot.Modules.Misc
{
    [Summary("Commands for giveaways")]
    public class GiveawayModule : SenkoSanModuleBase
    {
        public JsonDatabaseService DatabaseService { get; set; }
        public GiveawayService GiveawayService { get; set; }

        [Command("giveaway create")]
        [Summary("Create a giveaway")]
        public async Task CreateGiveawayAsync([Summary("Time giveaway will last(ex. 2 days)")] string time, [Summary("What to give away")] string content, uint count = 1) 
        {
            if(!DateTimeHelper.TryParseRelative(time, out DateTime end))
                await ReplyAsync("Couldn't parse time");
            
            IUserMessage message = await ReplyAsync($"Giving away {content}");
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
        public async Task SetEmojiAsync([Summary("Emoji to use")] string emote) 
        {
            DatabaseService.GetServerEntry(Context.Guild.Id).GiveawayReactionEmote = emote;
            DatabaseService.WriteData();
            await ReplyAsync($"Set giveaway emoji to {emote}");
        }
    }
}