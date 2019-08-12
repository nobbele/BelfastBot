using Newtonsoft.Json;
using SenkoSanBot.Services.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SenkoSanBot.Services.Database
{
    public class JsonDatabaseService
    {
        public static readonly string DbFilePath = $"db.json";
        public Dictionary<ulong, List<DatabaseUserEntry>> Db { get; private set; }

        private readonly object writeLock = new object();

        private readonly LoggingService m_logger;

        public JsonDatabaseService(LoggingService logger)
        {
            m_logger = logger;
        }

        public async Task InitializeAsync()
        {
            m_logger.LogInfo("Reader database from file");
            lock (writeLock)
            {
                if (!File.Exists(DbFilePath))
                    File.Create(DbFilePath).Dispose();
                string json = File.ReadAllText(DbFilePath);
                Db = string.IsNullOrEmpty(json) ? new Dictionary<ulong, List<DatabaseUserEntry>>() : JsonConvert.DeserializeObject<Dictionary<ulong, List<DatabaseUserEntry>>>(json);
            }
            m_logger.LogInfo("done reading database from file");
            await Task.CompletedTask;
        }

        public DatabaseUserEntry GetUserEntry(ulong serverId, ulong id)
        {
            List<DatabaseUserEntry> serverDb = Db.SingleOrDefault(server => server.Key == serverId).Value ?? Db.AddGet(new KeyValuePair<ulong, List<DatabaseUserEntry>>(serverId, new List<DatabaseUserEntry>())).Value;
            return serverDb.SingleOrDefault(user => user.Id == id) ?? serverDb.AddGet(DatabaseUserEntry.CreateNew(id));
        }

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
    }
}