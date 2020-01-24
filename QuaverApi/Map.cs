using Newtonsoft.Json;

namespace QuaverApi
{
    public class Map
    {
        [JsonProperty("id")]
        public uint Id;
        [JsonProperty("title")]
        public string Title;
        [JsonProperty("difficulty_name")]
        public string DifficultyName;
        [JsonProperty("artist")]
        public string Artist;
        [JsonProperty("creator_username")]
        public string Creator;
        [JsonProperty("difficulty_rating")]
        public float DifficultyRating;
        [JsonProperty("mapset_id")]
        public int MapSetId;
    }
}