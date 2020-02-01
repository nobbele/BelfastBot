namespace AnimeApi
{
    public struct UserResult
    {
        public ApiType ApiType;
        public string Name;
        public string AvatarImage;
        public string BannerImage;
        public string SiteUrl;
        public UserFavorite? AnimeFavorite;
        public UserFavorite? MangaFavorite;
        public UserFavorite? CharacterFavorite;
        //Statistics
        public UserStatistic AnimeStats;
        public UserStatistic MangaStats;
    }
}