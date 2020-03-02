using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TempusHubBlazor.Models.Tempus.PlayerStats
{
    public class BaseZoneStats
    {
        [JsonProperty(PropertyName = "count")]
        public int Count { get; set; }
        [JsonProperty(PropertyName = "points")]
        public double Points { get; set; }
    }
}
