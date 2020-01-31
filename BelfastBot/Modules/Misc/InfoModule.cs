using Discord;
using Discord.Commands;
using System.Linq;
using System.Threading.Tasks;
using BelfastBot.Services.Pagination;

namespace BelfastBot.Modules.Misc
{
    [Summary("Commands for information")]
    public class InfoModule : BelfastModuleBase
    {
        public CommandService Command { get; set; }
        public PaginatedMessageService PaginatedMessageService { get; set; }
        public IDiscordClient Client { get; set; } 
        public IClient Belfast { get; set; }

	    [Command("servers"), RequireOwner]
	    public async Task ServersAsync()
	    {
                await ReplyAsync(string.Join("\n", (await Client.GetGuildsAsync()).Select(guild => guild.Name)));
	    }

        [Command("help")]
        public async Task HelpAsync([Remainder] string command = null)
        {
            Logger.LogInfo($"{Context.User} asked for help about {command ?? "all commands"}");

            if (command == null)
                await ModulesHelp();
            else
                await CommandHelp(command);
        }

        private int modulesPerPage = 4;

        private async Task ModulesHelp()
        {
            ModuleInfo[] moduleInfos = Command.Modules.GroupBy(x => x.Name).Select(y => y.First()).ToArray();

            EmbedBuilder[] builders = new EmbedBuilder[(moduleInfos.Length / modulesPerPage) + 1];

            for(int j = 0; j < builders.Length; j++)
            {
                EmbedBuilder builder = builders[j] = new EmbedBuilder();
                builder.Description = $"Do \"{Prefix}help [command]\" to get more information about a command";
                builder.WithThumbnailUrl(Emotes.BelfastThink.Url);
                builder.WithColor(0xffae0d);
            }

            int i = 0;

            foreach (ModuleInfo module in moduleInfos)
            {
                string[] commandNames = module.Commands.Select(cmd => $"► **{cmd.Name}**: [{cmd.Summary ?? "None"}]").ToArray();

                EmbedBuilder builder = builders[i / modulesPerPage];

                if (commandNames.Length > 0)
                {
                    builder.AddField($"{i+1}", $"" +
                        $"__**{module.Name.Replace("Module", " ")} - {module.Summary ?? ""}**__\n" +
                        $"{commandNames.NewLineSeperatedString()}");
                }

                i++;
            }

            await PaginatedMessageService.SendPaginatedEmbedMessageAsync(Context.Channel, builders);
        }

        private async Task CommandHelp(string command)
        {
            SearchResult result = Command.Search(Context, command);
            if (!result.IsSuccess || result.Commands.Count == 0)
            {
                await ReplyAsync($"> Couldn't find command '{command}'");
                return;
            }

            foreach (CommandInfo commandInfo in result.Commands.Select(cmd => cmd.Command))
            {
                EmbedBuilder builder = new EmbedBuilder();

                builder.Title = $"{commandInfo.Name} {(commandInfo.Aliases.Count > 1 ? $"({commandInfo.Aliases[1]})" : "")} - {commandInfo.Summary ?? "No information about the command specified"}";
                builder.WithColor(0xffae0d);

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

        [Command("about")]
        public async Task About()
        {
            Logger.LogInfo($"{Context.User} requested about page");

            Embed embed = new EmbedBuilder()
                .WithColor(0x308ED6)
                .AddField("About ▼", $"" +
                $"► Made by: **Team Shinaosu**\n" +
                $"► Version: **{Belfast.Version}**\n" +
                $"► Developers: **[Nobbele](https://github.com/nobbele)** & **[TheJayDuck](https://github.com/thejayduck)**\n" +
                $"► Status: **Still In Development**\n" +
                $"[View Source Code](https://github.com/nobbele/BelfastBot)")
                .WithFooter("Created: 09/07-2019")
                .WithThumbnailUrl("https://cdn.discordapp.com/attachments/303528930634235904/629383238917292042/29692031.png")
                .Build();

            await ReplyAsync(embed: embed);
        }
    }
}