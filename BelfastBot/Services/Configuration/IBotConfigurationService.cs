namespace BelfastBot.Services.Configuration
{
    public interface IBotConfigurationService
    {
        BotConfiguration Configuration { get; }

        bool Initialize();

        void WriteData();
    }
}