using System.IO;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace SafeBooruApi
{
    public static class Client
    {
        public static readonly string BaseUrl = "https://safebooru.org/index.php?page=dapi&s=post&q=index&tags=";

        public static async Task<string> GetRandomPostAsync(string tag)
        {
            using (HttpClient client = new HttpClient())
            {
                Stream fileStream = await client.GetStreamAsync($"{BaseUrl}&tags={tag}");
                XDocument xdoc = await XDocument.LoadAsync(fileStream, LoadOptions.None, CancellationToken.None);
                XElement root = xdoc.Element("posts");
                var posts = root.Elements("post");
                XElement post = posts.Random();

                return post.Attribute("file_url").Value;
            }
        }
    }
}