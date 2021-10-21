using Newtonsoft.Json;
using TempusHubBlazor.Models.MySQL;

namespace TempusHubBlazor.Models.Tempus.Activity;

public class TempusRecordBase
{
    /// <summary>
    /// Not from the API, applied by TempusDataService when received however.
    /// </summary>
    public MapRecordCache CachedTime { get; set; }

    [JsonProperty(PropertyName = "record_info")]
    public RecordInfoShort RecordInfo { get; set; }

    [JsonProperty(PropertyName = "zone_info")]
    public ZoneInfo ZoneInfo { get; set; }

    [JsonProperty(PropertyName = "map_info")]
    public MapInfo MapInfo { get; set; }

    [JsonProperty(PropertyName = "player_info")]
    public PlayerInfo PlayerInfo { get; set; }
}