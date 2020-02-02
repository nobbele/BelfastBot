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

        #region Commands
        [Command("lmgtfy"), Alias("lmg")]
        [Summary("Creates lmgtfy url")]
        public async Task LmgtfySearchAsync([Summary("" +
            "► 0 = google\n" +
            "► 1 = yahoo\n" +
            "► 2 = bing\n" +
            "► 3 = ask\n" +
            "► 4 = aol.\n" +
            "► 5 = duckduckgo")]int type = 0, [Remainder] string search = "")
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

            search = search.Replace(' ', '+');
            string url = $"{BaseUrlLmgtfy}/?q={search}&{engine}";

            await ReplyAsync(embed: GetLmgUrlEmbed(url, search, new EmbedFooterBuilder()));
        }

        [Command("jisho"), Alias("jsh")]
        [Summary("Searches given word from jisho.org")]
        public async Task SearchWordAsync([Summary("Word to search for")] [Remainder] string searchWord)
        {
            Logger.LogInfo($"Searching for {searchWord} on jisho");

            JishoApi.SearchResult[] results = await Client.GetWordAsync(searchWord);

            if (results.Length > 0)
                await PaginatedMessageService.SendPaginatedDataMessageAsync(
                    Context.Channel,
                    results,
                    (result, index, footer) => GenerateEmbedFor(result, searchWord, footer)
                );
            else
                await ReplyAsync("No result found");
        }
        #endregion

        #region Embed
        private Embed GenerateEmbedFor(JishoApi.SearchResult result, string searchWord, EmbedFooterBuilder footer)
        {
            string japanese = result.Japanese.Select(j => $"• {j.Key} ({j.Value})").NewLineSeperatedString();

            EmbedFieldBuilder fieldBuilder = new EmbedFieldBuilder()
                .WithName(japanese);

            string value = string.Empty;

            int i = 1;
            foreach (EnglishDefinition def in result.English)
            {
                string meaning = def.English.CommaSeperatedString();
                string info = def.Info.CommaSeperatedString();

                string infoDisplay = $"({info})".NothingIfCheckNullOrEmpty(info);

                value += $"{i}. **{meaning} {infoDisplay}**\n";

                i++;
            }

            value = "Nothing found".IfTargetIsNullOrEmpty(value);

            fieldBuilder.WithValue(value);

            return new EmbedBuilder()
                .WithColor(0x53DF1D)
                .WithAuthor(author => {
                    author
                        .WithName($"Results For {searchWord}")
                        .WithUrl($"https://jisho.org/search/{HttpUtility.UrlEncode(searchWord)}")
                        .WithIconUrl("https://cdn.discordapp.com/attachments/303528930634235904/610152248265408512/LpCOJrnh6weuEKishpfZCw2YY82J4GRiTjbqmdkgqCVCpqlBM4yLyAAS-qLpZvbcCcg.png");
                })
                .AddField(fieldBuilder)
                .WithFooter(footer)
                .WithThumbnailUrl("https://cdn.discordapp.com/attachments/303528930634235904/610152248265408512/LpCOJrnh6weuEKishpfZCw2YY82J4GRiTjbqmdkgqCVCpqlBM4yLyAAS-qLpZvbcCcg.png")
                .Build();
        }
        private Embed GetLmgUrlEmbed(string url, string search, EmbedFooterBuilder footer) => new EmbedBuilder()
            .WithColor(0xffffff)
            .AddField("Link:", $"" +
            $"► [**Click Here to Learn More About \"{search}\"**]({url})\n")
            .WithFooter(footer)
            .Build();
        #endregion
    }
}