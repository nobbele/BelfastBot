using Discord;
using Discord.Commands;
using JishoApi;
using SenkoSanBot.Services.Pagination;
using System.Linq;
using System.Threading.Tasks;

namespace SenkoSanBot.Modules.Otaku
{
    [Summary("Commands for Japan related stuff")]
    public class JapaneseModule : SenkoSanModuleBase
    {
        public PaginatedMessageService PaginatedMessageService { get; set; }

        [Command("Jisho"), Alias("jsh")]
        [Summary("Searches given word from jisho.org")]
        public async Task SearchWordAsync([Summary("Word to search for")] [Remainder] string searchWord)
        {
            Logger.LogInfo($"Searching for {searchWord} on jisho");

            JishoApi.SearchResult[] results = await JishoApi.Client.GetWordAsync(searchWord);

            await PaginatedMessageService.SendPaginatedDataMessage(
                Context.Channel, 
                results, 
                (result, index, footer) => GenerateEmbedFor(result, searchWord, footer)
            );
        }

        private Embed GenerateEmbedFor(JishoApi.SearchResult result, string searchWord, EmbedFooterBuilder footer)
        {
            string japanese = result.Japanese.Select(j => $"• {j.Key} ({j.Value})").NewLineSeperatedString();
            string url = searchWord.Replace(" ", "%20");

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
                .WithAuthor(author => {
                    author
                        .WithName($"Results For {searchWord}")
                        .WithUrl($"https://jisho.org/search/{url}")
                        .WithIconUrl("https://cdn.discordapp.com/attachments/303528930634235904/610152248265408512/LpCOJrnh6weuEKishpfZCw2YY82J4GRiTjbqmdkgqCVCpqlBM4yLyAAS-qLpZvbcCcg.png");
                })
                .AddField(fieldBuilder)
                .WithFooter(footer)
                .WithThumbnailUrl("https://cdn.discordapp.com/attachments/303528930634235904/610152248265408512/LpCOJrnh6weuEKishpfZCw2YY82J4GRiTjbqmdkgqCVCpqlBM4yLyAAS-qLpZvbcCcg.png")
                .Build();
        }

        //Mal Module
        [Command("mal")]
        [Summary("Search for anime on myanimelist")]
        public async Task SearchAnimeAsync([Summary("Title to search")] [Remainder]string name)
        {
            Logger.LogInfo($"Searching for {name} on myanimelist");

            MalApi.SearchResult[] results = await MalApi.Client.SearchAnimeAsync(name);

            Embed GetEmbed(int i) => GenerateEmbedFor(results[i], new EmbedFooterBuilder()
                .WithText($"page {i + 1} out of {results.Length}"));

            IUserMessage message = await ReplyAsync(embed: GetEmbed(0));

            await message.AddReactionsAsync(PaginatedMessageService.ReactionEmotes);

            PaginatedMessageService.AddCallback(message.Id, results.Length, async (IUserMessage msg, int i) =>
            {
                await msg.ModifyAsync((MessageProperties properties) =>
                {
                    properties.Embed = Optional.Create(GetEmbed(i));
                });
            });
        }

        private Embed GenerateEmbedFor(MalApi.SearchResult result, EmbedFooterBuilder footer)
        {
            return new EmbedBuilder()
                    .WithColor(0x2E51A2)
                    .WithAuthor(author => {
                        author
                            .WithName($"{result.Title}")
                            .WithUrl($"{result.AnimeUrl}")
                            .WithIconUrl("https://image.myanimelist.net/ui/OK6W_koKDTOqqqLDbIoPAiC8a86sHufn_jOI-JGtoCQ");
                    })
                    .WithDescription(result.Synopsis)
                    .AddField("Type", result.Type, true)
                    .AddField("Episodes", result.Episodes, true)
                    .AddField("Score", result.Score, true)
                    .WithFooter(footer)
                    .WithImageUrl(result.ImageUrl)
                    .Build();
        }
    }
}