using System;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Discord.Commands;
using Discord.WebSocket;
using SenkoSanBot.Services.Configuration;
using SenkoSanBot.Services.Logging;

namespace SenkoSanBot.Services.Commands
{
    public class CommandHandlingService : ICommandHandlingService
    {
        private readonly IServiceProvider m_services;
        private readonly DiscordSocketClient m_client;
        private readonly CommandService m_command;
        private readonly IBotConfigurationService m_config;
        private readonly LoggingService m_logger;

        public CommandHandlingService(IServiceProvider services)
        {
            m_services = services;
            m_client = services.GetRequiredService<DiscordSocketClient>();
            m_command = services.GetRequiredService<CommandService>();
            m_config = services.GetRequiredService<IBotConfigurationService>();
            m_logger = services.GetRequiredService<LoggingService>();
        }

        public async Task InitializeAsync()
        {
            m_client.MessageReceived += HandleCommandAsync;

            await m_command.AddModulesAsync(assembly: Assembly.GetEntryAssembly(),
                                            services: m_services);
        }

        public async Task HandleCommandAsync(SocketMessage messageParam)
        {
            if (!(messageParam is SocketUserMessage))
                m_logger.LogWarning("Received a message that wasn't a SocketUserMessage");
            var message = messageParam as SocketUserMessage;

            int argPos = 0;

            if (!(message.HasStringPrefix(m_config.Configuration.Prefix, ref argPos, StringComparison.OrdinalIgnoreCase) ||
                message.HasMentionPrefix(m_client.CurrentUser, ref argPos)) ||
                message.Author.IsBot)
                return;

            m_logger.LogInfo("Handling command " + message.Content);

            var context = new SocketCommandContext(m_client, message);

            using (context.Channel.EnterTypingState())
            {

                var result = await m_command.ExecuteAsync(
                    context: context,
                    argPos: argPos,
                    services: m_services);

                if (!result.IsSuccess)
                {
                    await context.Channel.SendMessageAsync(result.ErrorReason);
                    m_logger.LogInfo($"Unknown command {result.ErrorReason}");
                }
                else
                {
                    m_logger.LogInfo("Successfully handled command");
                }
            }
        }
    }
}