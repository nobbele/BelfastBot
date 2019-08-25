using System;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Discord;
using Discord.WebSocket;
using Discord.Commands;
using SenkoSanBot.Services.Logging;
using SenkoSanBot.Services.Configuration;
using SenkoSanBot.Services.Database;
using SenkoSanBot.Services.Commands;
using SenkoSanBot.Services.Moderation;
using System.Runtime.CompilerServices;
using SenkoSanBot.Services.Pagination;
using SenkoSanBot.Services;

[assembly: InternalsVisibleTo("SenkoSanBotTests")]
namespace SenkoSanBot
{
    public class SenkoSan
    {
        public LoggingService Logger { get; private set; }
        public static SenkoSan Instance;

        static void Main(string[] args)
        {
            using (var services = ConfigureServices())
            {
                Instance = services.GetRequiredService<SenkoSan>();
                try
                {
                    var mainTask = Instance.MainAsync(services);
                    mainTask.GetAwaiter().GetResult();
                }
                catch (ConfigurationException e)
                {
                    Instance.Logger.LogCritical(e.Message);
                }
                catch (Exception e)
                {
                    Instance.Logger.LogCritical(e.ToString());
                }
            }
            Console.Write("\nPress any key to exit");
            Console.ReadKey();
        }

        public bool Stopped { get; private set; } = false;
        public void Stop()
        {
            Logger.LogInfo("Stopping");
            Stopped = true;
        }

        public async Task MainAsync(IServiceProvider services)
        {
            Logger = services.GetRequiredService<LoggingService>();
            await Logger.InitializeAsync();
            var config = services.GetRequiredService<IBotConfigurationService>();

            if (config.Initialize())
            {
                var client = services.GetRequiredService<DiscordSocketClient>();

                client.Log += LogMessageAsync;
                client.UserJoined += async (SocketGuildUser user) => 
                {
                    await user.Guild.SystemChannel.SendMessageAsync(string.Format(config.Configuration.WelcomeMessage, user.Mention));
                };
                client.Ready += async () =>
                {
                    await client.SetGameAsync($"{config.Configuration.StatusMessage}");
                    await client.SetStatusAsync(config.Configuration.OnlineStatus);
                };

                services.GetRequiredService<CommandService>().Log += LogMessageAsync;

                if (config.Configuration.Token == "YOUR TOKEN")
                    throw new ConfigurationException("Default token detected, please put your token in the config file");
                await client.LoginAsync(TokenType.Bot, config.Configuration.Token, true);
                await client.StartAsync();

                //await client.SetGameAsync($"{config.Configuration.StatusMessage}");
                //await client.SetStatusAsync(config.Configuration.OnlineStatus);

                Logger.LogInfo("Initializing services");
                await services.GetRequiredService<JsonDatabaseService>().InitializeAsync();
                await services.GetRequiredService<ICommandHandlingService>().InitializeAsync();
                await services.GetRequiredService<WordBlacklistService>().InitializeAsync();
                await services.GetRequiredService<PaginatedMessageService>().InitializeAsync();
                await services.GetRequiredService<InviteLinkDetectorService>().InitializeAsync();
                Logger.LogInfo("Initializing command line");
                await services.GetRequiredService<CommandLineHandlingService>().InitializeAsync();

            }
        }

        private async Task LogMessageAsync(LogMessage msg) => await Task.Factory.StartNew(() => {
            LogLevel level = LogLevel.Info;
            switch (msg.Severity)
            {
                case LogSeverity.Info:
                case LogSeverity.Verbose:
                case LogSeverity.Debug:
                    level = LogLevel.Info;
                    break;
                case LogSeverity.Warning:
                    level = LogLevel.Warning;
                    break;
                case LogSeverity.Critical:
                case LogSeverity.Error:
                    level = LogLevel.Critical;
                    break;
            }
            Logger.Log(level, msg.Message ?? msg.Exception.ToString());
        });

        private static ServiceProvider ConfigureServices() => new ServiceCollection()
                .AddSingleton<DiscordSocketClient>()
                .AddSingleton<IBotConfigurationService, BotConfigurationService>()
                .AddSingleton<CommandService>()
                .AddSingleton<ICommandHandlingService, CommandHandlingService>()
                .AddSingleton<CommandLineHandlingService>()
                .AddSingleton<HttpClient>()
                .AddSingleton<SenkoSan>()
                .AddSingleton<JsonDatabaseService>()
                .AddSingleton<WordBlacklistService>()
                .AddSingleton<LoggingService>()
                .AddSingleton<PaginatedMessageService>()
                .AddSingleton<InviteLinkDetectorService>()
                .BuildServiceProvider();
    }
}