using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TempusHubBlazor.Models.Tempus.PlayerStats
{
    public class ZoneCount
    {
        [JsonProperty(PropertyName = "count")]
        public long Count { get; set; }
    }
}
