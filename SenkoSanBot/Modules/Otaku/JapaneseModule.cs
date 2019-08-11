using Discord;
using Discord.Commands;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SenkoSanBot.Modules.Otaku
{
    public class JapaneseModule : ModuleBase<SocketCommandContext>
    {
        [Command("Jisho"), Alias("jsh")]
        [Summary("Search word from jisho.org")]
        public async Task SearchWordAsync([Summary("Word to search for")] [Remainder] string word)
        {
            JishoApi.SearchResult result = await JishoApi.Client.GetWordAsync(word);

            Embed embed = new EmbedBuilder()
                .WithTitle($"Search Results For **{word}**")
                .Build();

        }

    }
}
