using System.Collections.Generic;
using Discord;

namespace SenkoSanBot.Services.Database
{
    public class ServerEntry
    {
        public List<DatabaseUserEntry> Users = new List<DatabaseUserEntry>();
        public string GiveawayReactionEmote;
        public ulong Id = 0;
    }
}