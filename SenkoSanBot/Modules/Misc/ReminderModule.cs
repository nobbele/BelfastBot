using Discord.Commands;
using System.Threading.Tasks;
using System;
using SenkoSanBot.Modules;
using SenkoSanBot.Services.Scheduler;

namespace SenkoSanBot
{
    [Summary("Commands for reminders")]
    public class ReminderModule : SenkoSanModuleBase
    {
        public SchedulerService m_scheduler { get; set; }

        [Command("remind me")]
        public async Task AddReminder(string time, string content)
        {
            if(!DateTimeHelper.TryParseRelative(time, out DateTime end))
                await ReplyAsync("Couldn't parse time");

            m_scheduler.Add(end, new Func<Task>(async () => {
                await Context.Channel.SendMessageAsync($"{Context.User.Mention}, I am here to remind you about **{content}**");
            }));

            await ReplyAsync($"I will remind you about **{content}** at {end.ToUniversalTime()} UTC");
        }
    }
}