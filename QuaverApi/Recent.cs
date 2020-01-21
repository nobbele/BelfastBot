using Newtonsoft.Json;
using Quaver.API.Enums;
using Quaver.API.Helpers;

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
        [JsonProperty("mods")]
        public ModIdentifier Mods;

        public string ModsString => ModHelper.GetModsString(Mods);
    }
}