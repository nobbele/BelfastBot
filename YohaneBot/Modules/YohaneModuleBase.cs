using Discord;
using Discord.Commands;
using YohaneBot.Services.Configuration;
using YohaneBot.Services.Logging;
using System.Threading.Tasks;

namespace YohaneBot.Modules
{
    public abstract class YohaneModuleBase : ModuleBase<SocketCommandContext>
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
