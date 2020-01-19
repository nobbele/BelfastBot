using Newtonsoft.Json;

namespace OsuApi
{
    public class UserProfile
    {
        public uint Mode;

        [JsonProperty("user_id")]
        public ulong UserId;
        [JsonProperty("username")]
        public string UserName;
        [JsonProperty("pp_raw")]
        public float PP;
        [JsonProperty("level")]
        public float Level;
        [JsonProperty("playcount")]
        public ulong PlayCount;
        [JsonProperty("accuracy")]
        public float Accuracy;
        [JsonProperty("pp_rank")]
        public ulong GlobalRanking;
        [JsonProperty("pp_country_rank")]
        public ulong CountryRanking;
        [JsonProperty("country")]
        public string Country;
    }
}