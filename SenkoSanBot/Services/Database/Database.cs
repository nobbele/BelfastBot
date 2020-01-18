using System.Collections.Generic;
using SenkoSanBot.Services.Scheduler;
using System;

namespace SenkoSanBot.Services.Database
{
    public class Database
    {
        public Dictionary<ulong, ServerEntry> Servers = new Dictionary<ulong, ServerEntry>();
        public Dictionary<Guid, SchedulerEntry> Schedules = new Dictionary<Guid, SchedulerEntry>();
    }
}