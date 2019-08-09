using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;

namespace SenkoSanBot.Services
{
    public class BotConfigurationService : IDisposable
    {
        public static string ConfigurationFilePath => "config.xml";
        public static int FileBufferSize => 4096;

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
                m_logger.Log("Configuration not found");
                Configuration = BotConfiguration.GetDefault();
                return false;
            }
            else
            {
                m_logger.Log("Reading configuration");
                var serializer = new DataContractSerializer(typeof(BotConfiguration));
                using (var fs = new FileStream(ConfigurationFilePath, FileMode.Open, FileAccess.Read, FileShare.Read, bufferSize: FileBufferSize, useAsync: true))
                    Configuration = (BotConfiguration)serializer.ReadObject(fs);
                m_logger.Log("Done reading configuration");
                return true;
            }
        }

        public void Dispose()
        {
            m_logger.Log("Writing configuration");
            var settings = new XmlWriterSettings()
            {
                Indent = true,
            };
            DataContractSerializer serializer = new DataContractSerializer(typeof(BotConfiguration));
            using (var fs = new FileStream(ConfigurationFilePath, FileMode.Create, FileAccess.Write, FileShare.None, bufferSize: FileBufferSize, useAsync: true))
            using (var writer = XmlWriter.Create(fs, settings))
                serializer.WriteObject(writer, Configuration);
            m_logger.Log("Done writing configuration");
        }
    } 
}
