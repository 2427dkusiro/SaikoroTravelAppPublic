using SaikoroTravelCommon.IO;
using SaikoroTravelCommon.ResourceLoaders;
using SaikoroTravelCommon.Time;

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace SaikoroTravelCommon.Models
{
    /// <summary>
    /// 行程の管理を提供します。
    /// </summary>
    public class RouteManager
    {
        private readonly List<Route> routes = new List<Route>();
        private ITimer timer;

        public Users? User { get; private set; }

        /// <summary>
        /// 現在ロードされている行程のリストを取得します。
        /// </summary>
        public IReadOnlyList<Route> Routes => routes;

        /// <summary>
        /// <see cref="RouteManager"/> クラスの新しいインスタンスを初期化します。
        /// </summary>
        public RouteManager()
        {

        }

        /// <summary>
        /// 一意の ID から行程を取得します。
        /// </summary>
        /// <param name="id">検索する ID。</param>
        /// <returns></returns>
        public Route FindById(string id)
        {
            return routes.Single(x => x.Id == id);
        }

        /// <summary>
        /// 埋め込みリソースと指定の <see cref="IKVPStore"/> から行程をロードします。
        /// </summary>
        /// <param name="accesser">行程の状態を保存する <see cref="IKVPStore"/>。</param>
        /// <returns></returns>
        public void Load(Users? user, IKVPStore accesser, ITimer timer)
        {
            User = user;
            this.timer = timer;
            LoadResource(user, accesser);

            foreach (Route route in routes.Where(x => x.UserInfo.HasPermission(user)))
            {
                UpdateRouteFromTimer(timer.Now, route);
                RegistorRouteToUpdateTimer(timer, route);
            }
        }

        public void ForceUpdateNow()
        {
            foreach (Route route in routes.Where(x => x.UserInfo.HasPermission(User)))
            {
                UpdateRouteFromTimer(timer.Now, route);
            }
        }

        private const string asmName = "SaikoroTravelCommon";
        private const string stateTableName = "Route_State";
        private const string messageTableKey = "Route_Message_{0}";
        private const string destinationVisibleTableKey = "Route_DestinationVisible";

        private void LoadResource(Users? user, IKVPStore accesser)
        {
            var asm = Assembly.GetExecutingAssembly();
            using Stream stream = asm.GetManifestResourceStream($"{asmName}.Resources.Route.json");
            Route[] routes = RouteParser.Parse(stream, accesser, stateTableName, messageTableKey, destinationVisibleTableKey);
            AssertIdIsUnique(routes);
            this.routes.AddRange(routes);
            RegistorTimerEventToRoutes(routes.Where(x => x.UserInfo.HasPermission(user)));
        }

        private void AssertIdIsUnique(IEnumerable<Route> routes)
        {
            var hash = new HashSet<string>();
            foreach (Route route in routes)
            {
                if (hash.Contains(route.Id))
                {
                    throw new FormatException($"ID '{route.Id}' が重複しています");
                }
            }
        }

        private void RegistorTimerEventToRoutes(IEnumerable<Route> routes)
        {
            foreach (Route route in routes)
            {
                route.PropertyChanged += (sender, e) =>
                {
                    if (e.PropertyName == nameof(Route.RouteState))
                    {
                        UpdateRouteFromTimer(timer!.Now, route);
                        RegistorRouteToUpdateTimer(timer, route);
                        StateChanged?.Invoke(sender, e);
                    }
                };
            }
        }

        private static void UpdateRouteFromTimer(DateTime nowTime, Route route)
        {
            if (route.RouteState == RouteState.WaitingForActivation)
            {
                if (route.DepartureTime <= nowTime)
                {
                    route.SetRouteState(RouteState.Activated);
                }
            }
            else if (route.RouteState == RouteState.Activated)
            {
                if (route.DestinationTime is DateTime destTime)
                {
                    if (destTime < nowTime)
                    {
                        route.SetRouteState(RouteState.Executed);
                    }
                }
            }
        }

        private static void RegistorRouteToUpdateTimer(ITimer timer, Route route)
        {
            if (route.RouteState == RouteState.WaitingForActivation)
            {
                timer.AddCallback(route.DepartureTime, time =>
                {
                    if (route.RouteState == RouteState.WaitingForActivation)
                    {
                        route.SetRouteState(RouteState.Activated);
                    }
                });
            }
            if (route.RouteState == RouteState.Activated)
            {
                if (route.DestinationTime is DateTime time)
                {
                    timer.AddCallback(time.AddMinutes(1), time =>
                    {
                        if (route.RouteState == RouteState.Activated)
                        {
                            route.SetRouteState(RouteState.Executed);
                        }
                    });
                }
            }
        }

        /// <summary>
        /// 行程の状態が変化したときに発生するイベント。
        /// </summary>
        public event EventHandler<EventArgs>? StateChanged;
    }
}
