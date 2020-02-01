using System;
using System.Threading.Tasks;
using BelfastBot.Services.Database;
using Discord.Commands;

namespace BelfastBot.Modules.Fun
{
    [Summary("Commands for managing and interacting with commands that have to do with money")]
    public class MoneyManagement : BelfastModuleBase
    {
        public static readonly uint DailyCoinAmount = 10;

        [Summary("Get daily coins")]
        [Command("cdaily")]
        public async Task DailyAsync()
        {
            DatabaseUserEntry entry = Database.GetUserEntry(0, Context.Guild.Id);
            async Task giveCoins()
            {
                entry.LastDaily = DateTime.Now;
                entry.Coins += DailyCoinAmount;
                Database.WriteData();
                await ReplyAsync($"You have been given {DailyCoinAmount}, your total is now {entry.Coins}");
            }
            if(entry.LastDaily == null)
                await giveCoins();
            else
            {
                // Assume no one waits exactly a year, even if they do its not that bad
                if(entry.LastDaily.Value.DayOfYear != DateTime.Now.DayOfYear)
                    await giveCoins();
                else
                    await ReplyAsync($"You have already collected your daily today, please try again tomorrow");
            }
        }
    }
}