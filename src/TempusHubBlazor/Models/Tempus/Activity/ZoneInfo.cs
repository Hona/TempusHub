using Newtonsoft.Json;

namespace TempusHubBlazor.Models.Tempus.Activity;

public class ZoneInfo
{
    [JsonProperty(PropertyName = "type")]
    public string Type { get; set; }

    [JsonProperty(PropertyName = "map_id")]
    public int MapId { get; set; }

    [JsonProperty(PropertyName = "id")]
    public int Id { get; set; }

    [JsonProperty(PropertyName = "custom_name")]
    public object CustomName { get; set; }

    [JsonProperty(PropertyName = "zoneindex")]
    public int ZoneIndex { get; set; }
}