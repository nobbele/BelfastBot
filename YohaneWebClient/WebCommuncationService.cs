using System.Collections.Generic;
using System.Threading.Tasks;
using Discord;
using YohaneBot.Services.Communiciation;

namespace YohaneWebClient
{
    public class WebCommunicationService : ICommunicationService
    {
        public List<IUserMessage> Messages = new List<IUserMessage>();

        public async Task<IUserMessage> SendMessageAsync(IMessageChannel channel, string message = null, Embed embed = null)
        {
            WebUserMessage userMessage = new WebUserMessage(message, embed);
            Messages.Add(userMessage);
            return userMessage;
        }
    }
}