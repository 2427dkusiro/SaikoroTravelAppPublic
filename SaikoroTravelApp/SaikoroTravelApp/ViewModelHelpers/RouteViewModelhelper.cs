using Android.Content;

using SaikoroTravelApp.ViewModels;

using SaikoroTravelCommon.Models;

using System;
using System.Linq;

namespace SaikoroTravelApp.VuewModelHelpers
{
    /// <summary>
    /// <see cref="RouteViewModel"/> の作成を簡略化します。
    /// </summary>
    internal class RouteViewModelhelper
    {
        private readonly Route route;

        private readonly RouteManager routeManager;

        private readonly Users user;

        /// <summary>
        /// 現在の <see cref="Route"/> の状態から生成されるビューモデルを取得します。
        /// </summary>
        public RouteViewModel ViewModel { get; } = new RouteViewModel();

        /// <summary>
        /// <see cref="RouteViewModelhelper"/> クラスの新しいインスタンスを初期化します。
        /// </summary>
        /// <param name="route"></param>
        public RouteViewModelhelper(Context context, Route route)
        {
            this.route = route;
            routeManager = DependencyProvider.Default.RequestRouteManager(context);
            user = DependencyProvider.Default.RequestConfigManager(context).User;

            SetViewModel();
            this.route.PropertyChanged += (sender, e) => SetViewModel();
            foreach (RouteMessage message in route.Messages)
            {
                message.VisiabilityChanged += (sender, e) => SetViewModel();
            }
        }

        private void SetViewModel()
        {
            ViewModel.TitleText = GetTitle(route.DisplayName);
            ViewModel.DepartureName = route.DepartureName;
            ViewModel.DepartureTime = $"({GetTimeText(route.DepartureTime)}発)";
            ViewModel.DestinationName = route.IsDestinationVisible ? route.DestinationName ?? "" : "？？？";
            ViewModel.DestinationTime = route.IsDestinationVisible ? route.DestinationTime is DateTime time ? $"({GetTimeText(time)}着)" : "" : "(??:??着)";
            ViewModel.TrainName = route.TrainName ?? "";
            ViewModel.TrainDetail = route.TrainDetail ?? "";
            ViewModel.Price = route.Price is int num ? $"￥{num}" : "￥***";
            ViewModel.Message1 = "";
            ViewModel.Message2 = "";
            ViewModel.Message3 = "";
            SetMessage(ViewModel, route);
        }

        private string GetTitle(string displayName)
        {
            var routes = routeManager.Routes.Where(x => x.DisplayName == displayName && x.UserInfo.HasPermission(user) && (x.RouteState == RouteState.Activated || x.RouteState == RouteState.Executed))
                .OrderBy(x => x.DepartureTime).ToList();

            var index = routes.IndexOf(route);
            return $"{displayName}その{index + 1}";
        }

        private static string GetTimeText(DateTime dateTime)
        {
            return dateTime.Hour < 10 ? " " + dateTime.ToString("H:mm") : dateTime.ToString("HH:mm");
        }

        private static void SetMessage(RouteViewModel routeViewModel, Route route)
        {
            var texts = route.Messages.Where(x => x.IsVisible).Select(x => x.Message).ToArray();
            switch (texts.Length)
            {
                case 0:
                    routeViewModel.Message2 = GenText(route);
                    break;
                case 1:
                    routeViewModel.Message2 = texts[0];
                    break;
                case 2:
                    routeViewModel.Message1 = texts[0];
                    routeViewModel.Message2 = texts[1];
                    break;
                case 3:
                    routeViewModel.Message1 = texts[0];
                    routeViewModel.Message2 = texts[1];
                    routeViewModel.Message3 = texts[2];
                    break;
                default:
                    throw new NotSupportedException();
            }
        }

        private static string GenText(Route route)
        {
            var year = route.DepartureTime.Year.ToString();
            var month = route.DepartureTime.Month.ToString();
            month = month.Length == 1 ? "-" + month : month;
            var day = route.DepartureTime.Day.ToString();
            day = day.Length == 1 ? "-" + day : day;
            return $"{year}.{month}.{day}サイコロアプリの旅";
        }
    }
}