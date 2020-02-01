using System.Collections.Generic;
using osu.Game.Rulesets.Scoring;

namespace OsuApi
{
    public class OsuStdAccuracy : OsuAccuracy
    {
        public uint Count50;
        public uint Count100;
        public uint Count300;
        public uint CountMiss;

        public override float Accuracy => (float)((Count50 * 50) + (Count100 * 100) + (Count300 * 300)) / ((CountMiss + Count50 + Count100 + Count300) * 300);

        public override Dictionary<HitResult, int> Statistics => new Dictionary<HitResult, int>()
        {
            { HitResult.Miss, (int)CountMiss },
            { HitResult.Meh, (int)Count50 },
            { HitResult.Good, (int)Count100 },
            { HitResult.Great, (int)Count300 },
        };
    }
}