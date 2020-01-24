using System;
using System.Threading.Tasks;
using Discord.Commands;
using Discord;

namespace YohaneBot.Modules.Preconditions
{
    public class RequireGuildAttribute : PreconditionAttribute
    {
        public override Task<PreconditionResult> CheckPermissionsAsync(ICommandContext context, CommandInfo command, IServiceProvider services)
        {
            if (context.Channel is IGuildChannel)
                return Task.FromResult(PreconditionResult.FromSuccess());
            else
                return Task.FromResult(PreconditionResult.FromError("This command can only be run in a guild (aka server)"));
        }
    }
}