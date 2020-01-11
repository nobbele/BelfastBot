using SenkoSanBot.Services.Database;
using System.Timers;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;
using System.Linq;
using Discord;
using Discord.WebSocket;
using SenkoSanBot.Services.Configuration;

namespace SenkoSanBot.Services.Giveaway
{
    public class GiveawayService
    {
        private JsonDatabaseService m_db;
        private DiscordSocketClient m_client;
        private readonly IBotConfigurationService m_config;

        private Random m_random = new Random();

        public GiveawayService(JsonDatabaseService db, DiscordSocketClient client, IBotConfigurationService config) 
        {
            m_db = db;
            m_client = client;
            m_config = config;
        }

        public async Task InitializeAsync()
        {
            foreach(ServerEntry server in m_db.Db.Values)
            {
                foreach(GiveawayEntry entry in server.Giveaways) 
                {
                    AddGiveaway(entry, server.Id);
                }
            }

            await Task.CompletedTask;
        }

        public void AddGiveaway(GiveawayEntry entry, ulong serverId) 
        {
            TimeSpan delay = entry.End - DateTime.Now;
            if(delay.TotalMilliseconds < 0)
                return;
            Task.Delay(delay).ContinueWith(task => ExecuteGiveaway(entry, serverId));
        }

        public async Task ExecuteGiveaway(GiveawayEntry entry, ulong serverId) 
        {
            SocketGuild guild = m_client.GetGuild(serverId);
            ServerEntry server = m_db.GetServerEntry(serverId);
            SocketTextChannel channel = guild.GetTextChannel(entry.ChannelId);
            IUserMessage message = await channel.GetMessageAsync(entry.ReactionMessageId) as IUserMessage;

            var asyncparticipants = message.GetReactionUsersAsync(new Emoji(server.GiveawayReactionEmote),int.MaxValue);
            IEnumerable<IUser> users = await asyncparticipants.FlattenAsync();

            List<IUser> participants = users.Where(user => user.Id != m_client.CurrentUser.Id).ToList();

            List<IUser> winners = new List<IUser>();

            for(int i = 0; i < entry.Count; i++) {
                IUser winner;
                do {
                    winner = participants[m_random.Next(0, participants.Count)];
                } while(winners.Contains(winner) && entry.Count < participants.Count);
                winners.Add(winner);
            }
            
            await channel.SendMessageAsync($"Giveaway ended with {string.Join(' ', winners.Select(winner => winner.Mention))} winning!");
        }
    }
}