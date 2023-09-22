using SaikoroTravelCommon.Models;

using System;
using System.Collections.Generic;

namespace SaikoroTravelCommon.Time
{
    public class DebugTimer : ITimer
    {
        private readonly PriorityQueue<Action<DateTime>, DateTime> priorityQueue = new PriorityQueue<Action<DateTime>, DateTime>();

        public DateTime Now { get; set; }

        public void AddCallback(DateTime time, Action<DateTime> callback)
        {
            priorityQueue.Enqueue(callback, time);
        }

        public void AddInstructionNotification(Instruction instruction)
        {
            // do nothing
        }

        public void AddLotteryNotification(Lottery lottery)
        {
            // do nothing
        }

        public void Dispose()
        {
            // do nothing
        }

        public void MoveNext()
        {
            if (priorityQueue.TryDequeue(out Action<DateTime>? action, out DateTime time))
            {
                Now = time;
                action.Invoke(time);

                while (priorityQueue.TryPeek(out _, out DateTime time2) && time == time2)
                {
                    priorityQueue.Dequeue().Invoke(time);
                }
            }
        }
    }
}
