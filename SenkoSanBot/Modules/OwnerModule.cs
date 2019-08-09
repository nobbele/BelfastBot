using Discord.Commands;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SenkoSanBot.Modules
{
    public class OwnerModule : ModuleBase<SocketCommandContext>
    {
        public SenkoSan Senko { get; set; }

        [Command("stop")]
        public async Task StopAsync()
        {
            await ReplyAsync("Stopping...");

            BotCommandLineCommands.Stop(Senko);
        }
    }
}
