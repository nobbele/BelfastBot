using Discord;
using Discord.Commands;
using Discord.WebSocket;
using SenkoSanBot.Services;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SenkoSanBot.Modules.Moderation
{
    [Summary("Contains commands for chat moderation")]
    public class ChatModerationModule : SenkoSanModuleBase
    {
        [Command("purge")]
        [Summary("Deletes messages with a given amount")]
        [RequireUserPermission(GuildPermission.ManageMessages)]
        [RequireBotPermission(GuildPermission.ManageMessages)]
        public async Task PurgeModule([Summary("Amount of messages to purge")] int amount = 1)
        {
            Logger.LogInfo($"Purge request of {amount} messages from {Context.Channel} sent by {Context.User}");

            if (amount > 100)
            {
                IUserMessage botMessage = await ReplyAsync(":x: You cannot go higher than 100!");
                await Task.Delay(5000);
                await botMessage.DeleteAsync();
                return;
            }

            IEnumerable<IMessage> messages = await Context.Channel.GetMessagesAsync(amount + 1).FlattenAsync();
            await (Context.Channel as SocketTextChannel).DeleteMessagesAsync(messages);

            IUserMessage msg = await ReplyAsync($"Purged {amount} messages");
            await Task.Delay(5000);
            await msg.DeleteAsync();
        }
    }
}