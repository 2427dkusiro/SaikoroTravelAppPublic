using Android.App;
using Android.Content;
using Android.Gms.Tasks;

using Firebase.Messaging;

using SaikoroTravelApp;
using SaikoroTravelApp.Activities;
using SaikoroTravelApp.Backends;

using SaikoroTravelCommon.HttpModels;

using System;
using System.Threading.Tasks;

using Task = System.Threading.Tasks.Task;

namespace FCMClient
{
    [Service(Exported = false)]
    [IntentFilter(new[] { "com.google.firebase.MESSAGING_EVENT" })]
    public class MyFirebaseIIDService : FirebaseMessagingService
    {
        private Logger TryGetLogger()
        {
            DependencyProvider dependencyProvider = DependencyProvider.Default;
            ConfigManager config = dependencyProvider.RequestConfigManager(this);
            return config.TryGetUser(out SaikoroTravelCommon.Models.Users user) ? Logger.GetInstance(this, dependencyProvider.RequestTimer(this), user) : null;
        }

        public override void OnNewToken(string token)
        {
            TryGetLogger()?.Info("new token received", token);
            DependencyProvider dependencyProvider = DependencyProvider.Default;
            ConfigManager config = dependencyProvider.RequestConfigManager(this);
            if (config.TryGetUser(out SaikoroTravelCommon.Models.Users user))
            {
                _ = Task.Run(() => APIService.PostUserToken(this, new SaikoroTravelCommon.HttpModels.UserRegisterRequest()
                {
                    User = user,
                    Code = "", // this is a magic!!(笑),
                    Token = token
                }));
            }
        }

        public static async Task UpdateToken(Context context)
        {
            var token = await GetToken();
            DependencyProvider dependencyProvider = DependencyProvider.Default;
            ConfigManager config = dependencyProvider.RequestConfigManager(context);
            if (config.TryGetUser(out SaikoroTravelCommon.Models.Users user))
            {
                _ = await APIService.PostUserToken(context, new SaikoroTravelCommon.HttpModels.UserRegisterRequest()
                {
                    User = user,
                    Code = "", // this is a magic!!(笑),
                    Token = token
                });
            }
        }

        public static System.Threading.Tasks.Task<string> GetToken()
        {
            var taskCompletionSource = new TaskCompletionSource<string>();
            _ = FirebaseMessaging.Instance.GetToken().AddOnCompleteListener(new Listener(task =>
            {
                var token = (Java.Lang.String)task.Result;
                taskCompletionSource.SetResult((string)token);
            }));

            return taskCompletionSource.Task;
        }

        public override void OnMessageReceived(RemoteMessage remoteMessage)
        {
            if (remoteMessage != null)
            {
                // 通知メッセージ
                RemoteMessage.Notification notification = remoteMessage.GetNotification();
                if (notification != null)
                {
                    var title = notification.Title;
                    var body = notification.Body;

                    TryGetLogger()?.Info($"FCM通知'{title}'を受信。");
                    SaikoroTravelApp.Notifications.NotificationService.Show(this, title, body);
                    return;
                }

                // データメッセージ
                System.Collections.Generic.IDictionary<string, string> data = remoteMessage.Data;
                if (data != null)
                {
                    var action = data[FCMProtocol.Key_Action];
                    var type = data[FCMProtocol.Key_Type];
                    var id = data[FCMProtocol.Key_Id];
                    string number = null;
                    if (data.ContainsKey(FCMProtocol.Key_Lottery_Number))
                    {
                        number = data[FCMProtocol.Key_Lottery_Number];
                    }
                    TryGetLogger()?.Info($"FCMデータ受信,action='{action}',type='{type}',id='{id}'");
                    HandleData(action, type, id, number);
                }
            }
        }

        private void HandleData(string action, string type, string id, string number)
        {
            DependencyProvider dependencyProvider = DependencyProvider.Default;
            if (action == FCMProtocol.Action_Sync)
            {
                if (type == FCMProtocol.Type_Instruction)
                {
                    SaikoroTravelCommon.Models.Instruction instruction = dependencyProvider.RequestInstructionManager(this).FindById(id);
                    foreach (SaikoroTravelCommon.Models.RouteUpdateAction update in instruction.RouteUpdates)
                    {
                        update.Apply(dependencyProvider.RequestRouteManager(this));
                    }
                    instruction.SetState(SaikoroTravelCommon.Models.RouteState.Executed);
                    return;
                }
                if (type == FCMProtocol.Type_Lottery)
                {
                    SaikoroTravelCommon.Models.Lottery lottery = dependencyProvider.RequestLotteryManager(this).FindById(id);
                    SaikoroTravelCommon.Models.LotteryOption option = lottery.LotteryOptions[int.Parse(number) - 1];
                    foreach (SaikoroTravelCommon.Models.InstructionUpdateAction update in option.InstructionUpdates)
                    {
                        update.Apply(dependencyProvider.RequestInstructionManager(this));
                    }
                    lottery.SetLotteryState(SaikoroTravelCommon.Models.RouteState.Executed);
                    return;
                }
            }
            if (action == FCMProtocol.Action_Notification)
            {
                if (type == FCMProtocol.Type_Instruction)
                {
                    SaikoroTravelCommon.Models.Instruction instruction = dependencyProvider.RequestInstructionManager(this).FindById(id);
                    if (instruction.InstructionState != SaikoroTravelCommon.Models.RouteState.WaitingForActivation)
                    {
                        return;
                    }
                    var intent = new Intent(this, typeof(OpenInstructionActivity));
                    _ = intent.PutExtra("id", instruction.Id);
                    var pending = PendingIntent.GetActivity(this, 0, intent, PendingIntentFlags.UpdateCurrent | PendingIntentFlags.Immutable);
                    SaikoroTravelApp.Notifications.NotificationService.Show(this, "新しい指示", "新しい指示が公開されました。確認してください！", pending);
                    return;
                }
                if (type == FCMProtocol.Type_Lottery)
                {
                    SaikoroTravelCommon.Models.Lottery lottery = dependencyProvider.RequestLotteryManager(this).FindById(id);
                    if (lottery.LotteryState != SaikoroTravelCommon.Models.RouteState.WaitingForActivation)
                    {
                        return;
                    }
                    var intent = new Intent(this, typeof(OpenLotteryActivity));
                    _ = intent.PutExtra("id", lottery.Id);
                    var pending = PendingIntent.GetActivity(this, 0, intent, PendingIntentFlags.UpdateCurrent | PendingIntentFlags.Immutable);
                    SaikoroTravelApp.Notifications.NotificationService.Show(this, "新しい抽選", "新しい抽選が公開されました。実行してください！", pending);
                    return;
                }
            }
        }
    }

    public class Listener : Java.Lang.Object, IOnCompleteListener
    {
        private readonly Action<Android.Gms.Tasks.Task> action;

        public Listener(Action<Android.Gms.Tasks.Task> action)
        {
            this.action = action;
        }

        public void OnComplete(Android.Gms.Tasks.Task task)
        {
            action(task);
        }
    }
}