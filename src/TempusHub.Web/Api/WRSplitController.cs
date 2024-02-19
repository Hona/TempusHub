using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using TempusApi.Enums;
using TempusHub.Application.Services;
using TempusHub.Core.Models;
using TempusHub.Core.Utilities;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace TempusHub.Web.Api;

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
    public ActionResult<string> Get(long mapId, Class playerClass, ZoneType recordType, int zoneIndex = 1)
    {
        List<RecordWithZonedData> recentRecords;
        switch (recordType)
        {
            case ZoneType.Map:
                recentRecords = _tempusCacheService.RecentActivityWithZonedData.MapWr.ToList();
                break;
            case ZoneType.Course:
                recentRecords = _tempusCacheService.RecentActivityWithZonedData.CourseWr.ToList();
                break;
            case ZoneType.Bonus:
                recentRecords = _tempusCacheService.RecentActivityWithZonedData.BonusWr.ToList();
                break;
            default:
                return BadRequest(nameof(recordType));
        }

        var foundRecord = recentRecords.FirstOrDefault(x => x.Record.MapInfo.Id == mapId &&
                                                            x.Record.RecordInfo.Class == playerClass &&
                                                            x.Record.ZoneInfo.Zoneindex == zoneIndex);
        if (foundRecord == null)
        {
            return NotFound();
        }

        var cacheRecord = foundRecord.Record as CachedTempusRecordBase;

        return Ok(TempusUtilities.GetWrSplitString(cacheRecord?.CachedTime, foundRecord.ZonedData));
    }
}