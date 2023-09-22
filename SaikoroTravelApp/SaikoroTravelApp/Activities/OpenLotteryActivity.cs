using Android.App;
using Android.Content;
using Android.OS;
using Android.Views;
using Android.Widget;

using AndroidX.AppCompat.App;

using SaikoroTravelApp.Backends;
using SaikoroTravelApp.Components;

using SaikoroTravelCommon.HttpModels;
using SaikoroTravelCommon.Models;

using System;
using System.Linq;
using System.Threading.Tasks;

using AlertDialog = AndroidX.AppCompat.App.AlertDialog;

namespace SaikoroTravelApp.Activities
{
    [Activity(Label = "OpenLotteryActivity", Theme = "@style/AppTheme.NoActionBar", ScreenOrientation = Android.Content.PM.ScreenOrientation.Portrait)]
    public class OpenLotteryActivity : AppCompatActivity
    {
        private readonly Random random = new Random();
        private Logger logger;

        private Button startButton;

        private DependencyProvider dependencyProvider;
        private InstructionManager instructionManager;

        private Lottery lottery;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            SetContentView(Resource.Layout.activity_openLottery);
            if (Build.VERSION.SdkInt >= BuildVersionCodes.R)
            {
                Window.DecorView.WindowInsetsController.Hide(WindowInsets.Type.StatusBars());
            }

            dependencyProvider = DependencyProvider.Default;
            logger = Logger.GetInstance(this, dependencyProvider.RequestTimer(this), dependencyProvider.RequestConfigManager(this).User);
            var id = Intent.GetStringExtra("id");
            instructionManager = dependencyProvider.RequestInstructionManager(this);
            LotteryManager manager = dependencyProvider.RequestLotteryManager(this);

            try
            {
                lottery = manager.FindById(id);
            }
            catch (Exception ex)
            {
                logger.Error($"抽選アクティビティの引数のID'{lottery.Id}'が見つかりません", ex);
                throw;
            }
            logger.Info($"抽選'{id}'を開く抽選開封アクティビティが開始されました");

            TextView title = FindViewById<TextView>(Resource.Id.LotteryTitleText);
            title.Text = lottery.Title;

            LotteryView lotteryView = FindViewById<LotteryView>(Resource.Id.LotteryOptionLayout);
            lotteryView.Lottery = lottery;

            startButton = FindViewById<Button>(Resource.Id.StartLotteryButton);
            startButton.Click += StartButtonClick;
        }

        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, Android.Content.PM.Permission[] grantResults)
        {
            Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);

            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }

        private LotteryOption selected;

        private async void StartButtonClick(object sender, EventArgs e)
        {
            startButton.Enabled = false;

            // 権限を確認し
            var request = new LotteryRequest()
            {
                LotteryId = lottery.Id,
                User = dependencyProvider.RequestConfigManager(this).User
            };

            LotteryResponse response = await APIService.RequestLotteryOpen(this, request);

            if (response != null)
            {
                if (!response.IsOK)
                {
                    _ = new AlertDialog.Builder(this)
                    .SetTitle("抽選済み")
                    .SetMessage("この抽選はちょうど他の人が実行済みです")
                    .SetPositiveButton("OK", (sender, e) => { });
                }
                else
                {

                    selected = lottery.LotteryOptions[response.Selected - 1];
                    var intent = new Intent(this, typeof(SaikoroActivity));
                    _ = intent.PutExtra("number", selected.Number);
                    StartActivityForResult(intent, 1);
                }
            }
            else
            {
                _ = new AlertDialog.Builder(this)
                    .SetTitle("通信失敗")
                    .SetMessage("サーバーとの通信に失敗しました。\nこのまま抽選を実行することもできますが、複数人で抽選すると結果が矛盾することがあります。\n現場の責任にて抽選者を1人選んでください。\n抽選を継続しますか？")
                    .SetPositiveButton("OK", (sender, e) =>
                    {
                        selected = GetResultLocal();
                        var intent = new Intent(this, typeof(SaikoroActivity));
                        _ = intent.PutExtra("number", selected.Number);
                        StartActivityForResult(intent, 1);
                    })
                    .SetNegativeButton("キャンセル", (sender, e) => { })
                    .Show();
            }
            startButton.Enabled = true;
        }

        protected override async void OnActivityResult(int requestCode, [Android.Runtime.GeneratedEnum] Result resultCode, Intent data)
        {
            if (requestCode == 1)
            {
                if (resultCode == Result.Ok)
                {
                    await HandleOpen();
                }
            }
        }

        private async Task HandleOpen()
        {
            InstructionUpdateAction[] activation = selected.InstructionUpdates.Where(x => x is InstructionStateUpdateAction action &&
                                action.Value == RouteState.Activated && instructionManager.FindById(x.TargetId).UserInfo.HasPermission(dependencyProvider.RequestConfigManager(this).User)).ToArray();

            if (activation.Length == 1)
            {
                foreach (InstructionUpdateAction update in selected.InstructionUpdates)
                {
                    update.Apply(instructionManager);
                }
                lottery.SetLotteryState(RouteState.Executed);
                await logger.ReportLotteryOpened(lottery, selected.Number);

                var intent = new Intent(this, typeof(OpenInstructionActivity));
                _ = intent.PutExtra("id", activation[0].TargetId);
                StartActivity(intent);
                Finish();
            }
            else
            {
                foreach (InstructionUpdateAction update in selected.InstructionUpdates)
                {
                    update.Apply(instructionManager);
                }
                lottery.SetLotteryState(RouteState.Executed);
                await logger.ReportLotteryOpened(lottery, selected.Number);
                Finish();
            }
        }

        private LotteryOption GetResultLocal()
        {
            LotteryOption[] target = lottery.LotteryOptions.Where(x => x.Selectable).ToArray();

            var value = random.Next() % target.Length;
            return target[value];
        }
    }
}