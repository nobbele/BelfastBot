using Discord;
using Discord.Commands;
using BelfastBot.Services.Commands;
using BelfastBot.Services.Database;
using System.Threading.Tasks;
using System;
using System.Diagnostics;
using System.IO;

namespace BelfastBot.Modules.Misc
{
    [Summary("Commands that are only for owner")]
    public class OwnerModule : BelfastModuleBase
    {
        public IClient Belfast { get; set; }
        public JsonDatabaseService Db { get; set; }

        [Command("stop")]
        [Summary("Stops Belfast")]
        [RequireOwner]
        public async Task StopAsync()
        {
            Logger.LogInfo($"{Context.User} stopped Belfast");
            await ReplyAsync("Stopping...");

            Belfast.Stop();
        }

        [Command("coin add")]
        [Summary("Adds coin with given amount")]
        [RequireUserPermission(GuildPermission.Administrator)]
        public async Task AddCoinAsync([Summary("Amount to give")]int amount = 100, [Summary("Optional mention")]IUser target = null)
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