namespace BelfastBot
{
    public interface IClient
    {
        string Version { get; }

        bool Stopped { get; }
        void Stop(bool force = false);
    }
}