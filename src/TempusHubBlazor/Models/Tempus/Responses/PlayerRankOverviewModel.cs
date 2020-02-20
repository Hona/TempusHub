using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TempusHubBlazor.Models.Tempus.Responses
{
    public class PlayerRankOverviewModel
    {
        [JsonProperty(PropertyName = "id")]
        public int Id { get; set; }
        [JsonProperty(PropertyName = "steamid")]
        public int SteamId { get; set; }
        [JsonProperty(PropertyName = "name")]
        public int Name { get; set; }
        [JsonProperty(PropertyName = "points")]
        public double Points { get; set; }
        [JsonProperty(PropertyName = "rank")]
        public int Rank { get; set; }
    }
}
