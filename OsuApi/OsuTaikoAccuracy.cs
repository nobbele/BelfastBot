namespace OsuApi
{
    public class OsuTaikoAccuracy : OsuAccuracy
    {
        public uint CountBad;
        public uint CountGood;
        public uint CountGreat;

        public override float Accuracy => (float)((CountGood * 0.5f) + CountGreat) / (CountBad + CountGood + CountGreat);
    }
}