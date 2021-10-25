using Serilog;
using TempusApi;
using TempusApi.Models.Activity;
using TempusApi.Models.DetailedMapList;
using TempusApi.Models.Responses;
using TempusHub.Core.Models;
using TempusHub.Core.Utilities;
using TempusHub.Infrastructure;

namespace TempusHub.Application.Services;

public class TempusRecordCacheService
{
    private readonly Tempus _tempusDataService;
    private readonly TempusHubMySqlService _mySql;
    
    public TempusRecordCacheService(Tempus tempusDataService, TempusHubMySqlService mySql)
    {
        _tempusDataService = tempusDataService;
        _mySql = mySql;
    }
    
    public async Task CacheAllRecordsAsync()
    {
        var detailedMapList = await _tempusDataService.GetDetailedMapListAsync(allowCached: false);
        
        Log.Information("Caching all {Count} maps", detailedMapList.Count);
        foreach (var map in detailedMapList)
        {
            await CacheAllRecordsOnMapAsync(map).ConfigureAwait(false);
        }
    }
    
    private async Task CacheAllRecordsOnMapAsync(DetailedMapOverviewModel map)
    {
        var fullOverview = await _tempusDataService.GetFullMapOverViewAsync(map.Name).ConfigureAwait(false);

        await CacheClassRecordAsync(3, map, fullOverview).ConfigureAwait(false);
        await CacheClassRecordAsync(4, map, fullOverview).ConfigureAwait(false);
    }

    
    private async Task CacheClassRecordAsync(int classId, DetailedMapOverviewModel map, MapFullOverviewModel fullOverview)
    {
        try
        {
            // Map record
            var duration = fullOverview.GetClassRuns(classId).OrderByDuration().First().Duration;
            await CacheRecordAsync(map.Id, classId, duration, "map", 1).ConfigureAwait(false);

            // Course record
            for (var courseId = 1; courseId <= map.ZoneCounts.Course; courseId++)
            {
                try
                {
                    var runs = (await _tempusDataService.GetTopZonedTimes(map.Name, "course", courseId)
                        .ConfigureAwait(false)).Runs;
                    duration = runs.GetClassRuns(classId).OrderByDuration().First().Duration;
                    await CacheRecordAsync(map.Id, classId, duration, "course", courseId).ConfigureAwait(false);
                }
                catch (Exception e)
                {
                    Log.Error(e, "Unhandled exception while caching course records");
                }
            }

            // Bonus record
            for (int bonusId = 1; bonusId <= map.ZoneCounts.Bonus; bonusId++)
            {
                try
                {
                    var runs = (await _tempusDataService.GetTopZonedTimes(map.Name, "bonus", bonusId)
                        .ConfigureAwait(false)).Runs;
                    duration = runs.GetClassRuns(classId).OrderByDuration().First().Duration;
                    await CacheRecordAsync(map.Id, classId, duration, "bonus", bonusId).ConfigureAwait(false);
                }
                catch (Exception e)
                {
                    Log.Error(e, "Unhandled exception while caching bonus records");
                }
            }
        }
        catch (Exception e)
        {
            Log.Error(e, "Unhandled exception while caching class records");
        }
    }
    private async Task CacheRecordAsync(int mapId, int classId, double duration, string zoneType, int zoneIndex) =>
        await UpdateCachedWrDataAsync(null, new CachedTempusRecordBase
        {
            CachedTime = null,
            MapInfo = new MapInfo
            {
                Id = mapId
            },
            RecordInfo = new RecordInfoShort
            {
                Class = classId,
                Duration = duration
            },
            ZoneInfo = new ZoneInfo
            {
                Type = zoneType,
                Zoneindex = zoneIndex
            }
        }).ConfigureAwait(false);

    public async Task<MapRecordCache> UpdateCachedWrDataAsync(MapRecordCache? cached, CachedTempusRecordBase map)
    {
        if (map is null)
        {
            throw new ArgumentNullException(nameof(map));
        }
            
        var tempNewCache = cached;

        // Check for no data
        if (cached == null || !cached.CurrentWrDuration.HasValue && !cached.OldWrDuration.HasValue)
        {
            // No data
            tempNewCache = new MapRecordCache
            {
                MapId = map.MapInfo.Id,
                CurrentWrDuration = map.RecordInfo.Duration,
                ClassId = map.RecordInfo.Class,
                ZoneType = map.ZoneInfo.Type,
                OldWrDuration = null,
                ZoneId = map.ZoneInfo.Zoneindex
            };

            await _mySql.UpdateCachedRecordAsync(tempNewCache).ConfigureAwait(false);
        }
        // Check if the cached wr duration is slower to the new record
        else if (cached.CurrentWrDuration.HasValue && 
                 cached.CurrentWrDuration.Value.SlowerThan(map.RecordInfo.Duration))
        {
            tempNewCache = new MapRecordCache
            {
                MapId = map.MapInfo.Id,
                CurrentWrDuration = map.RecordInfo.Duration,
                OldWrDuration = cached.CurrentWrDuration,
                ClassId = map.RecordInfo.Class,
                ZoneType = map.ZoneInfo.Type,
                ZoneId = map.ZoneInfo.Zoneindex
            };

            await _mySql.UpdateCachedRecordAsync(tempNewCache).ConfigureAwait(false);
        }

        return tempNewCache;
    }

}