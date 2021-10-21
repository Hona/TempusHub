using Newtonsoft.Json;
using TempusHubBlazor.Models.Tempus.Activity;

namespace TempusHubBlazor.Models.Tempus.Responses;

public class RunInfoModel
{
    [JsonProperty(PropertyName = "record_info")]
    public RecordInfo RecordInfo { get; set; }
    [JsonProperty(PropertyName = "demo_info")]
    public ServerDemoModel DemoInfo { get; set; }
    [JsonProperty(PropertyName = "player_info")]
    public ServerPlayerModel Player { get; set; }
    [JsonProperty(PropertyName = "tier_info")]
    public TierInfoModel TierInfo { get; set; }
    [JsonProperty(PropertyName = "zone_info")]
    public RecordZoneInfo RecordZoneInfo { get; set; }
    [JsonProperty(PropertyName = "map_info")]
    public MapInfoModel MapInfo { get; set; }
}