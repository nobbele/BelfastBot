using System.Collections.Generic;
using osu.Game.Rulesets.Scoring;

namespace OsuApi
{
    public class OsuTaikoAccuracy : OsuAccuracy
    {
        public uint CountBad;
        public uint CountGood;
        public uint CountGreat;

        public override float Accuracy => (float)((CountGood * 0.5f) + CountGreat) / (CountBad + CountGood + CountGreat);

        public override Dictionary<HitResult, int> Statistics => new Dictionary<HitResult, int>()
        {
            
        };
    }
}