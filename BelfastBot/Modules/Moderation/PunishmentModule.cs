using Discord;
using Discord.Commands;
using Discord.WebSocket;
using BelfastBot.Modules.Preconditions;
using BelfastBot.Services.Database;
using System.Linq;
using System.Threading.Tasks;

namespace BelfastBot.Modules.Moderation
{
    [Summary("Contains commands used to punish users who behave badly")]
    public class PunishmentModule : BelfastModuleBase
    {
        public JsonDatabaseService Db { get; set; }

        private bool IsBotHigherRoleThan(SocketGuildUser target)
        {
            int targetMaxRole = target.Roles.Max(role => role.Position);
            int botMaxRole = Context.Guild.GetUserAsync(Context.User.Id).Result.RoleIds
                .Select(id => Context.Guild.GetRole(id))
                .Max(role => role.Position);

            return botMaxRole > targetMaxRole;
        }

        [Command("warn")]
        [Summary("Warns people who don't behave properly")]
        [RequireUserPermission(GuildPermission.KickMembers)]
        [RequireUserPermission(GuildPermission.BanMembers)]
        [RequireGuild]
        public async Task WarnAsync([Summary("User that will be warned")] SocketGuildUser target, 
                               [Summary("Reason for warning the user")] [Remainder] string reason = "No reason specified")
        {
            Logger.LogInfo($"Warning {target}");

            if (target.IsBot)
            {
                await ReplyAsync($"{Context.User.Mention} You cannot warn a bot");
                return;
            }

            if (target == Context.User)
            {
                await ReplyAsync($"{Context.User.Mention} You cannot warn yourself");
                return;
            }

            bool isHigherRole = IsBotHigherRoleThan(target);

            if (isHigherRole)
            {
                DatabaseUserEntry user = Db.GetUserEntry(target.Guild.Id, target.Id);

                user.Warns.Add(new Warn(reason, Context.User.Id));
                Db.WriteData();
                IDMChannel dm = await target.GetOrCreateDMChannelAsync();
                await dm.SendMessageAsync($"You have been warned on {Context.Guild.Name} for {reason}");
                await ReplyAsync($"Warned {target.Mention} for \"{reason}\"");

                if (user.Warns.Count == 2)
                    await KickUserAsync(target, reason);
                else if (user.Warns.Count >= Config.Configuration.MaxWarnAmount)
                    await BanUserAsync(target, reason);
            }
            else
            {
                await ReplyAsync($"Can't warn {target.Mention} with higher role than me");
            }
        }

        [Command("kick")]
        [Summary("Kicks people who don't behave properly")]
        [RequireUserPermission(GuildPermission.KickMembers)]
        [RequireGuild]
        public async Task KickUserAsync([Summary("The user who will be kicked")] SocketGuildUser target, [Remainder] string reason = "No reason specified")
        {
            Logger.LogInfo($"Kicking {target}");

            if (target.IsBot)
            {
                await ReplyAsync($"{Context.User.Mention} You cannot kick a bot");
                return;
            }

            if (target == Context.User)
            {
                await ReplyAsync($"{Context.User.Mention} You cannot kick yourself");
                return;
            }

            bool isHigherRole = IsBotHigherRoleThan(target);

            if (isHigherRole)
            {
                IDMChannel dm = await target.GetOrCreateDMChannelAsync();
                await dm.SendMessageAsync($"You have been kicked from {Context.Guild.Name}");
                await target.KickAsync(reason);
                await ReplyAsync($"Kicked {target.Mention} for having too many warns");
            }
            else
            {
                await ReplyAsync($"Can't kick {target.Mention} with higher role than me");
            }
        }

        [Command("ban")]
        [Summary("Bans people who don't behave properly")]
        [RequireUserPermission(GuildPermission.BanMembers)]
        [RequireGuild]
        public async Task BanUserAsync([Summary("The user who will be banned")] SocketGuildUser target, [Remainder] string reason = "No reason specified")
        {
            Logger.LogInfo($"Banning {target}");

            if (target.IsBot)
            {
                await ReplyAsync($"{Context.User.Mention} You cannot ban a bot");
                return;
            }

            if (target == Context.User)
            {
                await ReplyAsync($"{Context.User.Mention} You cannot ban yourself");
                return;
            }

            bool isHigherRole = IsBotHigherRoleThan(target);

            if (isHigherRole)
            {
                IDMChannel dm = await target.GetOrCreateDMChannelAsync();
                await dm.SendMessageAsync($"You have been banned from {Context.Guild.Name}");
                await target.BanAsync(0, reason);
                await ReplyAsync($"Banned {target.Mention}");
            }
            else
            {
                await ReplyAsync($"Can't ban {target.Mention} with higher role than me");
            }
        }

        [Command("warnings"), Alias("warns")]
        [Summary("Show warnings that a user has")]
        [RequireGuild]
        public async Task WarningsAsync([Summary("User who's warnings will be displayed")] SocketGuildUser target = null)
        {
            Logger.LogInfo($"Showing {target}'s warnings)");

            target = target ?? Context.User as SocketGuildUser;
            if (target == null)
            {
                await ReplyAsync("Couldn't find a user to target");
                return;
            }

            DatabaseUserEntry user = Db.GetUserEntry(target.Guild.Id, target.Id);

            EmbedFieldBuilder builder = new EmbedFieldBuilder()
                .WithName($"{target}'s Warnings");

            string value = string.Empty;

            int i = 1;
            foreach (Warn warn in user.Warns)
            {
                value += $"{i}. {warn.Reason}\n";
                i++;
            }

            if (string.IsNullOrEmpty(value))
                value = "No warnings";

            builder.WithValue(value);

            Embed embed = new EmbedBuilder()
                .WithColor(0xf09e24)
                .WithFields(builder)
                .Build();

            await ReplyAsync(embed: embed);
        }

        [Command("warndel")]
        [Summary("Deletes a specific warning from a user")]
        [RequireUserPermission(GuildPermission.KickMembers)]
        [RequireGuild]
        public async Task DeleteWarningAsync([Summary("User who's warning will be deleted")] SocketGuildUser target, int index)
        {
            Logger.LogInfo($"Deleting warning number {index} from {target}");

            int proIndex = index - 1;

            DatabaseUserEntry user = Db.GetUserEntry(target.Guild.Id, target.Id);
            if (proIndex > user.Warns.Count || proIndex < 0)
            {
                await ReplyAsync($"Out of bounds, user has {user.Warns.Count} warnings");
                return;
            }

            user.Warns.RemoveAt(proIndex);
            Db.WriteData();

            await ReplyAsync($"Deleted warning number {index} from {target.Mention}");
        }
    }
}