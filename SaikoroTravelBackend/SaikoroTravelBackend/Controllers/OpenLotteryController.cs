using Microsoft.AspNetCore.Mvc;

using SaikoroTravelBackend.Models;

using SaikoroTravelCommon.HttpModels;
using SaikoroTravelCommon.Models;

namespace SaikoroTravelBackend.Controllers;

[ApiController]
[Route("api/[controller]")]
public class OpenLotteryController : ControllerBase
{
    private static readonly object syncObj = new();

    private readonly UserContext context;

    public OpenLotteryController(UserContext context)
    {
        this.context = context;
    }

    [HttpPost]
    public async Task<ActionResult<LotteryResponse>> Post(LotteryRequest request)
    {
        lock (syncObj)
        {
            LotterySync? obj = context.LotterySyncs.FirstOrDefault(x => x.StrId == request.LotteryId);
            if (obj is null)
            {
                LotteryOption res = GetResultLocal(DependencyProvider.Default.RequestLotteryManager().FindById(request.LotteryId));
                var addr = new LotterySync()
                {
                    StrId = request.LotteryId,
                    Number = res.Number,
                    User = request.User,
                };
                _ = context.LotterySyncs.Add(addr);
                _ = context.SaveChanges();
                return new LotteryResponse()
                {
                    IsOK = true,
                    Selected = res.Number
                };
            }
            else
            {
                return new LotteryResponse()
                {
                    IsOK = true,
                    Selected = obj.Number,
                };
            }
        }
    }

    private static readonly Random random = new();

    private static LotteryOption GetResultLocal(Lottery lottery)
    {
        LotteryOption[] target = lottery.LotteryOptions.Where(x => x.Selectable).ToArray();

        var value = random.Next() % target.Length;
        return target[value];
    }
}
