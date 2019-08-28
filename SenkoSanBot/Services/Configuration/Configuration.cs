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
        public string AlphaCodersApiToken { get; internal set; } = "YOUR TOKEN";
        [DataMember]
        public string Prefix { get; internal set; } = ".";
        [DataMember]
        public string WelcomeMessage { get; internal set; } = "おかえりなのじや! Welcome {0} to {1}";
        [DataMember]
        public int MaxWarnAmount { get; internal set; } = 3;
        [DataMember]
        public string StatusMessage { get; internal set; } = "With My Tail";
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

        };
        [DataMember]
        public int GachaPrice { get; internal set; } = 20;
        [DataMember]
        public Dictionary<int, string> Packs { get; internal set; } = new Dictionary<int, string>
        {
            {1, "Fate (Series)"},
            {2, "Azur Lane"},
            {3, "Honkai Impact 3rd"},
            {4, "Pokemon (Series)"},
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
            MaxWarnAmount = MaxWarnAmount == 0 ? config.MaxWarnAmount : MaxWarnAmount;
            StatusMessage = StatusMessage ?? config.StatusMessage;
            // Assume no one that uses this bot has the log channel id be 0
            LogChannelID = LogChannelID == 0 ? config.LogChannelID : LogChannelID;
            BlacklistedWord = BlacklistedWord ?? config.BlacklistedWord;
            InviteLinkWhitelist = InviteLinkWhitelist ?? config.InviteLinkWhitelist;
            Tags = Tags ?? config.Tags;
            Packs = Packs ?? config.Packs;
            GachaPrice = GachaPrice == 0 ? config.GachaPrice : GachaPrice;
        }
    }
}