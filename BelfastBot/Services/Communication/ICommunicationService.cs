using System.Threading.Tasks;
using Discord;

namespace BelfastBot.Services.Communiciation
{
    public interface ICommunicationService
    {
        Task<IUserMessage> SendMessageAsync(IMessageChannel channel, string message = null, Embed embed = null);
    }
}