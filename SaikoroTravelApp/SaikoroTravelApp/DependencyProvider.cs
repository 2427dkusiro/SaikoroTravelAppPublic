using Android.Content;

using SaikoroTravelCommon.Models;
using SaikoroTravelCommon.Time;

namespace SaikoroTravelApp
{
    /// <summary>
    /// 依存関係を提供します。
    /// </summary>
    internal class DependencyProvider
    {
        private ITimer timer;

        public ITimer RequestTimer(Context context)
        {
            return timer ??= new DebugTimer();
            // return timer ??= new NotificationTimer(context);
        }

        private DependencyProvider()
        {

        }

        private static DependencyProvider instance;

        public static DependencyProvider Default => instance ??= new DependencyProvider();

        private ConfigManager configManager;

        public ConfigManager RequestConfigManager(Context context)
        {
            return configManager ??= new ConfigManager(context); ;
        }

        private RouteManager routeManager;

        /// <summary>
        /// <see cref="RouteManager"/> のインスタンスの取得を要求します。
        /// </summary>
        /// <returns></returns>
        public RouteManager RequestRouteManager(Context context)
        {
            if (routeManager is null)
            {
                routeManager = new RouteManager();
                var store = new SharedPrefWrapper(context);
                routeManager.Load(RequestConfigManager(context).User, store, RequestTimer(context));
            }
            return routeManager;
        }

        private InstructionManager instructionManager;

        /// <summary>
        /// <see cref="InstructionManager"/> クラスのインスタンスの取得を要求します。
        /// </summary>
        /// <returns></returns>
        public InstructionManager RequestInstructionManager(Context context)
        {
            if (instructionManager is null)
            {
                instructionManager = new InstructionManager();
                var store = new SharedPrefWrapper(context);
                instructionManager.Load(RequestConfigManager(context).User, store, RequestTimer(context));
            }
            return instructionManager;
        }

        private LotteryManager lotteryManager;

        public LotteryManager RequestLotteryManager(Context context)
        {
            if (lotteryManager is null)
            {
                lotteryManager = new LotteryManager();
                var store = new SharedPrefWrapper(context);
                lotteryManager.Load(RequestConfigManager(context).User, store, RequestTimer(context));
            }
            return lotteryManager;
        }
    }
}