using SaikoroTravelCommon.Models;

namespace SaikoroTravelBackend;

internal static class UserCheck
{
    private static readonly Dictionary<Users, int> keys = new()
    {
        { Users.User1, 891073 },
        { Users.User2 , 064617 },
        { Users.User3 , 961017 },
        { Users.User4 , 921224 }
    };

    public static bool Check(Users users, string code)
    {
        if (int.TryParse(code, out var res))
        {
            if (keys.TryGetValue(users, out var key))
            {
                return key == res;
            }
        }
        return false;
    }
}
