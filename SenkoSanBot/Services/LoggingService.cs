using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SenkoSanBot.Services
{
    public class LoggingService
    {
        /// <summary>
        /// Please use <seealso cref="Log(string)"/> if you can. This just calls that with <seealso cref="Task.Run(Action)"/>
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        public async Task LogAsync(string message = "\n") => await Task.Run(() => Log(message));

        public void Log(string message = "\n")
        {
            Console.WriteLine(message);
        }
    }
}
