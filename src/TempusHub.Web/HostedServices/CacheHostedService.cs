using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Serilog;
using TempusHub.Application.Services;

namespace TempusHub.Web.HostedServices;

public class CacheHostedService : IHostedService
{
    private readonly TempusCacheService _tempusCacheService;
    private readonly TempusRecordCacheService _tempusRecordCacheService;
    private readonly ILogger _log;

    public CacheHostedService(TempusCacheService tempusCacheService, TempusRecordCacheService tempusRecordCacheService, ILogger log)
    {
        _tempusCacheService = tempusCacheService;
        _tempusRecordCacheService = tempusRecordCacheService;
        _log = log;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        await _tempusCacheService.UpdateAllCachedDataAsync().ConfigureAwait(false);

        if (Environment.GetEnvironmentVariable("CACHE_ALL_RECORDS")?.ToLower() == "true")
        {
            await _tempusRecordCacheService.CacheAllRecordsAsync().ConfigureAwait(false);
        }
        _log.Information("Done startup cache tasks");
    }

    public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
}