using System.Collections.Generic;
using BelfastBot.Services.Scheduler;
using System;

namespace BelfastBot.Services.Database
{
    public class Database
    {
        public Dictionary<ulong, ServerEntry> Servers = new Dictionary<ulong, ServerEntry>();
        public Dictionary<Guid, SchedulerEntry> Schedules = new Dictionary<Guid, SchedulerEntry>();
    }
}