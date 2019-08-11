using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SenkoSanBot.Services
{
    public class WordBlacklistService
    {
        private readonly DiscordSocketClient m_client;
        private readonly BotConfigurationService m_config;
        private readonly LoggingService m_logger;

        public WordBlacklistService(DiscordSocketClient client, BotConfigurationService config, LoggingService logger)
        {
            m_client = client;
            m_config = config;
            m_logger = logger;
        }

        public async Task InitializeAsync()
        {
            m_client.MessageReceived += HandleMessageAsync;

            await Task.CompletedTask;
        }

        private async Task HandleMessageAsync(SocketMessage messageParam)
        {
            if (!(messageParam is SocketUserMessage))
                m_logger.LogWarning("Received a message that wasn't a SocketUserMessage");
            var message = messageParam as SocketUserMessage;

            string letterOnlyMessage = new string(message.Content.Where(c => char.IsLetter(c)).ToArray());

            if (m_config.Configuration.BlacklistedWord.Any(s => letterOnlyMessage.Contains(s, StringComparison.OrdinalIgnoreCase)))
            {
                await messageParam.DeleteAsync();
                await messageParam.Channel.SendMessageAsync($"{messageParam.Author.Mention}, I have deleted your message because it contained a bad word");
            }
        }
    }
}
