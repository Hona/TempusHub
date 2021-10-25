using Dapper.FluentMap;
using Serilog;
using TempusHub.Core.Models;
using TempusHub.Infrastructure.Mapping;

namespace TempusHub.Infrastructure;

public class TempusHubMySqlService : MySqlDataAccessBase
{
    public TempusHubMySqlService(ILogger log) : base(Environment.GetEnvironmentVariable("MYSQL_CONNECTION_STRING"), log)
    {
        FluentMapper.Initialize(config =>
        {
            config.AddMap(new MapRecordCacheMap());
        });
    }

    public async Task<MapRecordCache> GetCachedRecordsAsync(int mapId, int classId, string zoneType, int zoneId = 1)
    {
        const string query =
            @"select * from `worldRecordCache` where `mapId`=@MapId AND `classId`=@ClassId AND `zoneType`=@ZoneType and `zoneId`=@ZoneId";

        var param = new
        {
            MapId = mapId,
            ClassId = classId,
            ZoneType = zoneType,
            ZoneId = zoneId
        };

        var result = await QueryAsync<MapRecordCache>(query, param).ConfigureAwait(false);
        return result?.FirstOrDefault();
    }

    public async Task UpdateCachedRecordAsync(MapRecordCache newCache)
    {
        const string query = @"INSERT INTO `worldRecordCache` VALUES (@MapId, @ClassId, @ZoneType, @ZoneId, @CurrentRecordDuration, @OldRecordDuration) ON DUPLICATE KEY UPDATE `currentWrDuration`=@CurrentRecordDuration, `oldWrDuration`=@OldRecordDuration";

        var param = new
        {
            newCache.MapId,
            newCache.ClassId,
            newCache.ZoneType,
            newCache.ZoneId,
            CurrentRecordDuration = newCache.CurrentWrDuration,
            OldRecordDuration = newCache.OldWrDuration
        };

        await ExecuteAsync(query, param).ConfigureAwait(false);
    }
}