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

        [Command("jisho"), Alias("jsh")]
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

            value = "Nothing found".IfTargetIsNullOrEmpty(value);

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
        [Command("mal anime"), Alias("mala")]
        [Summary("Search for anime on myanimelist")]
        public async Task SearchAnimeAsync([Summary("Title to search")] [Remainder]string name)
        {
            Logger.LogInfo($"Searching for {name} on myanimelist");

            ulong[] ids = await MalApi.Client.GetAnimeIdAsync(name);
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

        [Command("mal manga"), Alias("malm")]
        public async Task SearchMangaAsync([Summary("Title to search")] [Remainder]string name)
        {
            Logger.LogInfo($"Searching for {name} on myanimelist");

            ulong[] ids = await MalApi.Client.GetMangaIdAsync(name);
            MalApi.MangaResult[] resultCache = new MalApi.MangaResult[ids.Length];

            await PaginatedMessageService.SendPaginatedDataAsyncMessageAsync(Context.Channel, ids, async (ulong id, int index, EmbedFooterBuilder footer) => {
                if (resultCache[index].Id != 0)
                    return GetMangaResultEmbed(resultCache[index], index, footer);
                else
                {
                    MalApi.MangaResult result = resultCache[index] = await MalApi.Client.GetDetailedMangaResultsAsync(id);
                    return GetMangaResultEmbed(result, index, footer);
                }
            });
        }

        private Embed GetMangaResultEmbed(MalApi.MangaResult result, int index, EmbedFooterBuilder footer) => new EmbedBuilder()
            .WithColor(0x2E51A2)
            .WithAuthor(author => {
                author
                    .WithName($"{result.Title}")
                    .WithUrl($"{result.MangaUrl}")
                    .WithIconUrl("https://image.myanimelist.net/ui/OK6W_koKDTOqqqLDbIoPAiC8a86sHufn_jOI-JGtoCQ");
            })
            .WithDescription(result.Synopsis.ShortenText())
            .AddField("Details",
            $"► Type: **{result.Type}**\n" +
            $"► Status: **{result.Status}**\n" +
            $"► Chapters: **{(result.Chapters != null ? result.Chapters.ToString() : "Unknown")} [Volumes: {result.Volumes}]** \n" +
            $"► Score: **{(result.Score != null ? result.Score.ToString() : "Unknown")}**\n" +
            $"► Author: **[{(result.Author != null ? result.Author : "Unknown")}]({result.AuthorUrl})**\n")
            .WithFooter(footer)
            .WithImageUrl(result.ImageUrl)
            .Build();

        private Embed GetAnimeResultEmbed(MalApi.AnimeResult result, int index, EmbedFooterBuilder footer) => new EmbedBuilder()
            .WithColor(0x2E51A2)
            .WithAuthor(author => {
                author
                    .WithName($"{result.Title}")
                    .WithUrl($"{result.AnimeUrl}")
                    .WithIconUrl("https://image.myanimelist.net/ui/OK6W_koKDTOqqqLDbIoPAiC8a86sHufn_jOI-JGtoCQ");
            })
            .WithDescription(result.Synopsis.ShortenText())
            .AddField("Details", 
            $"► Type: **{result.Type}** [Source: **{result.Source}**] \n" +
            $"► Status: **{result.Status}**\n" +
            $"► Episodes: **{(result.Episodes != null ? result.Episodes.ToString() : "Unknown")} [{result.Duration}]** \n" +
            $"► Score: **{(result.Score != null ? result.Score.ToString() : "Unknown")}**\n" +
            $"► Studio: **[{(result.Studio != null ? result.Studio : "Unknown")}]({result.StudioUrl})**\n" +
            $"Broadcast Time: **[{(result.Broadcast != null ? result.Broadcast.ToString() : "Unknown")}]**\n" +
            $"**{(result.TrailerUrl != null ? $"[Trailer]({result.TrailerUrl})" : "No trailer")}**\n")
            .WithFooter(footer)
            .WithImageUrl(result.ImageUrl)
            .Build();
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