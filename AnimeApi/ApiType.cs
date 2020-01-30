namespace AnimeApi
{
    public enum ApiType
    {
        AniList,
        MyAnimeList,
    }

    public static class ApiTypeExtensionMethods
    {
        public static string ToIconUrl(this ApiType type) => type switch
        {
            ApiType.MyAnimeList => "https://image.myanimelist.net/ui/OK6W_koKDTOqqqLDbIoPAiC8a86sHufn_jOI-JGtoCQ",
            ApiType.AniList => "https://cdn.discordapp.com/attachments/303528930634235904/670386826464460800/anilist-co_122132.png",
            _ => null,
        };
    }
}