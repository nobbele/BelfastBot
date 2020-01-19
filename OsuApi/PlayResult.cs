namespace OsuApi
{
    public struct PlayResult
    {
        public UserProfile PlayerData;
        public Beatmap BeatmapData;
        public ulong Score;
        public int Combo;
        public string Rank;
        public OsuAccuracy Accuracy;
    }
}