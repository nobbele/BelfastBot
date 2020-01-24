using Discord.Commands;
using System.Threading.Tasks;
using System;
using YohaneBot.Modules;
using YohaneBot.Services.Scheduler;
using Newtonsoft.Json;
using Discord.WebSocket;
using YohaneBot.Modules.Preconditions;

namespace YohaneBot.Modules.Misc
{
    [Summary("Commands for reminders")]
    public class ReminderModule : YohaneModuleBase
    {
        public SchedulerService m_scheduler { get; set; }
        public DiscordSocketClient m_client { get; set; }

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

        [Command("remindme")]
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
            SocketGuild server = m_client.GetGuild(schedulerData.serverId);
            SocketTextChannel channel = server.GetTextChannel(schedulerData.channelId);
            _ = channel.SendMessageAsync($"うやん～ {schedulerData.userMention}, I am here to remind you about **{schedulerData.content}**");
        }
    }
}