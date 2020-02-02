using Discord;
using Discord.Commands;
using BelfastBot.Services.Database;
using System.Threading.Tasks;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace BelfastBot.Modules.Misc
{
    [Summary("Commands that are only for owner")]
    public class OwnerModule : BelfastModuleBase
    {
        public IClient Belfast { get; set; }
        public JsonDatabaseService Db { get; set; }

        [Command("stop"), RequireOwner]
        [Summary("Stops Belfast")]
        public async Task StopAsync()
        {
            Logger.LogInfo($"{Context.User} stopped Belfast");
            await ReplyAsync("Stopping...");

            Belfast.Stop();
        }

        [Command("servers"), RequireOwner]
	    public async Task ServersAsync()
	    {
            await ReplyAsync(string.Join("\n", (await DiscordClient.GetGuildsAsync()).Select(guild => guild.Name)));
	    }

        [Command("addcoin"), RequireOwner]
        [Summary("Adds coin with given amount")]
        public async Task AddCoinAsync([Summary("Amount to give")]uint amount = 100, [Summary("Optional mention")]IUser target = null)
        {
            target = target ?? Context.Message.Author;

            if (target.IsBot)
                return;

            Logger.LogInfo($"Given {target.Username} {amount} Coins");

            Db.GetUserEntry(0, target.Id).Coins += amount;
            Db.WriteData();
            await ReplyAsync($"> Given {target.Mention} {amount} Coins {Emotes.DiscordCoin}");
        }
    }
}