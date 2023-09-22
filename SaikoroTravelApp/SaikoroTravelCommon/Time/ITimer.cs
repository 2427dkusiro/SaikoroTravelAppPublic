using SaikoroTravelCommon.Models;

using System;

namespace SaikoroTravelCommon.Time
{
    public interface ITimer
    {
        public DateTime Now { get; }

        public void AddCallback(DateTime time, Action<DateTime> callback);

        public void AddInstructionNotification(Instruction instruction);

        public void AddLotteryNotification(Lottery lottery);

        public void Dispose();
    }
}
