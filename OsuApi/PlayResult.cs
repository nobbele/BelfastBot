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
        public uint Combo;
        [JsonProperty("rank")]
        public string Rank;    
        [JsonProperty("enabled_mods")]
        public Mods Mods;
        [JsonProperty("pp")]
        public float PP = float.NaN;
    }
}