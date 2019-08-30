using Discord;
using Discord.Commands;
using Discord.WebSocket;
using SenkoSanBot.Services.Configuration;
using SenkoSanBot.Services.Database;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace SenkoSanBot.Services.Credits
{
    public class GiveCreditsPerMessagesService
    {
        private readonly DiscordSocketClient m_client;
        private readonly IBotConfigurationService m_config;
        private readonly JsonDatabaseService m_db;

        public GiveCreditsPerMessagesService(DiscordSocketClient client, IBotConfigurationService config, JsonDatabaseService db)
        {
            m_client = client;
            m_config = config;
            m_db = db;
        }

        public async Task InitializeAsync()
        {
            m_client.MessageReceived += async (SocketMessage msg) =>
            {
                SocketUserMessage message = msg as SocketUserMessage;
                int argPos = 0;
                if (!(message.HasStringPrefix(m_config.Configuration.Prefix, ref argPos, StringComparison.OrdinalIgnoreCase) 
                || message.HasMentionPrefix(m_client.CurrentUser, ref argPos) 
                || message.Author.IsBot))
                {
                    DatabaseUserEntry userDB = m_db.GetUserEntry(0, message.Author.Id);
                    IUser user = message.Author;

                    userDB.Coins++;
                    uint oldLevel = userDB.Level;
                    userDB.Xp += (ulong)(message.ToString().ToCharArray().Count());
                    uint newLevel = userDB.Level;

                    if(oldLevel != newLevel)
                    {
                        int awardedCoins = (int)Math.Log(userDB.Level);
                        userDB.Coins += awardedCoins;
                        ISocketMessageChannel channel = message.Channel;
                        Embed embed = new EmbedBuilder()
                            .WithColor(0xFF1288)
                            .WithThumbnailUrl(message.Author.GetAvatarUrl())
                            .WithTitle($"{user.Username} Leveled Up!🔼")
                            .AddField("Details",
                            $"► Level: **{oldLevel}** => **{userDB.Level}**\n" +
                            $"► Coins: **{userDB.Coins}**(**+{awardedCoins}**) {Emotes.DiscordCoin}")
                            .Build();

                        await channel.SendMessageAsync(embed: embed);
                    }
                }
                m_db.WriteData();
                await Task.CompletedTask;
            };

            await Task.CompletedTask;
        }
    }
}