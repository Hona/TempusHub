using Dapper.FluentMap;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TempusHubBlazor.Data.Mapping;
using TempusHubBlazor.Models.MySQL;

namespace TempusHubBlazor.Data
{
    public class TempusHubMySqlService : MySqlDataAccessBase
    {
        public TempusHubMySqlService(string connectionString) : base(connectionString) 
        {
            FluentMapper.Initialize(config =>
            {
                config.AddMap(new MapRecordCacheMap());
            });
        }

        internal async Task<MapRecordCache> GetCachedRecordsAsync(int mapId, int classId, string zoneType, int zoneId = 1)
        {
            var query =
                @"select * from `worldRecordCache` where `mapId`=@MapId AND `classId`=@ClassId AND `zoneType`=@ZoneType and `zoneId`=@ZoneId";

            var param = new
            {
                MapId = mapId,
                ClassId = classId,
                ZoneType = zoneType,
                ZoneId = zoneId
            };

            var result = (await QueryAsync<MapRecordCache>(query, param));
            return result?.FirstOrDefault();
        }

        internal async Task UpdateCachedRecordAsync(MapRecordCache newCache)
        {
            var query =
                @"INSERT INTO `worldRecordCache` VALUES (@MapId, @ClassId, @ZoneType, @ZoneId, @CurrentRecordDuration, @OldRecordDuration) ON DUPLICATE KEY UPDATE `currentWrDuration`=@CurrentRecordDuration, `oldWrDuration`=@OldRecordDuration";

            var param = new
            {
                MapId = newCache.MapId,
                ClassId = newCache.ClassId,
                ZoneType = newCache.ZoneType,
                ZoneId = newCache.ZoneId,
                CurrentRecordDuration = newCache.CurrentWRDuration ?? null,
                OldRecordDuration = newCache.OldWRDuration ?? null
            };

            await ExecuteAsync(query, param).ConfigureAwait(false);
        }
    }
}
