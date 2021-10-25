using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using TempusApi.Models.Responses;
using TempusHub.Application.Services;

namespace TempusHub.Web.Api;

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