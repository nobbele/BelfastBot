using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace OsuApi
{
    public class OsuAccuracyConverter : JsonConverter
    {
        public readonly uint Mode;
        
        public OsuAccuracyConverter(uint mode)
        {
            Mode = mode;
        }

        public override bool CanConvert(Type objectType) => objectType == typeof(OsuAccuracy);

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            JToken token = JToken.ReadFrom(reader);
            return Mode switch
            {
                0 => new OsuStdAccuracy()
                {
                    Count50 = token["count50"].ToObject<uint>(),
                    Count100 = token["count100"].ToObject<uint>(),
                    Count300 = token["count300"].ToObject<uint>(),
                },
                1 => new OsuTaikoAccuracy()
                {
                    CountBad = token["countmiss"].ToObject<uint>(),
                    CountGood = token["count100"].ToObject<uint>(),
                    CountGreat = token["count300"].ToObject<uint>(),
                },
                2 => new OsuCtbAccuracy()
                {
                    Count50 = token["count50"].ToObject<uint>(),
                    Count100 = token["count100"].ToObject<uint>(),
                    Count300 = token["count300"].ToObject<uint>(),
                    CountKatu = token["countkatu"].ToObject<uint>(),
                    CountMiss = token["countmiss"].ToObject<uint>(),
                },
                3 => new OsuManiaAccuracy()
                {
                    Count50 = token["count50"].ToObject<uint>(),
                    Count100 = token["count100"].ToObject<uint>(),
                    Count300 = token["count300"].ToObject<uint>(),
                    CountKatu = token["countkatu"].ToObject<uint>(),
                    CountGeki = token["countgeki"].ToObject<uint>(),
                    CountMiss = token["countmiss"].ToObject<uint>(),
                },
                _ => null,
            };
        }

        public override bool CanWrite => false;

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }
    }
}