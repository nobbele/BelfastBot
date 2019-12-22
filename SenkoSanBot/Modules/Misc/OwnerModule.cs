using Discord;
using Discord.Commands;
using SenkoSanBot.Services.Commands;
using SenkoSanBot.Services.Database;
using System.Threading.Tasks;
using System;
using System.Diagnostics;
using System.IO;

namespace SenkoSanBot.Modules.Misc
{
    [Summary("Commands that are only for owner")]
    public class OwnerModule : SenkoSanModuleBase
    {
        public SenkoSan Senko { get; set; }
        public JsonDatabaseService Db { get; set; }

        [Command("stop")]
        [Summary("Stops senko-san")]
        [RequireOwner]
        public async Task StopAsync()
        {
            Logger.LogInfo($"{Context.User} stopped senko");
            await ReplyAsync("Stopping...");

            BotCommandLineCommands.Stop(Senko);
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

        private async Task GitCommandAsync(string cmd, string path) 
        {
            await new Process()
            {
                StartInfo = new ProcessStartInfo
                {
                    WorkingDirectory = path,
                    FileName = "/usr/bin/git",
                    Arguments = cmd,
                    UseShellExecute = false,
                    CreateNoWindow = true,
                }
            }.StartAsync();
        }

        [Command("update")]
        [Summary("Updates senko-san")]
        [RequireOwner]
        public async Task UpdateAsync() 
        {
            string path = Config.Configuration.UpdatePath;
            if(!Directory.Exists(path))
                Directory.CreateDirectory(path);
            
            await ReplyAsync($"Updating source from {Config.Configuration.SourceCodeGit} to {path}");
            if(!Directory.Exists(Path.Combine(path, ".git"))) 
            {
                await GitCommandAsync($"clone {Config.Configuration.SourceCodeGit} .", path);
            }
            else
            {
                await GitCommandAsync($"pull origin master", path);
            }
            await ReplyAsync("Done updating source");

            await ReplyAsync($"Compiling");
            await new Process()
            {
                StartInfo = new ProcessStartInfo
                {
                    WorkingDirectory = path,
                    FileName = "/usr/bin/dotnet",
                    Arguments = $"build --output bin",
                    UseShellExecute = false,
                    CreateNoWindow = true,
                }
            }.StartAsync();
            await ReplyAsync("Done compiling");

            string binaryPath = Path.Combine(path, "SenkoSanBot/bin");

            if(File.Exists("update-old.sh"))
                File.Delete("update-old.sh");
            File.Move("update.sh", "update-old.sh");

            await ReplyAsync("Fixing update script permissions");
            await new Process()
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = "/usr/bin/chmod",
                    Arguments = $"+x ./update-old.sh",
                    UseShellExecute = false,
                    CreateNoWindow = true,
                }
            }.StartAsync();
            
            await ReplyAsync("Leaving for bash now, bye");
            new Process()
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = "/usr/bin/nohup",
                    Arguments = $"./update-old.sh \"{binaryPath}\" &",
                    UseShellExecute = false,
                    CreateNoWindow = true,
                }
            }.Start();
            await ReplyAsync("Stopping...");
            Senko.Stop(true);
        }
    }
}

public static class ProcessExtensionMethods
{
    // https://stackoverflow.com/a/50593453
    public static Task StartAsync(this Process process)
    {
        var tcs = new TaskCompletionSource<object>();
        process.EnableRaisingEvents = true;
        process.Exited += (s, e) => tcs.TrySetResult(null);
        if (!process.Start()) 
            tcs.SetException(new Exception("Failed to start process."));
        return tcs.Task;
    }
}