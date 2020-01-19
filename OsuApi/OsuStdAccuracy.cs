namespace OsuApi
{
    public class OsuStdAccuracy : OsuAccuracy
    {
        public uint Count50;
        public uint Count100;
        public uint Count300;

        public override float Accuracy => (float)((Count50 * 50) + (Count100 * 100) + (Count300 * 300)) / ((Count50 + Count100 + Count300) * 300);
    }
}