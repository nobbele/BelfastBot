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

            string japanese = string.Join("\n", result.Japanese.Select(j => $"• {j.Key} ({j.Value})"));

            EmbedFieldBuilder fieldBuilder = new EmbedFieldBuilder()
                .WithName(japanese);

            string value = string.Empty;

            int i = 0;
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