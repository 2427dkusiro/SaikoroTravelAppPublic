using SaikoroTravelCommon.Models;

namespace SaikoroTravelBackend.Models;

public class DbUserInfo
{
    public int Id { get; set; }

    public Users User { get; set; }

    public string Token { get; set; }
}
