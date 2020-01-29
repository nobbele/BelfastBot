using Discord.Commands;
using System;
using System.Reflection;
using System.Threading.Tasks;
using BelfastBot.Services.Configuration;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

namespace BelfastBot.Modules.Misc
{
    [Summary("Commands for configurations")]
    public class ConfigurationModule : BelfastModuleBase
    {
        [Command("configshow")]
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

        [Command("configset")]
        [Summary("Set configuration values")]
        [RequireOwner]
        public async Task SetConfigAsync(string name, [Remainder] string value)
        {
            IEnumerable<PropertyInfo> props = typeof(BotConfiguration).GetProperties()
                .Where(p => p.IsDefined(typeof(DataMemberAttribute), false));
            PropertyInfo prop = props.Single(p => p.Name == name);
            string old = prop.GetValue(Config.Configuration).ToString();
            prop.SetValue(Config.Configuration, ConvertType(value, prop.PropertyType));
            await ReplyAsync($"`{name} => {old} -> {value}`");
        }

        public object ConvertType(object value, Type type)
        {
            if (type.IsEnum)
            return Enum.Parse(type, value.ToString());

            return Convert.ChangeType(value, type);
        }

        [Command("configwrite")]
        [Summary("Write configuration values")]
        [RequireOwner]
        public async Task WriteConfigAsync()
        {
            Config.WriteData();
            await ReplyAsync("Wrote data");
        }
    }
}