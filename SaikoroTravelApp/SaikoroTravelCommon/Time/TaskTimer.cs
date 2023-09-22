using SaikoroTravelCommon.Models;

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace SaikoroTravelCommon.Time
{
    [Obsolete]
    public class TaskTimer : ITimer
    {
        private readonly PriorityQueue<Action<DateTime>, DateTime> priorityQueue = new PriorityQueue<Action<DateTime>, DateTime>();

        public DateTime Now => DateTime.Now;

        public void AddCallback(DateTime time, Action<DateTime> callback)
        {
            priorityQueue.Enqueue(callback, time);

            cancellationTokenSource?.Cancel();
            cancellationTokenSource = new CancellationTokenSource();
            _ = Task.Run(() => WaitNext(cancellationTokenSource.Token));
        }

        private CancellationTokenSource? cancellationTokenSource = default;

        private readonly TimeSpan margin = new TimeSpan(0, 0, 0, 0, 100);

        public async Task WaitNext(CancellationToken cancellationToken)
        {
            if (priorityQueue.TryPeek(out _, out DateTime time))
            {
                TimeSpan diff = time + margin - DateTime.Now;
                await Task.Delay(diff, cancellationToken);
                if (!cancellationToken.IsCancellationRequested)
                {
                    priorityQueue.Dequeue().Invoke(Now);
                    while (priorityQueue.TryPeek(out _, out DateTime time2) && time2 < Now)
                    {
                        priorityQueue.Dequeue().Invoke(time);
                    }
                }
            };
        }

        public void Dispose()
        {
            cancellationTokenSource?.Dispose();
        }

        public void AddInstructionNotification(Instruction instruction)
        {

        }

        public void AddLotteryNotification(Lottery lottery)
        {

        }
    }
}
