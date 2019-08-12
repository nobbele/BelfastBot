using System;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SenkoSanBot.Services.Logging
{
    public class LoggingService : IDisposable
    {
        public static readonly string LogFilePath = $"logs/{DateTime.Now:MM-dd-yyyy_HH-mm-ss}.log";

        private static readonly int logFileWriteInterval = (int)TimeSpan.FromSeconds(10).TotalMilliseconds;
        private static readonly int fileBufferSize = 4096;

        private readonly StringBuilder m_fileBuffer = new StringBuilder(fileBufferSize);
        private readonly CancellationTokenSource tokenSource = new CancellationTokenSource();
        private readonly object writeLock = new object();

        /// <summary>
        /// Don't await
        /// </summary>
        /// <returns></returns>
        public async Task InitializeAsync()
        {
            var _ = Task.Run(async () =>
            {
                while (true)
                {
                    try
                    {
                        WriteBuffer();
                    }
                    catch (Exception e)
                    {
                        LogCritical(e);
                    }

                    if (tokenSource.Token.IsCancellationRequested)
                        break;
                    await Task.Delay(logFileWriteInterval, tokenSource.Token);
                }
            }, tokenSource.Token);

            Directory.CreateDirectory(Path.GetDirectoryName(LogFilePath));

            await Task.CompletedTask;
        }

        public void LogInfo() => LogInfo("");
        public void LogInfo(object format, params object[] args) => Log(LogLevel.Info, format, args);

        public void LogWarning() => LogWarning("");
        public void LogWarning(object format, params object[] args) => Log(LogLevel.Warning, format, args);

        public void LogCritical() => LogCritical("");
        public void LogCritical(object format, params object[] args) => Log(LogLevel.Critical, format, args);

        public void Log(LogLevel logLevel, object format, params object[] args) => Log(true, logLevel, format, args);

        public void Log(bool writeToFile, LogLevel logLevel, object format, params object[] args)
        {
            string message = string.Format(format.ToString(), args);

            ConsoleColor color;
            string level = LogLevelToString(logLevel, out color);

            string prettyMessage = $"{DateTime.Now:MM/dd/yyyy HH:mm:ss} [{level}] {message}";

            lock (writeLock)
            {

                Console.Write($"{DateTime.Now:MM/dd/yyyy HH:mm:ss} [");
                ConsoleColor oldColor = Console.BackgroundColor;
                Console.BackgroundColor = color;
                Console.Write(level);
                Console.BackgroundColor = oldColor;
                Console.WriteLine($"] {message}");

                if (writeToFile)
                    m_fileBuffer.AppendLine(prettyMessage);

                //Things might go very bad after a critical thing happened so we just write instantly
                if (logLevel == LogLevel.Critical)
                    WriteBuffer();
            }
        }

        private string LogLevelToString(LogLevel logLevel, out ConsoleColor color)
        {
            switch(logLevel)
            {
                case LogLevel.Info:
                    color = ConsoleColor.Blue;
                    return "Info";
                case LogLevel.Warning:
                    color = ConsoleColor.Yellow;
                    return "Warning";
                case LogLevel.Critical:
                    color = ConsoleColor.Red;
                    return "Critical";
                default:
                    color = ConsoleColor.White;
                    return "Unknown";
            }
        }

        private void WriteBuffer()
        {
            LogInfo("Writing log buffer to file");
            lock (writeLock)
            {
                string data = m_fileBuffer.ToString().TrimEnd();
                using (StreamWriter writer = File.AppendText(LogFilePath))
                    writer.WriteLine(data);
                m_fileBuffer.Clear();
            }
            LogInfo("done writing log buffer to file");
        }

        public void Dispose()
        {
            tokenSource.Cancel();
        }
    }
}