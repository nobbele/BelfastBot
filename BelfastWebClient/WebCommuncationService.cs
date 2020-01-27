using System.Collections.Generic;
using System.Threading.Tasks;
using Discord;
using BelfastBot.Services.Communiciation;
using System.Collections.Concurrent;

namespace BelfastWebClient
{
    public class WebCommunicationService : ICommunicationService
    {
        public ConcurrentDictionary<ulong, ConcurrentBag<IUserMessage>> Channels = new ConcurrentDictionary<ulong, ConcurrentBag<IUserMessage>>();

        public async Task<IUserMessage> SendMessageAsync(IMessageChannel channel, string message = null, Embed embed = null)
        {
            WebUserMessage userMessage = new WebUserMessage(message, embed);
            Channels.GetOrAdd(channel.Id, new ConcurrentBag<IUserMessage>()).Add(userMessage);
            return userMessage;
        }
    }
}