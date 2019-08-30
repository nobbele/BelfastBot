using Discord.Commands;
using Discord.WebSocket;
using SenkoSanBot.Services.Configuration;
using SenkoSanBot.Services.Database;
using SenkoSanBot.Services.Logging;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SenkoSanBot.Services.Credits
{
    public class MessageRewardService
    {
        private readonly DiscordSocketClient m_client;
        private readonly IBotConfigurationService m_config;
        private readonly JsonDatabaseService m_db;
        private readonly LoggingService m_logger;

        public MessageRewardService(DiscordSocketClient client, IBotConfigurationService config, JsonDatabaseService db, LoggingService logger)
        {
            m_client = client;
            m_config = config;
            m_db = db;
            m_logger = logger;
        }


        public async Task InitializeAsync()
        {
            m_client.MessageReceived += async (SocketMessage msg) =>
            {
                SocketUserMessage message = msg as SocketUserMessage;
                int argPos = 0;
                if (!(message.HasStringPrefix(m_config.Configuration.Prefix, ref argPos, StringComparison.OrdinalIgnoreCase)
                    || message.HasMentionPrefix(m_client.CurrentUser, ref argPos)))
                {
                    m_db.GetUserEntry(0, message.Author.Id).Coins++;
                }

                await Task.CompletedTask;
            };

            await Task.CompletedTask;
        }
    }
}
