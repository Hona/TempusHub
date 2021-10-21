using Dapper.FluentMap.Mapping;
using TempusHubBlazor.Models.MySQL;

namespace TempusHubBlazor.Data.Mapping;

public class MapRecordCacheMap : EntityMap<MapRecordCache>
{
    public MapRecordCacheMap()
    {
        Map(config => config.MapId).ToColumn("mapId");
        Map(config => config.ClassId).ToColumn("classId");
        Map(config => config.ZoneType).ToColumn("zoneType");
        Map(config => config.ZoneId).ToColumn("zoneId");
        Map(config => config.CurrentWrDuration).ToColumn("currentWrDuration");
        Map(config => config.OldWrDuration).ToColumn("oldWrDuration");
    }
}