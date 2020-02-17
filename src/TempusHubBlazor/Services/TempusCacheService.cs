using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using TempusHubBlazor.Data;
using TempusHubBlazor.Models;
using TempusHubBlazor.Models.Tempus.Rank;
using TempusHubBlazor.Models.Tempus.Responses;

namespace TempusHubBlazor.Services
{
    public class TempusCacheService
    {
        private readonly Timer _updateTimer;
        private readonly TempusDataService TempusDataService;
        public RecentActivityModel RecentActivity { get; private set; }
        public RecentActivityWithZonedData RecentActivityWithZonedData { get; private set; } = new RecentActivityWithZonedData();
        public List<TopPlayerOnline> TopPlayersOnline { get; private set; } = new List<TopPlayerOnline>();

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

            tasks.Add(Task.Run(async () => 
            {
                var tempTopPlayersOnline = new List<TopPlayerOnline>();
                var servers = (await TempusDataService.GetServerStatusAsync()).Where(x => x != null).ToArray();
                var users = servers.Where(x => x.GameInfo != null &&
                                               (x.GameInfo != null || x.ServerInfo != null ||
                                                x.GameInfo.Users != null) &&
                                               x.GameInfo.Users.Count != 0)
                    .SelectMany(x => x.GameInfo.Users).Where(x => x?.Id != null).ToArray();

                var userIdStrings = (from user in users where user?.Id != null select user.Id.ToString()).ToList();

                var rankTasks = new List<Task<Rank>>();
                rankTasks.AddRange(userIdStrings.Select(TempusDataService.GetUserRankAsync));


                var ranks = await Task.WhenAll(rankTasks);
                var rankedUsers = ranks.ToDictionary(rank => users.First(x => x.Id == rank.PlayerInfo.Id), rank =>
                    rank.ClassRankInfo.DemoRank.Rank <= rank.ClassRankInfo.SoldierRank.Rank
                        ? rank.ClassRankInfo.DemoRank.Rank
                        : rank.ClassRankInfo.SoldierRank.Rank);

                var output = rankedUsers.OrderBy(x => x.Value);


                foreach (var (key, value) in output)
                {
                    if (key == null || value > 200) continue;
                    var server = servers
                        .FirstOrDefault(x =>
                            x.GameInfo?.Users != null &&
                            x.GameInfo.Users.Count(z => z.Id.HasValue && z.Id == key.Id) != 0);
                    if (server == null || key.Id == null) continue;
                    tempTopPlayersOnline.Add(new TopPlayerOnline 
                    { 
                        Rank = value,
                        SteamId = key.SteamId,
                        Name = key.Name,
                        Server = server
                    });
                }

                TopPlayersOnline = tempTopPlayersOnline;

                return Task.CompletedTask;
            }));

            await Task.WhenAll(tasks);

            RecentActivityWithZonedData = recentActivityWithZonedData;
        }
    }
}
