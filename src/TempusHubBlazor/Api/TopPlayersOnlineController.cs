using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TempusHubBlazor.Models;
using TempusHubBlazor.Models.Api;
using TempusHubBlazor.Services;
using TempusHubBlazor.Utilities;

namespace TempusHubBlazor.Api
{
    [Route("api/[controller]")]
    [ApiController]
    public class TopPlayersOnlineController : ControllerBase
    {
        private TempusCacheService _tempusCacheService;
        public TopPlayersOnlineController(TempusCacheService tempusCacheService)
        {
            _tempusCacheService = tempusCacheService;
        }
        [HttpGet]
        public ActionResult<IEnumerable<TopPlayerOnlineResponse>> Get()
        {
            var response = _tempusCacheService.TopPlayersOnline.Select(x => new TopPlayerOnlineResponse
            {
                Rank = x.Rank,
                RankClass = x.RankClass,
                TempusId = x.Id,
                SteamName = x.Name,
                RealName = _tempusCacheService.GetRealName(x.Id),
                ServerInfo = new ServerInfoShortResponse
                {
                    CurrentMap = x.Server.GameInfo.CurrentMap,
                    Id = x.Server.ServerInfo.Id,
                    CurrentPlayers = x.Server.GameInfo.PlayerCount,
                    MaxPlayers = x.Server.GameInfo.MaxPlayers,
                    Alias = x.Server.ServerInfo.Shortname,
                    IpAddress = x.Server.ServerInfo.Addr + ":" + x.Server.ServerInfo.Port,
                    Name = x.Server.ServerInfo.Name
                }
            });

            if (!response.Any())
            {
                return NoContent();
            }

            return Ok(response);
        }
    }
}