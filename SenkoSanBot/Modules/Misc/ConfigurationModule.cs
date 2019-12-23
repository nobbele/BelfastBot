using Discord;
using Discord.Commands;
using System;
using System.Threading;
using System.Reflection;
using System.Threading.Tasks;
using SenkoSanBot.Services.Configuration;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

namespace SenkoSanBot.Modules.Misc
{
    [Summary("Commands for configurations")]
    public class ConfigurationModule : SenkoSanModuleBase
    {
        [Command("config show")]
        [Summary("Show configurations")]
        public async Task ShowConfigAsync()
        {
            IEnumerable<PropertyInfo> props = typeof(BotConfiguration).GetProperties()
                .Where(prop => prop.IsDefined(typeof(DataMemberAttribute), false) && !prop.IsDefined(typeof(SensitiveAttribute), false));
            StringBuilder sb = new StringBuilder();
            foreach(PropertyInfo prop in props) 
            {
                object content = prop.GetValue(Config.Configuration);
                if(prop.DeclaringType != typeof(System.Collections.IDictionary))
                    sb.AppendLine($"`{prop.Name} => {string.Concat(content.ToString().Where(c => c != '`' && c != '\n'))}`");
            }
            await ReplyAsync(sb.ToString());
        }

        [Command("config set")]
        [Summary("Set configuration values")]
        [RequireOwner]
        public async Task SetConfigAsync(string name, [Remainder] string value)
        {
            IEnumerable<PropertyInfo> props = typeof(BotConfiguration).GetProperties()
                .Where(p => p.IsDefined(typeof(DataMemberAttribute), false));
            PropertyInfo prop = props.Single(p => p.Name == name);
            string old = prop.GetValue(Config.Configuration).ToString();
            prop.SetValue(Config.Configuration, Convert.ChangeType(value, prop.PropertyType));
            await ReplyAsync($"`{name} => {old} -> {value}`");
        }

        [Command("config write")]
        [Summary("Write configuration values")]
        [RequireOwner]
        public async Task WriteConfigAsync()
        {
            Config.WriteData();
            await ReplyAsync("Wrote data");
        }
    }
}