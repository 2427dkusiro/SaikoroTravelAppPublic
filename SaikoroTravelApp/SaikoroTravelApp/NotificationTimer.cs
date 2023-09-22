using Android.Content;

using SaikoroTravelCommon.Models;
using SaikoroTravelCommon.Time;

using System;

using Xamarin.Essentials;

namespace SaikoroTravelApp
{
    internal class NotificationTimer : TimerBase
    {
        private readonly Context context;

        public NotificationTimer(Context context)
        {
            this.context = context;
        }

        public override void Dispatch(Action action)
        {
            MainThread.BeginInvokeOnMainThread(action);
        }

        public override void AddInstructionNotification(Instruction instruction)
        {
            Notifications.AlarmService.SetInstructionAlarm(context, instruction);
        }

        public override void AddLotteryNotification(Lottery lottery)
        {
            Notifications.AlarmService.SetLotteryAlarm(context, lottery);
        }
    }
}