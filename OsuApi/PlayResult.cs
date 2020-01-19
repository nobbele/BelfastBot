using Newtonsoft.Json;

namespace OsuApi
{
    public class PlayResult
    {
        public UserProfile PlayerData;
        public Beatmap BeatmapData;
        public OsuAccuracy Accuracy;

        [JsonProperty("score")]
        public ulong Score;
        [JsonProperty("maxcombo")]
        public int Combo;
        [JsonProperty("rank")]
        public string Rank;      
    }
}