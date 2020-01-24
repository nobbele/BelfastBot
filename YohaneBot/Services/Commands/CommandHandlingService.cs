using System;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Discord.Commands;
using Discord.WebSocket;
using YohaneBot.Services.Configuration;
using YohaneBot.Services.Logging;

#nullable enable

namespace YohaneBot.Services.Commands
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

            await m_command.AddModulesAsync(assembly: Assembly.GetAssembly(typeof(CommandHandlingService)),
                                            services: m_services);
        }

        public async Task HandleCommandAsync(SocketMessage messageParam)
        {
            var message = messageParam as SocketUserMessage;

            if (message == null)
                m_logger.LogWarning("Received a message that wasn't a SocketUserMessage");

            int argPos = 0;

            if (!(message.HasStringPrefix(m_config.Configuration.Prefix, ref argPos, StringComparison.OrdinalIgnoreCase) ||
                message.HasMentionPrefix(m_client.CurrentUser, ref argPos)) ||
                message!.Author.IsBot)
                return;

            var context = new SocketCommandContext(m_client, message);

            Task<IResult> task = m_command.ExecuteAsync(
                context: context,
                argPos: argPos,
                services: m_services);

            IResult? result = null;

            if((await Task.WhenAny(task, Task.Delay(TimeSpan.FromSeconds(10)))) == task)
                result = task.Result;

            if(result == null)
            {
                await context.Channel.SendMessageAsync($"{Emotes.SenkoPout} うや～！ Command timed out");
            }
            else if (!result.IsSuccess)
            {
                await context.Channel.SendMessageAsync($"{Emotes.SenkoShock}うや～！\n" +
                    $"{result.ErrorReason} try **{m_config.Configuration.Prefix}help** for lists of commands");
            }
            else
            {
                m_logger.LogInfo("Successfully handled command");
            }
        }
    }
}