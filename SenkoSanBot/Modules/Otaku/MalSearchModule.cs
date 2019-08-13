using Discord;
using Discord.Commands;
using MalApi;
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
        [Command("mal")]
        public async Task SearchAnimeAsync([Summary("Title to search")] string name, int limit = 5)
        {
            Logger.LogInfo($"Searching for {name} on myanimelist");

            MalApi.SearchResult[] results = await Client.SearchAnimeAsync(name, limit);

            foreach (MalApi.SearchResult result in results)
            {

                if (result.Title == "None")
                {
                    Embed noneEmbed = new EmbedBuilder()
                        .WithColor(0xff0000)
                        .WithTitle("No results found")
                        .Build();

                    await ReplyAsync(embed: noneEmbed);
                    return;
                }

                Embed embed = new EmbedBuilder()
                    .WithColor(0x53DF1D)
                    .WithTitle($"{results.Length} Results For **{name}**")
                    .AddField("Results", result.Title)
                    .WithFooter(footer =>
                    {
                        footer
                            .WithText($"Requested by {Context.User}")
                            .WithIconUrl(Context.User.GetAvatarUrl());
                    })
                    .WithThumbnailUrl(result.ImageUrl)
                    .Build();

                await ReplyAsync(embed: embed);
            }
        }

    }
}
