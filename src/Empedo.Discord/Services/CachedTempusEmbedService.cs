using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DSharpPlus.Entities;
using Empedo.Discord.Models;
using TempusApi.Models.Responses;

namespace Empedo.Discord.Services
{
    public class CachedTempusEmbedService : ITempusEmbedService
    {
        private readonly TempusEmbedService _tempusEmbedService;

        private readonly Dictionary<string, TempusEmbedsCache> _embedsCaches = new();
        
        public CachedTempusEmbedService(TempusEmbedService tempusEmbedService)
        {
            _tempusEmbedService = tempusEmbedService;
        }

        private bool TryGetUnexpiredCache(string key, out List<DiscordEmbedBuilder> cache)
        {
            if (_embedsCaches.TryGetValue(key, out var tempCache) && (DateTime.Now - tempCache.UpdatedDate).TotalMinutes < 4.5)
            {
                cache = tempCache.Embeds;
                return true;
            }

            cache = null;
            return false;
        }

        private async Task<List<DiscordEmbedBuilder>> GetCacheOrAsync(string cacheKey, Func<Task<List<DiscordEmbedBuilder>>> asyncAction)
        {
            if (TryGetUnexpiredCache(cacheKey, out var cache))
            {
                return cache;
            }

            var output = await asyncAction.Invoke();

            _embedsCaches[cacheKey] = new TempusEmbedsCache(output);

            return output;
        }

        public async Task<List<DiscordEmbedBuilder>> GetServerOverviewAsync(List<ServerStatusModel> servers = null, bool decorateAllEmbeds = false) 
            => await GetCacheOrAsync("GetServerOverviewAsync", 
                async () => await _tempusEmbedService.GetServerOverviewAsync(servers, decorateAllEmbeds));

        public async Task<List<DiscordEmbedBuilder>> GetTopPlayersOnlineAsync(List<ServerStatusModel> servers = null, bool decorateAllEmbeds = false)
            => await GetCacheOrAsync("GetTopPlayersOnlineAsync", 
                async () => await _tempusEmbedService.GetTopPlayersOnlineAsync(servers, decorateAllEmbeds));

        public async Task<List<DiscordEmbedBuilder>> GetRecentMapRecordsAsync(RecentActivityModel activity = null, bool decorateAllEmbeds = false)
            => await GetCacheOrAsync("GetRecentMapRecordsAsync", 
                async () => await _tempusEmbedService.GetRecentMapRecordsAsync(activity, decorateAllEmbeds));

        public async Task<List<DiscordEmbedBuilder>> GetRecentCourseRecordsAsync(RecentActivityModel activity = null, bool decorateAllEmbeds = false)
            => await GetCacheOrAsync("GetRecentCourseRecordsAsync", 
                async () => await _tempusEmbedService.GetRecentCourseRecordsAsync(activity, decorateAllEmbeds));

        public async Task<List<DiscordEmbedBuilder>> GetRecentBonusRecordsAsync(RecentActivityModel activity = null, bool decorateAllEmbeds = false)
            => await GetCacheOrAsync("GetRecentBonusRecordsAsync", 
                async () => await _tempusEmbedService.GetRecentBonusRecordsAsync(activity, decorateAllEmbeds));

        public async Task<List<DiscordEmbedBuilder>> GetRecentMapTopTimesAsync(RecentActivityModel activity = null, bool decorateAllEmbeds = false)
            => await GetCacheOrAsync("GetRecentMapTopTimesAsync", 
                async () => await _tempusEmbedService.GetRecentMapTopTimesAsync(activity, decorateAllEmbeds));

        public async Task<List<DiscordEmbedBuilder>> GetServerListAsync(List<ServerStatusModel> servers = null)
            => await GetCacheOrAsync("GetServerListAsync", 
                async () => await _tempusEmbedService.GetServerListAsync(servers));

        public async Task<List<DiscordEmbedBuilder>> GetMapOverviewAsync(string mapName)
            => await _tempusEmbedService.GetMapOverviewAsync(mapName);
    }
}