using Newtonsoft.Json;

namespace TempusHubBlazor.Models.Tempus.Responses;

public class RecordZoneInfo
{
    [JsonProperty(PropertyName = "id")]
    public int Id { get; set; }
    [JsonProperty(PropertyName = "map_id")]
    public int MapId { get; set; }
    [JsonProperty(PropertyName = "type")]
    public string Type { get; set; }
    [JsonProperty(PropertyName = "zoneindex")]
    public int ZoneIndex { get; set; }
    [JsonProperty(PropertyName = "index")]
    private int Index { set { ZoneIndex = value; } }
       
    [JsonProperty(PropertyName = "custom_name")]
    public string CustomName { get; set; }
}