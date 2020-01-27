namespace BelfastBot
{
    public static class BotHelper
    {
        public static IClient Client { get; private set; }
        public static void SetClient(IClient client)
        {
            Client = client;
        }
    }
}