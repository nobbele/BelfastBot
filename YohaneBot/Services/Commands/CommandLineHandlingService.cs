using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using YohaneBot.Services.Configuration;
using YohaneBot.Services.Database;
using YohaneBot.Services.Logging;
using System;
using System.Threading.Tasks;

namespace YohaneBot.Services.Commands
{
    public class CommandLineHandlingService
    {
        private readonly DiscordSocketClient m_client;
        private readonly CommandService m_command;
        private readonly IBotConfigurationService m_config;
        private readonly LoggingService m_logger;
        private readonly Yohane m_senko;
        private readonly JsonDatabaseService m_database;

        public CommandLineHandlingService(IServiceProvider services)
        {
            m_client = services.GetRequiredService<DiscordSocketClient>();
            m_command = services.GetRequiredService<CommandService>();
            m_config = services.GetRequiredService<IBotConfigurationService>();
            m_logger = services.GetRequiredService<LoggingService>();
            m_senko = services.GetRequiredService<Yohane>();
            m_database = services.GetRequiredService<JsonDatabaseService>();
        }

        public static readonly int KeyBufferSize = 64;
        private readonly char[] keyBuffer = new char[KeyBufferSize];
        private int keyBufferPosition = 0;

        public async Task InitializeAsync()
        {
            while (true)
            {
                if (m_senko.Stopped)
                    break;
                if (Console.KeyAvailable)
                {
                    char c = Console.ReadKey().KeyChar;

                    if (c == '\n' || c == '\r')
                    {
                        Console.Write('\n');
                        var command = new string(keyBuffer).Substring(0, keyBufferPosition);
                        keyBufferPosition = 0;

                        await HandleCommandAsync(command);
                    }
                    else if (c == '\b')
                    {
                        Console.Write(" \b");
                        if(keyBufferPosition > 0)
                            keyBufferPosition--;
                    }
                    else
                    {
                        if (keyBufferPosition >= KeyBufferSize)
                        {
                            Console.WriteLine("\nKeyBuffer overflown, resetting");
                            keyBufferPosition = 0;
                        }
                        keyBuffer[keyBufferPosition] = c;
                        keyBufferPosition++;
                    }
                }
            }
            await Task.CompletedTask;
        }

        //Might do something fancier later
        public async Task HandleCommandAsync(string message)
        {
            string command;
            string arguments;

            int firstSpaceIndex = message.IndexOf(' ');
            if (firstSpaceIndex == -1)
            {
                command = message;
                arguments = string.Empty;
            }
            else
            {
                command = message.Substring(0, firstSpaceIndex);
                arguments = message.Substring(firstSpaceIndex + 1);
            }

            switch(command)
            {
                case "stop":
                case "kill":
                    BotCommandLineCommands.Stop(m_senko);
                break;

                case "db":
                    switch(arguments.Split(' ')[0])
                    {
                        case "write":
                            m_database.WriteData();
                        break;
                    }
                break;

                default:
                    m_logger.Log(false, LogLevel.Critical, "No such command as '{0}'", command);
                break;
            }

            await Task.CompletedTask;
        }
    }
}