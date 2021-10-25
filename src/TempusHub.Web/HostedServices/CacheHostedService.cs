using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using TempusHub.Application.Services;

namespace TempusHub.Web.HostedServices;

public class CacheHostedService : IHostedService
{
    private readonly TempusCacheService _tempusCacheService;
    private readonly TempusRecordCacheService _tempusRecordCacheService;

    public CacheHostedService(TempusCacheService tempusCacheService, TempusRecordCacheService tempusRecordCacheService)
    {
        _tempusCacheService = tempusCacheService;
        _tempusRecordCacheService = tempusRecordCacheService;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        var cachedDataTask = _tempusCacheService.UpdateAllCachedDataAsync();
        var cacheRecordsTask = _tempusRecordCacheService.CacheAllRecordsAsync();

        var tasks = new[] {cachedDataTask, cacheRecordsTask};

        await Task.WhenAll(tasks).ConfigureAwait(false);
    }

    public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
}