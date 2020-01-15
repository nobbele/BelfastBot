using System;
using System.Threading.Tasks;

namespace SenkoSanBot.Services.Scheduler
{
    public class SchedulerService
    {
        public void Add(DateTime time, Action func)
        {
            TimeSpan delay = time - DateTime.Now;
            if(delay.TotalMilliseconds < 0)
                return;
            Task.Delay(delay).ContinueWith(task => func?.Invoke());
        }

        public void Add(DateTime time, Func<Task, object> func)
        {
            TimeSpan delay = time - DateTime.Now;
            if(delay.TotalMilliseconds < 0)
                return;
            Task.Delay(delay).ContinueWith(func);
        }
    }
}