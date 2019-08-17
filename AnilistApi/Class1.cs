using System;
using System.Threading.Tasks;
using GraphQL;
using GraphQL.Types;

namespace AnilistApi
{
    public static class Class1
    {
        public static readonly Uri BaseUrl = new Uri("https://graphql.anilist.co");

        public static async Task GetAnilistAsync()
        {



            var schema = Schema.For(@"
              type Query {
                hello: String
              }
            ");

            var root = new { Hello = "Hello World!" };
            var json = schema.Execute(_ =>
            {
                _.Query = "{ hello }";
                _.Root = root;
            });

            Console.WriteLine(json);
        }
    }
}
