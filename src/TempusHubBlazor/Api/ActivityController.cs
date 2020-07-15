using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MySql.Data.MySqlClient;
using TempusHubBlazor.Models;
using TempusHubBlazor.Models.Api;
using TempusHubBlazor.Models.Tempus.Activity;
using TempusHubBlazor.Models.Tempus.Responses;
using TempusHubBlazor.Services;

namespace TempusHubBlazor.Api
{
    [Route("api/[controller]")]
    [ApiController]
    public class ActivityController : ControllerBase
    {
        private TempusCacheService _tempusCacheService;
        public ActivityController(TempusCacheService tempusCacheService)
        {
            _tempusCacheService = tempusCacheService;
        }

        private List<RecordResponse> TransformRecordData(List<RecordWithZonedData> recordBase)
        {
            var output = recordBase.Select(x => new RecordResponse
            {
                PlayerInfo = new PlayerResponse
                {
                    Id = x.Record.PlayerInfo.Id,
                    Steamid = x.Record.PlayerInfo.Steamid,
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
                if (realName != null)
                {
                    var realNamePlayer = new PlayerResponse
                    {
                        Id = record.PlayerInfo.Id,
                        Name = record.PlayerInfo.Name,
                        Steamid = record.PlayerInfo.Steamid,
                        RealName = realName
                    };

                    record.PlayerInfo = realNamePlayer;
                }
            }

            return output;
        }

        [HttpGet("MapWR")]
        public ActionResult<List<RecordResponse>> GetMapWR()
        {
            if (_tempusCacheService.RecentActivityWithZonedData == null)
            {
                return NoContent();
            }

            return TransformRecordData(_tempusCacheService.RecentActivityWithZonedData.MapWR);
        }

        [HttpGet("CourseWR")]
        public ActionResult<List<RecordResponse>> GetCourseWR()
        {
            if (_tempusCacheService.RecentActivityWithZonedData == null)
            {
                return NoContent();
            }

            return TransformRecordData(_tempusCacheService.RecentActivityWithZonedData.CourseWR);
        }

        [HttpGet("BonusWR")]
        public ActionResult<List<RecordResponse>> GetBonusWR()
        {
            if (_tempusCacheService.RecentActivityWithZonedData == null)
            {
                return NoContent();
            }

            return TransformRecordData(_tempusCacheService.RecentActivityWithZonedData.BonusWR);
        }

        [HttpGet("MapTT")]
        public ActionResult<List<RecordResponse>> GetMapTT()
        {
            if (_tempusCacheService.RecentActivityWithZonedData == null)
            {
                return NoContent();
            }

            return TransformRecordData(_tempusCacheService.RecentActivityWithZonedData.MapTT);
        }
    }
}
