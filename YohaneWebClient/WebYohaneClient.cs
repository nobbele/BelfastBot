using YohaneBot;

namespace YohaneWebClient
{
    public class WebYohaneClient : IClient
    {
        public string Version => "1.2-Web";

        public bool Stopped => throw new System.NotImplementedException();

        public void Stop(bool force = false)
        {
            throw new System.NotImplementedException();
        }
    }
}