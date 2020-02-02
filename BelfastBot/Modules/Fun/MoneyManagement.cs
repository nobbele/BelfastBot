using System;
using System.Threading.Tasks;
using BelfastBot.Modules.Preconditions;
using BelfastBot.Services.Database;
using Discord;
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
            DatabaseUserEntry entry = Database.GetUserEntry(0, Context.Message.Author.Id);
            async Task giveCoins()
            {
                entry.LastDaily = DateTime.Now;
                entry.Coins += DailyCoinAmount;
                Database.WriteData();
                await ReplyAsync($"> You have been given **{DailyCoinAmount}**, your total is now **{entry.Coins}**.");
            }
            if(entry.LastDaily == null)
                await giveCoins();
            else
            {
                // Assume no one waits exactly a year, even if they do its not that bad
                if(entry.LastDaily.Value.DayOfYear != DateTime.Now.DayOfYear)
                    await giveCoins();
                else
                    await ReplyAsync($"> You have already collected your daily today, please try again tomorrow.");
            }
        }

        [Summary("Transfer coin to a user")]
        [Command("ctransfer")]
        [RequireGuild]
        public async Task TransferCoinAsync(IUser target, uint amount)
        {
            if (target.IsBot)
                return;

            DatabaseUserEntry entry = Database.GetUserEntry(0, Context.Message.Author.Id);
            DatabaseUserEntry targetUser = Database.GetUserEntry(0, target.Id);

            if (entry.Coins > amount)
                await transferCoins();
            else
                await ReplyAsync($"> {Emotes.BelfastPout} You don't seem to have {amount} coins to transfer.");

            async Task transferCoins()
            {
                entry.Coins -= amount;
                targetUser.Coins += amount;
                Database.WriteData();
                await ReplyAsync($"> {Emotes.BelfastHappy} You have given **{target.Mention}** {amount} coins!");
            }
        }
    }
}