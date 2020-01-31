using Discord.Commands;
using System.Threading.Tasks;
using System;
using Discord;
using BelfastBot.Services.Scheduler;
using Newtonsoft.Json;
using BelfastBot.Modules.Preconditions;

namespace BelfastBot.Modules.Misc
{
    [Summary("Commands for reminders")]
    public class ReminderModule : BelfastModuleBase
    {
        public SchedulerService m_scheduler { get; set; }
        public IDiscordClient m_client { get; set; }

        private struct ReminderSchedulerData
        {
            public string userMention;
            public string content;
            public ulong serverId;
            public ulong channelId;

            public ReminderSchedulerData(string userMention, string content, ulong serverId, ulong channelId)
            {
                this.userMention = userMention;
                this.content = content;
                this.serverId = serverId;
                this.channelId = channelId;
            }
        }

        [Command("remind")]
        [RequireGuild] // For now it cant be run in dm
        public async Task AddReminder(string time, string content)
        {
            if(!DateTimeHelper.TryParseRelative(time, out DateTime end))
                await ReplyAsync("Couldn't parse time");

            string data = JsonConvert.SerializeObject(new ReminderSchedulerData(Context.User.Mention, content, Context.Guild.Id, Context.Channel.Id));

            m_scheduler.Add<ReminderModule>(end, nameof(ReminderSchedulerCallback), data);

            await ReplyAsync($"> I will remind you about **{content}** at {end.ToUniversalTime()} UTC");
        }

        public void ReminderSchedulerCallback(string data)
        {
            ReminderSchedulerData schedulerData = JsonConvert.DeserializeObject<ReminderSchedulerData>(data);
            IGuild server = m_client.GetGuildAsync(schedulerData.serverId).Result;
            ITextChannel channel = server.GetTextChannelAsync(schedulerData.channelId).Result;
            _ = channel.SendMessageAsync($"" +
                $"Commander {schedulerData.userMention}!\n" +
                $"I Belfast am here to remind you about \"**{schedulerData.content}**\"");
        }
    }
}