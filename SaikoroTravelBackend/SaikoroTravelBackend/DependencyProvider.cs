using SaikoroTravelCommon.Models;
using SaikoroTravelCommon.Time;

namespace SaikoroTravelBackend;

public class DependencyProvider
{
    private ITimer? timer;

    public ITimer RequestTimer()
    {
        return timer ??= new ServerTimer();
    }

    public DependencyProvider()
    {

    }

    private static DependencyProvider instance;

    public static DependencyProvider Default => instance ??= new DependencyProvider();

    private RouteManager? routeManager;
    private readonly DummySotre store = new();

    /// <summary>
    /// <see cref="RouteManager"/> のインスタンスの取得を要求します。
    /// </summary>
    /// <returns></returns>
    public RouteManager RequestRouteManager()
    {
        if (routeManager is null)
        {
            routeManager = new RouteManager();
            routeManager.Load(null, store, RequestTimer());
        }
        return routeManager;
    }

    private InstructionManager? instructionManager;

    /// <summary>
    /// <see cref="InstructionManager"/> クラスのインスタンスの取得を要求します。
    /// </summary>
    /// <returns></returns>
    public InstructionManager RequestInstructionManager()
    {
        if (instructionManager is null)
        {
            instructionManager = new InstructionManager();
            instructionManager.Load(null, store, RequestTimer());
        }
        return instructionManager;
    }

    private LotteryManager? lotteryManager;

    public LotteryManager RequestLotteryManager()
    {
        if (lotteryManager is null)
        {
            lotteryManager = new LotteryManager();
            lotteryManager.Load(null, store, RequestTimer());
        }
        return lotteryManager;
    }
}
