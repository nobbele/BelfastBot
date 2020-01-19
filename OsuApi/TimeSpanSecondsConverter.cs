using System;
using Newtonsoft.Json;

namespace OsuApi
{
    public class TimeSpanSecondsConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType) => objectType == typeof(TimeSpan);

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            string strSeconds = (string)reader.Value;
            uint seconds = uint.Parse(strSeconds);
            return TimeSpan.FromSeconds(seconds);
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            TimeSpan time = (TimeSpan)value;
            writer.WriteValue(time.TotalSeconds);
        }
    }
}