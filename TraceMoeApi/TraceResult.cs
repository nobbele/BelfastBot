using Newtonsoft.Json;

namespace TraceMoeApi
{
    public struct TraceResult
    {
        [JsonProperty("title_english")]
        public string Title;
        [JsonProperty("anilist_id")]
        public long AlId;
    }
}
