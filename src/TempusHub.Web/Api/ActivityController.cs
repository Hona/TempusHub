using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using TempusHub.Application.Services;
using TempusHub.Core.Models;
using TempusHub.Web.ViewModels;

namespace TempusHub.Web.Api;

[Route("api/[controller]")]
[ApiController]
public class ActivityController : ControllerBase
{
    private readonly TempusCacheService _tempusCacheService;
    public ActivityController(TempusCacheService tempusCacheService)
    {
        _tempusCacheService = tempusCacheService;
    }

    private List<RecordResponse> TransformRecordData(IEnumerable<RecordWithZonedData> recordBase)
    {
        var output = recordBase.Select(x =>
        {
            var cachedRecord = x.Record as CachedTempusRecordBase;
            
            return new RecordResponse
            {
                PlayerInfo = new PlayerResponse
                {
                    Id = x.Record.PlayerInfo.Id,
                    Steamid = x.Record.PlayerInfo.Steamid,
                    Name = x.Record.PlayerInfo.Name,
                    RealName = null
                },
                CachedTime = cachedRecord?.CachedTime,
                MapInfo = x.Record.MapInfo,
                RecordInfo = x.Record.RecordInfo,
                ZoneInfo = x.Record.ZoneInfo
            };
        }).ToList();

        foreach (var record in output)
        {
            var realName = _tempusCacheService.GetRealName(record.PlayerInfo.Id);

            if (realName == null) continue;

            var realNamePlayer = new PlayerResponse
            {
                Id = record.PlayerInfo.Id,
                Name = record.PlayerInfo.Name,
                Steamid = record.PlayerInfo.Steamid,
                RealName = realName
            };

            record.PlayerInfo = realNamePlayer;
        }

        return output;
    }

    [HttpGet("MapWR")]
    public ActionResult<List<RecordResponse>> GetMapWr()
    {
        if (_tempusCacheService.RecentActivityWithZonedData == null)
        {
            return NoContent();
        }

        return TransformRecordData(_tempusCacheService.RecentActivityWithZonedData.MapWr);
    }

    [HttpGet("CourseWR")]
    public ActionResult<List<RecordResponse>> GetCourseWr()
    {
        if (_tempusCacheService.RecentActivityWithZonedData == null)
        {
            return NoContent();
        }

        return TransformRecordData(_tempusCacheService.RecentActivityWithZonedData.CourseWr);
    }

    [HttpGet("BonusWR")]
    public ActionResult<List<RecordResponse>> GetBonusWr()
    {
        if (_tempusCacheService.RecentActivityWithZonedData == null)
        {
            return NoContent();
        }

        return TransformRecordData(_tempusCacheService.RecentActivityWithZonedData.BonusWr);
    }

    [HttpGet("MapTT")]
    public ActionResult<List<RecordResponse>> GetMapTt()
    {
        if (_tempusCacheService.RecentActivityWithZonedData == null)
        {
            return NoContent();
        }

        return TransformRecordData(_tempusCacheService.RecentActivityWithZonedData.MapTt);
    }
}