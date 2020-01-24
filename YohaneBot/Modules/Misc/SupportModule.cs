using Discord;
using Discord.Commands;
using YohaneBot.Services.Pagination;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace YohaneBot.Modules.Misc
{
    [Summary("Commands for guiding users")]
    public class SupportModule : YohaneModuleBase
    {
        public string BaseUrlLmgtfy = "https://lmgtfy.com";

        public PaginatedMessageService PaginatedMessageService { get; set; }

        #region Tags
        [Command("tags")]
        [Summary("Shows list of available tags")]
        public async Task TagsAsync()
        {
            if (Config.Configuration.Tags.Count <= 0)
            {
                await ReplyAsync("> No tags found");
                return;
            }

            Embed embed = new EmbedBuilder()
                .WithColor(0xb39df2)
                .WithTitle($"List of available tags")
                .AddField("Tags", $"{(Config.Configuration.Tags.Select(tag => $"► [**{tag.Key}** : **{tag.Value}**]").NewLineSeperatedString())}")
                .WithFooter(footer => {
                    footer
                        .WithText($"Requested by {Context.User}")
                        .WithIconUrl(Context.User.GetAvatarUrl());
                })
                .Build();

            await ReplyAsync(embed: embed);
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
                await ReplyAsync("> Invalid tag");
                return;
            }
            await ReplyAsync(content);
        }

        [Command("tagadd")]
        [Summary("Adds a tag with the given value")]
        [RequireOwner]
        public async Task AddTagAsync(string tag, [Remainder] string value)
        {
            Config.Configuration.Tags.Add(tag, value);
            Config.WriteData();
            await ReplyAsync("> Added Tag");
        }

        [Command("tagedit")]
        [Summary("Edits a tag with given value")]
        [RequireOwner]
        public async Task EditTagAsync(string tag, [Remainder] string value)
        {
            if (!Config.Configuration.Tags.TryGetValue(tag, out string content))
            {
                await ReplyAsync("> Invalid tag");
                return;
            }
            Config.Configuration.Tags[tag] = value;
            Config.WriteData();
            await ReplyAsync("> Edited Tag");
        }

        [Command("tagdelete"), Alias("tagdel")]
        [Summary("Deletes a tag with given name")]
        [RequireOwner]
        public async Task DeleteTagAsync([Remainder] string tag)
        {
            if (!Config.Configuration.Tags.TryGetValue(tag, out string content))
            {
                await ReplyAsync("> Invalid tag");
                return;
            }
            Config.Configuration.Tags.Remove(tag);
            Config.WriteData();
            await ReplyAsync("> Removed Tag");
        }
        #endregion

        [Command("lmgtfy"), Alias("lmg")]
        [Summary("Creates lmgtfy url")]
        public async Task LmgtfySearchAsync([Summary("0 = google\n" +
            "1 = yahoo\n" +
            "2 = bing\n" +
            "3 = ask\n" +
            "4 = aol.\n" +
            "5 = duckduckgo")]int type = 0, [Remainder] string search = "")
        {
            string engine = null;
            switch (type)
            {
                case 0:
                    engine = "p=1&s=g&t=w";
                    break;
                case 1:
                    engine = "p=1&s=y&t=w";
                    break;
                case 2:
                    engine = "p=1&s=b&t=w";
                    break;
                case 3:
                    engine = "p=1&s=k&t=w";
                    break;
                case 4:
                    engine = "p=1&s=a&t=w";
                    break;
                case 5:
                    engine = "p=1&s=d&t=w";
                    break;
            }

            search = search.Replace(' ', '+');
            string url = $"<{BaseUrlLmgtfy}/?q={search}&{engine}>";

            await ReplyAsync(url);
        }
    }
}