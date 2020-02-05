using System;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using System.Net.Http.Headers;

namespace TraceMoeApi
{
    public class Client
    {
        public static readonly Uri BaseUri = new Uri("https://trace.moe/api");

        public static async Task<TraceResult[]> GetTraceResultsAsync(string imageUrl, int count = 10)
        {
            using (HttpClient httpClient = new HttpClient())
            {
                string json = await httpClient.GetStringAsync($"{BaseUri}/search?url={imageUrl}&limit={count}");

                JObject obj = JObject.Parse(json);

                JArray jsonResults = obj["docs"] as JArray;
                TraceResult[] results = new TraceResult[jsonResults.Count];

                for (int i = 0; i < results.Length; i++) 
                    results[i] = jsonResults[i].ToObject<TraceResult>();

                return results;
            }
        }
        public static async Task<TraceResult[]> GetTraceResultsFromBase64Async(string base64, int count = 10)
        {
            using (HttpClient httpClient = new HttpClient())
            {
                HttpContent content = new StringContent($"{{\"image\": \"{base64}\"}}");
                content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                HttpResponseMessage response = await httpClient.PostAsync($"{BaseUri}/search?limit={count}", content);
                string json = await response.Content.ReadAsStringAsync();

                JObject obj = JObject.Parse(json);

                JArray jsonResults = obj["docs"] as JArray;
                TraceResult[] results = new TraceResult[jsonResults.Count];

                for (int i = 0; i < results.Length; i++)
                    results[i] = jsonResults[i].ToObject<TraceResult>();

                return results;
            }
        }
    }
}