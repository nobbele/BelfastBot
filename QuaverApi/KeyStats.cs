using Newtonsoft.Json;

namespace QuaverApi
{
    public class KeyStats
    {
        [JsonProperty("overall_performance_rating")]
        public float PerformanceRating;
        [JsonProperty("overall_accuracy")]
        public float Accuracy;
        [JsonProperty("play_count")]
        public uint PlayCount;
    }
}