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
    public class MessageRewardService
    {
        private readonly DiscordSocketClient m_client;
        private readonly IBotConfigurationService m_config;
        private readonly JsonDatabaseService m_db;

        public MessageRewardService(DiscordSocketClient client, IBotConfigurationService config, JsonDatabaseService db)
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
                    userDB.Xp += (uint)Math.Pow(message.Content.Length / 10, 1.1);
                    uint newLevel = userDB.Level;

                    if(oldLevel != newLevel)
                    {
                        int oldCoins = userDB.Coins;
                        int awardedCoins = (int)Math.Pow(userDB.Level / 5, 2);
                        userDB.Coins += awardedCoins;

                        Embed embed = new EmbedBuilder()
                            .WithColor(0xFF1288)
                            .WithThumbnailUrl(message.Author.GetAvatarUrl())
                            .WithTitle($"{user.Username} Leveled Up!🔼")
                            .AddField("Details",
                            $"► Level: **{oldLevel}** => **{userDB.Level}**\n" +
                            $"► Coins: **{oldCoins}** {Emotes.DiscordCoin} => **{userDB.Coins}** {Emotes.DiscordCoin} (**+{awardedCoins}**{Emotes.DiscordCoin})")
                            .Build();

                        await message.Channel.SendMessageAsync(embed: embed);
                    }
                }
                await Task.CompletedTask;
            };

            await Task.CompletedTask;
        }
    }
}