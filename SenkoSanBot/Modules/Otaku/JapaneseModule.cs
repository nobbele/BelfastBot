using Discord;
using Discord.Commands;
using JishoApi;
using SenkoSanBot.Services.Pagination;
using System;
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

            await PaginatedMessageService.SendPaginatedDataMessageAsync(
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
        public async Task SearchAnimeAsync([Summary("Title to search")] string name, int limit = 5)
        {
            Logger.LogInfo($"Searching for {name} on myanimelist");

            ulong[] ids = await MalApi.Client.GetAnimeIdAsync(name, limit);
            MalApi.AnimeResult[] resultCache = new MalApi.AnimeResult[ids.Length];

            await PaginatedMessageService.SendPaginatedDataAsyncMessageAsync(Context.Channel, ids, async (ulong id, int index, EmbedFooterBuilder footer) => {
                if (resultCache[index].Id != 0)
                    return GetAnimeResultEmbed(resultCache[index], index, footer);
                else
                {
                    MalApi.AnimeResult result = resultCache[index] = await MalApi.Client.GetDetailedAnimeResultsAsync(id);
                    return GetAnimeResultEmbed(result, index, footer);
                }
            });
        }

        [Command("studio")]
        public async Task SearchStudioAsync([Summary("Title to search")] int id = 43)
        {
            //Logger.LogInfo($"Searching for studio with {id} on myanimelist");
            MalApi.StudioResult result = await MalApi.Client.GetStudioResultsAsync(id);
            await ReplyAsync(result.Name);
        }

        private Embed GetAnimeResultEmbed(MalApi.AnimeResult result, int index, EmbedFooterBuilder footer) => new EmbedBuilder()
                .WithColor(0x2E51A2)
                .WithAuthor(author => {
                    author
                        .WithName($"{result.Title}")
                        .WithUrl($"{result.AnimeUrl}")
                        .WithIconUrl("https://image.myanimelist.net/ui/OK6W_koKDTOqqqLDbIoPAiC8a86sHufn_jOI-JGtoCQ");
                })
                .WithDescription(result.Synopsis.ShortenText())
                .AddField("Details", $"► Type: **{result.Type}** | Status: **{GetAiringType(result.Airing)}**\n" +
                $"► Episodes: **{result.Episodes}** | Duration: **{result.Duration}**" +
                $"\n► [**Trailer**]({result.TrailerUrl}) | Studio: **[{result.Studio}]({result.StudioUrl})**" +
                $"\n[Broadcast Time: {result.Broadcast}]")
                .WithFooter(footer)
                .WithImageUrl(result.ImageUrl)
                .Build();

        private string GetAiringType(bool state)
        {
            switch (state)
            {
                case false:
                    return "Finished";
                case true:
                    return "Airing";
            }
            return "Unknown";
        }
    }

    public static class StringExtentionMethods
    {
        public static string ShortenText(this string text)
        {
            if(text.Length > 256)
                text = text?.Substring(0, 256) + "...";

            return text;
        }
    }
}