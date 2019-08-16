using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace SafeBooruApi
{
    public static class Client
    {
        public static readonly string BaseUrl = "https://safebooru.org/index.php?page=dapi&s=post&q=index&tags=";
        public static readonly string[] BlacklistedTags = new string[]
        {
            "ass",
            "nude",
            "anus",
            "uncensored",
            "penis",
            "pussy",
            "panties",
            "underwear",
            "pantyshot",
            "skirt_lift",
        };

        public static async Task<string> GetRandomPostAsync(string tag)
        {
            if (BlacklistedTags.Contains(tag))
                return null;

            using (HttpClient client = new HttpClient())
            {
                Stream fileStream = await client.GetStreamAsync($"{BaseUrl}&tags={tag}");
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