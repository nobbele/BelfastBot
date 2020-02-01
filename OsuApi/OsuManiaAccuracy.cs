using System.Collections.Generic;
using osu.Game.Rulesets.Scoring;

namespace OsuApi
{
    public class OsuManiaAccuracy : OsuAccuracy
    {
        public uint Count50;
        public uint Count100;
        public uint Count300;
        public uint CountMiss;
        public uint CountKatu;
        public uint CountGeki;

        public override float Accuracy => (float)((Count50 * 50) + (Count100 * 100) + (CountKatu * 200) + (CountGeki + Count300) * 300) / ((CountMiss + Count50 + Count100 + CountKatu + Count300 + CountGeki) * 300);

        public override Dictionary<HitResult, int> Statistics => new Dictionary<HitResult, int>()
        {
            { HitResult.Meh, (int)Count50 },
            { HitResult.Ok, (int)Count100 },
            { HitResult.Good, (int)CountKatu },
            { HitResult.Great, (int)CountGeki },
            { HitResult.Perfect, (int)Count300 },
        };
    }
}