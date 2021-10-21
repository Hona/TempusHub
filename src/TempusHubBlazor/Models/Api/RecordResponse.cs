using TempusHubBlazor.Models.MySQL;
using TempusHubBlazor.Models.Tempus.Activity;

namespace TempusHubBlazor.Models.Api;

public class RecordResponse
{
    public MapRecordCache CachedTime { get; set; }

    public RecordInfoShort RecordInfo { get; set; }

    public ZoneInfo ZoneInfo { get; set; }

    public MapInfo MapInfo { get; set; }

    public PlayerResponse PlayerInfo { get; set; }
}