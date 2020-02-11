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

        internal async Task<MapRecordCache> GetCachedRecordsAsync(int mapId)
        {
            var query =
                @"select * from `worldRecordCache` where `mapId`=@MapId";

            var param = new
            {
                MapId = mapId
            };

            var result = (await QueryAsync<MapRecordCache>(query, param)).ToList();
            return result.FirstOrDefault();
        }

        internal async Task UpdateCachedRecordAsync(MapRecordCache newCache)
        {
            var query =
                @"INSERT INTO `worldRecordCache` VALUES (@MapId, @CurrentRecordDuration, @OldRecordDuration) ON DUPLICATE KEY UPDATE `currentWrDuration`=@CurrentRecordDuration, `oldWrDuration`=@OldRecordDuration";


            var param = new
            {
                MapId = newCache.MapId,
                CurrentRecordDuration = newCache.CurrentWRDuration,
                OldRecordDuration = newCache.OldWRDuration
            };

            await ExecuteAsync(query, param);
        }
    }
}
