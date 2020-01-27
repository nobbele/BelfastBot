using System;

namespace BelfastBot
{
    public static class DateTimeHelper
    {
        public static bool TryParseRelative(string input, out DateTime time)
        {
            time = DateTime.Now;

            string[] split = input.Split(' ');
            int value = int.Parse(split[0]);
            string scale = split[1];
            switch(scale) 
            {
                case "second":
                case "seconds": 
                {
                    time = time.AddSeconds(value);
                } break;
                case "minute":
                case "minutes": 
                {
                    time = time.AddMinutes(value);
                } break;
                case "hour":
                case "hours": 
                {
                    time = time.AddHours(value);
                } break;
                case "day":
                case "days": 
                {
                    time = time.AddDays(value);
                } break;
                default: 
                {
                    return false;
                }
            }

            return true;
        }
    }
}