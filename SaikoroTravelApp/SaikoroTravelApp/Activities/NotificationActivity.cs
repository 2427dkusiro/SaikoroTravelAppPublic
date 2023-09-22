using Android.App;
using Android.OS;

using SaikoroTravelCommon.Models;

using System;
using System.Linq;
using System.Threading.Tasks;

using Intent = Android.Content.Intent;
using Logger = SaikoroTravelApp.Backends.Logger;

namespace SaikoroTravelApp.Activities
{
    [Activity(Label = "NotificationActivity")]
    public class NotificationActivity : Activity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            DependencyProvider dependencyProvider = DependencyProvider.Default;
            var logger = Logger.GetInstance(this, dependencyProvider.RequestTimer(this), dependencyProvider.RequestConfigManager(this).User);

            var id = Intent.GetStringExtra("id");
            if (id is null)
            {
                var ex = new InvalidOperationException("idがintentに設定されていません");
                logger.Error("idがintentに設定されていません", ex);
                throw ex;
            }

            var opType = Intent.GetStringExtra("type");
            if (opType is null)
            {
                var ex = new InvalidOperationException("opTypeがintentに設定されていません");
                logger.Error("opTypeがintentに設定されていません", ex);
                throw ex;
            }

            InstructionManager instructionManager = dependencyProvider.RequestInstructionManager(this);
            LotteryManager lotteryManager = dependencyProvider.RequestLotteryManager(this);
            ConfigManager configManager = dependencyProvider.RequestConfigManager(this);

            Instruction canOpenInstruction = instructionManager?.Instructions
                ?.Where(instruction => instruction.UserInfo.HasPermission(configManager.User) && instruction.InstructionState == RouteState.Activated)
                ?.OrderBy(instruction => instruction.DesiredNotificationTime)
                ?.FirstOrDefault();

            Lottery canOpenLottery = lotteryManager?.Lotteries
               ?.Where(lottery => lottery.UserInfo.HasPermission(configManager.User) && lottery.LotteryState == RouteState.Activated)
               ?.OrderBy(lottery => lottery.DesiredNotificationTime)
               ?.FirstOrDefault();

            if (canOpenInstruction is null && canOpenLottery is null)
            {
                logger.Warn("通知が開かれましたが、開封可能な指示および抽選がありません");
                Finish();
                return;
            }

            if (opType == "Instruction")
            {
                if (canOpenLottery != null)
                {
                    logger.Warn("指示と抽選がともに開けるので、通知からの指示直接起動を拒否します。");
                    Finish();
                    return;
                }
                if (canOpenInstruction?.Id != id)
                {
                    logger.Warn($"開かれた通知の指示ID'{id}'が、開ける指示ID'{canOpenInstruction?.Id}'と異なるため、通知からの指示直接起動を拒否します。");
                    Finish();
                    return;
                }

                _ = Task.Run(async () => await logger.ReportInstructionTimerWaked(canOpenInstruction));
                var intent = new Intent(this, typeof(OpenInstructionActivity));
                _ = intent.PutExtra("id", id);
                StartActivity(intent);
                Finish();
            }
            else if (opType == "Lottery")
            {
                if (canOpenInstruction != null)
                {
                    logger.Warn("指示と抽選がともに開けるので、通知からの直接起動を拒否します。");
                    Finish();
                    return;
                }
                if (canOpenLottery?.Id != id)
                {
                    logger.Warn($"開かれた通知の抽選ID'{id}'が、開ける抽選ID'{canOpenInstruction?.Id}'と異なるため、通知からの抽選直接起動を拒否します。");
                    Finish();
                    return;
                }

                _ = Task.Run(async () => await logger.ReportLotteryTimerWaked(canOpenLottery));
                var intent = new Intent(this, typeof(OpenLotteryActivity));
                _ = intent.PutExtra("id", id);
                StartActivity(intent);
            }
            else
            {
                logger.Warn($"予期せぬ操作タイプ'{opType}'です。");
                Finish();
                return;
            }
        }
    }
}