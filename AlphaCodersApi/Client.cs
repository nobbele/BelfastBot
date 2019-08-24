using Newtonsoft.Json.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace AlphaCodersApi
{
    public class Client
    {
        public static readonly string BaseUrl = "https://wall.alphacoders.com/api2.0/get.php";

        public static async Task<ulong[]> GetWallpaperIdAsync(string token, string search, int pageIndex = 1)
        {
            using (HttpClient httpClient = new HttpClient())
            {   
                string json = await httpClient.GetStringAsync($"{BaseUrl}?auth={token}&method=search&term={search}&page={pageIndex}");

                dynamic obj = JObject.Parse(json);

                dynamic[] jsonResults = (dynamic[])obj.wallpapers.ToObject<dynamic[]>();

                ulong[] results = new ulong[jsonResults.Length];
                int i = 0;
                foreach (dynamic jsonResult in jsonResults)
                {
                    results[i] = jsonResult.id;
                    i++;
                }
                return results;
            }
        }

        public static async Task<WallpaperResult> GetDetailedWallpaperResultsAsync(string token, ulong id)
        {
            using (HttpClient httpClient = new HttpClient())
            {
                string json = await httpClient.GetStringAsync($"{BaseUrl}?auth={token}&method=wallpaper_info&id={id}"); //&width=1920&height=1080");

                dynamic obj = JObject.Parse(json);

                dynamic jsonResult = (dynamic)obj.wallpaper.ToObject<dynamic>();

                return new WallpaperResult
                {
                    Id = jsonResult.id.ToObject<ulong>(),
                    Name = jsonResult.name,
                    Category = jsonResult.category,
                    Width = jsonResult.width,
                    Height = jsonResult.height,
                    FileSize = jsonResult.file_size.ToObject<ulong>(),
                    PageUrl = jsonResult.url_page,
                    ImageUrl = jsonResult.url_image,
                    ImageThumbUrl = jsonResult.url_thumb,
                };
            }
        }
    }
}