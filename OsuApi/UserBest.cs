using Newtonsoft.Json;

namespace OsuApi
{
    public class UserBest
    {
        public UserProfile PlayerData;
        public Beatmap BeatmapData;
        public OsuAccuracy Accuracy;

        [JsonProperty("rank")]
        public string Rank;
        [JsonProperty("score")]
        public ulong Score;
        [JsonProperty("maxcombo")]
        public uint Combo;
        [JsonProperty("pp")]
        public float PP;
    }
}