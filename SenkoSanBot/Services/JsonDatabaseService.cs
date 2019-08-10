using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SenkoSanBot.Services
{
    public class JsonDatabaseService : IDisposable
    {
        public static readonly string DbFilePath = $"db.json";
        public List<DatabaseUserEntry> Db { get; private set; }

        private static readonly int writeInterval = (int)TimeSpan.FromMinutes(5).TotalMilliseconds;
        private readonly CancellationTokenSource tokenSource = new CancellationTokenSource();
        private readonly object writeLock = new object();

        private readonly LoggingService m_logger;

        public JsonDatabaseService(LoggingService logger)
        {
            m_logger = logger;
        }

        public async Task InitializeAsync()
        {
            var _ = Task.Run(async () =>
            {
                while (true)
                {
                    try
                    {
                        WriteData();
                    }
                    catch (Exception e)
                    {
                        m_logger.LogCritical(e);
                    }

                    if (tokenSource.Token.IsCancellationRequested)
                        break;
                    await Task.Delay(writeInterval, tokenSource.Token);
                }
            }, tokenSource.Token);

            m_logger.LogInfo("Reader database from file");
            lock (writeLock)
            {
                File.Create(DbFilePath).Dispose();
                string json = File.ReadAllText(DbFilePath);
                Db = string.IsNullOrEmpty(json) ? new List<DatabaseUserEntry>() : JsonConvert.DeserializeObject<List<DatabaseUserEntry>>(json);
            }
            m_logger.LogInfo("done reading database from file");
            await Task.CompletedTask;
        }

        public DatabaseUserEntry GetUserEntry(ulong id) => Db.SingleOrDefault(user => user.Id == id) ?? Db.AddGet(DatabaseUserEntry.CreateNew(id));

        public void WriteData()
        {
            m_logger.LogInfo("Writing database to file");
            lock (writeLock)
            {
                string json = JsonConvert.SerializeObject(Db, Formatting.Indented);
                File.WriteAllText(DbFilePath, json);
            }
            m_logger.LogInfo("done writing database to file");
        }

        public void Dispose()
        {
            tokenSource.Cancel();
        }
    }
    public static class ListExtensionMethods
    {
        public static T AddGet<T>(this List<T> me, T item)
        {
            me.Add(item);
            return item;
        }
    }
}
