using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TempusHubBlazor.Models.Tempus.Activity;

namespace TempusHubBlazor.Models.Tempus.Responses
{
    public class PlayerMapSearchResult
    {
        [JsonProperty(PropertyName = "players")]
        public List<ServerPlayerModel> Players { get; set; }

        [JsonProperty(PropertyName = "maps")]
        public List<MapInfo> Maps { get; set; }
    }
}
