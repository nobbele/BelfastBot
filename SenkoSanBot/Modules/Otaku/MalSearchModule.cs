using Discord;
using Discord.Commands;
using Discord.WebSocket;
using MalApi;
using SenkoSanBot.Services.Pagination;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SenkoSanBot.Modules.Otaku
{
    [Summary("Command for searching anime on mal")]
    public class MalSearchModule : SenkoSanModuleBase
    {
        public PaginatedMessageService PaginatedMessageService { get; set; }

        [Summary("Search for anime on mal")]
        [Command("mal")]
        public async Task SearchAnimeAsync([Summary("Title to search")] string name, int limit = 5)
        {
            Logger.LogInfo($"Searching for {name} on myanimelist");

            MalApi.SearchResult[] results = await Client.SearchAnimeAsync(name, limit);

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
                    .WithColor(0x53DF1D)
                    .WithTitle($"**{result.Title}**")
                    .AddField("Results", result.Title)
                    .WithFooter(footer)
                    .WithThumbnailUrl(result.ImageUrl)
                    .Build();
        }
    }
}
