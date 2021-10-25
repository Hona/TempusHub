﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TempusHubBlazor.Models.Tempus.DetailedMapList;
using TempusHubBlazor.Models.Tempus.Rank;
using TempusHubBlazor.Models.Tempus.Responses;
using Newtonsoft.Json;
using TempusHubBlazor.Models.MySQL;
using System.Net.Http;
using System.Web;
using Serilog;
using TempusHubBlazor.Models.Tempus.Activity;
using TempusHubBlazor.Models;
using TempusHubBlazor.Utilities;
using TempusHubBlazor.Models.Tempus.PlayerStats;

namespace TempusHubBlazor.Data;

public class TempusDataService
{
    public TempusDataService(TempusHubMySqlService dataService)
    {
        TempusHubMySqlService = dataService;
        _tempusRecordCacheService = new TempusRecordCacheService(this);
        UpdateMapListAsync().GetAwaiter().GetResult();
    }
    public async Task CacheAllWRsAsync()
    {
        await _tempusRecordCacheService.CacheAllRecordsAsync().ConfigureAwait(false);
    }
    private readonly HttpClient _httpClient = new()
    {
        BaseAddress = new Uri("https://tempus.xyz")
    };
    private readonly TempusRecordCacheService _tempusRecordCacheService;
    private TempusHubMySqlService TempusHubMySqlService { get; }
    private List<DetailedMapOverviewModel> _mapList;
    public List<DetailedMapOverviewModel> MapList
    {
        get
        {
            if (_mapList != null && (_mapList != null || _mapList.Count != 0)) return _mapList;
                
            // Bad to use async like this
            UpdateMapListAsync().GetAwaiter().GetResult();
            return MapList;
        }
        private set => _mapList = value;
    }

    private List<string> MapNameList { get; set; }
    private static string GetFullApiPath(string partial) => "/api" + partial;
    private async Task<T> GetResponseAsync<T>(string request, bool throwOnFail = true)
    {
        var fullPath = GetFullApiPath(request);
            
        try
        {
            var startTime = DateTime.Now;
            var response = await _httpClient.GetAsync(fullPath).ConfigureAwait(false);

            response.EnsureSuccessStatusCode();

            object stringValue = await response.Content
                .ReadAsStringAsync().ConfigureAwait(false);
            var duration = DateTime.Now - startTime;

            Log.Information("Tempus /api {Request} {Milliseconds} ms", request, duration.TotalMilliseconds);
            // If T is a string, don't deserialise
            return typeof(T) == typeof(string)
                ? (T)stringValue
                : JsonConvert.DeserializeObject<T>((string)stringValue);
        }
        catch (Exception e)
        {
            if (throwOnFail)
            {
                throw new Exception("Failed on: " + fullPath, e);
            }
            
            return default;
        }
    }

