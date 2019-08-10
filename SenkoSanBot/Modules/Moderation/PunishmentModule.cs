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

        [Command("warn")]
        [Summary("Warns people don't behave properly")]
        [RequireUserPermission(GuildPermission.KickMembers)]
        [RequireUserPermission(GuildPermission.BanMembers)]
        public async Task Warn(SocketGuildUser target, [Remainder] string reason = "No reason specified")
        {
            await Context.Message.DeleteAsync();

            DatabaseUserEntry user = Db.GetUserEntry(target.Id);

            user.Warns.Add(new Warn(reason, Context.User.Id));
            await ReplyAsync($"Warned {target.Mention} for \"{reason}\"");

            int targetMaxRole = target.Roles.Max(role => role.Position);
            int botMaxRole = Context.Guild.GetUser(Context.User.Id).Roles.Max(role => role.Position);

            if (user.Warns.Count == 2)
            {
                if (botMaxRole > targetMaxRole)
                {
                    await target.KickAsync(reason);
                    await ReplyAsync($"Kicked {target.Mention} for having too many warns");
                }
                else
                {
                    await ReplyAsync($"Can't kick {target.Mention} with higher role than me");
                }
            }
            else if (user.Warns.Count >= 3)
            {
                if (botMaxRole > targetMaxRole)
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
}
