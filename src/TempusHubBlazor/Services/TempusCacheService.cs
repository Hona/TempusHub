using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using TempusHubBlazor.Data;
using TempusHubBlazor.Models;
using TempusHubBlazor.Models.Tempus.Responses;

namespace TempusHubBlazor
{
    public class TempusCacheService
    {
        private readonly Timer _updateTimer;
        private readonly TempusDataService TempusDataService;
        public RecentActivityModel RecentActivity { get; private set; }
        public RecentActivityWithZonedData RecentActivityWithZonedData { get; private set; } = new RecentActivityWithZonedData();

        public TempusCacheService(TempusDataService tempusDataService)
        {
            TempusDataService = tempusDataService;
            UpdateAllCachedDataAsync().GetAwaiter().GetResult();
            _updateTimer = new Timer(async callback => await UpdateAllCachedDataAsync(), null, TimeSpan.Zero, TimeSpan.FromMinutes(1.5));
        }
        
        private async Task UpdateAllCachedDataAsync()
        {
            RecentActivity = await TempusDataService.GetRecentActivityAsync();

            // Reset model
            var recentActivityWithZonedData = new RecentActivityWithZonedData();

            var tasks = new List<Task>();

            tasks.AddRange(RecentActivity.MapRecords.Select(async x => recentActivityWithZonedData.MapWR.Add(await TempusDataService.PopulateRecordDataAsync(x))));
            tasks.AddRange(RecentActivity.CourseRecords.Select(async x => recentActivityWithZonedData.CourseWR.Add(await TempusDataService.PopulateRecordDataAsync(x))));
            tasks.AddRange(RecentActivity.BonusRecords.Select(async x => recentActivityWithZonedData.BonusWR.Add(await TempusDataService.PopulateRecordDataAsync(x))));
            tasks.AddRange(RecentActivity.MapTopTimes.Select(async x => recentActivityWithZonedData.MapTT.Add(await TempusDataService.PopulateRecordDataAsync(x))));

            await Task.WhenAll(tasks);

            RecentActivityWithZonedData = recentActivityWithZonedData;
        }
    }
}
