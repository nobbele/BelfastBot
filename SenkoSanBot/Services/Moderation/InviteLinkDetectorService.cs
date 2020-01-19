using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using SenkoSanBot.Services.Configuration;
using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace SenkoSanBot.Services.Moderation
{
    public class InviteLinkDetectorService
    {
        public readonly Regex Regex = new Regex(@"\b([H ][T ][T ][P ][: ]([S])\/\/)?discord.gg\/[\w\d\S]*\b", RegexOptions.Compiled | RegexOptions.IgnoreCase);

        private DiscordSocketClient m_client;
        private IBotConfigurationService m_config;

        public InviteLinkDetectorService(IServiceProvider services)
        {
            m_client = services.GetRequiredService<DiscordSocketClient>();
            m_config = services.GetRequiredService<IBotConfigurationService>();
        }

        public async Task InitializeAsync()
        {
            m_client.MessageReceived += async (SocketMessage message) =>
            {
                if ((message.Author as SocketGuildUser)?.GuildPermissions.Administrator ?? true)
                    return;
                Match match = Regex.Match(message.Content);
                if (match.Success && !match.Captures.Any(capture => m_config.Configuration.InviteLinkWhitelist == capture.Value))
                {
                    await message.DeleteAsync();
                    await message.Channel.SendMessageAsync($"Don't send links to other discord servers {message.Author.Mention}");
                }
            };

            await Task.CompletedTask;
        }
    }
}
