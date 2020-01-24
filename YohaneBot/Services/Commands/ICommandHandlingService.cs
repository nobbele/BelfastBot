using System.Threading.Tasks;
using Discord.WebSocket;

namespace YohaneBot.Services.Commands
{
    public interface ICommandHandlingService
    {
        Task InitializeAsync();
        Task HandleCommandAsync(SocketMessage messageParam);
    }
}