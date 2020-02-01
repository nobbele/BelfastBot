using System;
using Newtonsoft.Json;
using osu.Game.Beatmaps;

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
        public float Bpm;
        [JsonProperty("total_length"), JsonConverter(typeof(TimeSpanSecondsConverter))]
        public TimeSpan Length;
        [JsonProperty("creator")]
        public string CreatorName;
        [JsonProperty("creator_id")]
        public ulong CreatorId;
        [JsonProperty(@"diff_drain")]
        public float DrainRate { get; set; }
        [JsonProperty(@"diff_size")]
        public float CircleSize { get; set; }
        [JsonProperty(@"diff_approach")]
        public float ApproachRate { get; set; }
        [JsonProperty(@"diff_overall")]
        public float OverallDifficulty { get; set; }
    }
}