using SaikoroTravelCommon.IO;
using SaikoroTravelCommon.Models;

namespace SaikoroTravelBackend;

public class DummySotre : IKVPStore
{
    // private const string routeStateTableName = "Route_State";
    // private const string messageTableKey = "Route_Message_{0}";
    // private const string destinationVisibleTableKey = "Route_DestinationVisible";
    // private const string instructionStateTableName = "Instruction_State";
    // private const string lotteryStateTableName = "Lottery_State";

    public DummySotre()
    {

    }

    public bool ContainsKey(string table, string key)
    {
        return true;
    }

    public void DeleteTable(string table)
    {
        return;
    }

    public bool ExistsTable(string table)
    {
        return true;
    }

    public string Get(string table, string key)
    {
        return table.Contains("State") ? RouteState.Activated.GetJsonValue() : "true";
    }

    public void Set(string table, string key, string value)
    {
        return;
    }
}
