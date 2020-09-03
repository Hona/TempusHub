using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using TempusHubBlazor.Constants;
using TempusHubBlazor.Data;
using TempusHubBlazor.Logging;
using TempusHubBlazor.Models;
using TempusHubBlazor.Models.Tempus.DetailedMapList;
using TempusHubBlazor.Models.Tempus.Rank;
using TempusHubBlazor.Models.Tempus.Responses;

namespace TempusHubBlazor.Services
{
    public class TempusCacheService
    {
        private readonly Timer _updateTimer;
        private readonly TempusDataService TempusDataService;
        public event EventHandler DataUpdated;
        public RecentActivityModel RecentActivity { get; private set; }
        public RecentActivityWithZonedData RecentActivityWithZonedData { get; private set; } = new RecentActivityWithZonedData();
        public List<TopPlayerOnline> TopPlayersOnline { get; private set; } = new List<TopPlayerOnline>();
        public List<DetailedMapOverviewModel> DetailedMapList { get; set; }
        public PlayerLeaderboards PlayerLeaderboards { get; set; }
        public List<ServerStatusModel> ServerStatusList { get; set; }
        public List<TempusRealName> RealNames { get; set; }
        public List<TempusRankColor> TempusRankColors { get; set; }


        public TempusCacheService(TempusDataService tempusDataService)
        {
            TempusDataService = tempusDataService;
            UpdateAllCachedDataAsync().GetAwaiter().GetResult();
            _updateTimer = new Timer(async callback => await UpdateAllCachedDataAsync().ConfigureAwait(false), null, TimeSpan.FromMinutes(1.5), TimeSpan.FromMinutes(1.5));
        }
        
