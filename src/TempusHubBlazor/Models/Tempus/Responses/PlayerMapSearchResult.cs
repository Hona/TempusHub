using Newtonsoft.Json;
using System.Collections.Generic;
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
