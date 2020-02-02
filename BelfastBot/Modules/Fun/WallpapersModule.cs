using Discord;
using Discord.Commands;
using BelfastBot.Services.Pagination;
using System.Threading.Tasks;

namespace BelfastBot.Modules.Fun
{
    [Summary("Commands for getting wallpapers")]
    public class WallpapersModule : BelfastModuleBase
    {
        public PaginatedMessageService PaginatedMessageService { get; set; }

        [Command("wp")]
        [RateLimit(typeof(WallpapersModule), perMinute: 45)]
        [Summary("Gets wallpaper from alphacoders")]
        public async Task SearchWallpaperAsync([Summary("Image to search")] [Remainder] string name = "Azur Lane")
        {
            Logger.LogInfo($"Searching for {name} on alphacoders");

            ulong[] ids = await AlphaCodersApi.Client.GetWallpaperIdAsync(Config.Configuration.AlphaCodersApiToken, name);
            AlphaCodersApi.WallpaperResult[] resultCache = new AlphaCodersApi.WallpaperResult[ids.Length];

            await PaginatedMessageService.SendPaginatedDataAsyncMessageAsync(Context.Channel, ids, async (ulong id, int index, EmbedFooterBuilder footer) => {
                if (resultCache[index].Id != 0)
                    return GetWallpaperResultEmbed(resultCache[index], index, footer);
                else
                {
                    AlphaCodersApi.WallpaperResult result = resultCache[index] = await AlphaCodersApi.Client.GetDetailedWallpaperResultsAsync(Config.Configuration.AlphaCodersApiToken, id);
                    return GetWallpaperResultEmbed(result, index, footer);
                }
            });
        }   

        private Embed GetWallpaperResultEmbed(AlphaCodersApi.WallpaperResult result, int index, EmbedFooterBuilder footer) => new EmbedBuilder()
            .WithColor(0x2999EF)
            .WithAuthor(author => {
                author
                    .WithName($"Wallpaper Name {(result.Name != string.Empty ? result.Name : "Unknown")}")
                    .WithUrl($"{result.PageUrl}")
                    .WithIconUrl(result.ImageThumbUrl);
            })
            .AddField("Details ▼",
            $"► Category: **{result.Category}**\n" +
            $"► Width: **{result.Width}** Height: **{result.Height}**\n" +
            $"► File Size: **{((float)result.FileSize / 1024/ 1024):F2} MB** \n")
            .WithFooter(footer)
            .WithImageUrl(result.ImageUrl)
            .Build();
    }
}