using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;
using Empedo.Discord.Helpers;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using TempusApi;
using TempusHub.Application.Services;

namespace Empedo.Discord.Services
{
    public class LambdaHostedService : IHostedService, IDisposable
    {
        private readonly ILogger<LambdaHostedService> _logger;
        private readonly ITempusEmbedService _tempusEmbedService;
        private readonly IConfiguration _configuration;
        private readonly DiscordClient _discordClient;
        private readonly TempusCacheService _tempusCacheService;
        private DateTimeOffset _lastUpdate = DateTimeOffset.UtcNow;

        public LambdaHostedService(ILogger<LambdaHostedService> logger, ITempusEmbedService tempusEmbedService, IConfiguration configuration, DiscordClient discordClient, TempusCacheService tempusCacheService)
        {
            _logger = logger;
            _tempusEmbedService = tempusEmbedService;
            _configuration = configuration;
            _discordClient = discordClient;
            _tempusCacheService = tempusCacheService;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _discordClient.GuildDownloadCompleted += InitializeTimer;

            return Task.CompletedTask;
        }

        private Task InitializeTimer(DiscordClient discordClient, GuildDownloadCompletedEventArgs e)
        {
            _tempusCacheService.DataUpdated += OnDataUpdated; 
            
            return Task.CompletedTask;
        }

        private void OnDataUpdated(object sender, EventArgs e)
        {
            TickAsync();
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _tempusCacheService.DataUpdated -= OnDataUpdated;
            
            return Task.CompletedTask;
        }
        
        private async void TickAsync()
        {
            _logger.LogInformation("Updating...");
            var tasks = new List<Task>
            {
                UpdateOverviewsAsync(),
                UpdateRecentActivityAsync(),
                UpdateServerListAsync(),
                SendWorldRecordNotificationsAsync()
            };

            await Task.WhenAll(tasks);
            _lastUpdate = DateTimeOffset.UtcNow;
        }
        
        private async Task SendWorldRecordNotificationsAsync()
        {
            var records = _tempusCacheService.RecentActivityWithZonedData.MapWr
                .Where(x => TempusHelper.GetDateFromTimestamp(x.Record.RecordInfo.Date) > _lastUpdate)
                .ToList();
            
            var channel = await _discordClient.GetChannelAsync(1210787489061806122);
            
            var messages = await _tempusEmbedService.GetWorldRecordNotificationsAsync(records);

            foreach (var message in messages)
            {
                await message.SendAsync(channel);
            }
        }

        private async Task WipeChannelAsync(DiscordChannel discordChannel)
        {
            var messages = await discordChannel.GetMessagesAsync();

            if (!messages.Any())
            {
                return;
            }
            
            await discordChannel.DeleteMessagesAsync(messages);
        }

        private async Task<DiscordChannel> GetAndWipeChannelAsync(string configurationPath)
        {
            var channel = await _discordClient.GetChannelAsync(ulong.Parse(_configuration[configurationPath]));

            await WipeChannelAsync(channel);

            return channel;
        }

        private async Task UpdateOverviewsAsync()
        {
            var channel = await _discordClient.GetChannelAsync(ulong.Parse(_configuration["Lambda:OverviewsChannelId"]));

            var servers = _tempusCacheService.ServerStatusList;
            
            var serverOverviewEmbeds = await _tempusEmbedService.GetServerOverviewAsync(servers);
            var topPlayerOnlineEmbeds = await _tempusEmbedService.GetTopPlayersOnlineAsync(servers);
            
            await WipeChannelAsync(channel);
            
            await serverOverviewEmbeds.SendAll(channel);
            await topPlayerOnlineEmbeds.SendAll(channel);
        }

        private async Task UpdateRecentActivityAsync()
        {
            var channel = await _discordClient.GetChannelAsync(ulong.Parse(_configuration["Lambda:ActivityChannelId"]));

            var activity = _tempusCacheService.RecentActivity;

            var recentMapRecordEmbeds = await _tempusEmbedService.GetRecentMapRecordsAsync(activity);
            var recentCourseRecordEmbeds = await _tempusEmbedService.GetRecentCourseRecordsAsync(activity);
            var recentBonusRecordEmbeds = await _tempusEmbedService.GetRecentBonusRecordsAsync(activity);
            var recentMapTopTimeEmbeds = await _tempusEmbedService.GetRecentMapTopTimesAsync(activity);

            await WipeChannelAsync(channel);

            await recentMapRecordEmbeds.SendAll(channel);
            await recentCourseRecordEmbeds.SendAll(channel);
            await recentBonusRecordEmbeds.SendAll(channel);
            await recentMapTopTimeEmbeds.SendAll(channel);
        }

        private async Task UpdateServerListAsync()
        {
            var channel = await _discordClient.GetChannelAsync(ulong.Parse(_configuration["Lambda:ServersChannelId"]));

            var serverEmbeds = await _tempusEmbedService.GetServerListAsync();

            await WipeChannelAsync(channel);

            await serverEmbeds.SendAll(channel);
        }

        public void Dispose()
        {
            _tempusCacheService.DataUpdated -= OnDataUpdated;
        }
    }
}