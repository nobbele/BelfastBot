using Discord;
using Discord.Commands;
using BelfastBot.Services.Pagination;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using JishoApi;
using Common;
using System.Web;

namespace BelfastBot.Modules.Misc
{
    [Summary("Commands for guiding users")]
    public class SupportModule : BelfastModuleBase
    {
        public string BaseUrlLmgtfy = "https://lmgtfy.com";

        public PaginatedMessageService PaginatedMessageService { get; set; }

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
        [RateLimit(typeof(SupportModule), perMinute: 45)]
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

        [Command("lmgtfy"), Alias("lmg")]
        [RateLimit(typeof(SupportModule), perMinute: 45)]
        [Summary("Creates lmgtfy url")]
        public async Task LmgtfySearchAsync([Summary("" +
            "► 0 = google\n" +
            "► 1 = yahoo\n" +
            "► 2 = bing\n" +
            "► 3 = ask\n" +
            "► 4 = aol.\n" +
            "► 5 = duckduckgo")]int type = 0, [Remainder] string search = "Belfast")
        {
            string engine = type switch
            {
                0 => "p=1&s=g&t=w",
                1 => "p=1&s=y&t=w",
                2 => "p=1&s=b&t=w",
                3 => "p=1&s=k&t=w",
                4 => "p=1&s=a&t=w.",
                5 => "p=1&s=d&t=w",
                _ => "p=1&s=g&t=w",
            };

            string url = $"{BaseUrlLmgtfy}/?q={HttpUtility.UrlEncode(search)}&{engine}";

            await ReplyAsync(embed: GetLmgUrlEmbed(url, search, new EmbedFooterBuilder()));
        }    

        private Embed GetLmgUrlEmbed(string url, string search, EmbedFooterBuilder footer) => new EmbedBuilder()
            .WithColor(0xffffff)
            .AddField("Link:", $"" +
            $"► [**Click Here to Learn More About \"{search}\"**]({url})\n")
            .WithFooter(footer)
            .Build();
    }
}