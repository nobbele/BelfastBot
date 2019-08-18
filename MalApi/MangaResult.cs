namespace MalApi
{
    public struct MangaResult
    {
        public ulong Id;
        public string Status;
        public string Title;
        public string Synopsis;
        public string Type;
        public int? Volumes;
        public int? Chapters;
        public float? Score;
        public string ImageUrl;
        public string MangaUrl;
        public Author[] Authors;
    }
}