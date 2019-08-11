using Discord;
using Discord.Commands;
using JishoApi;
using System.Linq;
using System.Threading.Tasks;

namespace SenkoSanBot.Modules.Otaku
{
    public class JapaneseModule : ModuleBase<SocketCommandContext>
    {
        [Command("Jisho"), Alias("jsh")]
        [Summary("Search word from jisho.org")]
        public async Task SearchWordAsync([Summary("Word to search for")] [Remainder] string word)
        {
            JishoApi.SearchResult result = await Client.GetWordAsync(word);

            string japanese = string.Join(", ", result.Japanese.Select(j => $"{j.Key} ({j.Value})"));

            EmbedFieldBuilder fieldBuilder = new EmbedFieldBuilder()
                .WithName(japanese);

            int i = 0;
            foreach(EnglishDefinition def in result.English)
            {
                string meaning = string.Join(", ", def.English);
                string info = string.Join(", ", def.Info);

                //Don't display anything if info doesnt exist
                string infoDisplay = string.IsNullOrEmpty("info") 
                                     ? string.Empty 
                                     : $"({info})";

                string display = $"{i}. {meaning} {infoDisplay}";

                fieldBuilder.WithValue(display);

                i++;
            }

            Embed embed = new EmbedBuilder()
                .WithColor(0x53DF1D)
                .WithTitle($"First Search Result For **{word}**")
                .AddField(fieldBuilder)
                .Build();

            await ReplyAsync(embed: embed);
        }

    }
}