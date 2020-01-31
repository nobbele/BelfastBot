using Discord;
using Discord.Commands;
using BelfastBot.Services.Configuration;
using BelfastBot.Services.Logging;
using System.Threading.Tasks;
using BelfastBot.Services.Communiciation;
using System.Linq;
using System;
using BelfastBot.Services.Database;

namespace BelfastBot.Modules
{
    public abstract class BelfastModuleBase : ModuleBase<BelfastCommandContext>
    {
        public LoggingService Logger { get; set; }
        public IBotConfigurationService Config { get; set; }
        public IDiscordClient DiscordClient { get; set; }
        public ICommunicationService CommunicationService { get; set; }
        public JsonDatabaseService Database { get; set; }

        public string Prefix => Config.Configuration.Prefix;

        #nullable enable

        protected override Task<IUserMessage> ReplyAsync(string? message = null, bool isTTS = false, Embed? embed = null, RequestOptions? options = null)
        {
            Logger.LogInfo($"[ReplyAsync] {message ?? $"*Embed*{embed?.Title}"}");
            return CommunicationService.SendMessageAsync(Context.Channel, message, embed);
        }

        protected async Task<T?> TryGetUserData<T>(string userArg, Func<IUser, T?> accessor, Func<string, T?>? stringAccessor = null) where T : class
        {
            IUser target = await DiscordClient.GetUserAsync(Context.Message.MentionedUserIds.FirstOrDefault());

            if(target != null)
                return accessor.Invoke(target);
            else
            {
                if(!string.IsNullOrEmpty(userArg))
                    return stringAccessor?.Invoke(userArg) ?? userArg as T;
                else
                    return accessor.Invoke(Context.User);
            }
        }

        protected async Task<T?> TryGetUserData<T>(string userArg, Func<IUser, T?> accessor, Func<string, T?> stringAccessor) where T : struct
        {
            IUser target = await DiscordClient.GetUserAsync(Context.Message.MentionedUserIds.FirstOrDefault());

            if(target != null)
                return accessor.Invoke(target);
            else
            {
                if(!string.IsNullOrEmpty(userArg))
                    return stringAccessor.Invoke(userArg);
                else
                    return accessor.Invoke(Context.User);
            }
        }

        protected string? NotNullOrEmptyStringDatabaseAccessor(IUser user, Func<DatabaseUserEntry, string?> accessor)
        {
            string? result = accessor.Invoke(Database.GetUserEntry(0, user.Id));
            if(string.IsNullOrEmpty(result))
                return null;
            else
                return result;
        }

        protected T NormalDatabaseAccessor<T>(IUser user, Func<DatabaseUserEntry, T> accessor)
            => accessor.Invoke(Database.GetUserEntry(0, user.Id));
    }
}