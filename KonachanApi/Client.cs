using Common;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace KonachanApi
{
    public static class Client
    {
        public static readonly string BaseUrl = "https://konachan.net/post.xml";
        public static readonly string[] BlacklistedTags = new string[]
        {

        };

        public static async Task<string> GetRandomPostAsync(string tag)
        {
            if (BlacklistedTags.Contains(tag))
                return null;

            using (HttpClient client = new HttpClient())
            {
                Stream fileStream = await client.GetStreamAsync($"{BaseUrl}?tags={tag}");
                XDocument xdoc = await XDocument.LoadAsync(fileStream, LoadOptions.None, CancellationToken.None);
                XElement root = xdoc.Element("posts");
                var posts = root.Elements("post");
                XElement post;
                string[] tags;

                int tries = 0;
                do
                {
                    if (tries > 25)
                    {
                        post = null;
                        break;
                    }
                    post = posts.Random();
                    tags = post.Attribute("tags").Value.Split(' ');
                    tries++;
                } while (BlacklistedTags.Any(tags.Contains));

                return post?.Attribute("file_url")?.Value;
            }
        }
    }
}