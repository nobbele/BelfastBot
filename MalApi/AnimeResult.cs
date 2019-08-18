namespace MalApi
{
    public struct AnimeResult
    {
        public ulong Id;
        public string Status;
        public string Title;
        public string Synopsis;
        public string Type;
        public int? Episodes;
        public float? Score;
        public string ImageUrl;
        public string AnimeUrl;
        //Detailed Information
        public string Source;
        public string Duration;
        public string Broadcast;
        public string TrailerUrl;
        public string Studio;
        public string StudioUrl;
    }
}