using System;

namespace OsuApi
{
    public struct Beatmap
    {
        public string Name;
        public ulong Id;
        public ulong SetId;
        public float StarRating;
        public int Bpm;
        public TimeSpan Lenght;
        public string CreatorName;
        public ulong CreatorId;
    }
}