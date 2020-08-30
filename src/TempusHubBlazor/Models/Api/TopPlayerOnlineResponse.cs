using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TempusHubBlazor.Models.Api
{
    public class TopPlayerOnlineResponse
    {
        public string SteamName { get; set; }
        public string RealName { get; set; }
        public ServerInfoShortResponse ServerInfo { get; set; }
        public int TempusId { get; set; }
        public int Rank { get; set; }
        public int RankClass { get; set; }
    }
}
