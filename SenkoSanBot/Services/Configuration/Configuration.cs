using Discord;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace SenkoSanBot.Services.Configuration
{
    [DataContract(Name = "Configuration", Namespace = "")]
    public class BotConfiguration : IExtensibleDataObject
    {
        [DataMember]
        public string Token { get; internal set; } = "YOUR TOKEN";
        [DataMember]
        public string OsuApiToken { get; internal set; } = "YOUR TOKEN";
        [DataMember]
        public string Prefix { get; internal set; } = ".";
        [DataMember]
        public string WelcomeMessage { get; internal set; } = "おかえりなのじや! Welcome {0}";
        [DataMember]
        public string StatusMessage { get; internal set; } = "With Her Tail";
        [DataMember]
        public UserStatus OnlineStatus = UserStatus.DoNotDisturb;
        [DataMember]
        public ulong LogChannelID { get; internal set; } = 0;
        [DataMember]
        public string[] BlacklistedWord { get; internal set; } = new string[] { "bad word" };
        [DataMember]
        public string InviteLinkWhitelist { get; internal set; } = "http://discord.gg/MYSERVERINV";
        [DataMember]
        public Dictionary<string, string> Tags { get; internal set; } = new Dictionary<string, string>
        {
            {"test", "test response"}
        };

        public ExtensionDataObject ExtensionData { get; set; }

        [OnDeserializing]
        private void SetDefault(StreamingContext c)
        {
            BotConfiguration config = new BotConfiguration();
            Token = Token ?? config.Token;
            OsuApiToken = OsuApiToken ?? config.OsuApiToken;
            Prefix = Prefix ?? config.Prefix;
            WelcomeMessage = WelcomeMessage ?? config.WelcomeMessage;
            StatusMessage = StatusMessage ?? config.StatusMessage;
            // Assume no one that uses this bot has the log channel id be 0
            LogChannelID = LogChannelID == 0 ? config.LogChannelID : LogChannelID;
            BlacklistedWord = BlacklistedWord ?? config.BlacklistedWord;
            InviteLinkWhitelist = InviteLinkWhitelist ?? config.InviteLinkWhitelist;
            Tags = Tags ?? config.Tags;
        }
    }
}