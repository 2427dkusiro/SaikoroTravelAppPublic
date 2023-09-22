using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;

using AndroidX.Core.App;

namespace SaikoroTravelApp.Notifications
{
    internal static class NotificationService
    {
        private const string channelIdRealtime = "realTime";
        private const string realTimeName = "リアルタイム通知";
        private const string realTimeDescription = "旅程に関するリアルタイムの通知";

        private static int id = 0;
        private static bool isInitialized = false;

        public static void Show(Context context, string title, string message)
        {
            if (!isInitialized)
            {
                CreateChannel(
                    context.GetSystemService(Java.Lang.Class.FromType(typeof(NotificationManager))).JavaCast<NotificationManager>());
                isInitialized = true;
            }

            NotificationCompat.Builder builder = new NotificationCompat.Builder(context, channelIdRealtime)
                .SetSmallIcon(Resource.Mipmap.ic_launcher_round)
                .SetContentTitle(title)
                .SetContentText(message)
                .SetPriority(NotificationCompat.PriorityHigh)
                .SetAutoCancel(true);
            var notificationManagerCompat = NotificationManagerCompat.From(context);
            notificationManagerCompat.Notify(id++, builder.Build());
        }

        public static void Show(Context context, string title, string message, PendingIntent intent)
        {
            if (!isInitialized)
            {
                CreateChannel(
                    context.GetSystemService(Java.Lang.Class.FromType(typeof(NotificationManager))).JavaCast<NotificationManager>());
                isInitialized = true;
            }

            NotificationCompat.Builder builder = new NotificationCompat.Builder(context, channelIdRealtime)
                .SetSmallIcon(Resource.Mipmap.ic_launcher_round)
                .SetContentTitle(title)
                .SetContentText(message)
                .SetPriority(NotificationCompat.PriorityHigh)
                .SetContentIntent(intent)
                .SetAutoCancel(true);
            var notificationManagerCompat = NotificationManagerCompat.From(context);
            notificationManagerCompat.Notify(id++, builder.Build());
        }

        private static void CreateChannel(NotificationManager notificationManager)
        {
            if (Build.VERSION.SdkInt >= BuildVersionCodes.O)
            {
                var notificationChannel = new NotificationChannel(channelIdRealtime, realTimeName, NotificationImportance.High)
                {
                    Description = realTimeDescription
                };
                notificationManager.CreateNotificationChannel(notificationChannel);
            }
        }
    }
}