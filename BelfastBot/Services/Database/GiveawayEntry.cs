using System;
using System.Collections.Generic;

namespace BelfastBot.Services.Database
{
    public class GiveawayEntry
    {
        public DateTime End;
        public string Content;
        public ulong ChannelId;
        public ulong ReactionMessageId;
        public uint Count;
    }
}