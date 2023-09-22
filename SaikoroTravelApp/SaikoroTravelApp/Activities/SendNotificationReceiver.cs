using Android.App;
using Android.Content;

using SaikoroTravelApp.Backends;

using System;

namespace SaikoroTravelApp.Activities
{
    [BroadcastReceiver]
    public class SendNotificationReceiver : BroadcastReceiver
    {
        private Logger logger;

        public override void OnReceive(Context context, Intent intent)
        {
            DependencyProvider dependencyProvider = DependencyProvider.Default;
            logger = Logger.GetInstance(context, dependencyProvider.RequestTimer(context), dependencyProvider.RequestConfigManager(context).User);
            logger.Info("通知レシーバー開始");

            var id = intent.GetStringExtra("id");
            if (id is null)
            {
                var ex = new InvalidOperationException("idがintentに設定されていません");
                logger.Error("idがintentに設定されていません", ex);
                throw ex;
            }

            var opType = intent.GetStringExtra("type");
            if (opType is null)
            {
                var ex = new InvalidOperationException("opTypeがintentに設定されていません");
                logger.Error("opTypeがintentに設定されていません", ex);
                throw ex;
            }

            Send(context, opType, id);
        }

        private void Send(Context context, string opType, string id)
        {
            if (opType == "Instruction")
            {
                var intent = new Intent(context, typeof(NotificationActivity));
                _ = intent.PutExtra("type", opType);
                _ = intent.PutExtra("id", id);
                var pendingIntent = PendingIntent.GetActivity(context, 0, intent, PendingIntentFlags.UpdateCurrent | PendingIntentFlags.Immutable);
                Notifications.NotificationService.Show(context, "新しい指示", "新しい指示を実行してください", pendingIntent);
            }
            else if (opType == "Lottery")
            {
                var intent = new Intent(context, typeof(NotificationActivity));
                _ = intent.PutExtra("type", opType);
                _ = intent.PutExtra("id", id);
                var pendingIntent = PendingIntent.GetActivity(context, 0, intent, PendingIntentFlags.UpdateCurrent | PendingIntentFlags.Immutable);
                Notifications.NotificationService.Show(context, "新しい抽選", "新しい抽選を実行してください", pendingIntent);
            }
            else if (opType == "Test")
            {
                Notifications.NotificationService.Show(context, "テスト通知", "配信される通知のテストです");
            }
            else
            {
                logger.Error($"'{opType}'は無効な指定です");
            }
        }
    }
}