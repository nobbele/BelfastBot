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
        public async Task SearchAnimeAsync([Summary("Title to search")] [Remainder] string name, int limit = 5)
        {
            Logger.LogInfo($"Searching for {name} on myanimelist");

            MalApi.SearchResult result = await Client.SearchAnimeAsync(name, limit);

            if(result.Title == "None")
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
                .WithTitle($"{limit} Results For **{name}**")
                .AddField("Results", result.Title)
                .WithFooter(footer => {
                    footer
                        .WithText($"Requested by {Context.User}")
                        .WithIconUrl(Context.User.GetAvatarUrl());
                })
                .WithThumbnailUrl("https://cdn.discordapp.com/attachments/303528930634235904/610152248265408512/LpCOJrnh6weuEKishpfZCw2YY82J4GRiTjbqmdkgqCVCpqlBM4yLyAAS-qLpZvbcCcg.png")
                .Build();

            await ReplyAsync(embed: embed);
        }

    }
}
