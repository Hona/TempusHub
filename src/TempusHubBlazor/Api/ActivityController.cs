using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using TempusHubBlazor.Models;
using TempusHubBlazor.Models.Api;
using TempusHubBlazor.Services;

namespace TempusHubBlazor.Api;

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
        var output = recordBase.Select(x => new RecordResponse
        {
            PlayerInfo = new PlayerResponse
            {
                Id = x.Record.PlayerInfo.Id,
                SteamId = x.Record.PlayerInfo.SteamId,
                Name = x.Record.PlayerInfo.Name,
                RealName = null
            },
            CachedTime = x.Record.CachedTime,
            MapInfo = x.Record.MapInfo,
            RecordInfo = x.Record.RecordInfo,
            ZoneInfo = x.Record.ZoneInfo
        }).ToList();

        foreach (var record in output)
        {
            var realName = _tempusCacheService.GetRealName(record.PlayerInfo.Id);

            if (realName == null) continue;

            var realNamePlayer = new PlayerResponse
            {
                Id = record.PlayerInfo.Id,
                Name = record.PlayerInfo.Name,
                SteamId = record.PlayerInfo.SteamId,
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