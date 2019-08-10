using Discord.Commands;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace SenkoSanBot.Modules.Fun
{
    public class SafeBooruModule : ModuleBase<SocketCommandContext>
    {
        [Command("sbi")]
        public async Task GetImage(string tag)
        {
            string url = await SafeBooruApi.Client.GetRandomPostAsync(tag);
            using (HttpClient client = new HttpClient())
            {
                Stream fileStream = await client.GetStreamAsync(url);
                await Context.Channel.SendFileAsync(fileStream, Path.GetFileName(new Uri(url).LocalPath));
            }
        }

    }
}
