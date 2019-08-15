﻿using Discord.Commands;
using SenkoSanBot.Services.Commands;
using System.Threading.Tasks;

namespace SenkoSanBot.Modules.Misc
{
    [Summary("Commands that are only for owner")]
    public class OwnerModule : SenkoSanModuleBase
    {
        public SenkoSan Senko { get; set; }

        [Command("stop")]
        [Summary("Stops senko-san")]
        [RequireOwner]
        public async Task StopAsync()
        {
            Logger.LogInfo($"{Context.User} stopped senko");
            await ReplyAsync("Stopping...");

            BotCommandLineCommands.Stop(Senko);
        }
    }
}