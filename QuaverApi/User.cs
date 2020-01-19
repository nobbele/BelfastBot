using Newtonsoft.Json;

namespace QuaverApi
{
    public class User
    {
        [JsonProperty("id")]
        public uint Id;
        [JsonProperty("steam_id")]
        public string SteamId;
        [JsonProperty("username")]
        public string Username;
        [JsonProperty("country")]
        public string Country;
        [JsonProperty("avatar_url")]
        public string AvatarUrl;
        public KeyInfo FourKeys;
        public KeyInfo SevenKeys;
    }
}