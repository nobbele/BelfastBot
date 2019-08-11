using Discord;
using Discord.Commands;
using SenkoSanBot.Services;
using System.Linq;
using System.Threading.Tasks;

namespace SenkoSanBot.Modules
{
    [Summary("Miscellaneous commands")]
    public class MiscModule : SenkoSanModuleBase
    {
        public BotConfigurationService Config { get; set; }
        public CommandService Command { get; set; }

        [Command("help")]
        public async Task HelpAsync(string command = null)
        {
            Logger.LogInfo($"{Context.User} asked for help about {command ?? "all commands"}");

            string prefix = Config.Configuration.Prefix;

            if (command == null)
            {
                EmbedBuilder builder = new EmbedBuilder();

                builder.Description = $"Do {prefix}help [command] to get more information about a command";

                foreach (ModuleInfo module in Command.Modules.GroupBy(x => x.Name).Select(y => y.First()))
                {
                    string[] commandNames = module.Commands.Select(cmd => cmd.Name).ToArray();
                    if (commandNames.Length > 0)
                    {
                        builder.AddField($"{module.Name.Replace("Module", " ")} - {module.Summary ?? ""}", string.Join(", ", commandNames));
                    }
                }

                await ReplyAsync(embed: builder.Build());
            }
            else
            {
                SearchResult result = Command.Search(Context, command);
                if (!result.IsSuccess || result.Commands.Count == 0)
                {
                    await ReplyAsync($"Couldn't find command '{command}'");
                    return;
                }

                foreach (CommandInfo commandInfo in result.Commands.Select(cmd => cmd.Command))
                {
                    EmbedBuilder builder = new EmbedBuilder();

                    builder.Title = commandInfo.Name;

                    foreach (ParameterInfo parameter in commandInfo.Parameters)
                    {
                        string name;
                        if (parameter.IsOptional) name = $"({parameter.Name}) defaults to \"{parameter.DefaultValue}\"";
                        else name = $"[{parameter.Name}]";
                        builder.AddField(name, parameter.Summary ?? "No information specified");
                    }

                    await ReplyAsync(embed: builder.Build());
                }
            }
        }

        [Command("about")]
        public async Task About()
        {
            Logger.LogInfo($"{Context.User} requested about page");

            Embed embed = new EmbedBuilder()
                .WithTitle("Made by: Team Shinaosu")
                .WithColor(0xcfbadb)
                .AddField("Developers", "Nobbele & JayDuck", true)
                .WithFooter("09/07-2019")
                .WithThumbnailUrl("https://cdn.discordapp.com/attachments/303528930634235904/571686869163704320/Shinaosu.png")
                .Build();

            await ReplyAsync(embed: embed);
        }
    }
}