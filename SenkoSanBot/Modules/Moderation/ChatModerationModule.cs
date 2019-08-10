using Discord;
using Discord.Commands;
using System.Threading.Tasks;

namespace SenkoSanBot.Modules.Moderation
{
    [Summary("Contains commands for chat moderation")]
    public class ChatModerationModule : ModuleBase<SocketCommandContext>
    {
        [Command("purge")]
        [Summary("Deletes messages with a given amount")]
        [RequireUserPermission(GuildPermission.ManageMessages)]
        [RequireBotPermission(GuildPermission.ManageMessages)]
        public async Task PurgeModule(int amount = 1)
        {
            if (amount > 100)
            {
                var botMessage_1 = await Context.Channel.SendMessageAsync(":x: You cannot go higher than 100!");
                await Task.Delay(2000);
                await botMessage_1.DeleteAsync();
                return;
            }

            var messages = await Context.Channel.GetMessagesAsync(amount + 1).FlattenAsync();
            foreach(IMessage message in messages)
            {
                await Context.Channel.DeleteMessageAsync(message);
            }
            var botMessage_2 = await Context.Channel.SendMessageAsync("Purged " + amount + " messages.");
            await Task.Delay(2000);
            await botMessage_2.DeleteAsync();
        }
    }
}
