using Discord;
using Discord.Commands;
using JishoApi;
using SenkoSanBot.Services.Pagination;
using System.Linq;
using System.Threading.Tasks;

namespace SenkoSanBot.Modules.Otaku
{
    [Summary("Commands for japanese translation")]
    public class JapaneseModule : SenkoSanModuleBase
    {
        public PaginatedMessageService PaginatedMessageService { get; set; }

        [Command("Jisho"), Alias("jsh")]
        [Summary("Searches given word from jisho.org")]
        public async Task SearchWordAsync([Summary("Word to search for")] [Remainder] string searchWord)
        {
            Logger.LogInfo($"Searching for {searchWord} on jisho");

            JishoApi.SearchResult[] results = await Client.GetWordAsync(searchWord);

            await PaginatedMessageService.SendPaginatedDataMessage(
                Context.Channel, 
                results, 
                (result, index, footer) => GenerateEmbedFor(result, searchWord, footer)
            );
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

                value += $"{i}. {meaning} {infoDisplay}\n";

                i++;
            }

            value = "Nothing found".IfTargetNullOrEmpty(value);

            fieldBuilder.WithValue(value);

            return new EmbedBuilder()
                .WithColor(0x53DF1D)
                .WithTitle($"Search Result For **{searchWord}**")
                .AddField(fieldBuilder)
                .WithDescription($"\n Link: https://jisho.org/search/{searchWord}")
                .WithFooter(footer)
                .WithThumbnailUrl("https://cdn.discordapp.com/attachments/303528930634235904/610152248265408512/LpCOJrnh6weuEKishpfZCw2YY82J4GRiTjbqmdkgqCVCpqlBM4yLyAAS-qLpZvbcCcg.png")
                .Build();
        }
    }
}