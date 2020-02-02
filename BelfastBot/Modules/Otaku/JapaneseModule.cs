using System.Linq;
using System.Threading.Tasks;
using System.Web;
using BelfastBot.Services.Pagination;
using Common;
using Discord;
using Discord.Commands;
using JishoApi;

namespace BelfastBot.Modules.Otaku
{
    [Summary("Command for the japanese language")]
    public class JapaneseModule : BelfastModuleBase
    {
        public PaginatedMessageService PaginatedMessageService { get; set; }

        [Command("jisho"), Alias("jsh")]
        [RateLimit(typeof(JapaneseModule), perMinute: 45)]
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
    }
}