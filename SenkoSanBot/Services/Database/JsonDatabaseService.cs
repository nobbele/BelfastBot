using Newtonsoft.Json;
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
        public List<DatabaseUserEntry> Db { get; private set; }

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
    }
}