        private async Task UpdateAllCachedDataAsync()
        {
            try 
            {
                var tasks = new List<Task>
                {
                    Task.Run(async () =>
                    {
                        await UpdateRecentActivityAsync().ConfigureAwait(false);
                        await UpdateRecentActivityWithZonedDataAsync().ConfigureAwait(false);
                    }),
                    UpdateDetailedMapListAsync(),
                    UpdatePlayerLeaderboardsAsync(),
                    Task.Run(async () =>
                    {
                        await UpdateServerStatusListAsync().ConfigureAwait(false);
                        await UpdateTopOnlinePlayersAsync().ConfigureAwait(false);
                    }),
                    UpdateRealNamesAsync(),
                    UpdateTempusColorsAsync()
                };

                await Task.WhenAll(tasks).ConfigureAwait(false);
                // Refreshes any clients
                OnDataUpdated(EventArgs.Empty);
            }
            catch (Exception e)
            {
                Logger.LogException(e);
            }
        }
        private async Task UpdateRecentActivityAsync()
        {
            RecentActivity = await TempusDataService.GetRecentActivityAsync().ConfigureAwait(false);
        }
        private async Task UpdateRecentActivityWithZonedDataAsync()
        {
            var recentActivityWithZonedData = new RecentActivityWithZonedData();

            var tasks = new List<Task>();
            tasks.AddRange(RecentActivity.MapRecords.Select(async x => recentActivityWithZonedData.MapWR.Add(await TempusDataService.PopulateRecordDataAsync(x).ConfigureAwait(false))));
            tasks.AddRange(RecentActivity.CourseRecords.Select(async x => recentActivityWithZonedData.CourseWR.Add(await TempusDataService.PopulateRecordDataAsync(x).ConfigureAwait(false))));
            tasks.AddRange(RecentActivity.BonusRecords.Select(async x => recentActivityWithZonedData.BonusWR.Add(await TempusDataService.PopulateRecordDataAsync(x).ConfigureAwait(false))));
            tasks.AddRange(RecentActivity.MapTopTimes.Select(async x => recentActivityWithZonedData.MapTT.Add(await TempusDataService.PopulateRecordDataAsync(x).ConfigureAwait(false))));
            await Task.WhenAll(tasks).ConfigureAwait(false);

            RecentActivityWithZonedData = recentActivityWithZonedData;
        }
        private async Task UpdateTopOnlinePlayersAsync()
        {
            var tempTopPlayersOnline = new List<TopPlayerOnline>();

            if (ServerStatusList.Count == 0)
            {
                Logger.LogError("Could not get any server status's");
                return;
            }

            // Get all valid online users
            var validUsers = ServerStatusList.Where(x => x.GameInfo != null &&
                                                (x.GameInfo != null || x.ServerInfo != null ||
                                                 x.GameInfo.Users != null) &&
                                                x.GameInfo.Users.Count != 0)
                .SelectMany(x => x.GameInfo.Users).ToList();
            var usersWithId = validUsers.Where(x => x?.Id != null).ToList();


            // Get the user IDs as strings
            var userIdStrings = (usersWithId.Where(user => user?.Id != null).Select(user => user.Id.ToString())).ToList().Distinct();

            // Query all at once for all users ranks
            var rankTasks = new List<Task<Rank>>();
            rankTasks.AddRange(userIdStrings.Select(TempusDataService.GetUserRankAsync));
            var ranks = await Task.WhenAll(rankTasks).ConfigureAwait(false);

            // Get the users that actually have a rank (exclude unranks), and select the higher rank
            // Dictionary<User, BestRank>

            var classRankedUsers = ranks
                .Select(rankedUser => new
                {
                    rankedUser,
                    isDemoHigherRank = rankedUser.ClassRankInfo.DemoRank.Rank <=
                                       rankedUser.ClassRankInfo.SoldierRank.Rank
                })
                .Select(classRankedUser => new ClassRank
                {
                    Class = classRankedUser.isDemoHigherRank ? 4 : 3,
                    Player = usersWithId.First(x => x.Id == classRankedUser.rankedUser.PlayerInfo.Id),
                    Rank = classRankedUser.isDemoHigherRank
                        ? classRankedUser.rankedUser.ClassRankInfo.DemoRank.Rank
                        : classRankedUser.rankedUser.ClassRankInfo.SoldierRank.Rank
                }).ToList();

            // Sort by the best rank
            var output = classRankedUsers.OrderBy(x => x.Rank);

            // Limit it to the best 100 players
            foreach (var rankedUser in output.Take(100))
            {
                if (rankedUser.Player == null) continue;
                var server = ServerStatusList
                    .FirstOrDefault(x =>
                        x.GameInfo?.Users != null &&
                        x.GameInfo.Users.Any(z => (z.Id.HasValue && z.Id == rankedUser.Player.Id) || z.SteamId == rankedUser.Player.SteamId));
                if (server == null || rankedUser.Player.Id == null) continue;

                tempTopPlayersOnline.Add(new TopPlayerOnline
                {
                    Id = rankedUser.Player.Id.Value,
                    Rank = rankedUser.Rank,
                    RankClass = rankedUser.Class,
                    SteamId = rankedUser.Player.SteamId,
                    Name = rankedUser.Player.Name,
                    Server = server
                });
            }

            TopPlayersOnline = tempTopPlayersOnline;
        }
        private async Task UpdateDetailedMapListAsync()
        {
            DetailedMapList = await TempusDataService.GetDetailedMapListAsync();
            await Task.Run(() =>
            {
                var lines = File.ReadAllLines(LocalFileConstants.MapClasses);
                foreach (var line in lines)
                {
                    var lineData = line.Split(',');

                    // Will always only be 1 char, also converts it to a char by calling .First()
                    var classChar = lineData[0].First();
                    var mapName = lineData[1];
                    var matchedDetailedMapInfo = DetailedMapList.FirstOrDefault(x => x.Name.ToLower() == mapName.ToLower());
                    if (matchedDetailedMapInfo == null)
                    {
                        Logger.LogWarning("Could not find map data for: " + mapName);
                    }
                    else
                    {
                        matchedDetailedMapInfo.IntendedClass = classChar;
                    }
                }

                // Check if there are maps with no 
                var noClassDataMaps = DetailedMapList.Where(x => x.IntendedClass == default).ToArray();
                for (int i = 0; i < noClassDataMaps.Length; i++)
                {
                    Logger.LogWarning("No class data for " + noClassDataMaps[i].Name);
                    noClassDataMaps[i].IntendedClass = 'B';
                }
            });
        }
        private async Task UpdatePlayerLeaderboardsAsync()
        {
            PlayerLeaderboards = new PlayerLeaderboards
            {
                Overall = await TempusDataService.GetOverallRanksOverview(),
                Soldier = await TempusDataService.GetSoldierRanksOverview(),
                Demoman = await TempusDataService.GetDemomanRanksOverview()
            };
        }
        private async Task UpdateServerStatusListAsync()
        {
            ServerStatusList = await TempusDataService.GetServerStatusAsync().ConfigureAwait(false);

            var servers = ServerStatusList.Where(x => x != null).ToList();

            // Get all valid online users
            var validUsers = servers.Where(x => x.GameInfo != null &&
                                                (x.GameInfo != null || x.ServerInfo != null ||
                                                 x.GameInfo.Users != null) &&
                                                x.GameInfo.Users.Count != 0)
                .SelectMany(x => x.GameInfo.Users).ToList();

            var usersWithoutId = validUsers.Where(x => x != null && x.Id == null).ToList();

            if (usersWithoutId.Any())
            {
                var tasks = usersWithoutId.Select(async x => (await TempusDataService.GetSearchResultAsync(x.Name).ConfigureAwait(false))?.Players?.FirstOrDefault(y => y.SteamId == x.SteamId));
                var searchResults = await Task.WhenAll(tasks).ConfigureAwait(false);

                foreach (var server in ServerStatusList.Where(x => x?.GameInfo?.Users != null))
                {
                    var nullUsers = server.GameInfo.Users.Where(x => x.Id == null);

                    foreach (var nullUser in nullUsers)
                    {
                        nullUser.Id = searchResults.FirstOrDefault(x => x?.SteamId == nullUser.SteamId)?.Id;
                    }
                }
            }
        }
        private async Task UpdateRealNamesAsync()
        {
            await Task.Run(() => 
            {
                var jsonText = File.ReadAllText(LocalFileConstants.TempusNames);
                RealNames = JsonConvert.DeserializeObject<List<TempusRealName>>(jsonText);
            }).ConfigureAwait(false);
        }
        private async Task UpdateTempusColorsAsync()
        {
            await Task.Run(() =>
            {
                var jsonText = File.ReadAllText(LocalFileConstants.TempusColors);
                TempusRankColors = JsonConvert.DeserializeObject<List<TempusRankColor>>(jsonText);
            }).ConfigureAwait(false);
        }
        public string GetRealName(int tempusId)
        {
            return RealNames.FirstOrDefault(x => x.Id == tempusId)?.RealName;
        }

        protected virtual void OnDataUpdated(EventArgs e) 
            => DataUpdated?.Invoke(this, e);
    }
}
