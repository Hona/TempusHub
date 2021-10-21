using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using TempusHubBlazor.Models;
using TempusHubBlazor.Services;
using TempusHubBlazor.Utilities;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace TempusHubBlazor.Api;

[Route("api/[controller]")]
[ApiController]
public class WrSplitController : ControllerBase
{
    private readonly TempusCacheService _tempusCacheService;
    public WrSplitController(TempusCacheService tempusCacheService)
    {
        _tempusCacheService = tempusCacheService;
    }

    // GET api/<WRSplitController>/jabroni/4
    [HttpGet("{mapId:int}/{playerClass:int}/{recordType}/{zoneIndex:int?}")]
    public ActionResult<string> Get(int mapId, int playerClass, string recordType, int zoneIndex = 1)
    {
        if (recordType is null)
        {
            throw new ArgumentNullException(nameof(recordType));
        }
            
        List<RecordWithZonedData> recentRecords;
        switch (recordType.ToUpperInvariant())
        {
            case "MAP":
                recentRecords = _tempusCacheService.RecentActivityWithZonedData.MapWr;
                break;
            case "COURSE":
                recentRecords = _tempusCacheService.RecentActivityWithZonedData.CourseWr;
                break;
            case "BONUS":
                recentRecords = _tempusCacheService.RecentActivityWithZonedData.BonusWr;
                break;
            default:
                return BadRequest(nameof(recordType));
        }

        var foundRecord = recentRecords.FirstOrDefault(x => x.Record.MapInfo.Id == mapId &&
                                                            x.Record.RecordInfo.Class == playerClass &&
                                                            x.Record.ZoneInfo.ZoneIndex == zoneIndex);
        if (foundRecord == null)
        {
            return NotFound();
        }

        return Ok(TempusUtilities.GetWrSplitString(foundRecord.Record.CachedTime, foundRecord.ZonedData));
    }
}