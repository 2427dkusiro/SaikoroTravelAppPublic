using SaikoroTravelCommon.Models;

namespace SaikoroTravelBackend.Models;

public class LotterySync
{
    public int Id { get; set; }

    public string StrId { get; set; }

    public Users User { get; set; }

    public int Number { get; set; }
}
