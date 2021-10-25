namespace TempusHub.Core.Models;

public class MapRecordCache
{
    public int MapId { get; set; }
    public int ClassId { get; set; }
    public string ZoneType { get; set; }
    public int ZoneId { get; set; }
    public double? CurrentWrDuration { get; set; }
    public double? OldWrDuration { get; set; }
}