using SaikoroTravelCommon.Models;

namespace SaikoroTravelCommon.HttpModels
{
    public class LotteryRequest
    {
        public string LotteryId { get; set; }

        public Users User { get; set; }
    }
}
