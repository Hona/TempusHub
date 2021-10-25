using Newtonsoft.Json;
using Serilog;
using TempusApi;
using TempusApi.Models.Activity;
using TempusApi.Models.DetailedMapList;
using TempusApi.Models.Rank;
using TempusApi.Models.Responses;
using TempusHub.Core.Constants;
using TempusHub.Core.Models;
using TempusHub.Core.Models.Demystify;
using TempusHub.Core.Utilities;
using TempusHub.Infrastructure;

namespace TempusHub.Application.Services;

public sealed class TempusCacheService : IDisposable
{
    private readonly Timer _updateTimer;
    private readonly Tempus _tempusDataService;

    private readonly TempusHubMySqlService _mySql;
    
    public event EventHandler? DataUpdated;
    public RecentActivityModel RecentActivity { get; private set; }
    public RecentActivityWithZonedData RecentActivityWithZonedData { get; private set; } = new();
    public List<TopPlayerOnline> TopPlayersOnline { get; private set; } = new();
    public List<DetailedMapOverviewModel> DetailedMapList { get; set; }
    public PlayerLeaderboards PlayerLeaderboards { get; set; }
    public List<ServerStatusModel> ServerStatusList { get; set; }
    public List<TempusRealName> RealNames { get; set; }
    public List<TempusRankColor> TempusRankColors { get; set; }


    public TempusCacheService(Tempus tempusDataService, TempusHubMySqlService mySql)
    {
        _tempusDataService = tempusDataService;
        _mySql = mySql;
        
        _updateTimer = new Timer(async _ => await UpdateAllCachedDataAsync().ConfigureAwait(false), null, TimeSpan.FromMinutes(1.5), TimeSpan.FromMinutes(1.5));
    }

