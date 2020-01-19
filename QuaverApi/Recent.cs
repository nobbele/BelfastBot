using Newtonsoft.Json;

namespace QuaverApi
{
    public class Recent
    {
        public Map Map;

        [JsonProperty("id")]
        public uint Id;
        [JsonProperty("performance_rating")]
        public float PerformanceRating;
        [JsonProperty("accuracy")]
        public float Accuracy;
        [JsonProperty("max_combo")]
        public uint Combo;
        [JsonProperty("total_score")]
        public ulong Score;
        [JsonProperty("grade")]
        public string Grade;
    }
}