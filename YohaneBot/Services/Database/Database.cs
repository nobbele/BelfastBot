using System.Collections.Generic;
using YohaneBot.Services.Scheduler;
using System;

namespace YohaneBot.Services.Database
{
    public class Database
    {
        public Dictionary<ulong, ServerEntry> Servers = new Dictionary<ulong, ServerEntry>();
        public Dictionary<Guid, SchedulerEntry> Schedules = new Dictionary<Guid, SchedulerEntry>();
    }
}