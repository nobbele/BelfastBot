using System;
using System.Linq;
using System.Threading.Tasks;
using GraphQL.Client;
using GraphQL.Common.Request;
using GraphQL.Common.Response;

namespace AnimeApi
{
    public static class AnilistClient
    {
        public static readonly string BaseUrl = "https://graphql.anilist.co";
        public static readonly GraphQLClient Client = new GraphQLClient(BaseUrl);

        public static readonly string Query = @"
            query GetAnimeByName($name: String) {
                Media(search: $name, type: ANIME) {
                    id
                    idMal
                    title {
                        english
                        romaji
                        native
                    }
                    status
                    description
                    type
                    episodes
                    averageScore
                    coverImage {
                        large
                    }
                    siteUrl
                    source
                    duration
                    airingSchedule(notYetAired: true, page: 1, perPage: 1) {
                        nodes {
                            airingAt
                        }
                    }
                    trailer {
                        id
                        site
                    }
                    studios(isMain: true) {
                        nodes {
                            name
                            siteUrl
                        }
                    }
                }
            }
            query GetMangaByName($name: String) {
              Media(search: $name, type: MANGA) {
                  id
                  idMal
                  title {
                    english
                    romaji
                    native
                  }
                  format
                  status
                  description
                  type
                  chapters
                  volumes
                  averageScore
                  coverImage {
                    large
                  }
                  siteUrl
                  source
                  staff {
                    nodes {
                      name {
                        full
                        native
                      }
                      siteUrl
                    }
                  }
                }
            }
        ";

        public static async Task<AnimeResult> GetAnimeAsync(string name)
        {
            GraphQLResponse response = await Client.PostAsync(new GraphQLRequest()
            {
                Query = Query,
                OperationName = "GetAnimeByName",
                Variables = new {
                    name = name,
                }
            });

            dynamic data = response.Data.Media;

            return new AnimeResult()
            {
                MalId = data.idMal,
                AlId = data.id,
                Title = data.title.english ?? data.title.romaji ?? data.title.native,
                Status = data.status,
                Synopsis = data.description,
                Type = data.type,
                Episodes = data.episodes,
                Score = (float)data.averageScore,
                ImageUrl = data.coverImage.large,
                SiteUrl = data.siteUrl,
                Source = data.source,
                Duration = data.duration,
                Broadcast = data.airingSchedule.nodes.Count > 0
                    ? DateTimeOffset.FromUnixTimeSeconds((long)(data.airingSchedule.nodes[0].airingAt)).UtcDateTime.ToString("dddd, dd MMMM HH:mm (UTC)") 
                    : null,
                TrailerUrl = data.trailer.site == "youtube" ? $"https://www.youtube.com/watch?v={data.trailer.id}" : $"https://www.dailymotion.com/video/{data.trailer.id}",
                Studio = data.studios.nodes.Count > 0 ? data.studios.nodes[0].name : null,
                StudioUrl = data.studios.nodes.Count > 0 ? data.studios.nodes[0].siteUrl : null,
                ApiType = ApiType.AniList,
            };
        }

        public static async Task<MangaResult> GetMangaAsync(string name)
        {
            GraphQLResponse response = await Client.PostAsync(new GraphQLRequest()
            {
                Query = Query,
                OperationName = "GetMangaByName",
                Variables = new
                {
                    name = name,
                }
            });

            dynamic data = response.Data.Media;
            dynamic[] staff = (dynamic[])data.staff.nodes.ToObject<dynamic[]>();

            return new MangaResult()
            {
                MalId = data.idMal,
                AlId = data.id,
                Title = data.title.english ?? data.title.romaji ?? data.title.native,
                Status = data.status,
                Synopsis = data.description,
                Type = data.type,
                Chapters = data.chapters,
                Volumes = data.volume,
                Score = (float)data.averageScore,
                ImageUrl = data.coverImage.large,
                SiteUrl = data.siteUrl,
                Source = data.source,
                Staff = staff.Select(author => new Staff()
                {
                    Name = (string)author.name.full.ToObject<string>(),
                    Url = (string)author.siteUrl.ToObject<string>()
                }).ToArray(),
                ApiType = ApiType.AniList,
            };
        }
    }
}