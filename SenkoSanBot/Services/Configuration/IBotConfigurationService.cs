namespace SenkoSanBot.Services.Configuration
{
    public interface IBotConfigurationService
    {
        BotConfiguration Configuration { get; }

        bool Initialize();
    }
}