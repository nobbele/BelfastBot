using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace SenkoSanBot
{
    [DataContract(Name = "Configuration", Namespace = "")]
    public struct BotConfiguration : IExtensibleDataObject
    {
        [DataMember]
        public string Token { get; private set; }
        [DataMember]
        public string Prefix { get; private set; }
        public ExtensionDataObject ExtensionData { get; set; }
        [DataMember]
        public string WelcomeMessage { get; private set; }

        public static BotConfiguration GetDefault() => new BotConfiguration()
        {
            Token = "YOUR TOKEN",
            Prefix = ".",
            WelcomeMessage = "おかえりなのじや! Welcome {0}",
        };
    }
}
