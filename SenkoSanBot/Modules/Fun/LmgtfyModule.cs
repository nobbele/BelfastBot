using Discord.Commands;
using System.Threading.Tasks;

namespace SenkoSanBot.Modules.Fun
{
    public class LmgtfyModule : SenkoSanModuleBase
    {
        public string BaseUrl = "https://lmgtfy.com";

        [Command("lmgtfy"), Alias("lmg")]
        [Summary("")]
        public async Task LmgtfySearchAsync([Summary("0 = google,\n" +
            "1 = yahoo\n" +
            "2 = bing\n" +
            "3 = ask\n" +
            "4 = aol.\n" +
            "5 = duckduckgo")]int type = 0, [Remainder] string search = "")
        {
            string engine = null;
            //"/?q=gay+gay&"
            switch (type)
            {
                case 0:
                    engine = "p=1&s=g&t=w";
                    break;
                case 1:
                    engine = "p=1&s=y&t=w";
                    break;
                case 2:
                    engine = "p=1&s=b&t=w";
                    break;
                case 3:
                    engine = "p=1&s=k&t=w";
                    break;
                case 4:
                    engine = "p=1&s=a&t=w";
                    break;
                case 5:
                    engine = "p=1&s=d&t=w";
                    break;
            }

            search = search.Replace(' ', '+');
            string url = $"<{BaseUrl}/?q={search}&{engine}>";

            await ReplyAsync(url);
        }
    }
}
