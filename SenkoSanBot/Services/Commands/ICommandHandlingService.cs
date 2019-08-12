using System.Threading.Tasks;
using Discord.WebSocket;

namespace SenkoSanBot.Services.Commands
{
    public interface ICommandHandlingService
    {
        Task InitializeAsync();
        Task HandleCommandAsync(SocketMessage messageParam);
    }
}