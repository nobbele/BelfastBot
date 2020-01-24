using System;
using System.Net.Http;
using System.Threading.Tasks;
using System.Diagnostics;
using Microsoft.Extensions.DependencyInjection;
using Discord;
using Discord.WebSocket;
using Discord.Commands;
using YohaneBot.Services.Logging;
using YohaneBot.Services.Configuration;
using YohaneBot.Services.Database;
using YohaneBot.Services.Commands;
using YohaneBot.Services.Moderation;
using System.Runtime.CompilerServices;
using YohaneBot.Services.Pagination;
using YohaneBot.Services.Credits;
using YohaneBot.Services.Giveaway;
using YohaneBot.Services.Scheduler;
using YohaneBot;
using System.Threading;

namespace YohaneDiscordClient
{
    public class Yohane : IClient
    {
        public string Version => "1.2-Discord";

        public LoggingService Logger { get; private set; }
        public static Yohane Instance;

        private Random m_random = new Random(Guid.NewGuid().GetHashCode());

        static void Main(string[] args)
        {
            using (var services = ConfigureServices())
            {
                Instance = services.GetRequiredService<Yohane>();
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
        }

        CancellationTokenSource tokenSource = new CancellationTokenSource();

        public bool Stopped { get; private set; } = false;
        public void Stop(bool force = false)
        {
            Logger.LogInfo("Stopping");
            Stopped = true;
            tokenSource.Cancel();
            if(force)
                Process.GetCurrentProcess().Kill();
        }

        public async Task MainAsync(IServiceProvider services)
        {
            Logger = services.GetRequiredService<LoggingService>();
            await Logger.InitializeAsync();
            var config = services.GetRequiredService<IBotConfigurationService>();

            if (config.Initialize())
            {
                var client = services.GetRequiredService<IDiscordClient>() as DiscordSocketClient;

                client.Log += LogMessageAsync;
                client.UserJoined += async (SocketGuildUser user) =>
                {
                    int welcomeMessageIndex = m_random.Next(0, config.Configuration.WelcomeMessages.Length);
                    await user.Guild.SystemChannel.SendMessageAsync(string.Format(config.Configuration.WelcomeMessages[welcomeMessageIndex], user.Mention, user.Guild.Name));
                };
                client.Ready += async () =>
                {
                    await client.SetGameAsync(config.Configuration.StatusMessage.Replace(":serverCount:", client.Guilds.Count.ToString()), type: config.Configuration.Activity);
                    await client.SetStatusAsync(config.Configuration.OnlineStatus);
                };

                services.GetRequiredService<CommandService>().Log += LogMessageAsync;

                if (config.Configuration.Token == "YOUR TOKEN")
                    throw new ConfigurationException("Default token detected, please put your token in the config file");
                await client.LoginAsync(TokenType.Bot, config.Configuration.Token, true);
                await client.StartAsync();

                Logger.LogInfo("Initializing services!");
                await services.GetRequiredService<JsonDatabaseService>().InitializeAsync();
                await services.GetRequiredService<ICommandHandlingService>().InitializeAsync();
                await services.GetRequiredService<WordBlacklistService>().InitializeAsync();
                await services.GetRequiredService<PaginatedMessageService>().InitializeAsync();
                await services.GetRequiredService<InviteLinkDetectorService>().InitializeAsync();
                await services.GetRequiredService<MessageRewardService>().InitializeAsync();
                services.GetRequiredService<SchedulerService>().Initialize();

                if(Environment.UserInteractive && !Console.IsInputRedirected) 
                {
                    Logger.LogInfo("Initializing command line");
                    await services.GetRequiredService<CommandLineHandlingService>().InitializeAsync();
                }
                else
                {
                    Logger.LogInfo("Not initializing command line, non-interactive environment");
                    try
                    {
                        await Task.Delay(-1, tokenSource.Token);
                    }
                    catch(TaskCanceledException)
                    {
                        // do nothing
                    }
                }
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
                .AddSingleton<IDiscordClient, DiscordSocketClient>()
                .AddSingleton<IBotConfigurationService, BotConfigurationService>()
                .AddSingleton<CommandService>()
                .AddSingleton<ICommandHandlingService, CommandHandlingService>()
                .AddSingleton<CommandLineHandlingService>()
                .AddSingleton<HttpClient>()
                .AddSingleton<Yohane>()
                    .AddSingleton<IClient>(coll => coll.GetRequiredService<Yohane>())
                .AddSingleton<JsonDatabaseService>()
                .AddSingleton<WordBlacklistService>()
                .AddSingleton<LoggingService>()
                .AddSingleton<SchedulerService>()
                .AddSingleton<PaginatedMessageService>()
                .AddSingleton<InviteLinkDetectorService>()
                .AddSingleton<MessageRewardService>()
                .AddSingleton<GiveawayService>()
                .BuildServiceProvider();
    }
}