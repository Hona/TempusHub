using Newtonsoft.Json;

namespace TempusHubBlazor.Models.Tempus.PlayerStats;

public class BaseZoneStats
{
    [JsonProperty(PropertyName = "count")]
    public int Count { get; set; }
    [JsonProperty(PropertyName = "points")]
    public double Points { get; set; }
}