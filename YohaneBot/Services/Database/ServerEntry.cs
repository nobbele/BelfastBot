using System.Collections.Generic;
using Discord;

namespace YohaneBot.Services.Database
{
    public class ServerEntry
    {
        public List<DatabaseUserEntry> Users = new List<DatabaseUserEntry>();
        public string GiveawayReactionEmote;
        public ulong Id = 0;
    }
}