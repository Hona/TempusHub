using System;
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

namespace TempusHubBlazor.Data
{
    public class TempusDataService
    {
        private static readonly HttpClient _httpClient = new HttpClient();
        public TempusDataService(TempusHubMySqlService dataService)
        {
            TempusHubMySqlService = dataService;
            _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }
        public TempusHubMySqlService TempusHubMySqlService { get; set; }
        private static readonly Stopwatch Stopwatch = new Stopwatch();
        private List<DetailedMapOverviewModel> _mapList;
        private List<MapFullOverviewModel> _fullOverviewCache = new List<MapFullOverviewModel>(TempusDataConstants.FullMapOverviewCacheSize);

        public List<DetailedMapOverviewModel> MapList
        {
            get
            {
                if (_mapList != null) return _mapList;
                UpdateMapListAsync().GetAwaiter().GetResult();
                return MapList;
            }
            private set => _mapList = value;
        }

        private void AddMapOverviewCacheItem(MapFullOverviewModel fullOverview)
        {
            _fullOverviewCache.Insert(0, fullOverview);
            var countToRemove = _fullOverviewCache.Count - TempusDataConstants.FullMapOverviewCacheSize;
            if (countToRemove <= 0) return;
            _fullOverviewCache.RemoveRange(TempusDataConstants.FullMapOverviewCacheSize - 1, _fullOverviewCache.Count - TempusDataConstants.FullMapOverviewCacheSize);

        }
        public List<string> MapNameList { get; set; }
        private static string GetFullAPIPath(string partial) => "https://tempus.xyz/api" + partial;
        private static async Task<T> GetResponseAsync<T>(string request)
        {
            Logger.LogInfo("Attempting: " + request);
            Stopwatch.Restart();
            try
            {
                var response = await _httpClient.GetAsync(GetFullAPIPath(request));

                if (response.IsSuccessStatusCode)
                {
                    Logger.LogInfo("Response success code");
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
                    Logger.LogError("Couldn't get Tempus API request");
                    return default;
                }
            }
            catch (Exception e)
            {
                Logger.LogException(e);
                return default;
            }
        }

        public async Task<MapFullOverviewModel> GetFullMapOverViewAsync(string map)
        {
            try
            {
                return _fullOverviewCache.First(x => x.MapInfo.Name == ParseMapName(map));
            }
            catch
            {
                // The map isn't in the cache
                var overview = await
                    GetResponseAsync<MapFullOverviewModel>($"/maps/name/{ParseMapName(map)}/fullOverview");
                AddMapOverviewCacheItem(overview);
                return overview;
            }

        }

        public async Task<ZonedRecordsModel> GetTopZonedTimes(string mapName, string zoneType, int zoneId = 1) 
            => await GetResponseAsync<ZonedRecordsModel>($"/maps/name/{mapName}/zones/typeindex/{zoneType}/{zoneId}/records/list");
        public async Task<RecentActivityModel> GetRecentActivityAsync()
        {
            var activity = await GetResponseAsync<RecentActivityModel>("/activity");
            foreach (var map in activity.MapRecords)
            {
                MapRecordCache tempNewCache = null;

                // Fetch the latest cache
                var cached = await TempusHubMySqlService.GetCachedRecordsAsync(map.MapInfo.Id, map.RecordInfo.Class, map.ZoneInfo.Type);

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
                map.CachedTime = tempNewCache;
            }
            return activity;
        }

        public async Task<List<ServerStatusModel>> GetServerStatusAsync() =>
            await GetResponseAsync<List<ServerStatusModel>>("/servers/statusList");

        public async Task<List<ShortMapInfoModel>> GetMapListAsync() =>
            await GetResponseAsync<List<ShortMapInfoModel>>("/maps/list");

        public async Task<List<DetailedMapOverviewModel>> GetDetailedMapListAsync() =>
            await GetResponseAsync<List<DetailedMapOverviewModel>>("/maps/detailedList");

        public async Task<Rank> GetUserRankAsync(string id) => await GetResponseAsync<Rank>($"/players/id/{id}/rank");

        private string ParseMapName(string map)
        {
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
            MapList = maps;
            MapNameList = maps.ConvertAll(x => x.Name).Where(x => !string.IsNullOrWhiteSpace(x)).ToList();
        }
    }
}
