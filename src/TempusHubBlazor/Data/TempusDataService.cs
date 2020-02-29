﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using TempusHubBlazor.Constants;
using TempusHubBlazor.Models.Tempus.DetailedMapList;
using TempusHubBlazor.Models.Tempus.Rank;
using TempusHubBlazor.Models.Tempus.Responses;
using TempusHubBlazor.Logging;
using Newtonsoft.Json;
using TempusHubBlazor.Models.MySQL;
using System.Net.Http;
using System.Net.Http.Headers;
using TempusHubBlazor.Models.Tempus.Activity;
using TempusHubBlazor.Models;
using TempusHubBlazor.Utilities;

namespace TempusHubBlazor.Data
{
    public class TempusDataService
    {
        public TempusDataService(TempusHubMySqlService dataService)
        {
            TempusHubMySqlService = dataService;
            TempusRecordCacheService = new TempusRecordCacheService(this);
            UpdateMapListAsync().GetAwaiter().GetResult();
        }
        public async Task CacheAllWRsAsync()
        {
            await TempusRecordCacheService.CacheAllRecordsAsync();
        }
        private static readonly HttpClient _httpClient = new HttpClient
        {
            BaseAddress = new Uri("https://tempus.xyz")
        };
        private readonly TempusRecordCacheService TempusRecordCacheService;   
        public TempusHubMySqlService TempusHubMySqlService { get; set; }
        private static readonly Stopwatch Stopwatch = new Stopwatch();
        private List<DetailedMapOverviewModel> _mapList;
        public List<DetailedMapOverviewModel> MapList
        {
            get
            {
                if (_mapList != null || _mapList.Count != 0) return _mapList;
                UpdateMapListAsync().GetAwaiter().GetResult();
                return MapList;
            }
            private set => _mapList = value;
        }

