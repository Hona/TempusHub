using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using TempusHubBlazor.Models.Tempus.Responses;
using TempusHubBlazor.Services;

namespace TempusHubBlazor.Api;

[Route("api/[controller]")]
[ApiController]
public class ServersOverviewController : ControllerBase
{
    private readonly TempusCacheService _tempusCacheService;
    public ServersOverviewController(TempusCacheService tempusCacheService)
    {
        _tempusCacheService = tempusCacheService;
    }

    [HttpGet]
    public ActionResult<IEnumerable<ServerStatusModel>> Get()
    {
        if (_tempusCacheService.ServerStatusList == null || _tempusCacheService.ServerStatusList.Count == 0)
        {
            return NoContent();
        }

        return _tempusCacheService.ServerStatusList;
    }
}