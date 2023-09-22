using Microsoft.AspNetCore.Mvc;

using SaikoroTravelBackend.Models;

using SaikoroTravelCommon.Models;

namespace SaikoroTravelBackend.Controllers;

[ApiController]
[Route("api/[controller]")]
public class DebugController : ControllerBase
{
    private readonly UserContext context;

    public DebugController(UserContext context)
    {
        this.context = context;
    }

    [HttpGet]
    public ActionResult<TestResponse> Get()
    {
        return new TestResponse()
        {
            Message = "Hello world!",
            TimeStamp = DateTime.Now,
        };
    }

    [HttpPost]
    public async Task<ActionResult> Post(string arg)
    {
        InstructionManager instructions = DependencyProvider.Default.RequestInstructionManager();
        LotteryManager lotteries = DependencyProvider.Default.RequestLotteryManager();

        foreach (Instruction instruction in instructions.Instructions)
        {
            if (instruction.DesiredNotificationTime < DateTime.Now && DateTime.Now < instruction.LimitTime)
            {
                if (context.Instructions.Where(x => x.State == "Executed" && x.StrId == instruction.Id).Any())
                {
                    continue;
                }
                await DiscordService.SendPublic($"{DiscordService.Mention} \n新しい指示を確認してください！");
                var tokens = context.UserTokenInfos.ToArray().Where(x => instruction.UserInfo.HasPermission(x.User)).Select(x => x.Token).ToList();
                await FCMService.SendInstructonNotification(tokens, instruction.Id);
            }
        }

        foreach (Lottery lottery in lotteries.Lotteries)
        {
            if (lottery.DesiredNotificationTime < DateTime.Now && DateTime.Now < lottery.LimitTime)
            {
                if (context.Lotteries.Where(x => x.State == "Executed" && x.StrId == lottery.Id).Any())
                {
                    continue;
                }
                await DiscordService.SendPublic($"{DiscordService.Mention} \n新しい抽選を確認してください！");
                var tokens = context.UserTokenInfos.ToArray().Where(x => lottery.UserInfo.HasPermission(x.User)).Select(x => x.Token).ToList();
                await FCMService.SendLotteryNotification(tokens, lottery.Id);
            }
        }

        return Ok();
    }

    public class TestResponse
    {
        public string? Message { get; set; }

        public DateTime TimeStamp { get; set; }
    }
}

[Controller]
[Route("api/[controller]")]
public class SNIController : Controller
{
    private readonly UserContext context;

    public SNIController(UserContext userContext)
    {
        context = userContext;
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<string>> Get(string id)
    {
        IQueryable<string> token = context.UserTokenInfos.Select(x => x.Token);
        Instruction instruction = DependencyProvider.Default.RequestInstructionManager().FindById(id);
        await FCMService.SendInstructonNotification(token, instruction.Id);

        return "OK";
    }
}

[Controller]
[Route("api/[controller]")]
public class SNLController : Controller
{
    private readonly UserContext context;

    public SNLController(UserContext context)
    {
        this.context = context;
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<string>> Get(string id)
    {
        IQueryable<string> token = context.UserTokenInfos.Select(x => x.Token);
        Lottery lottery = DependencyProvider.Default.RequestLotteryManager().FindById(id);
        await FCMService.SendLotteryNotification(token, lottery.Id);

        return "OK";
    }
}
