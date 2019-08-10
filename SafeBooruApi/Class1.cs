using System;
using System.IO;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace SafeBooruApi
{
    public class Class1
    {
        public static readonly string BaseUrl = "https://safebooru.org/index.php?page=dapi&s=post&q=index&tags=";

        public async Task GetBooruImage(string tag)
        {

            using (HttpClient client = new HttpClient())
            {
                Stream fileStream = await client.GetStreamAsync($"{BaseUrl}&tags={tag}");
                XDocument xdoc = await XDocument.LoadAsync(fileStream, LoadOptions.None, CancellationToken.None);
                
            }


        }

    }
}
