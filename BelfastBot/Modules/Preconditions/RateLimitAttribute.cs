using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;

namespace BelfastBot.Modules
{
    public class RateLimitAttribute : PreconditionAttribute
    {
        private static Dictionary<int, Dictionary<IUser, List<DateTime>>> ExecutionAmountsPerCommands = new Dictionary<int, Dictionary<IUser, List<DateTime>>>();

        public int PerMinute;

        private int id;

        public RateLimitAttribute(Type type, int perMinute)
        {
            PerMinute = perMinute;
            this.id = type.GetHashCode();
        }

        #pragma warning disable CS1998
        public override async Task<PreconditionResult> CheckPermissionsAsync(ICommandContext context, CommandInfo command, IServiceProvider services)
        {
            if(!ExecutionAmountsPerCommands.TryGetValue(id, out Dictionary<IUser, List<DateTime>> ExecutionAmounts))
                ExecutionAmounts = ExecutionAmountsPerCommands.AddGet(KeyValuePair.Create(id, new Dictionary<IUser, List<DateTime>>())).Value;

            if(!ExecutionAmounts.TryGetValue(context.User, out List<DateTime> times))
                times = ExecutionAmounts.AddGet(KeyValuePair.Create(context.User, new List<DateTime>())).Value;

            ExecutionAmounts[context.User] = ExecutionAmounts[context.User].Where(date => date > DateTime.Now.AddMinutes(-1)).Append(DateTime.Now).ToList();

            if(ExecutionAmounts[context.User].Count > PerMinute)
                return PreconditionResult.FromError("Rate limit exceeded, Try again later");
            else
                return PreconditionResult.FromSuccess();
        }
        #pragma warning restore CS1998
    }
}