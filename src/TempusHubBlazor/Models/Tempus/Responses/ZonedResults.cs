using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TempusHubBlazor.Models.Tempus.Activity;

namespace TempusHubBlazor.Models.Tempus.Responses
{
    public class ZonedResults
    {
        [JsonProperty(PropertyName = "soldier")]
        public List<RecordInfoShort> SoldierRuns { get; set; }
        [JsonProperty(PropertyName = "demoman")]
        public List<RecordInfoShort> DemomanRuns { get; set; }
    }
}
