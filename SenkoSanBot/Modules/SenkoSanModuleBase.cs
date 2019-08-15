﻿using Discord;
using Discord.Commands;
using SenkoSanBot.Services.Configuration;
using SenkoSanBot.Services.Logging;
using System.Threading.Tasks;

namespace SenkoSanBot.Modules
{
    public abstract class SenkoSanModuleBase : ModuleBase<SocketCommandContext>
    {
        public LoggingService Logger { get; set; }
        public IBotConfigurationService Config { get; set; }

        public string Prefix => Config.Configuration.Prefix;

        protected override Task<IUserMessage> ReplyAsync(string message = null, bool isTTS = false, Embed embed = null, RequestOptions options = null)
        {
            Logger.LogInfo($"{Context.Channel}: '{message ?? $"*Embed*{embed?.Title}"}'");
            return base.ReplyAsync(message, isTTS, embed, options);
        }
    }
}