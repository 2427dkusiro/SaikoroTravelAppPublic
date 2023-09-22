using Android.Content;
using Android.OS;
using Android.Views;
using Android.Widget;

using AndroidX.Fragment.App;

using SaikoroTravelApp.Activities;
using SaikoroTravelApp.Backends;
using SaikoroTravelApp.Components;
using SaikoroTravelApp.VuewModelHelpers;

using SaikoroTravelCommon.Models;

using System;
using System.Linq;

namespace SaikoroTravelApp.Fragments.MainTab
{
    public class Home : Fragment
    {
        private Logger logger;
        private DependencyProvider _dependencyProvider;

        private ConfigManager configManager;
        private RouteManager routeManager;
        private InstructionManager instructionManager;
        private LotteryManager lotteryManager;

        private LinearLayout currentRouteLayout;
        private LinearLayout nextRouteLayout;
        private LinearLayout bottomLayout;
        private Button openInstructionButton;
        private Button openLotteryButton;

        private RouteView nextRoute;
        private RouteView currentRoute;

        private RouteViewModelhelper currentHelper;
        private RouteViewModelhelper nextHelper;

        private Instruction canOpenInstruction;
        private Lottery canOpenLottery;

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            var layout = Resource.Layout.fragment_home;
            View view = inflater.Inflate(layout, container, false);

            bottomLayout = view.FindViewById<LinearLayout>(Resource.Id.HomeBottomUI);
            currentRouteLayout = view.FindViewById<LinearLayout>(Resource.Id.CurrentRouteLayout);
            nextRouteLayout = view.FindViewById<LinearLayout>(Resource.Id.NextRouteLayout);
            nextRoute = view.FindViewById<RouteView>(Resource.Id.NextRoute);
            currentRoute = view.FindViewById<RouteView>(Resource.Id.CurrentRoute);
            openInstructionButton = view.FindViewById<Button>(Resource.Id.OpenInstructionButton);
            openInstructionButton.Click += OnOpenInstructionClicked;
            openLotteryButton = view.FindViewById<Button>(Resource.Id.OpenLotteryButton);
            openLotteryButton.Click += OnOpenLotteryClicked;

            _dependencyProvider = DependencyProvider.Default;
            configManager = _dependencyProvider.RequestConfigManager(Context);

            logger = Logger.GetInstance(Context, _dependencyProvider.RequestTimer(Context), configManager.User);
            routeManager = _dependencyProvider.RequestRouteManager(Context);
            instructionManager = _dependencyProvider.RequestInstructionManager(Context);
            lotteryManager = _dependencyProvider.RequestLotteryManager(Context);

            SetRouteView();
            routeManager.StateChanged += (sender, e) => SetRouteView();
            SetInstructionOrLottery();
            instructionManager.StateChanged += (sender, e) => SetInstructionOrLottery();
            lotteryManager.StateChanged += (sender, e) => SetInstructionOrLottery();

            LinearLayout debugPanel = view.FindViewById<LinearLayout>(Resource.Id.DebugPanel);
            // debugPanel.Visibility = ViewStates.Gone;

            TextView timeText = view.FindViewById<TextView>(Resource.Id.Debug_NowTimeText);
            Button skipTimeButton = view.FindViewById<Button>(Resource.Id.Debug_SkipTimeButton);
            if (_dependencyProvider.RequestTimer(Context) is SaikoroTravelCommon.Time.DebugTimer debugTimer)
            {
                timeText.Text = $"疑似時刻:{debugTimer.Now}";
                skipTimeButton.Click += (sender, e) =>
                {
                    debugTimer.MoveNext();
                    timeText.Text = $"疑似時刻:{debugTimer.Now}";
                };
            }

            return view;
        }

        private void OnOpenInstructionClicked(object sender, EventArgs e)
        {
            if (canOpenInstruction is null)
            {
                logger?.Warn("指示開封ボタンが押されましたが、指示がnullでした");
                SetBottomVisibility();
                return;
            }
            logger?.Info($"ホーム画面より指示'{canOpenInstruction.Id}'を開きます");

            var intent = new Intent(Context, typeof(OpenInstructionActivity));
            _ = intent.PutExtra("id", canOpenInstruction.Id);
            StartActivity(intent);
        }

        private void OnOpenLotteryClicked(object sender, EventArgs e)
        {
            if (canOpenLottery is null)
            {
                logger?.Warn("抽選開封ボタンが押されましたが、抽選がnullでした");
                SetBottomVisibility();
                return;
            }
            logger?.Info($"ホーム画面より抽選'{canOpenLottery.Id}'を開きます");

            var intent = new Intent(Context, typeof(OpenLotteryActivity));
            _ = intent.PutExtra("id", canOpenLottery.Id);
            StartActivity(intent);
        }

        private void SetRouteView()
        {
            Route[] activatedRoute = routeManager.Routes
               .Where(route => route.UserInfo.HasPermission(configManager.User) && route.RouteState == RouteState.Activated)
               .OrderBy(route => route.DepartureTime)
               .Take(2)
               .ToArray();

            currentRouteLayout.Visibility = ViewStates.Gone;
            nextRouteLayout.Visibility = ViewStates.Gone;

            if (activatedRoute.Length > 0)
            {
                currentHelper = new RouteViewModelhelper(Context, activatedRoute[0]);
                currentRoute.ViewModel = currentHelper.ViewModel;
                currentRouteLayout.Visibility = ViewStates.Visible;
            }
            if (activatedRoute.Length > 1)
            {
                nextHelper = new RouteViewModelhelper(Context, activatedRoute[1]);
                nextRoute.ViewModel = nextHelper.ViewModel;
                nextRouteLayout.Visibility = ViewStates.Visible;
            }
        }

        private void SetInstructionOrLottery()
        {
            canOpenInstruction = instructionManager?.Instructions
                ?.Where(instruction => instruction.UserInfo.HasPermission(configManager.User) && instruction.InstructionState == RouteState.Activated)
                ?.OrderBy(instruction => instruction.DesiredNotificationTime)
                ?.FirstOrDefault();

            canOpenLottery = lotteryManager?.Lotteries
               ?.Where(lottery => lottery.UserInfo.HasPermission(configManager.User) && lottery.LotteryState == RouteState.Activated)
               ?.OrderBy(lottery => lottery.DesiredNotificationTime)
               ?.FirstOrDefault();

            if (canOpenInstruction != null && canOpenLottery != null)
            {
                if (canOpenInstruction.DesiredNotificationTime < canOpenLottery.DesiredNotificationTime)
                {
                    canOpenLottery = null;
                }
                else
                {
                    canOpenInstruction = null;
                }
            }

            SetBottomVisibility();
        }

        private void SetBottomVisibility()
        {
            openInstructionButton.Visibility = canOpenInstruction is null ? ViewStates.Gone : ViewStates.Visible;
            openLotteryButton.Visibility = canOpenLottery is null ? ViewStates.Gone : ViewStates.Visible;

            bottomLayout.Visibility = (canOpenInstruction is null && canOpenLottery is null) ? ViewStates.Gone : ViewStates.Visible;
        }

        public override void OnStart()
        {
            base.OnStart();
            routeManager?.ForceUpdateNow();
            instructionManager?.ForceUpdate();
            lotteryManager?.ForceUpdateNow();
        }
    }
}