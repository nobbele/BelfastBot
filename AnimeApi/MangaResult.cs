namespace AnimeApi
{
    public struct MangaResult
    {
        public ApiType ApiType;
        public ulong MalId;
        public ulong AlId;
        public string Status;
        public string Title;
        public string Synopsis;
        public string Type;
        public int? Volumes;
        public int? Chapters;
        public int? Score;
        public string ImageUrl;
        public string SiteUrl;
        //Detailed Information
        public string Source;   
        public Staff[] Staff;
    }
}