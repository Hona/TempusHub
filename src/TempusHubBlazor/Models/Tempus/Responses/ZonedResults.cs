using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using TempusHubBlazor.Models.Tempus.Activity;

namespace TempusHubBlazor.Models.Tempus.Responses;

public class ZonedResults
{
    [JsonProperty(PropertyName = "soldier")]
    public List<RecordInfoShort> SoldierRuns { get; set; }
    [JsonProperty(PropertyName = "demoman")]
    public List<RecordInfoShort> DemomanRuns { get; set; }
    public List<RecordInfoShort> GetClassRuns(int id)
    {
        return id switch
        {
            3 => SoldierRuns,
            4 => DemomanRuns,
            _ => throw new ArgumentOutOfRangeException(nameof(id)),
        };
    }
}