using Discord;
using Discord.Commands;
using Discord.WebSocket;
using SenkoSanBot.Services;
using System.Linq;
using System.Threading.Tasks;

namespace SenkoSanBot.Modules.Moderation
{
    [Summary("Contains commands used to punish users who behave badly")]
    public class PunishmentModule : ModuleBase<SocketCommandContext>
    {
        public JsonDatabaseService Db { get; set; }

        private bool IsBotHigherRoleThan(SocketGuildUser target)
        {
            int targetMaxRole = target.Roles.Max(role => role.Position);
            int botMaxRole = Context.Guild.GetUser(Context.User.Id).Roles.Max(role => role.Position);

            return botMaxRole > targetMaxRole;
        }

        [Command("warn")]
        [Summary("Warns people who don't behave properly")]
        [RequireUserPermission(GuildPermission.KickMembers)]
        [RequireUserPermission(GuildPermission.BanMembers)]
        public async Task Warn([Summary("User that will be warned")] SocketGuildUser target, 
                               [Summary("Reason for warning the user")] [Remainder] string reason = "No reason specified")
        {
            await Context.Message.DeleteAsync();

            if (target == Context.User)
            {
                await ReplyAsync($"{Context.User.Mention} You cannot warn yourself");
                return;
            }

            DatabaseUserEntry user = Db.GetUserEntry(target.Id);

            user.Warns.Add(new Warn(reason, Context.User.Id));
            await ReplyAsync($"Warned {target.Mention} for \"{reason}\"");

            bool isHigherRole = IsBotHigherRoleThan(target);

            if (user.Warns.Count == 2)
                await KickUserAsync(target, reason);
            else if (user.Warns.Count >= 3)
                await BanUserAsync(target, reason);
        }

        [Command("kick")]
        [Summary("Kicks people who don't behave properly")]
        [RequireUserPermission(GuildPermission.KickMembers)]
        public async Task KickUserAsync(SocketGuildUser target, [Remainder] string reason = "No reason specified")
        {
            if (target == Context.User)
            {
                await ReplyAsync($"{Context.User.Mention} You cannot kick yourself");
                return;
            }

            bool isHigherRole = IsBotHigherRoleThan(target);

            if (isHigherRole)
            {
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
        public async Task BanUserAsync(SocketGuildUser target, [Remainder] string reason = "No reason specified")
        {
            if (target == Context.User)
            {
                await ReplyAsync($"{Context.User.Mention} You cannot ban yourself");
                return;
            }

            bool isHigherRole = IsBotHigherRoleThan(target);

            if (isHigherRole)
            {
                await target.BanAsync(0, reason);
                await ReplyAsync($"Banned {target.Mention} for having too many warns");
            }
            else
            {
                await ReplyAsync($"Can't ban {target.Mention} with higher role than me");
            }
        }
    }
}