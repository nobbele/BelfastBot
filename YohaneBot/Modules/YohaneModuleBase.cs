using Discord;
using Discord.Commands;
using YohaneBot.Services.Configuration;
using YohaneBot.Services.Logging;
using System.Threading.Tasks;
using Discord.WebSocket;
using YohaneBot.Services.Communiciation;

namespace YohaneBot.Modules
{
    public abstract class YohaneModuleBase : ModuleBase<YohaneCommandContext>
    {
        public LoggingService Logger { get; set; }
        public IBotConfigurationService Config { get; set; }
        public IDiscordClient DiscordClient { get; set; }
        public ICommunicationService CommunicationService { get; set; }

        public string Prefix => Config.Configuration.Prefix;

        protected override Task<IUserMessage> ReplyAsync(string message = null, bool isTTS = false, Embed embed = null, RequestOptions options = null)
        {
            Logger.LogInfo($"[ReplyAsync] {message ?? $"*Embed*{embed?.Title}"}");
            return CommunicationService.SendMessageAsync(Context.Channel, message, embed);
        }
    }
}
