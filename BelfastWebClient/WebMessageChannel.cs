using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Discord;
using IAsyncEnumerableFix;

namespace BelfastWebClient
{
    public class WebMessageChannel : MessageChannel
    {
        private ulong _id = 0;
        public override ulong Id => _id;

        public WebMessageChannel(ulong id = 0)
        {
            _id = id;
        }
    }
}