using SenkoSanBot.Services.Logging;
using System;
using System.IO;
using System.Runtime.Serialization;
using System.Xml;

namespace SenkoSanBot.Services.Configuration
{
    public class BotConfigurationService : IDisposable
    {
        public static string ConfigurationFilePath => "config.xml";
        public static readonly int FileBufferSize = 4096;

        public BotConfiguration Configuration { get; private set; }

        private readonly LoggingService m_logger;

        public BotConfigurationService(LoggingService logger)
        {
            m_logger = logger;
        }

        public bool Initialize()
        {
            if (!File.Exists(ConfigurationFilePath))
            {
                m_logger.LogCritical("Configuration not found");
                Configuration = BotConfiguration.GetDefault();
                return false;
            }
            else
            {
                m_logger.LogInfo("Reading configuration");
                var serializer = new DataContractSerializer(typeof(BotConfiguration));
                using (var fs = new FileStream(ConfigurationFilePath, FileMode.Open, FileAccess.Read, FileShare.Read, bufferSize: FileBufferSize, useAsync: true))
                    Configuration = (BotConfiguration)serializer.ReadObject(fs);
                m_logger.LogInfo("Done reading configuration");
                return true;
            }
        }

        public void Dispose()
        {
            m_logger.LogInfo("Writing configuration");
            var settings = new XmlWriterSettings()
            {
                Indent = true,
            };
            DataContractSerializer serializer = new DataContractSerializer(typeof(BotConfiguration));
            using (var fs = new FileStream(ConfigurationFilePath, FileMode.Create, FileAccess.Write, FileShare.None, bufferSize: FileBufferSize, useAsync: true))
            using (var writer = XmlWriter.Create(fs, settings))
                serializer.WriteObject(writer, Configuration);
            m_logger.LogInfo("Done writing configuration");
        }
    } 
}