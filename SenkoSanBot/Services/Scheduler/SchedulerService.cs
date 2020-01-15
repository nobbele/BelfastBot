using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Threading;

namespace SenkoSanBot.Services.Scheduler
{
    public class SchedulerService
    {
        private Dictionary<Guid, DateTime> schedules;

        public void Add<T>(DateTime time, T func)
        {
            Guid guid = Guid.NewGuid();
            TimeSpan delay = time - DateTime.Now;
            if(delay.TotalMilliseconds < 0)
                return;
            if(func is Action a)
                Task.Delay(delay).ContinueWith(task => OnDone(a, guid));
            if(func is Func<Task> f)
                Task.Delay(delay).ContinueWith(task => OnDone(f, guid));
            else
                throw new ArgumentException("Unsupported function type");
            schedules.Add(guid, time);
        }

        private void OnDone(Guid guid)
        {
            schedules.Remove(guid);
        }

        private void OnDone(Action func, Guid guid)
        {
            func?.Invoke();
            OnDone(guid);
        }

        private void OnDone(Func<Task> func, Guid guid)
        {
            func?.Invoke();
            OnDone(guid);
        }
    }
}