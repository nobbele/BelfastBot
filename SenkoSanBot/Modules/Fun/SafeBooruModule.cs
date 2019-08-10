using Discord;
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
            Embed embed = new EmbedBuilder()
                .WithColor(new Color(0xb39df2))
                .WithTitle("Image From Safebooru.org")
                .WithImageUrl(url)
                .Build();

            await ReplyAsync("", embed: embed);
        }

    }
}