    public async Task UpdateAllCachedDataAsync()
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
            Log.Error(e, "Unhandled exception while caching all data");
        }
    }
    private async Task UpdateRecentActivityAsync()
    {
        RecentActivity = await _tempusDataService.GetRecentActivityAsync().ConfigureAwait(false);
    }
    private async Task UpdateRecentActivityWithZonedDataAsync()
    {
        var recentActivityWithZonedData = new RecentActivityWithZonedData();

        var tasks = new List<Task>();
        tasks.AddRange(RecentActivity.MapRecords.Select(async x => recentActivityWithZonedData.MapWr.Add(await PopulateRecordDataAsync(x).ConfigureAwait(false))));
        tasks.AddRange(RecentActivity.CourseRecords.Select(async x => recentActivityWithZonedData.CourseWr.Add(await PopulateRecordDataAsync(x).ConfigureAwait(false))));
        tasks.AddRange(RecentActivity.BonusRecords.Select(async x => recentActivityWithZonedData.BonusWr.Add(await PopulateRecordDataAsync(x).ConfigureAwait(false))));
        tasks.AddRange(RecentActivity.MapTopTimes.Select(async x => recentActivityWithZonedData.MapTt.Add(await PopulateRecordDataAsync(x).ConfigureAwait(false))));
        await Task.WhenAll(tasks).ConfigureAwait(false);

        RecentActivityWithZonedData = recentActivityWithZonedData;
    }
    private async Task UpdateTopOnlinePlayersAsync()
    {
        var tempTopPlayersOnline = new List<TopPlayerOnline>();

        if (ServerStatusList.Count == 0)
        {
            Log.Error("Could not get any server status's");
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
        var userIdStrings = usersWithId.Where(user => user?.Id != null).Select(user => user.Id.ToString()).ToList().Distinct();

        // Query all at once for all users ranks
        var rankTasks = new List<Task<Rank>>();
        rankTasks.AddRange(userIdStrings.Select(_tempusDataService.GetUserRankAsync));
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
                    x.GameInfo.Users.Any(z => z.Id.HasValue && z.Id == rankedUser.Player.Id || z.SteamId == rankedUser.Player.SteamId));
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
        DetailedMapList = await _tempusDataService.GetDetailedMapListAsync().ConfigureAwait(false);
        var lines = await File.ReadAllLinesAsync(LocalFileConstants.MapClasses).ConfigureAwait(false);
        foreach (var line in lines)
        {
            var lineData = line.Split(',');

            // Will always only be 1 char, also converts it to a char by calling .First()
            var classChar = lineData[0].First();
            var mapName = lineData[1];
            var matchedDetailedMapInfo = DetailedMapList.FirstOrDefault(x => string.Equals(x.Name, mapName, StringComparison.InvariantCultureIgnoreCase));
            if (matchedDetailedMapInfo == null)
            {
                Log.Warning("Could not find map data for: {MapName}", mapName);
            }
            else
            {
                matchedDetailedMapInfo.IntendedClass = classChar;
            }
        }

        // Check if there are maps with no
        var noClassDataMaps = DetailedMapList.Where(x => x.IntendedClass == default).ToArray();
        foreach (var noClassDataMap in noClassDataMaps)
        {
            Log.Warning("No class data for {MapName}", noClassDataMap.Name);
            noClassDataMap.IntendedClass = 'B';
        }
    }
    private async Task UpdatePlayerLeaderboardsAsync()
    {
        PlayerLeaderboards = new PlayerLeaderboards
        {
            Overall = await _tempusDataService.GetOverallRanksOverview().ConfigureAwait(false),
            Soldier = await _tempusDataService.GetSoldierRanksOverview().ConfigureAwait(false),
            Demoman = await _tempusDataService.GetDemomanRanksOverview().ConfigureAwait(false)
        };
    }
    private async Task UpdateServerStatusListAsync()
    {
        ServerStatusList = await _tempusDataService.GetServerStatusAsync().ConfigureAwait(false);

        var servers = ServerStatusList.Where(x => x != null).ToList();

        // Get all valid online users
        var validUsers = servers.Where(x => x.GameInfo != null &&
                                            (x.GameInfo != null || x.ServerInfo != null ||
                                             x.GameInfo.Users != null) &&
                                            x.GameInfo.Users.Count != 0)
            .SelectMany(x => x.GameInfo.Users).ToList();

        var usersWithoutId = validUsers.Where(x => x is { Id: null }).ToList();

        if (usersWithoutId.Any())
        {
            var tasks = usersWithoutId.Select(async x => (await _tempusDataService.GetSearchResultAsync(x.Name).ConfigureAwait(false))?.Players?.FirstOrDefault(y => y.SteamId == x.SteamId));
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
        var jsonText = await File.ReadAllTextAsync(LocalFileConstants.TempusNames).ConfigureAwait(false);
        RealNames = JsonConvert.DeserializeObject<List<TempusRealName>>(jsonText);
    }
    private async Task UpdateTempusColorsAsync()
    {
        var jsonText = await File.ReadAllTextAsync(LocalFileConstants.TempusColors).ConfigureAwait(false);
        TempusRankColors = JsonConvert.DeserializeObject<List<TempusRankColor>>(jsonText);
    }
    public string GetRealName(int tempusId)
    {
        return RealNames.FirstOrDefault(x => x.Id == tempusId)?.RealName;
    }
    
    private async Task<RecordWithZonedData> PopulateRecordDataAsync(TempusRecordBase recordBase)
    {
        if (recordBase is null)
        {
            throw new ArgumentNullException(nameof(recordBase));
        }

        recordBase = recordBase.ToCached();

        var cacheRecordBase = (CachedTempusRecordBase)recordBase;

        var zonedData = await _tempusDataService.GetTopZonedTimes(cacheRecordBase.MapInfo.Name, cacheRecordBase.ZoneInfo.Type, cacheRecordBase.ZoneInfo.Zoneindex).ConfigureAwait(false);

        if (cacheRecordBase.CachedTime?.CurrentWrDuration != null)
            return new RecordWithZonedData
            {
                Record = cacheRecordBase,
                ZonedData = zonedData
            };
            
        cacheRecordBase.CachedTime = new MapRecordCache
        {
            ClassId = cacheRecordBase.RecordInfo.Class,
            MapId = cacheRecordBase.MapInfo.Id,
            ZoneId = cacheRecordBase.ZoneInfo.Zoneindex,
            ZoneType = cacheRecordBase.ZoneInfo.Type,
            CurrentWrDuration = cacheRecordBase.RecordInfo.Class == 4 
                ? zonedData.Runs.DemomanRuns.OrderByDuration().First().Duration 
                : zonedData.Runs.SoldierRuns.OrderByDuration().First().Duration
        };

        return new RecordWithZonedData
        {
            Record = cacheRecordBase,
            ZonedData = zonedData
        };     
    }
    
    private void OnDataUpdated(EventArgs e)
        => DataUpdated?.Invoke(this, e);

    public void Dispose()
    {
        _updateTimer?.Dispose();
    }
}