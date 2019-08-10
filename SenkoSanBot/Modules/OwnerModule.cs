using Discord.Commands;
using System.Threading.Tasks;

namespace SenkoSanBot.Modules
{
    public class OwnerModule : ModuleBase<SocketCommandContext>
    {
        public SenkoSan Senko { get; set; }

        [Command("stop")]
        [RequireOwner]
        public async Task StopAsync()
        {
            await ReplyAsync("Stopping...");

            BotCommandLineCommands.Stop(Senko);
        }
    }
}
