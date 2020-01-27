using BelfastBot.Services.Database;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;
using System.Linq;
using Discord;
using Discord.WebSocket;
using BelfastBot.Services.Configuration;
using BelfastBot.Services.Scheduler;
using Newtonsoft.Json;

namespace BelfastBot.Services.Giveaway
{
    public class GiveawayService
    {
        private JsonDatabaseService m_db;
        private IDiscordClient m_client;
        private SchedulerService m_scheduler;
        private readonly IBotConfigurationService m_config;

        private Random m_random = new Random();

        public GiveawayService(JsonDatabaseService db, IDiscordClient client, IBotConfigurationService config, SchedulerService scheduler) 
        {
            m_db = db;
            m_client = client;
            m_config = config;
            m_scheduler = scheduler;
        }

        private struct GiveawaySchedulerData
        {
            public GiveawayEntry entry;
            public ulong serverId;

            public GiveawaySchedulerData(GiveawayEntry entry, ulong serverId)
            {
                this.entry = entry;
                this.serverId = serverId;
            }
        }

        public void AddGiveaway(GiveawayEntry entry, ulong serverId) 
        {
            GiveawaySchedulerData data = new GiveawaySchedulerData(entry, serverId);
            string json = JsonConvert.SerializeObject(data);
            m_scheduler.Add<GiveawayService>(entry.End, nameof(GiveawaySchedulerCallback), json);
        }

        public void GiveawaySchedulerCallback(string data)
        {
            GiveawaySchedulerData schedulerData = JsonConvert.DeserializeObject<GiveawaySchedulerData>(data);
            _ = ExecuteGiveaway(schedulerData.entry, schedulerData.serverId);
        }

        public async Task ExecuteGiveaway(GiveawayEntry entry, ulong serverId) 
        {
            IGuild guild = await m_client.GetGuildAsync(serverId);
            ServerEntry server = m_db.GetServerEntry(serverId);
            ITextChannel channel = await guild.GetTextChannelAsync(entry.ChannelId);
            IUserMessage message = await channel.GetMessageAsync(entry.ReactionMessageId) as IUserMessage;

            var asyncparticipants = message.GetReactionUsersAsync(new Emoji(server.GiveawayReactionEmote),int.MaxValue);
            IEnumerable<IUser> users = await asyncparticipants.FlattenAsync();

            List<IUser> participants = users.Where(user => user.Id != m_client.CurrentUser.Id).ToList();

            List<IUser> winners = new List<IUser>();

            if(participants.Count <= 0)
            {
                await channel.SendMessageAsync($"No one participated for {entry.Content}");
            }
            else
            {
                for(int i = 0; i < entry.Count; i++) {
                    IUser winner;
                    do {
                        winner = participants[m_random.Next(0, participants.Count)];
                    } while(winners.Contains(winner) && entry.Count < participants.Count);
                    winners.Add(winner);
                }
                
                await channel.SendMessageAsync($"{string.Join(' ', winners.Select(winner => winner.Mention))} won {entry.Content}!");
            }

            
        }
    }
}