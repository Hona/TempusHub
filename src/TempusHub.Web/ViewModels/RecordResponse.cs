using TempusApi.Models.Activity;
using TempusHub.Core.Models;

namespace TempusHub.Web.ViewModels;

public class RecordResponse
{
    public MapRecordCache CachedTime { get; set; }

    public RecordInfoShort RecordInfo { get; set; }

    public ZoneInfo ZoneInfo { get; set; }

    public MapInfo MapInfo { get; set; }

    public PlayerResponse PlayerInfo { get; set; }
}