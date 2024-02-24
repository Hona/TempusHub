using System.Collections.Generic;
using System.Threading.Tasks;
using DSharpPlus.Entities;
using TempusApi.Models.Responses;

namespace Empedo.Discord.Services
{
    public interface ITempusEmbedService
    {
        Task<List<DiscordEmbedBuilder>> GetServerOverviewAsync(List<ServerStatusModel> servers = null, bool decorateAllEmbeds = false);
        Task<List<DiscordEmbedBuilder>> GetTopPlayersOnlineAsync(List<ServerStatusModel> servers = null, bool decorateAllEmbeds = false);
        Task<List<DiscordEmbedBuilder>> GetRecentMapRecordsAsync(RecentActivityModel activity = null, bool decorateAllEmbeds = false);
        Task<List<DiscordEmbedBuilder>> GetRecentCourseRecordsAsync(RecentActivityModel activity = null, bool decorateAllEmbeds = false);
        Task<List<DiscordEmbedBuilder>> GetRecentBonusRecordsAsync(RecentActivityModel activity = null, bool decorateAllEmbeds = false);
        Task<List<DiscordEmbedBuilder>> GetRecentMapTopTimesAsync(RecentActivityModel activity = null, bool decorateAllEmbeds = false);
        Task<List<DiscordEmbedBuilder>> GetServerListAsync(List<ServerStatusModel> servers = null);
        Task<List<DiscordEmbedBuilder>> GetMapOverviewAsync(string mapName);
    }
}