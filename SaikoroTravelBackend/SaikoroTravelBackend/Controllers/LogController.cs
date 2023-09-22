using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

using SaikoroTravelBackend.Models;

using SaikoroTravelCommon.HttpModels;

using System.Text.Json;

namespace SaikoroTravelBackend.Controllers;

[ApiController]
[Route("api/[controller]")]
public class LogController : ControllerBase
{
    private readonly UserContext context;

    public LogController(UserContext context)
    {
        this.context = context;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Log>>> Get()
    {
        return await context.Logs.ToListAsync();
    }

    [HttpPost]
    public async Task<IActionResult> Post(Log log)
    {
        if (log.LogLevel == SaikoroTravelCommon.HttpModels.LogLevel.Event_Instruction_Opened)
        {
            var id = log.AdditionalObject!;
            _ = context.Instructions.Add(new DbInstruction()
            {
                State = "Executed",
                StrId = id
            });
            _ = await context.SaveChangesAsync();
            await FCMService.SendInstructonReports(context.UserTokenInfos.Where(x => x.User != log.User).Select(x => x.Token), id);
        }
        else if (log.LogLevel == SaikoroTravelCommon.HttpModels.LogLevel.Event_Lottery_Opened)
        {
            LotteryReport? add = JsonSerializer.Deserialize<LotteryReport>(log.AdditionalObject!);
            _ = context.Lotteries.Add(new DbLottery()
            {
                State = "Executed",
                StrId = add.Id,
                Value = add.Number
            });
            _ = await context.SaveChangesAsync();
            await FCMService.SendLotteryReports(context.UserTokenInfos.Where(x => x.User != log.User).Select(x => x.Token), add.Id, add.Number);
        }

        await LogHandle.Handle(log);

        log.Id = default;
        _ = context.Logs.Add(log);
        _ = await context.SaveChangesAsync();
        return Ok();
    }
}
