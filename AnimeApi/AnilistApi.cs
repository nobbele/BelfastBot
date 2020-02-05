using System;
using System.Linq;
using System.Threading.Tasks;
using Common;
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
            query GetAnime($name: String, $id: Int) {
                Media(search: $name, id: $id, type: ANIME) {
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
            query GetUserByName($name: String) {
              User(search: $name) {
                id
                name
                siteUrl
                avatar {
                  large
                }
                bannerImage
                favourites {
                  anime {
                    nodes {
                      title {
                        userPreferred
                      }
                      siteUrl
                    }
                  }
                  manga {
                    nodes {
                      title {
                        userPreferred
                      }
                      siteUrl
                    }
                  }
                  characters {
                    nodes {
                      name {
                        full
                        native
                      }
                      siteUrl
                      image {
                        large
                      }
                    }
                  }
                }
                statistics {
                  anime {
                    count
                    episodesWatched
                    meanScore
                  }
                  manga {
                    count
                    chaptersRead
                    meanScore
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
                OperationName = "GetAnime",
                Variables = new
                {
                    name = name,
                }
            });

            return GetAnimeResultFromResponse(response);
        }

        public static async Task<AnimeResult> GetAnimeAsync(long id)
        {
            GraphQLResponse response = await Client.PostAsync(new GraphQLRequest()
            {
                Query = Query,
                OperationName = "GetAnime",
                Variables = new
                {
                    id = id,
                }
            });

            return GetAnimeResultFromResponse(response);
        }

        public static AnimeResult GetAnimeResultFromResponse(GraphQLResponse response)
        {
            dynamic data = response.Data.Media;

            return new AnimeResult()
            {
                MalId = data.idMal,
                AlId = data.id,
                Title = (data.title.native.ToString() as string).IfTargetIsNullOrEmpty((data.title.romaji.ToString() as string).IfTargetIsNullOrEmpty(data.title.english.ToString() as string)),
                Status = data.status,
                Synopsis = data.description,
                Type = data.type,
                Episodes = data.episodes,
                Score = data.averageScore,
                ImageUrl = data.coverImage.large,
                SiteUrl = data.siteUrl,
                Source = data.source,
                Duration = data.duration,
                Broadcast = data.airingSchedule.nodes.Count > 0
                    ? DateTimeOffset.FromUnixTimeSeconds((long)(data.airingSchedule.nodes[0].airingAt)).UtcDateTime.ToString("dddd, dd MMMM HH:mm (UTC)")
                    : null,
                TrailerUrl = data.trailer != null ? (data.trailer.site == "youtube" ? $"https://www.youtube.com/watch?v={data.trailer.id}" : $"https://www.dailymotion.com/video/{data.trailer.id}") : null,
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
                Title = (data.title.native.ToString() as string).IfTargetIsNullOrEmpty((data.title.romaji.ToString() as string).IfTargetIsNullOrEmpty(data.title.english.ToString() as string)),
                Status = data.status,
                Synopsis = data.description,
                Type = data.type,
                Chapters = data.chapters,
                Volumes = data.volumes,
                Score = data.averageScore,
                ImageUrl = data.coverImage.large,
                SiteUrl = data.siteUrl,
                Source = data.source,
                Staff = staff.Select(author => new Staff()
                {
                    Name = (string)author.name.full.ToObject<string>(),
                    SiteUrl = (string)author.siteUrl.ToObject<string>()
                }).ToArray(),
                ApiType = ApiType.AniList,
            };
        }

        public static async Task<UserResult?> GetUserAsync(string name)
        {
            GraphQLResponse response = await Client.PostAsync(new GraphQLRequest()
            {
                Query = Query,
                OperationName = "GetUserByName",
                Variables = new
                {
                    name = name,
                }
            });

            dynamic data = response.Data.User;

            if (data == null)
                return null;

            return new UserResult()
            {
                ApiType = ApiType.AniList,
                Name = data.name,
                SiteUrl = data.siteUrl,
                AvatarImage = data.avatar.large,
                BannerImage = data.bannerImage,
                //Favorites
                AnimeFavorite = new UserFavorite
                {
                    Name = data.favourites.anime.nodes.Count > 0 ? data.favourites.anime.nodes[0].title.userPreferred : null,
                    SiteUrl = data.favourites.anime.nodes.Count > 0 ? data.favourites.anime.nodes[0].siteUrl : null,
                },
                MangaFavorite = new UserFavorite
                {
                    Name = data.favourites.manga.nodes.Count > 0 ? data.favourites.manga.nodes[0].title.userPreferred : null,
                    SiteUrl = data.favourites.manga.nodes.Count > 0 ? data.favourites.manga.nodes[0].siteUrl : null,
                },
                CharacterFavorite = new UserFavorite
                {
                    Name = data.favourites.characters.nodes.Count > 0 ? data.favourites.characters.nodes[0].name.full : null,
                    SiteUrl = data.favourites.characters.nodes.Count > 0 ? data.favourites.characters.nodes[0].siteUrl : null,
                    ImageUrl = data.favourites.characters.nodes.Count > 0 ? data.favourites.characters.nodes[0].image.large : null,
                },
                //Statistics
                AnimeStats = new UserStatistic
                {
                    Count = data.statistics.anime.count,
                    Amount = data.statistics.anime.episodesWatched,
                    MeanScore = data.statistics.anime.meanScore,
                },
                MangaStats = new UserStatistic
                {
                    Count = data.statistics.manga.count,
                    Amount = data.statistics.manga.chaptersRead,
                    MeanScore = data.statistics.manga.meanScore,
                }
            };
        }
    }
}