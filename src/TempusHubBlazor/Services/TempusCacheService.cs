using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using TempusHubBlazor.Data;
using TempusHubBlazor.Models.Tempus.Responses;

namespace TempusHubBlazor
{
    public class TempusCacheService
    {
        private readonly Timer _updateTimer;
        public RecentActivityModel RecentActivity { get; private set; }

        private readonly TempusDataService TempusDataService;
        public TempusCacheService(TempusDataService tempusDataService)
        {
            TempusDataService = tempusDataService;
            UpdateAllCachedDataAsync().GetAwaiter().GetResult();
            _updateTimer = new Timer(async callback => await UpdateAllCachedDataAsync(), null, TimeSpan.Zero, TimeSpan.FromMinutes(1.5));
        }
        
        private async Task UpdateAllCachedDataAsync()
        {
            var tasks = new List<Task>
            {
                Task.Run(async () => { RecentActivity = await TempusDataService.GetRecentActivityAsync(); })
            };

            await Task.WhenAll(tasks);
        }
    }
}
