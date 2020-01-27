using Discord;
using Discord.Commands;
using Discord.WebSocket;

namespace BelfastBot.Modules
{
    public class BelfastCommandContext : ICommandContext
    {
        public IDiscordClient Client { get; set; }

        public IGuild Guild { get; set; }

        public IMessageChannel Channel { get; set; }

        public IUser User { get; set; }

        public IUserMessage Message { get; set; }

        public BelfastCommandContext() {}

        public BelfastCommandContext(DiscordSocketClient client, SocketUserMessage msg)
        {
            Client = client;
            Guild = (msg.Channel as SocketGuildChannel)?.Guild;
            Channel = msg.Channel;
            User = msg.Author;
            Message = msg;
        }
    }
}