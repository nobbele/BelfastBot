using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;

namespace BelfastBot.Services.Commands
{
    public interface ICommandHandlingService
    {
        Task InitializeAsync();
        Task HandleCommandAsync(IUserMessage messageParam, bool parsePrefix);
    }
}