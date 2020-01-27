using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Discord;
using Discord.Rest;
using Discord.WebSocket;
using IAsyncEnumerableFix;

namespace BelfastWebClient
{
    public class WebUserMessage : UserMessage
    {
        private List<Embed> _embeds = new List<Embed>();
        public override IReadOnlyCollection<IEmbed> Embeds => _embeds;

        private string _content = string.Empty;
        public override string Content => _content;

        public WebUserMessage(string message, Embed embed)
        {
            if(_content != null)
                _content = message;
            if(embed != null)
                _embeds.Add(embed);
        }
    }
}