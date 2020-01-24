using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Threading;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Discord.Commands;
using System.Linq;
using YohaneBot.Services.Database;

#nullable enable

namespace YohaneBot.Services.Scheduler
{
    public struct ScheduleCallData
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
    public class SchedulerEntry
    {
        public ScheduleCallData CallData;
        public DateTime Time;

        public SchedulerEntry(ScheduleCallData callData, DateTime time)
        {
            CallData = callData;
            Time = time;
        }
    }
    public class SchedulerService : IDisposable
    {
        private IServiceProvider _serviceProvider;
        private CommandService _commandService;
        private JsonDatabaseService _database;

        public SchedulerService(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
            _commandService = _serviceProvider.GetRequiredService<CommandService>();
            _database = _serviceProvider.GetRequiredService<JsonDatabaseService>();
        }

        public void Initialize()
        {
            IEnumerable<SchedulerEntry> dbSchedules = _database.Db.Schedules.Values;
            SchedulerEntry[] missedSchedules = dbSchedules.Where(entry => entry.Time <= DateTime.Now).ToArray();
            SchedulerEntry[] futureSchedules = dbSchedules.Where(entry => entry.Time > DateTime.Now).ToArray();
            _database.Db.Schedules.Clear();
            foreach(SchedulerEntry entry in missedSchedules)
                OnDone(entry.CallData, null);
            foreach(SchedulerEntry entry in futureSchedules)
                Add(entry.Time, entry.CallData, false);
            _database.WriteData();
        }

        public void Add<T>(DateTime time, string func, string data)
        {
            string typeName = typeof(T).FullName!;
            ScheduleCallData callData = new ScheduleCallData(typeName, func, data);
            Add(time, callData);
        }

        public void Add(DateTime time, ScheduleCallData callData, bool writeData = true)
        {
            Guid guid = Guid.NewGuid();
            TimeSpan delay = time - DateTime.Now;
            if(delay.TotalMilliseconds < 0)
                return;
            Task.Delay(delay).ContinueWith(task => OnDone(callData, guid));
            _database.Db.Schedules.Add(guid, new SchedulerEntry(callData, time));
            if(writeData)
                _database.WriteData();
        }

        private void OnDone(ScheduleCallData data, Guid? guid)
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
            if(guid != null)
            {
                Guid id = guid.Value;
                if(_database.Db.Schedules.ContainsKey(id))
                {
                    _database.Db.Schedules.Remove(id);
                    _database.WriteData();
                }
            }
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

        public void Dispose()
        {

        }
    }
}