using Android.App;
using Android.Content;
using Android.OS;
using Android.Views;
using Android.Widget;

using SaikoroTravelApp.Activities;

using Fragment = AndroidX.Fragment.App.Fragment;

namespace SaikoroTravelApp.Fragments.MainTab
{
    public class Other : Fragment
    {
        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            var layout = Resource.Layout.fragment_other;
            View view = inflater.Inflate(layout, container, false);

            Button testNotificationButton = view.FindViewById<Button>(Resource.Id.TestNotificationButton);
            testNotificationButton.Click += (sender, e) =>
            {
                Notifications.NotificationService.Show(Context, "テスト通知", "配信される通知のテストです");
            };

            Button testTimerButton = view.FindViewById<Button>(Resource.Id.TestTimerButton);
            testTimerButton.Click += (sender, e) =>
            {
                Toast.MakeText(Context, "30秒後にテスト通知が配信されます", ToastLength.Short).Show();
                var intent = new Intent(Context, typeof(SendNotificationReceiver));
                _ = intent.PutExtra("type", "Test");
                _ = intent.PutExtra("id", "00");
                var pending = PendingIntent.GetBroadcast(Context, 0, intent, PendingIntentFlags.UpdateCurrent | PendingIntentFlags.Immutable);
                Notifications.AlarmService.SetAlarm(Context, pending, 30);
            };

            // for debug only
            /*
            Button instructionNotificationButton = view.FindViewById<Button>(Resource.Id.TestInstructionNotificationButton);
            instructionNotificationButton.Click += (sender, e) =>
            {
                var intent = new Intent(Context, typeof(OpenInstructionActivity));
                _ = intent.PutExtra("id", "01");
                var pending = PendingIntent.GetActivity(Context, 0, intent, PendingIntentFlags.UpdateCurrent);
                Notifications.NotificationService.Show(Context, "新しい指示", "新しい指示が公開されました。確認してください！", pending);
            };

            Button instructionTimerButton = view.FindViewById<Button>(Resource.Id.TestInstructionTimerButton);
            instructionTimerButton.Click += (sender, e) =>
            {
                var intent = new Intent(Context, typeof(SendNotificationReceiver));
                _ = intent.PutExtra("type", "Instruction");
                _ = intent.PutExtra("id", "01");
                var pending = PendingIntent.GetBroadcast(Context, 0, intent, PendingIntentFlags.UpdateCurrent);
                Notifications.AlarmService.SetAlarm(Context, pending);
            };

            Button lotteryNotificationButton = view.FindViewById<Button>(Resource.Id.TestLotteryNotificationButton);
            lotteryNotificationButton.Click += (sender, e) =>
            {
                var intent = new Intent(Context, typeof(OpenLotteryActivity));
                _ = intent.PutExtra("id", "01");
                var pending = PendingIntent.GetActivity(Context, 0, intent, PendingIntentFlags.UpdateCurrent);
                Notifications.NotificationService.Show(Context, "新しい抽選", "新しい抽選が公開されました。実行してください！", pending);
            };

            Button lotteryTimerButton = view.FindViewById<Button>(Resource.Id.TestLotteryTimerButton);
            lotteryTimerButton.Click += (sender, e) =>
            {
                var intent = new Intent(Context, typeof(SendNotificationReceiver));
                _ = intent.PutExtra("type", "Lottery");
                _ = intent.PutExtra("id", "01");
                var pending = PendingIntent.GetBroadcast(Context, 0, intent, PendingIntentFlags.UpdateCurrent);
                Notifications.AlarmService.SetAlarm(Context, pending);
            };
            */

            return view;
        }
    }
}