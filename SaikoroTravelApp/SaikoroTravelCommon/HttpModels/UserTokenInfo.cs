using SaikoroTravelCommon.Models;

namespace SaikoroTravelCommon.HttpModels
{
    public class UserRegisterRequest
    {
        public string Code { get; set; }

        public Users User { get; set; }

        public string Token { get; set; }
    }
}
