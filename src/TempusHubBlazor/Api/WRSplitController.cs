using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using TempusHubBlazor.Constants;
using TempusHubBlazor.Data;
using TempusHubBlazor.Models;
using TempusHubBlazor.Models.Tempus.Activity;
using TempusHubBlazor.Services;
using TempusHubBlazor.Utilities;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace TempusHubBlazor.Api
{
    [Route("api/[controller]")]
    [ApiController]
    public class WRSplitController : ControllerBase
    {
        private TempusCacheService _tempusCacheService;
        public WRSplitController(TempusCacheService tempusCacheService)
        {
            _tempusCacheService = tempusCacheService;
        }

        // GET api/<WRSplitController>/jabroni/4
        [HttpGet("{mapId}/{playerClass}/{recordType}/{zoneIndex?}")]
        public ActionResult<string> Get(int mapId, int playerClass, string recordType, int zoneIndex = 1)
        {
            List<RecordWithZonedData> recentRecords;
            switch (recordType.ToLowerInvariant())
            {
                case "map":
                    recentRecords = _tempusCacheService.RecentActivityWithZonedData.MapWR;
                    break;
                case "course":
                    recentRecords = _tempusCacheService.RecentActivityWithZonedData.CourseWR;
                    break;
                case "bonus":
                    recentRecords = _tempusCacheService.RecentActivityWithZonedData.BonusWR;
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

            return Ok(TempusUtilities.GetWRSplitString(foundRecord.Record.CachedTime, foundRecord.ZonedData));
        }
    }
}
