namespace OsuApi
{
    public struct PlayResult
    {
        public UserProfile PlayerData;
        public Beatmap BeatmapData;
        public ulong Score;
        public int Combo;
        public string Rank;
        public uint Count50;
        public uint Count100;
        public uint Count300;
        public float Accuracy => (float)((Count50 * 50) + (Count100 * 100) + (Count300 * 300)) / ((Count50 + Count100 + Count300) * 300);
    }
}