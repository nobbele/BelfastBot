using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Threading;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Discord.Commands;
using System.Linq;

#nullable enable

namespace SenkoSanBot.Services.Scheduler
{
    public class SchedulerService
    {
        private struct ScheduleCallData
        {
            public string Type;
            public string Function;
            public string Data;

            public ScheduleCallData(string type, string function, string data)
            {
                Type = type;
                Function = function;
                Data = data;
            }
        }

        private Dictionary<Guid, DateTime> schedules = new Dictionary<Guid, DateTime>();

        private IServiceProvider _serviceProvider;
        private CommandService _commandService;

        public SchedulerService(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
            _commandService = _serviceProvider.GetRequiredService<CommandService>();
        }

        public void Add<T>(DateTime time, string func, string data)
        {
            Guid guid = Guid.NewGuid();
            TimeSpan delay = time - DateTime.Now;
            if(delay.TotalMilliseconds < 0)
                return;
            string typeName = typeof(T).FullName!;
            Task.Delay(delay).ContinueWith(task => OnDone(new ScheduleCallData(typeName, func, data), guid));
            schedules.Add(guid, time);
        }

        private void OnDone(ScheduleCallData data, Guid guid)
        {
            Type? type = Type.GetType(data.Type);
            if(type == null)
                throw new Exception($"Invalid type {type}");
            MethodInfo? method = type.GetMethod(data.Function);
            if(method == null)
                throw new Exception($"Invalid method {method} in {type}");
            object obj = CreateObject(type);
            if(obj == null)
                throw new Exception($"Couldn't create {type} from services");
            method.Invoke(obj, new object?[] { data.Data });
            schedules.Remove(guid);
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