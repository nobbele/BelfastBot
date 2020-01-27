using Discord;
using Discord.Commands;
using Discord.WebSocket;
using BelfastBot.Services.Configuration;
using BelfastBot.Services.Database;
using System;
using System.Threading.Tasks;
using BelfastBot.Services.Logging;

namespace BelfastBot.Services.Credits
{
    public class MessageRewardService
    {
        private readonly DiscordSocketClient m_client;
        private readonly IBotConfigurationService m_config;
        private readonly JsonDatabaseService m_db;
        private readonly LoggingService m_logger;

        public MessageRewardService(IDiscordClient client, IBotConfigurationService config, JsonDatabaseService db, LoggingService logger)
        {
            m_client = client as DiscordSocketClient;
            m_config = config;
            m_db = db;
            m_logger = logger;
        }

        public async Task InitializeAsync()
        {
            if(m_client == null)
            {
                m_logger.LogWarning($"[{nameof(MessageRewardService)}] m_client is null, ignoring");
                return;
            }
            m_client.MessageReceived += async (SocketMessage msg) =>
            {
                SocketUserMessage message = msg as SocketUserMessage;
                int argPos = 0;
                if (!(message.HasStringPrefix(m_config.Configuration.Prefix, ref argPos, StringComparison.OrdinalIgnoreCase) 
                    || message.HasMentionPrefix(m_client.CurrentUser, ref argPos) 
                    || message.Author.IsBot))
                {
                    //DatabaseUserEntry userDB = m_db.GetUserEntry((message.Channel as SocketGuildChannel).Guild.Id, message.Author.Id);
                    DatabaseUserEntry userDB = m_db.GetUserEntry(0, message.Author.Id);
                    IUser user = message.Author;

                    userDB.Coins++;
                    uint oldLevel = userDB.Level;
                    userDB.Xp += (uint)Math.Log(message.Content.Length + 1);
                    uint newLevel = userDB.Level;

                    if(oldLevel != newLevel)
                    {
                        int oldCoins = userDB.Coins;
                        int awardedCoins = (int)Math.Pow(userDB.Level / 2f, 4f);
                        userDB.Coins += awardedCoins;

                        Embed embed = new EmbedBuilder()
                            .WithColor(0xFF1288)
                            .WithThumbnailUrl(Emotes.SenkoHappy.Url)
                            .WithAuthor(author => {
                                author
                                    .WithName($"Profile of {message.Author.Username}")
                                    .WithIconUrl($"{message.Author.GetAvatarUrl()}");
                            })
                            .WithTitle($"{user.Username} Leveled Up! ▲")
                            .AddField("Details",
                            $"► Level: **{oldLevel}** => **{userDB.Level}**\n" +
                            $"► Coins: **{oldCoins}** => **{userDB.Coins}** {Emotes.DiscordCoin} (**+{awardedCoins}**{Emotes.DiscordCoin})")
                            .Build();

                        m_db.WriteData();

                        //await message.Channel.SendMessageAsync(embed: embed);
                    }
                }
                await Task.CompletedTask;
            };

            await Task.CompletedTask;
        }
    }
}