namespace OsuApi
{
    public struct UserBest
    {
        public UserProfile PlayerData;
        public Beatmap BeatmapData;
        public string Rank;
        public ulong Score;
        public uint Combo;
        public float PP;
        public uint Count50;
        public uint Count100;
        public uint Count300;
        public float Accuracy => (float)((Count50 * 50) + (Count100 * 100) + (Count300 * 300)) / ((Count50 + Count100 + Count300) * 300);
    }
}