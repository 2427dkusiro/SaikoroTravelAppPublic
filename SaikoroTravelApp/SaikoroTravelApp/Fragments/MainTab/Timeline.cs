using Android.OS;
using Android.Views;
using Android.Widget;

using AndroidX.Fragment.App;

using SaikoroTravelApp.Components;
using SaikoroTravelApp.VuewModelHelpers;

using SaikoroTravelCommon.Models;

using System;
using System.Collections.Generic;
using System.Linq;

using Orientation = Android.Widget.Orientation;
using Route = SaikoroTravelCommon.Models.Route;

namespace SaikoroTravelApp.Fragments.MainTab
{
    public class Timeline : Fragment
    {
        private ScrollView scrollView;
        private LinearLayout linearLayout;

        private RouteManager routeManager;
        private InstructionManager instructionManager;
        private LotteryManager lotteryManager;
        private Users user;

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            var layout = Resource.Layout.fragment_timeline;
            View view = inflater.Inflate(layout, container, false);

            scrollView = view.FindViewById<ScrollView>(Resource.Id.TimeLineMain);
            linearLayout = view.FindViewById<LinearLayout>(Resource.Id.TimeLineLayout);

            DependencyProvider dependencyProvider = DependencyProvider.Default;
            routeManager = dependencyProvider.RequestRouteManager(Context);
            instructionManager = dependencyProvider.RequestInstructionManager(Context);
            lotteryManager = dependencyProvider.RequestLotteryManager(Context);
            user = dependencyProvider.RequestConfigManager(Context).User;

            routeManager.StateChanged += (sender, e) => Render();
            instructionManager.StateChanged += (sender, e) => Render();
            lotteryManager.StateChanged += (sender, e) => Render();

            renderFlag = true;
            var listener = new Listener(() =>
            {
                if (renderFlag)
                {
                    Render();
                    renderFlag = false;
                }
            });
            scrollView.ViewTreeObserver.AddOnGlobalLayoutListener(listener);
            return view;
        }

        private bool renderFlag;


        private void Render()
        {
            IEnumerable<TimeLineObject> routes = routeManager.Routes.Where(x => x.UserInfo.HasPermission(user) && x.RouteState == RouteState.Executed
            && x.DestinationTime is DateTime).Select(x => new TimeLineObject() { Obj = x, Time = (DateTime)x.DestinationTime });

            IEnumerable<TimeLineObject> instructions = instructionManager.Instructions.Where(x => x.UserInfo.HasPermission(user)
            && (x.InstructionState == RouteState.Executed))
                .Select(x => new TimeLineObject() { Obj = x, Time = x.DesiredNotificationTime });

            IEnumerable<TimeLineObject> lotteries = lotteryManager.Lotteries.Where(x => x.UserInfo.HasPermission(user)
            && (x.LotteryState == RouteState.Executed))
                .Select(x => new TimeLineObject() { Obj = x, Time = x.DesiredNotificationTime });

            var objs = (new[] { routes, instructions, lotteries }).SelectMany(x => x)
                .OrderByDescending(x => x.Time)
                .Select(x => x.Obj)
                .ToArray();

            CreateLayout(objs);
        }

        private const int displayCountUnit = 5;
        private int displayCount = displayCountUnit;

        private void CreateLayout(IEnumerable<object> objs)
        {
            linearLayout.RemoveAllViews();
            var arr = objs.ToArray();

            foreach (var obj in arr.Take(displayCount))
            {
                if (obj is Route route)
                {
                    linearLayout.AddView(CreateRouteLayout(route));
                }
                if (obj is Instruction instruction)
                {
                    linearLayout.AddView(CreateInstructionLayout(instruction));
                }
                if (obj is Lottery lottery)
                {
                    linearLayout.AddView(CreateLotteryLayout(lottery));
                }
            }

            if (arr.Length > displayCount)
            {
                var button = new Button(Context);
                var param = new ViewGroup.LayoutParams(ViewGroup.LayoutParams.MatchParent, ViewGroup.LayoutParams.WrapContent);
                button.LayoutParameters = param;
                button.Text = "さらに読み込む";
                button.Click += (sender, e) =>
                {
                    displayCount += displayCountUnit;
                    Render();
                };
                linearLayout.AddView(button);
            }
        }

        private View CreateRouteLayout(Route route)
        {
            var param = new ViewGroup.LayoutParams(ViewGroup.LayoutParams.MatchParent, ViewGroup.LayoutParams.WrapContent);
            var routeView = new RouteView(Context, null)
            {
                LayoutParameters = param
            };
            var helper = new RouteViewModelhelper(Context, route);
            routeView.ViewModel = helper.ViewModel;

            return CreateLayoutCommon(routeView, "行程を完了しました", (DateTime)route.DestinationTime);
        }

        private View CreateInstructionLayout(Instruction instruction)
        {
            var param = new ViewGroup.LayoutParams(ViewGroup.LayoutParams.MatchParent, ViewGroup.LayoutParams.WrapContent);
            var instructionView = new InstructionView(Context, null)
            {
                LayoutParameters = param,
                ViewModel = new ViewModels.InstructionViewModel()
                {
                    Content = instruction.Content,
                    IsOpend = true,
                }
            };

            return CreateLayoutCommon(instructionView, "指示を開封しました", instruction.DesiredNotificationTime);
        }

        private View CreateLotteryLayout(Lottery lottery)
        {
            var height = (int)(scrollView.Height * 0.7);
            var param = new LinearLayout.LayoutParams(ViewGroup.LayoutParams.MatchParent, height);
            param.SetMargins(12, 12, 12, 12);
            var lotteryView = new LotteryView(Context, null)
            {
                LayoutParameters = param,
                Lottery = lottery
            };

            return CreateLayoutCommon(lotteryView, "サイコロを振りました", lottery.DesiredNotificationTime);
        }

        private View CreateLayoutCommon(View view, string title, DateTime time)
        {
            var linearLayout = new LinearLayout(Context);
            var lParam = new LinearLayout.LayoutParams(ViewGroup.LayoutParams.MatchParent, ViewGroup.LayoutParams.WrapContent);
            lParam.SetMargins(6, 36, 6, 36);
            linearLayout.LayoutParameters = lParam;
            linearLayout.Orientation = Orientation.Vertical;

            var param = new ViewGroup.LayoutParams(ViewGroup.LayoutParams.MatchParent, ViewGroup.LayoutParams.WrapContent);
            var textView = new TextView(Context)
            {
                LayoutParameters = param,
                Text = $"{title}({time:yyyy'年'MM'月'dd'日' HH':'mm})"
            };
            linearLayout.AddView(textView);

            linearLayout.AddView(view);

            return linearLayout;
        }

        private class TimeLineObject
        {
            public object Obj { get; set; }

            public DateTime Time { get; set; }
        }
    }

    class Listener : Java.Lang.Object, ViewTreeObserver.IOnGlobalLayoutListener
    {
        private Action action;

        public Listener(Action action)
        {
            this.action = action;
        }

        public void OnGlobalLayout()
        {
            action();
        }
    }
}