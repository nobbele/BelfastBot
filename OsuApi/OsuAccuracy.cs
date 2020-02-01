using System.Collections.Generic;
using osu.Game.Rulesets.Scoring;

namespace OsuApi
{
    public abstract class OsuAccuracy
    {
        public abstract float Accuracy { get; }

        public abstract Dictionary<HitResult, int> Statistics { get; }
    }
}