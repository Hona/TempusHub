using Newtonsoft.Json;
using System.Collections.Generic;

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
