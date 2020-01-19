using System;
using Newtonsoft.Json;

namespace OsuApi
{
    public class Beatmap
    {
        [JsonProperty("title")]
        public string Name;
        [JsonProperty("beatmap_id")]
        public ulong Id;
        [JsonProperty("beatmapset_id")]
        public ulong SetId;
        [JsonProperty("difficultyrating")]
        public float StarRating;
        [JsonProperty("bpm")]
        public int Bpm;
        [JsonProperty("total_length"), JsonConverter(typeof(TimeSpanSecondsConverter))]
        public TimeSpan Length;
        [JsonProperty("creator")]
        public string CreatorName;
        [JsonProperty("creator_id")]
        public ulong CreatorId;
    }
}