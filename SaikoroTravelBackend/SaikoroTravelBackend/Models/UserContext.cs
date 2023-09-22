using Microsoft.EntityFrameworkCore;

using SaikoroTravelCommon.HttpModels;

namespace SaikoroTravelBackend.Models;

public class UserContext : DbContext
{
    public UserContext(DbContextOptions<UserContext> dbContextOptions) : base(dbContextOptions)
    {

    }

    public DbSet<DbUserInfo> UserTokenInfos { get; set; } = null!;

    public DbSet<DbRoute> Routes { get; set; } = null!;

    public DbSet<DbInstruction> Instructions { get; set; } = null!;

    public DbSet<DbLottery> Lotteries { get; set; } = null!;

    public DbSet<LotterySync> LotterySyncs { get; set; } = null!;

    public DbSet<Log> Logs { get; set; } = null!;
}
