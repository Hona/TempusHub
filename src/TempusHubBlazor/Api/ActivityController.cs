using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TempusHubBlazor.Models;
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

        [HttpGet]
        public ActionResult<RecentActivityWithZonedData> Get()
        {
            if (_tempusCacheService.RecentActivityWithZonedData == null)
            {
                return NoContent();
            }

            return _tempusCacheService.RecentActivityWithZonedData;
        }
    }
}
