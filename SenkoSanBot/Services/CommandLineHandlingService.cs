using Discord;
using Discord.Audio;
using Discord.Commands;
using Discord.Rest;
using Discord.WebSocket;
using SenkoSanBot.Modules;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace SenkoSanBot.Services
{
    public class CommandLineHandlingService
    {
        private readonly DiscordSocketClient m_client;
        private readonly CommandService m_command;
        private readonly BotConfigurationService m_config;
        private readonly LoggingService m_logger;
        private readonly SenkoSan m_senko;

        public CommandLineHandlingService(DiscordSocketClient client, CommandService command, BotConfigurationService config, LoggingService logger, SenkoSan senko)
        {
            m_command = command;
            m_client = client;
            m_config = config;
            m_logger = logger;
            m_senko = senko;
        }

        public static readonly int KeyBufferSize = 64;
        private char[] keyBuffer = new char[KeyBufferSize];
        private int keyBufferPosition = 0;

        public async Task InitializeAsync()
        {
            m_logger.Log("Initialized Command Line");
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
            }

            await Task.CompletedTask;
        }
    }
}
