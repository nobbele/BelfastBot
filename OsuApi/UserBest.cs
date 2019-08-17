namespace OsuApi
{
    public struct UserBest
    {
        public UserProfile PlayerData;
        public Beatmap BeatmapData;
        public string Rank;
        public ulong Score;
        public int Combo;
        public float PP;
    }
}