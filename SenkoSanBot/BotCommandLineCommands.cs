using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SenkoSanBot
{
    public static class BotCommandLineCommands
    {
        public static void Stop(SenkoSan senko)
        {
            senko.Stop();
        }
    }
}
