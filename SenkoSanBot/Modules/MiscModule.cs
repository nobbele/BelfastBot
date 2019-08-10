using Discord;
using Discord.Commands;
using SenkoSanBot.Services;
using System.Linq;
using System.Threading.Tasks;

namespace SenkoSanBot.Modules
{
    public class MiscModule : ModuleBase<SocketCommandContext>
    {
        public BotConfigurationService Config { get; set; }
        public CommandService Command { get; set; }

        [Command("help")]
        public async Task HelpAsync()
        {
            string prefix = Config.Configuration.Prefix;

            EmbedBuilder builder = new EmbedBuilder();

            builder.Description = $"Do {prefix}help [command] to get more information about a command";

            foreach(ModuleInfo module in Command.Modules.GroupBy(x => x.Name).Select(y => y.First()))
            {
                string[] commandNames = module.Commands.Select(cmd => cmd.Name).ToArray();
                if (commandNames.Length > 0)
                {
                    builder.AddField($"{module.Name.Replace("Module", " ")} - {module.Summary ?? ""}", string.Join(", ", commandNames));
                }
            }

            await ReplyAsync(embed: builder.Build());
        }

        [Command("help")]
        public async Task ModuleHelpAsync(string command)
        {
            string prefix = Config.Configuration.Prefix;

            SearchResult result = Command.Search(Context, command);
            if(!result.IsSuccess || result.Commands.Count == 0)
            {
                await ReplyAsync($"Couldn't find command '{command}'");
                return;
            }

            foreach (CommandInfo commandInfo in result.Commands.Select(cmd => cmd.Command))
            {
                EmbedBuilder builder = new EmbedBuilder();

                builder.Title = commandInfo.Name;

                foreach(ParameterInfo parameter in commandInfo.Parameters)
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
}
