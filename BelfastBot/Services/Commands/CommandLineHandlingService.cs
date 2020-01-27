using Discord.Commands;
using Microsoft.Extensions.DependencyInjection;
using BelfastBot.Services.Configuration;
using BelfastBot.Services.Database;
using BelfastBot.Services.Logging;
using System;
using System.Threading.Tasks;

namespace BelfastBot.Services.Commands
{
    public class CommandLineHandlingService
    {
        private readonly CommandService m_command;
        private readonly IBotConfigurationService m_config;
        private readonly LoggingService m_logger;
        private readonly IClient m_Belfast;
        private readonly JsonDatabaseService m_database;

        public CommandLineHandlingService(IServiceProvider services)
        {
            m_command = services.GetRequiredService<CommandService>();
            m_config = services.GetRequiredService<IBotConfigurationService>();
            m_logger = services.GetRequiredService<LoggingService>();
            m_Belfast = services.GetRequiredService<IClient>();
            m_database = services.GetRequiredService<JsonDatabaseService>();
        }

        public static readonly int KeyBufferSize = 64;
        private readonly char[] keyBuffer = new char[KeyBufferSize];
        private int keyBufferPosition = 0;

        public async Task InitializeAsync()
        {
            while (true)
            {
                if (m_Belfast.Stopped)
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
                    m_Belfast.Stop();
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