using Discord;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System;

namespace BelfastBot.Services.Configuration
{
    public class SensitiveAttribute : Attribute {}

    [DataContract(Name = "Configuration", Namespace = "")]
    public class BotConfiguration : IExtensibleDataObject
    {
        [Sensitive, DataMember]
        public string Token { get; internal set; } = "YOUR TOKEN";
        [Sensitive, DataMember]
        public string OsuApiToken { get; internal set; } = "YOUR TOKEN";
        [Sensitive, DataMember]
        public string AlphaCodersApiToken { get; internal set; } = "YOUR TOKEN";
        [DataMember]
        public string Prefix { get; internal set; } = "bel!";
        [DataMember]
        public string[] WelcomeMessages { get; internal set; } = new string[] 
        {
            "I Belfast welcome you {0} to our base {1}",
            "It is my pleasure to welcome you {0} to {1}",
        };
        [DataMember]
        public int MaxWarnAmount { get; internal set; } = 3;
        [DataMember]
        public string StatusMessage { get; internal set; } = "Over :serverCount: servers | :prefix:help";
        [DataMember]
        public ActivityType Activity { get; internal set; } = ActivityType.Watching;
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
        public uint GachaPrice { get; internal set; } = 20;
        [DataMember]
        public Dictionary<int, string> Packs { get; internal set; } = new Dictionary<int, string>
        {
            {1, "Azur Lane"},
            {2, "Fate (Series)"},
            {3, "Arknights"},
            {4, "Honkai Impact 3rd"},
            {5, "Pokemon (Series)"},
            {6, "One Piece (Series)"},
            {7, "Touhou Project"},
        };
        [DataMember]
        public uint SSRCardPrice { get; internal set; } = 200;
        [DataMember]
        public uint SRCardPrice { get; internal set; } = 50;
        [DataMember]
        public uint RareCardPrice { get; internal set; } = 30;
        [DataMember]
        public uint CommonCardPrice { get; internal set; } = 5;
        [DataMember]
        public string SourceCodeGit { get; internal set; } = "https://github.com/nobbele/BelfastBot.git";

        public ExtensionDataObject ExtensionData { get; set; }

        [OnDeserializing]
        private void SetDefault(StreamingContext c)
        {
            BotConfiguration config = new BotConfiguration();
            Token = Token ?? config.Token;
            OsuApiToken = OsuApiToken ?? config.OsuApiToken;
            Prefix = Prefix ?? config.Prefix;
            WelcomeMessages = WelcomeMessages ?? config.WelcomeMessages;
            MaxWarnAmount = MaxWarnAmount == 0 ? config.MaxWarnAmount : MaxWarnAmount;
            StatusMessage = StatusMessage ?? config.StatusMessage;
            Activity = Activity;
            // Assume no one that uses this bot has the log channel id be 0
            LogChannelID = LogChannelID == 0 ? config.LogChannelID : LogChannelID;
            BlacklistedWord = BlacklistedWord ?? config.BlacklistedWord;
            InviteLinkWhitelist = InviteLinkWhitelist ?? config.InviteLinkWhitelist;
            Tags = Tags ?? config.Tags;
            Packs = Packs ?? config.Packs;
            GachaPrice = GachaPrice == 0 ? config.GachaPrice : GachaPrice;
            SRCardPrice = SRCardPrice == 0 ? config.SRCardPrice : SRCardPrice;
            RareCardPrice = RareCardPrice == 0 ? config.RareCardPrice : RareCardPrice;
            CommonCardPrice = CommonCardPrice == 0 ? config.CommonCardPrice : CommonCardPrice;
        }
    }
}