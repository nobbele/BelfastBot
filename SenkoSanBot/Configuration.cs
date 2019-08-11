using System.Runtime.Serialization;

namespace SenkoSanBot
{
    [DataContract(Name = "Configuration", Namespace = "")]
    public struct BotConfiguration : IExtensibleDataObject
    {
        [DataMember]
        public string Token { get; private set; }
        [DataMember]
        public string Prefix { get; private set; }
        [DataMember]
        public string WelcomeMessage { get; private set; }
        [DataMember]
        public ulong LogChannelID { get; private set; }
        public ExtensionDataObject ExtensionData { get; set; }

        public static BotConfiguration GetDefault() => new BotConfiguration()
        {
            Token = "YOUR TOKEN",
            Prefix = ".",
            WelcomeMessage = "おかえりなのじや! Welcome {0}",
            LogChannelID = 0,
        };
    }
}
