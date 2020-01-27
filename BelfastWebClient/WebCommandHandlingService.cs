using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Discord.Commands;
using Microsoft.Extensions.DependencyInjection;
using BelfastBot;
using BelfastBot.Modules;
using BelfastBot.Services.Communiciation;
using Moq;
using Discord;

namespace BelfastWebClient
{
    public class WebCommandHandlingService
    {
        private DummyType _dummy;
        private IServiceProvider _serviceProvider;
        private CommandService _commandService;
        private WebCommunicationService _communication;

        public (BelfastModuleBase instance, MethodInfo method, CommandAttribute attribute, AliasAttribute alias)[] Commands;

        public WebCommandHandlingService(DummyType dummy, IServiceProvider serviceProvider, CommandService commandService, ICommunicationService communication)
        {
            _dummy = dummy;
            _serviceProvider = serviceProvider;
            _commandService = commandService;
            _communication = communication as WebCommunicationService;
        }

        public void Initialize()
        {
            Assembly assembly = _dummy.GetType().Assembly;

            _commandService.AddModulesAsync(assembly, _serviceProvider);

            IEnumerable<Type> modules = assembly.GetTypes().Where(type => {
                type = type.UnderlyingSystemType;
                if(type.IsAbstract || !type.IsSubclassOf(typeof(BelfastModuleBase)))
                    return false;
                string[] names = type.Namespace.Split('.');
                if(names.Length < 2)
                    return false;
                return names[1] == "Modules";
            });
            List<(BelfastModuleBase, MethodInfo, CommandAttribute, AliasAttribute)> commandsList = new List<(BelfastModuleBase, MethodInfo, CommandAttribute, AliasAttribute)>();
            foreach(Type module in modules) {
                BelfastModuleBase o = CreateObject(module) as BelfastModuleBase;
                IEnumerable<(BelfastModuleBase, MethodInfo, CommandAttribute, AliasAttribute)> commands = module
                    .GetMethods(BindingFlags.Public | BindingFlags.Instance)
                    .Where(m => !m.IsSpecialName)
                    .Select(method => 
                        (
                            o, 
                            method, 
                            method.GetCustomAttribute(typeof(CommandAttribute)) as CommandAttribute, 
                            method.GetCustomAttribute(typeof(AliasAttribute)) as AliasAttribute
                        )
                    )
                    .Where(tup => tup.Item3 != null);
                commandsList.AddRange(commands);
            }
            Commands = commandsList.ToArray();
        }

        private object CreateObject(Type type)
        {
            object obj = ActivatorUtilities.CreateInstance(_serviceProvider, type);
            foreach(PropertyInfo prop in GetProperties(type.GetTypeInfo()))
            {
                prop.SetValue(obj, GetMember(prop.PropertyType, type.GetTypeInfo()));
            }
            return obj;
        }

        public async Task HandleCommandAsync(string cmd, string[] argsStr)
        {
            (BelfastModuleBase instance, MethodInfo method, CommandAttribute attribute, AliasAttribute alias) command = Commands
                .Single(command => command.attribute.Text == cmd || (command.alias?.Aliases.Contains(cmd) ?? false));
            IMessageChannel channel = _communication.CreateChannelWithId(0);
            await channel.SendMessageAsync($"{cmd} {argsStr.DelimeterSeperatedString(" ")}");
            BelfastCommandContext commandContext = new BelfastCommandContext()
            {
                Channel = channel
            };
            PropertyInfo contextProp = typeof(BelfastModuleBase).GetProperty("Context").DeclaringType.GetProperty("Context");
            contextProp.SetValue(command.instance, commandContext, BindingFlags.NonPublic | BindingFlags.Instance, null, null, null);
            object[] args = command.method.GetParameters()
                .Zip(argsStr)
                .Select(o => Convert.ChangeType(o.Second, o.First.ParameterType))
                .ToArray();
            await (command.method.Invoke(command.instance, args) as Task);
        }

        // https://github.com/discord-net/Discord.Net/blob/dev/src/Discord.Net.Commands/Utilities/ReflectionUtils.cs
        private PropertyInfo[] GetProperties(TypeInfo ownerType)
        {
            TypeInfo ObjectTypeInfo = typeof(object).GetTypeInfo();
            var result = new List<System.Reflection.PropertyInfo>();
            while (ownerType != ObjectTypeInfo)
            {
                foreach (var prop in ownerType.DeclaredProperties)
                {
                    if (prop.SetMethod?.IsStatic == false && prop.SetMethod?.IsPublic == true && prop.GetCustomAttribute<DontInjectAttribute>() == null)
                        result.Add(prop);
                }
                ownerType = ownerType.BaseType!.GetTypeInfo();
            }
            return result.ToArray();
        }

        // https://github.com/discord-net/Discord.Net/blob/dev/src/Discord.Net.Commands/Utilities/ReflectionUtils.cs
        private object GetMember(Type memberType, TypeInfo ownerType)
        {
            if (memberType == typeof(IServiceProvider) || memberType == _serviceProvider.GetType())
                return _serviceProvider;
            var service = _serviceProvider.GetService(memberType);
            if (service != null)
                return service;
            throw new InvalidOperationException($"Failed to create \"{ownerType.FullName}\", dependency \"{memberType.Name}\" was not found.");
        }
    }
}