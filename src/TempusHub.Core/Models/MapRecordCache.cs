using TempusApi.Enums;

namespace TempusHub.Core.Models;

public class MapRecordCache
{
    public long MapId { get; set; }
    public Class ClassId { get; set; }
    public ZoneType ZoneType { get; set; }
    public long ZoneId { get; set; }
    public double? CurrentWrDuration { get; set; }
    public double? OldWrDuration { get; set; }
}