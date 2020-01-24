using Discord.WebSocket;
using YohaneBot.Services.Configuration;
using YohaneBot.Services.Logging;
using System;
using System.Linq;
using System.Threading.Tasks;
using Discord;

namespace YohaneBot.Services.Moderation
{
    public class WordBlacklistService
    {
        private readonly DiscordSocketClient m_client;
        private readonly IBotConfigurationService m_config;
        private readonly LoggingService m_logger;

        public WordBlacklistService(IDiscordClient client, IBotConfigurationService config, LoggingService logger)
        {
            m_client = client as DiscordSocketClient;
            m_config = config;
            m_logger = logger;
        }

        public async Task InitializeAsync()
        {
            if(m_client == null)
            {
                m_logger.LogWarning($"[{nameof(WordBlacklistService)}] m_client is null, ignoring");
                return;
            }
            m_client.MessageReceived += HandleMessageAsync;

            await Task.CompletedTask;
        }

        private async Task HandleMessageAsync(SocketMessage messageParam)
        {
            if (!(messageParam is SocketUserMessage))
                m_logger.LogWarning("Received a message that wasn't a SocketUserMessage");
            if ((messageParam.Author as SocketGuildUser)?.GuildPermissions.Administrator ?? false)
            {
                m_logger.LogInfo($"The message was sent by an administrator, ignoring.");
                return;
            }
            var message = messageParam as SocketUserMessage;

            string letterOnlyMessage = new string(message.Content.Where(c => char.IsLetter(c) || char.IsWhiteSpace(c)).ToArray());

            if (m_config.Configuration.BlacklistedWord.Any(s => letterOnlyMessage.Contains(s, StringComparison.OrdinalIgnoreCase)))
            {
                m_logger.LogInfo($"Deleting bad message");
                await messageParam.DeleteAsync();
                await messageParam.Channel.SendMessageAsync($"{messageParam.Author.Mention}, I have deleted your message because it contained a bad word");
                m_logger.LogInfo($"Deleted {messageParam.Author} message");
            }
        }
    }
}