        public List<string> MapNameList { get; set; }
        private static string GetFullAPIPath(string partial) => "/api" + partial;
        private static async Task<T> GetResponseAsync<T>(string request)
        {
            var fullPath = GetFullAPIPath(request);
            Stopwatch.Restart();
            try
            {
                var response = await _httpClient.GetAsync(fullPath);

                if (response.IsSuccessStatusCode)
                {
                    object stringValue = await response.Content.ReadAsStringAsync();
                    
                    Stopwatch.Stop();
                    Logger.LogInfo("Tempus /api" + request + " " + Stopwatch.ElapsedMilliseconds + "ms");
                    // If T is a string, don't deserialise
                    return typeof(T) == typeof(string)
                        ? (T)stringValue
                        : JsonConvert.DeserializeObject<T>((string)stringValue);
                }
                else
                {
                    Logger.LogError("Couldn't get Tempus API request: " + fullPath);
                    return default;
                }
            }
            catch (Exception e)
            {
                Logger.LogException(e, "Failed on: " + fullPath);
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
                GetResponseAsync<MapFullOverviewModel>($"/maps/name/{ParseMapName(map)}/fullOverview");
            return overview;
        }
        public async Task<MapRecordCache> UpdateCachedWRDataAsync(MapRecordCache cached, TempusRecordBase map)
        {
            MapRecordCache tempNewCache = cached;

            // Check for no data
            if (cached == null || !cached.CurrentWRDuration.HasValue && !cached.OldWRDuration.HasValue)
            {
                // No data
                tempNewCache = new MapRecordCache
                {
                    MapId = map.MapInfo.Id,
                    CurrentWRDuration = map.RecordInfo.Duration,
                    ClassId = map.RecordInfo.Class,
                    ZoneType = map.ZoneInfo.Type,
                    OldWRDuration = null,
                    ZoneId = map.ZoneInfo.Zoneindex
                };

                await TempusHubMySqlService.UpdateCachedRecordAsync(tempNewCache);
            }
            // Check if the cached wr duration is different to the new record
            else if (cached.CurrentWRDuration.HasValue && cached.CurrentWRDuration.Value != map.RecordInfo.Duration)
            {
                tempNewCache = new MapRecordCache
                {
                    MapId = map.MapInfo.Id,
                    CurrentWRDuration = map.RecordInfo.Duration,
                    OldWRDuration = cached.CurrentWRDuration,
                    ClassId = map.RecordInfo.Class,
                    ZoneType = map.ZoneInfo.Type,
                    ZoneId = map.ZoneInfo.Zoneindex
                };

                await TempusHubMySqlService.UpdateCachedRecordAsync(tempNewCache);
            }

            return tempNewCache;
        }
        public async Task<ZonedRecordsModel> GetTopZonedTimes(string mapName, string zoneType, int zoneId = 1) 
            => await GetResponseAsync<ZonedRecordsModel>($"/maps/name/{mapName}/zones/typeindex/{zoneType}/{zoneId}/records/list");
        public async Task<RecentActivityModel> GetRecentActivityAsync()
        {
            var activity = await GetResponseAsync<RecentActivityModel>("/activity");
            var worldRecordActivity = new List<TempusRecordBase>();

            // We are basically excluding the map top times
            worldRecordActivity.AddRange(activity.BonusRecords);
            worldRecordActivity.AddRange(activity.CourseRecords);
            worldRecordActivity.AddRange(activity.MapRecords);

            foreach (var map in worldRecordActivity)
            {
                // Fetch the latest cache
                var cached = await TempusHubMySqlService.GetCachedRecordsAsync(map.MapInfo.Id, map.RecordInfo.Class, map.ZoneInfo.Type, map.ZoneInfo.Zoneindex);
                map.CachedTime = await UpdateCachedWRDataAsync(cached, map);
            }

            // Just apply the map wr to top time
            foreach (var record in activity.MapTopTimes)
            {
                record.CachedTime = await TempusHubMySqlService.GetCachedRecordsAsync(record.MapInfo.Id, record.RecordInfo.Class, record.ZoneInfo.Type);
            }

            return activity;
        }

        public async Task<List<ServerStatusModel>> GetServerStatusAsync() =>
            await GetResponseAsync<List<ServerStatusModel>>("/servers/statusList");

        public async Task<List<ShortMapInfoModel>> GetMapListAsync() =>
            await GetResponseAsync<List<ShortMapInfoModel>>("/maps/list");

        public async Task<List<DetailedMapOverviewModel>> GetDetailedMapListAsync() =>
            await GetResponseAsync<List<DetailedMapOverviewModel>>("/maps/detailedList");
        public async Task<RanksOverviewModel> GetOverallRanksOverview() =>
            await GetResponseAsync<RanksOverviewModel>("/ranks/overall");
        public async Task<RanksOverviewModel> GetDemomanRanksOverview() =>
            await GetResponseAsync<RanksOverviewModel>("/ranks/class/4");
        public async Task<RanksOverviewModel> GetSoldierRanksOverview() =>
            await GetResponseAsync<RanksOverviewModel>("/ranks/class/3");
        public async Task<Rank> GetUserRankAsync(string id) => await GetResponseAsync<Rank>($"/players/id/{id}/rank");
        public async Task<RecordWithZonedData> PopulateRecordDataAsync(TempusRecordBase recordBase)
        {
            var zonedData = await GetTopZonedTimes(recordBase.MapInfo.Name, recordBase.ZoneInfo.Type, recordBase.ZoneInfo.Zoneindex);

            if (recordBase.CachedTime == null || !recordBase.CachedTime.CurrentWRDuration.HasValue)
            {
                recordBase.CachedTime = new MapRecordCache
                {
                    ClassId = recordBase.RecordInfo.Class,
                    MapId = recordBase.MapInfo.Id,
                    ZoneId = recordBase.ZoneInfo.Zoneindex,
                    ZoneType = recordBase.ZoneInfo.Type,
                };
                if (recordBase.RecordInfo.Class == 4)
                {
                    recordBase.CachedTime.CurrentWRDuration = zonedData.Runs.DemomanRuns.OrderByDuration().First().Duration;
                }
                else
                {
                    recordBase.CachedTime.CurrentWRDuration = zonedData.Runs.SoldierRuns.OrderByDuration().First().Duration;
                }
            }

            return new RecordWithZonedData
            {
                Record = recordBase,
                ZonedData = zonedData
            };     
        }
        public async Task<List<ServerDemoModel>> GetServerDemosAsync(int serverId)
            => (await GetResponseAsync<List<ServerDemoModel>>($"/servers/{serverId}/demos"));

        public async Task<ServerDemoFullOverview> GetDemoInfoAsync(int demoId)
            => await GetResponseAsync<ServerDemoFullOverview>($"/demos/id/{demoId}/overview");
        //RunInfoModel
        public async Task<RunInfoModel> GetRunInfoAsync(int runId)
            => await GetResponseAsync<RunInfoModel>($"/records/id/{runId}/overview");

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

        public async Task UpdateMapListAsync()
        {
            var maps = await GetDetailedMapListAsync();
            MapList = maps.OrderBy(x => x.Name).ToList();
            MapNameList = maps.ConvertAll(x => x.Name).Where(x => !string.IsNullOrWhiteSpace(x)).ToList();
        }
    }
}
