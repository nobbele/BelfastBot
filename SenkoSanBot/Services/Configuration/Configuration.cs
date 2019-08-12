using System.Runtime.Serialization;

namespace SenkoSanBot.Services.Configuration
{
    [DataContract(Name = "Configuration", Namespace = "")]
    public class BotConfiguration : IExtensibleDataObject
    {
        [DataMember]
        public string Token { get; private set; } = "YOUR TOKEN";
        [DataMember]
        public string Prefix { get; private set; } = ".";
        [DataMember]
        public string WelcomeMessage { get; private set; } = "おかえりなのじや! Welcome {0}";
        [DataMember]
        public string StatusMessage { get; private set; } = "Brushes her Tail";
        [DataMember]
        public ulong LogChannelID { get; private set; } = 0;
        [DataMember]
        public string[] BlacklistedWord { get; private set; } = new string[] { "thejayduck" };

        public ExtensionDataObject ExtensionData { get; set; }

        [OnDeserializing]
        private void SetDefault(StreamingContext c)
        {
            BotConfiguration config = new BotConfiguration();
            Token = Token ?? config.Token;
            Prefix = Prefix ?? config.Prefix;
            WelcomeMessage = WelcomeMessage ?? config.WelcomeMessage;
            StatusMessage = StatusMessage ?? config.StatusMessage;
            // Assume no one that uses this bot has the log channel id be 0
            LogChannelID = LogChannelID == 0 ? config.LogChannelID : LogChannelID;
            BlacklistedWord = BlacklistedWord ?? config.BlacklistedWord;
        }
    }
}