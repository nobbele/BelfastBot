using System.Threading.Tasks;
using Discord;

namespace YohaneBot.Services.Communiciation
{
    public interface ICommunicationService
    {
        Task<IUserMessage> SendMessageAsync(IMessageChannel channel, string message = null, Embed embed = null);
    }
}