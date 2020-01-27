using System.Threading.Tasks;
using Discord;

namespace BelfastBot.Services.Communiciation
{
    public class DiscordCommunicationService : ICommunicationService
    {
        public Task<IUserMessage> SendMessageAsync(IMessageChannel channel, string message = null, Embed embed = null)
        {
            return channel.SendMessageAsync(message, false, embed, null);
        }
    }
}