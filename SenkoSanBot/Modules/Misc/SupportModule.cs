using Discord.Commands;
using SenkoSanBot.Services.Configuration;
using System.Threading.Tasks;

namespace SenkoSanBot.Modules.Misc
{
    public class SupportModule : SenkoSanModuleBase
    {
        public IBotConfigurationService Config { get; set; }

        [Command("tag")]
        public async Task TagAsync([Remainder] string tag)
        {
            if(!Config.Configuration.Tags.TryGetValue(tag, out string content))
            {
                await ReplyAsync("Invalid tag");
                return;
            }

            await ReplyAsync(content);

        }
    }
}
