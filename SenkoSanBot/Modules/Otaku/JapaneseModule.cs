using Discord;
using Discord.Commands;
using JishoApi;
using System.Linq;
using System.Threading.Tasks;

namespace SenkoSanBot.Modules.Otaku
{
    [Summary("Commands for japanese translation")]
    public class JapaneseModule : SenkoSanModuleBase
    {
        [Command("Jisho"), Alias("jsh")]
        [Summary("Searches given word from jisho.org")]
        public async Task SearchWordAsync([Summary("Word to search for")] [Remainder] string word)
        {
            Logger.LogInfo($"Searching for {word} on jisho");

            JishoApi.SearchResult result = await Client.GetWordAsync(word);

            if(result.Word == "None")
            {
                Embed noneEmbed = new EmbedBuilder()
                    .WithColor(0xff0000)
                    .WithTitle("No results found")
                    .Build();

                await ReplyAsync(embed: noneEmbed);
                return;
            }

            string japanese = string.Join("\n", result.Japanese.Select(j => $"• {j.Key} ({j.Value})"));

            EmbedFieldBuilder fieldBuilder = new EmbedFieldBuilder()
                .WithName(japanese);

            string value = string.Empty;

            int i = 1;
            foreach(EnglishDefinition def in result.English)
            {
                string meaning = string.Join(", ", def.English);
                string info = string.Join(", ", def.Info);

                //Don't display anything if info doesnt exist
                string infoDisplay = string.IsNullOrEmpty(info) 
                                     ? string.Empty 
                                     : $"({info})";

                value += $"{i}. {meaning} {infoDisplay}\n";

                i++;
            }

            if (string.IsNullOrEmpty(value))
                value = "Nothing found";

            fieldBuilder.WithValue(value);

            Embed embed = new EmbedBuilder()
                .WithColor(0x53DF1D)
                .WithTitle($"First Search Result For **{word}**")
                .AddField(fieldBuilder)
                .WithDescription($"\n Link: https://jisho.org/search/{word}")
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