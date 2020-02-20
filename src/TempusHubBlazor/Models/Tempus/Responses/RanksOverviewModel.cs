using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TempusHubBlazor.Models.Tempus.Responses
{
    public class RanksOverviewModel
    {
        [JsonProperty(PropertyName = "count")]
        public int PlayerCount { get; set; }
        [JsonProperty(PropertyName = "players")]
        public List<PlayerRankOverviewModel> TopPlayers { get; set; }
    }
}
