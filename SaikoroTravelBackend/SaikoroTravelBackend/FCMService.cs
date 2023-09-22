using FirebaseAdmin;
using FirebaseAdmin.Messaging;

using Google.Apis.Auth.OAuth2;

using SaikoroTravelCommon.HttpModels;

namespace SaikoroTravelBackend;

public class FCMService
{
    private static readonly string credential =
        @"{your_credential_json}";

    public static void Init()
    {
        _ = FirebaseApp.Create(new AppOptions()
        {
            Credential = GoogleCredential.FromJson(credential)
        });
    }

    public static async Task SendInstructonReports(IEnumerable<string> tokens, string id)
    {
        foreach (var token in tokens)
        {
            var message = new Message()
            {
                Data = new Dictionary<string, string>()
                {
                    { FCMProtocol.Key_Action, FCMProtocol.Action_Sync },
                    { FCMProtocol.Key_Type, FCMProtocol.Type_Instruction },
                    { FCMProtocol.Key_Id, id },
                },
                Token = token,
                Android = new AndroidConfig()
                {
                    Priority = Priority.Normal,
                }
            };
            _ = await FirebaseMessaging.DefaultInstance.SendAsync(message);
        }
    }

    public static async Task SendLotteryReports(IEnumerable<string> tokens, string id, int number)
    {
        foreach (var token in tokens)
        {
            var message = new Message()
            {
                Data = new Dictionary<string, string>()
                {
                    { FCMProtocol.Key_Action, FCMProtocol.Action_Sync },
                    { FCMProtocol.Key_Type, FCMProtocol.Type_Lottery },
                    { FCMProtocol.Key_Id, id },
                    { FCMProtocol.Key_Lottery_Number , number.ToString() }
                },
                Token = token,
                Android = new AndroidConfig()
                {
                    Priority = Priority.Normal,
                }
            };
            _ = await FirebaseMessaging.DefaultInstance.SendAsync(message);
        }
    }

    public static async Task SendInstructonNotification(IEnumerable<string> tokens, string id)
    {
        foreach (var token in tokens)
        {
            var message = new Message()
            {
                Data = new Dictionary<string, string>()
                {
                    { FCMProtocol.Key_Action, FCMProtocol.Action_Notification },
                    { FCMProtocol.Key_Type, FCMProtocol.Type_Instruction },
                    { FCMProtocol.Key_Id, id },
                },
                Token = token,
                Android = new AndroidConfig()
                {
                    Priority = Priority.High,
                }
            };
            _ = await FirebaseMessaging.DefaultInstance.SendAsync(message);
        }
    }

    public static async Task SendLotteryNotification(IEnumerable<string> tokens, string id)
    {
        foreach (var token in tokens)
        {
            var message = new Message()
            {
                Data = new Dictionary<string, string>()
                {
                    { FCMProtocol.Key_Action, FCMProtocol.Action_Notification },
                    { FCMProtocol.Key_Type, FCMProtocol.Type_Lottery },
                    { FCMProtocol.Key_Id, id },
                },
                Token = token,
                Android = new AndroidConfig()
                {
                    Priority = Priority.High
                }
            };
            _ = await FirebaseMessaging.DefaultInstance.SendAsync(message);
        }
    }
}