    public async Task<MapFullOverviewModel> GetFullMapOverViewAsync(string map)
    {
        if (map == null)
        {
            return null;
        }

        var overview = await
            GetResponseAsync<MapFullOverviewModel>($"/maps/name/{ParseMapName(map)}/fullOverview").ConfigureAwait(false);
        return overview;
    }
    public async Task<MapRecordCache> UpdateCachedWrDataAsync(MapRecordCache cached, TempusRecordBase map)
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
                ZoneId = map.ZoneInfo.ZoneIndex
            };

            await TempusHubMySqlService.UpdateCachedRecordAsync(tempNewCache).ConfigureAwait(false);
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
                ZoneId = map.ZoneInfo.ZoneIndex
            };

            await TempusHubMySqlService.UpdateCachedRecordAsync(tempNewCache).ConfigureAwait(false);
        }

        return tempNewCache;
    }
    public async Task<ZonedRecordsModel> GetTopZonedTimes(string mapName, string zoneType, int zoneIndex = 1) 
        => await GetResponseAsync<ZonedRecordsModel>($"/maps/name/{mapName}/zones/typeindex/{zoneType}/{zoneIndex}/records/list").ConfigureAwait(false);
    public async Task<RecentActivityModel> GetRecentActivityAsync()
    {
        var activity = await GetResponseAsync<RecentActivityModel>("/activity").ConfigureAwait(false);
        var worldRecordActivity = new List<TempusRecordBase>();

        // We are basically excluding the map top times
        worldRecordActivity.AddRange(activity.BonusRecords);
        worldRecordActivity.AddRange(activity.CourseRecords);
        worldRecordActivity.AddRange(activity.MapRecords);

        foreach (var map in worldRecordActivity)
        {
            // Fetch the latest cache
            var cached = await TempusHubMySqlService.GetCachedRecordsAsync(map.MapInfo.Id, map.RecordInfo.Class, map.ZoneInfo.Type, map.ZoneInfo.ZoneIndex).ConfigureAwait(false);
            map.CachedTime = await UpdateCachedWrDataAsync(cached, map).ConfigureAwait(false);
        }

        // Just apply the map wr to top time
        foreach (var record in activity.MapTopTimes)
        {
            record.CachedTime = await TempusHubMySqlService.GetCachedRecordsAsync(record.MapInfo.Id, record.RecordInfo.Class, record.ZoneInfo.Type).ConfigureAwait(false);
        }

        return activity;
    }
    public async Task<PlayerMapSearchResult> GetSearchResultAsync(string query) =>
        await GetResponseAsync<PlayerMapSearchResult>($"/search/playersAndMaps/{HttpUtility.UrlEncode(query)}").ConfigureAwait(false);
    public async Task<List<ServerStatusModel>> GetServerStatusAsync() =>
        await GetResponseAsync<List<ServerStatusModel>>("/servers/statusList").ConfigureAwait(false);

    public async Task<List<ShortMapInfoModel>> GetMapListAsync() =>
        await GetResponseAsync<List<ShortMapInfoModel>>("/maps/list").ConfigureAwait(false);

    public async Task<List<DetailedMapOverviewModel>> GetDetailedMapListAsync() =>
        await GetResponseAsync<List<DetailedMapOverviewModel>>("/maps/detailedList").ConfigureAwait(false);
    public async Task<RanksOverviewModel> GetOverallRanksOverview() =>
        await GetResponseAsync<RanksOverviewModel>("/ranks/overall").ConfigureAwait(false);
    public async Task<RanksOverviewModel> GetDemomanRanksOverview() =>
        await GetResponseAsync<RanksOverviewModel>("/ranks/class/4").ConfigureAwait(false);
    public async Task<RanksOverviewModel> GetSoldierRanksOverview() =>
        await GetResponseAsync<RanksOverviewModel>("/ranks/class/3").ConfigureAwait(false);
    public async Task<Rank> GetUserRankAsync(string id) => await GetResponseAsync<Rank>($"/players/id/{id}/rank").ConfigureAwait(false);
    public async Task<PlayerStatsModel> GetUserStatsAsync(string id) =>
        await GetResponseAsync<PlayerStatsModel>($"/players/id/{id}/stats", throwOnFail: false).ConfigureAwait(false);
    public async Task<RecordWithZonedData> PopulateRecordDataAsync(TempusRecordBase recordBase)
    {
        if (recordBase is null)
        {
            throw new ArgumentNullException(nameof(recordBase));
        }
            
        var zonedData = await GetTopZonedTimes(recordBase.MapInfo.Name, recordBase.ZoneInfo.Type, recordBase.ZoneInfo.ZoneIndex).ConfigureAwait(false);

        if (recordBase.CachedTime?.CurrentWrDuration != null)
            return new RecordWithZonedData
            {
                Record = recordBase,
                ZonedData = zonedData
            };
            
        recordBase.CachedTime = new MapRecordCache
        {
            ClassId = recordBase.RecordInfo.Class,
            MapId = recordBase.MapInfo.Id,
            ZoneId = recordBase.ZoneInfo.ZoneIndex,
            ZoneType = recordBase.ZoneInfo.Type,
            CurrentWrDuration = recordBase.RecordInfo.Class == 4 
                ? zonedData.Runs.DemomanRuns.OrderByDuration().First().Duration 
                : zonedData.Runs.SoldierRuns.OrderByDuration().First().Duration
        };

        return new RecordWithZonedData
        {
            Record = recordBase,
            ZonedData = zonedData
        };     
    }
    public async Task<List<ServerDemoModel>> GetServerDemosAsync(int serverId)
        => await GetResponseAsync<List<ServerDemoModel>>($"/servers/{serverId}/demos").ConfigureAwait(false);

    public async Task<ServerDemoFullOverview> GetDemoInfoAsync(int demoId)
        => await GetResponseAsync<ServerDemoFullOverview>($"/demos/id/{demoId}/overview").ConfigureAwait(false);
    //RunInfoModel
    public async Task<RunInfoModel> GetRunInfoAsync(int runId)
        => await GetResponseAsync<RunInfoModel>($"/records/id/{runId}/overview").ConfigureAwait(false);

    public string ParseMapName(string map)
    {
        if (MapNameList == null || MapNameList.Count == 0)
        {
            UpdateMapListAsync().GetAwaiter().GetResult();
        }

        if (string.IsNullOrWhiteSpace(map))
        {
            return string.Empty;
        }
        map = map.ToLower();
        if (MapNameList.Contains(map)) return map;

        foreach (var mapName in MapNameList)
        {
            var mapParts = mapName.Split('_');
            if (mapParts.Any(mapPart => map == mapPart)) return mapName;
        }

        throw new Exception("Map not found");
    }

    private async Task UpdateMapListAsync()
    {
        var maps = await GetDetailedMapListAsync().ConfigureAwait(false);
        MapList = maps.OrderBy(x => x.Name).ToList();
        MapNameList = maps.ConvertAll(x => x.Name).Where(x => !string.IsNullOrWhiteSpace(x)).ToList();
    }
}