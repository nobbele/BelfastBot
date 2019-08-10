using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace NekosLifeApi
{
    public static class Client
    {
        public static readonly Uri BaseUri = new Uri("https://api.nekos.dev/api/v3/images/");

        public static readonly Uri SfwImage = BaseUri.Append("sfw/img");
        public static readonly Uri SfwGif = BaseUri.Append("sfw/gif");

        public static async Task<string[]> GetSfwImageCategoriesAsync()
        {
            using (HttpClient httpClient = new HttpClient())
            {
                string json = await httpClient.GetStringAsync(SfwImage);

                dynamic obj = JObject.Parse(json);

                // Shinobu tag is broken for whatever reason
                return ((string[])obj.data.response.categories.ToObject<string[]>()).Where(s => s != "shinobu").ToArray();
            }
        }

        public static async Task<string[]> GetSfwGifCategoriesAsync()
        {
            using (HttpClient httpClient = new HttpClient())
            {
                string json = await httpClient.GetStringAsync(SfwGif);

                dynamic obj = JObject.Parse(json);

                return obj.data.response.categories.ToObject<string[]>();
            }
        }

        public static async Task<string> GetSfwImageAsync(string category)
        {
            using (HttpClient httpClient = new HttpClient())
            {
                string json = await httpClient.GetStringAsync(SfwImage.Append(category));

                dynamic obj = JObject.Parse(json);

                return obj.data.response.url;
            }
        }

        public static async Task<string> GetSfwGifAsync(string category)
        {
            using (HttpClient httpClient = new HttpClient())
            {
                string json = await httpClient.GetStringAsync(SfwGif.Append(category));

                dynamic obj = JObject.Parse(json);

                return obj.data.response.url;
            }
        }
    }
}
