using System.Linq;
using System.Threading.Tasks;
using Serilog;
using TempusHubBlazor.Models.Tempus.DetailedMapList;
using TempusHubBlazor.Models.Tempus.Responses;
using TempusHubBlazor.Models.Tempus.Activity;
using TempusHubBlazor.Utilities;

namespace TempusHubBlazor.Data
{
    public class TempusRecordCacheService
    {
        private readonly TempusDataService _tempusDataService;
        public TempusRecordCacheService(TempusDataService tempusDataService)
        {
            _tempusDataService = tempusDataService;
        }
        public async Task CacheAllRecordsAsync()
        {
            Log.Information("Caching all {Count} maps", _tempusDataService.MapList.Count);
            foreach (var map in _tempusDataService.MapList)
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
                        var runs = (await _tempusDataService.GetTopZonedTimes(map.Name, "course", courseId).ConfigureAwait(false)).Runs;
                        duration = runs.GetClassRuns(classId).OrderByDuration().First().Duration;
                        await CacheRecordAsync(map.Id, classId, duration, "course", courseId).ConfigureAwait(false);
                    }
                    catch { }
                }

                // Bonus record
                for (int bonusId = 1; bonusId <= map.ZoneCounts.Bonus; bonusId++)
                {
                    try
                    {
                        var runs = (await _tempusDataService.GetTopZonedTimes(map.Name, "bonus", bonusId).ConfigureAwait(false)).Runs;
                        duration = runs.GetClassRuns(classId).OrderByDuration().First().Duration;
                        await CacheRecordAsync(map.Id, classId, duration, "bonus", bonusId).ConfigureAwait(false);
                    }
                    catch { }
                }
            }
            catch { }
        }
        private async Task CacheRecordAsync(int mapId, int classId, double duration, string zoneType, int zoneIndex)
        {
            await _tempusDataService.UpdateCachedWrDataAsync(null, new TempusRecordBase
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
                ZoneInfo = new Models.Tempus.Activity.ZoneInfo
                {
                    Type = zoneType,
                    Zoneindex = zoneIndex
                }
            }).ConfigureAwait(false);
        }
    }
}
