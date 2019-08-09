using System;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Discord;
using Discord.WebSocket;
using Discord.Commands;
using SenkoSanBot.Services;
using System.Threading;

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
                Instance.Logger = services.GetRequiredService<LoggingService>();
                try
                {
                    var mainTask = Instance.MainAsync(services);
                    mainTask.GetAwaiter().GetResult();
                }
                catch (Exception e)
                {
                    Instance.Logger.Log(e.ToString());
                    Console.ReadKey();
                }
            }
        }

        public bool Stopped { get; private set; } = false;
        public void Stop()
        {
            Logger.Log("Stopping");
            Stopped = true;
        }

        public async Task MainAsync(IServiceProvider services)
        {
            var config = services.GetRequiredService<BotConfigurationService>();

            if (config.Initialize())
            {
                var client = services.GetRequiredService<DiscordSocketClient>();

                //Logging method for DiscordSocketClient.Log and CommandService.Log
                async Task LogMessageAsync(LogMessage msg) => await Logger.LogAsync(msg.ToString());

                client.Log += LogMessageAsync;
                services.GetRequiredService<CommandService>().Log += LogMessageAsync;

                await client.LoginAsync(TokenType.Bot, config.Configuration.Token, true);
                await client.StartAsync();

                await services.GetRequiredService<CommandHandlingService>().InitializeAsync();
                await services.GetRequiredService<CommandLineHandlingService>().InitializeAsync();
            }
        }

        private static ServiceProvider ConfigureServices() => new ServiceCollection()
                .AddSingleton<LoggingService>()
                .AddSingleton<DiscordSocketClient>()
                .AddSingleton<BotConfigurationService>()
                .AddSingleton<CommandService>()
                .AddSingleton<CommandHandlingService>()
                .AddSingleton<CommandLineHandlingService>()
                .AddSingleton<HttpClient>()
                .AddSingleton<SenkoSan>()
                .BuildServiceProvider();
    }
}
