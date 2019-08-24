using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using SenkoSanBot.Services.Configuration;
using SenkoSanBot.Services.Logging;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace SenkoSanBot.Services
{
    public class SenkoSanPersonalityMessageService
    {
        private readonly DiscordSocketClient m_client;
        private readonly IBotConfigurationService m_config;
        private readonly LoggingService m_logger;

        public SenkoSanPersonalityMessageService(IServiceProvider services)
        {
            m_client = services.GetRequiredService<DiscordSocketClient>();
            m_config = services.GetRequiredService<IBotConfigurationService>();
            m_logger = services.GetRequiredService<LoggingService>();
        }

        public async Task InitializeAsync()
        {
            m_client.MessageReceived += HandleReactionAsync;
        }

        public async Task HandleReactionAsync(SocketMessage messageParam)
        {
            var message = messageParam as SocketUserMessage;

            int argPos = 0;

            if (!message.Content.Contains(m_client.CurrentUser.Mention) || message.Author.IsBot)
                return;

            await message.Channel.SendMessageAsync("うや～");
        }
    }
}