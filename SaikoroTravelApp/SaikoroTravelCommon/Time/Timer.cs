using SaikoroTravelCommon.Models;

using System;
using System.Collections.Generic;

namespace SaikoroTravelCommon.Time
{
    public abstract class TimerBase : ITimer
    {
        public DateTime Now => DateTime.Now;

        private readonly Stack<System.Timers.Timer> timers = new Stack<System.Timers.Timer>();

        public void AddCallback(DateTime time, Action<DateTime> callback)
        {
            var delay = (time - Now).TotalMilliseconds;
            var timer = new System.Timers.Timer(delay);
            timer.Elapsed += (sender, e) => Dispatch(() => callback(Now));
            timer.AutoReset = false;
            timer.Enabled = true;

            timers.Push(timer);
        }

        public void Dispose()
        {
            foreach (System.Timers.Timer timer in timers)
            {
                timer.Dispose();
            }
            timers.Clear();
        }

        public virtual void Dispatch(Action action)
        {
            action();
        }

        public abstract void AddInstructionNotification(Instruction instruction);

        public abstract void AddLotteryNotification(Lottery lottery);
    }
}
