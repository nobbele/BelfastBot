using Discord.Commands;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SenkoSanBot.Modules
{
    public class MiscModule : ModuleBase<SocketCommandContext>
    {
        [Command("echo")]
        public async Task EchoAsync([Remainder] string message)
        {
            await ReplyAsync(message);
        }
    }
}
