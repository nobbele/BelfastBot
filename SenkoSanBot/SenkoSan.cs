using System;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Discord;
using Discord.WebSocket;
using Discord.Commands;
using SenkoSanBot.Services;

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
                catch (Exception e)
                {
                    Instance.Logger.LogCritical(e.ToString());
                    Console.ReadKey();
                }
            }
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
            var config = services.GetRequiredService<BotConfigurationService>();

            if (config.Initialize())
            {
                var client = services.GetRequiredService<DiscordSocketClient>();

                client.Log += LogMessageAsync;
                client.UserJoined += async (SocketGuildUser user) => 
                {
                    await user.Guild.SystemChannel.SendMessageAsync(string.Format(config.Configuration.WelcomeMessage, user.Mention));
                };

                services.GetRequiredService<CommandService>().Log += LogMessageAsync;

                await client.LoginAsync(TokenType.Bot, config.Configuration.Token, true);
                await client.StartAsync();

                await services.GetRequiredService<JsonDatabaseService>().InitializeAsync();
                await services.GetRequiredService<CommandHandlingService>().InitializeAsync();
                await services.GetRequiredService<CommandLineHandlingService>().InitializeAsync();
            }
        }

        private async Task LogMessageAsync(LogMessage msg) => await Task.Run(() => {
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
                .AddSingleton<LoggingService>()
                .AddSingleton<DiscordSocketClient>()
                .AddSingleton<BotConfigurationService>()
                .AddSingleton<CommandService>()
                .AddSingleton<CommandHandlingService>()
                .AddSingleton<CommandLineHandlingService>()
                .AddSingleton<HttpClient>()
                .AddSingleton<SenkoSan>()
                .AddSingleton<JsonDatabaseService>()
                .BuildServiceProvider();
    }
}
