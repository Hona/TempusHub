using Dapper.FluentMap.Mapping;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TempusHubBlazor.Models.MySQL;

namespace TempusHubBlazor.Data.Mapping
{
    public class MapRecordCacheMap : EntityMap<MapRecordCache>
    {
        public MapRecordCacheMap()
        {
            Map(config => config.MapId).ToColumn("mapId");
            Map(config => config.CurrentWRDuration).ToColumn("currentWrDuration");
            Map(config => config.OldWRDuration).ToColumn("oldWrDuration");
        }
    }
}
