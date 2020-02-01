using System.Collections.Generic;
using osu.Game.Rulesets.Scoring;

namespace OsuApi
{
    public class OsuCtbAccuracy : OsuAccuracy
    {
        public uint Count50;
        public uint Count100;
        public uint Count300;
        public uint CountMiss;
        public uint CountKatu;

        public override float Accuracy => (float)(Count50 + Count100 + Count300) / (CountMiss + Count50 + Count100 + Count300 + CountKatu);

        public override Dictionary<HitResult, int> Statistics => new Dictionary<HitResult, int>()
        {
            { HitResult.Miss, (int)CountMiss },
            { HitResult.Perfect, (int)Count300 }
        };
    }
}