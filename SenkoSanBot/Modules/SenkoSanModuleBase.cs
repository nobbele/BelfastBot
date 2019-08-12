using Discord;
using Discord.Commands;
using SenkoSanBot.Services;
using SenkoSanBot.Services.Logging;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SenkoSanBot.Modules
{
    public abstract class SenkoSanModuleBase : ModuleBase<SocketCommandContext>
    {
        public LoggingService Logger { get; set; }

        protected override Task<IUserMessage> ReplyAsync(string message = null, bool isTTS = false, Embed embed = null, RequestOptions options = null)
        {
            Logger.LogInfo($"{Context.Channel}: '{message ?? $"*Embed*{embed?.Title}"}'");
            return base.ReplyAsync(message, isTTS, embed, options);
        }
    }
}
