using Discord.Commands;
using Discord.WebSocket;
using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;

namespace SenkoSanBot.Modules.Fun
{
    public class NekosLifeModule : ModuleBase<SocketCommandContext>
    {
        public DiscordSocketClient Client { get; set; }

        [Command("nli")]
        public async Task GetImage(string category)
        {
            string url = await NekosLifeApi.Client.GetSfwImageAsync(category);
            using (HttpClient client = new HttpClient())
            {
                Stream fileStream = await client.GetStreamAsync(url);
                await Context.Channel.SendFileAsync(fileStream, Path.GetFileName(new Uri(url).LocalPath));
            }
        }

        [Command("nlg")]
        public async Task GetGif(string category)
        {
            string url = await NekosLifeApi.Client.GetSfwGifAsync(category);
            using (HttpClient client = new HttpClient())
            {
                Stream fileStream = await client.GetStreamAsync(url);
                await Context.Channel.SendFileAsync(fileStream, Path.GetFileName(new Uri(url).LocalPath));
            }
        }
    }
}
