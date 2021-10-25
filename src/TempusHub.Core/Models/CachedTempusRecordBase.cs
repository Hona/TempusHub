using TempusApi.Models.Activity;

namespace TempusHub.Core.Models;

public static class TempusRecordBaseExtensions
{
    public static CachedTempusRecordBase ToCached(this TempusRecordBase record)
        => new()
        {
            MapInfo = record.MapInfo,
            PlayerInfo = record.PlayerInfo,
            RecordInfo = record.RecordInfo,
            ZoneInfo = record.ZoneInfo
        };
}

public class CachedTempusRecordBase : TempusRecordBase
{
    /// <summary>
    /// Not from the API, applied by TempusDataService when received however.
    /// </summary>
    public MapRecordCache? CachedTime { get; set; }
}