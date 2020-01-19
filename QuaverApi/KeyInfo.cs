using Newtonsoft.Json;

namespace QuaverApi
{
    public class KeyInfo
    {
        public int KeyCount;

        [JsonProperty("stats")]
        public KeyStats Stats;
        [JsonProperty("globalRank")]
        public ulong GlobalRanking;
        [JsonProperty("countryRank")]
        public ulong CountryRanking;
    }
}