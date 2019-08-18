using Discord;
using Discord.Commands;
using SenkoSanBot.Services.Pagination;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SenkoSanBot.Modules.Misc
{
    [Summary("Commands for supporting users")]
    public class SupportModule : SenkoSanModuleBase
    {
        public PaginatedMessageService PaginatedMessageService { get; set; }

        [Command("tags")]
        [Summary("Shows list of available tags")]
        public async Task TagsAsync()
        {
            if (Config.Configuration.Tags.Count <= 0)
            {
                await ReplyAsync("No tags found");
                return;
            }

            await PaginatedMessageService.SendPaginatedDataMessageAsync(Context.Channel, Config.Configuration.Tags.ToArray(), GetTagEmbed);
        }

        private Embed GetTagEmbed(KeyValuePair<string, string> data, int index, EmbedFooterBuilder footer) => new EmbedBuilder()
            .WithTitle($"Tag Name: {data.Key}")
            .WithDescription(data.Value)
            .WithFooter(footer)
            .Build();

        [Command("tag")]
        [Summary("Calls an existing tag")]
        public async Task TagAsync([Remainder] string tag)
        {
            if(!Config.Configuration.Tags.TryGetValue(tag, out string content))
            {
                await ReplyAsync("Invalid tag");
                return;
            }
            await ReplyAsync(content);
        }

        [Command("tagadd")]
        [Summary("Adds a tag with the given value")]
        public async Task AddTagAsync(string tag, [Remainder] string value)
        {
            Config.Configuration.Tags.Add(tag, value);
            Config.WriteData();
            await ReplyAsync("> Added Tag");
        }

        [Command("tagedit")]
        [Summary("Edits a tag with given value")]
        public async Task EditTagAsync(string tag, [Remainder] string value)
        {
            if (!Config.Configuration.Tags.TryGetValue(tag, out string content))
            {
                await ReplyAsync("Invalid tag");
                return;
            }
            Config.Configuration.Tags[tag] = value;
            Config.WriteData();
            await ReplyAsync("> Edited Tag");
        }

        [Command("tagdelete"), Alias("tagdel")]
        [Summary("Deletes a tag with given name")]
        public async Task DeleteTagAsync([Remainder] string tag)
        {
            if (!Config.Configuration.Tags.TryGetValue(tag, out string content))
            {
                await ReplyAsync("Invalid tag");
                return;
            }
            Config.Configuration.Tags.Remove(tag);
            Config.WriteData();
            await ReplyAsync("> Removed Tag");
        }
    }
}
