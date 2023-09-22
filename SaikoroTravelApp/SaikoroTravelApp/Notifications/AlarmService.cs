using Android.App;
using Android.Content;
using Android.Runtime;

using SaikoroTravelApp.Activities;

using SaikoroTravelCommon.Models;

using System;
using System.Collections.Generic;

namespace SaikoroTravelApp.Notifications
{
    internal class AlarmService
    {
        public static void SetInstructionAlarm(Context context, Instruction instruction)
        {
            DateTime target = instruction.DesiredNotificationTime;
            var intent = new Intent(context, typeof(SendNotificationReceiver));
            _ = intent.PutExtra("type", "Instruction");
            _ = intent.PutExtra("id", instruction.Id);
            var pending = PendingIntent.GetBroadcast(context, GetRequestCode(instruction), intent, PendingIntentFlags.UpdateCurrent | PendingIntentFlags.Immutable);
            SetAlarm(context, pending, target);
        }

        public static void SetInstructionAlarms(Context context, IEnumerable<Instruction> instructions)
        {
            foreach (Instruction instruction in instructions)
            {
                SetInstructionAlarm(context, instruction);
            }
        }

        public static void SetLotteryAlarm(Context context, Lottery lottery)
        {
            DateTime target = lottery.DesiredNotificationTime;
            var intent = new Intent(context, typeof(SendNotificationReceiver));
            _ = intent.PutExtra("type", "Lottery");
            _ = intent.PutExtra("id", lottery.Id);
            var pending = PendingIntent.GetBroadcast(context, GetRequestCode(lottery), intent, PendingIntentFlags.UpdateCurrent | PendingIntentFlags.Immutable);
            SetAlarm(context, pending, target);
        }

        public static void SetLotteryAlarms(Context context, IEnumerable<Lottery> lotteries)
        {
            foreach (Lottery lottery in lotteries)
            {
                SetLotteryAlarm(context, lottery);
            }
        }

        private const int instructionCode = 0b_0100_0000 << 24;
        private const int lotteryCode = 0b_0010_0000 << 24;

        private static int GetRequestCode(Instruction instruction)
        {
            return instructionCode | instruction.GetHashCode();
        }

        private static int GetRequestCode(Lottery lottery)
        {
            return lotteryCode | lottery.GetHashCode();
        }

        public static void SetAlarm(Context context, PendingIntent intent, int delay = 10)
        {
            DateTime target = DateTime.Now.AddSeconds(delay).ToUniversalTime();
            AlarmManager alarmManager = context.GetSystemService(Java.Lang.Class.FromType(typeof(AlarmManager))).JavaCast<AlarmManager>();
            alarmManager.SetExactAndAllowWhileIdle(AlarmType.RtcWakeup, ToMillis(target), intent);
        }

        public static void SetAlarm(Context context, PendingIntent intent, DateTime time)
        {
            DateTime target = time.ToUniversalTime();
            AlarmManager alarmManager = context.GetSystemService(Java.Lang.Class.FromType(typeof(AlarmManager))).JavaCast<AlarmManager>();
            alarmManager.SetExactAndAllowWhileIdle(AlarmType.RtcWakeup, ToMillis(target), intent);
        }

        private static long ToMillis(DateTime dateTime)
        {
            var time = DateTime.SpecifyKind(dateTime, DateTimeKind.Local);
            var unix = new DateTimeOffset(time).ToUnixTimeMilliseconds();
            return unix;
        }
    }
}