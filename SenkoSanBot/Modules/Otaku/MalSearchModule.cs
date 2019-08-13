using Discord;
using Discord.Commands;
using MalApi;
using SenkoSanBot.Services.Pagination;
using System.Threading.Tasks;

namespace SenkoSanBot.Modules.Otaku
{
    [Summary("Command for searching anime on mal")]
    public class MalSearchModule : SenkoSanModuleBase
    {
        public PaginatedMessageService PaginatedMessageService { get; set; }

        [Command("mal")]
        [Summary("Search for anime on myanimelist")]
        public async Task SearchAnimeAsync([Summary("Title to search")] [Remainder]string name  )
        {
            Logger.LogInfo($"Searching for {name} on myanimelist");

            MalApi.SearchResult[] results = await Client.SearchAnimeAsync(name);

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
                    .WithTitle($"**{result.Title}**")
                    .WithDescription(result.Synopsis)
                    .AddField("Title", $"**[{result.Title}]({result.AnimeUrl})**")
                    .AddField("Type", result.Type, true)
                    .AddField("Episodes", result.Episodes, true)
                    .AddField("Score", result.Score)
                    .WithFooter(footer)
                    .WithImageUrl(result.ImageUrl)
                    .Build();
        }
    }
}
