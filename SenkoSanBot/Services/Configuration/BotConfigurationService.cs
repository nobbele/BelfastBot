using SenkoSanBot.Services.Logging;
using System;
using System.IO;
using System.Runtime.Serialization;
using System.Xml;

namespace SenkoSanBot.Services.Configuration
{
    public class BotConfigurationService : IDisposable, IBotConfigurationService
    {
        public static string ConfigurationFilePath => "config.xml";
        public static readonly int FileBufferSize = 4096;

        public BotConfiguration Configuration { get; private set; }

        private bool m_failure = false;

        private readonly LoggingService m_logger;

        public BotConfigurationService(LoggingService logger)
        {
            m_logger = logger;
        }

        public bool Initialize()
        {
            if (!File.Exists(ConfigurationFilePath))
            {
                m_logger.LogCritical("Configuration not found, creating one");
                Configuration = new BotConfiguration();
                WriteData();
                return WithFailure(false);
            }
            else
            {
                m_logger.LogInfo("Reading configuration");
                try
                {
                    var serializer = new DataContractSerializer(typeof(BotConfiguration));
                    using (var fs = new FileStream(ConfigurationFilePath, FileMode.Open, FileAccess.Read, FileShare.Read, bufferSize: FileBufferSize, useAsync: true))
                        Configuration = (BotConfiguration)serializer.ReadObject(fs);
                    if (Configuration == null)
                        throw new Exception("Configuration is null");
                    m_logger.LogInfo("Done reading configuration");
                    WriteData();
                    return WithFailure(false);
                } catch (Exception e)
                {
                    m_logger.LogCritical($"Couldn't load configuration. {e.Message}");
                    return WithFailure(true);
                }
            }
        }

        /// <summary>
        /// Sets failure flag and returns opposite
        /// </summary>
        private bool WithFailure(bool value) => !(m_failure = value);

        public void WriteData()
        {
            if (m_failure)
            {
                m_logger.LogInfo("Not writing due to invalid configuration");
                return;
            }
            else
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

        public void Dispose() => WriteData();
    }
}