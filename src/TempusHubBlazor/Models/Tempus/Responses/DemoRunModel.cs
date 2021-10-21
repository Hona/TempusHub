using Newtonsoft.Json;

namespace TempusHubBlazor.Models.Tempus.Responses
{
    public class DemoRunModel
    {
        [JsonProperty(PropertyName = "player_info")]
        public ServerPlayerModel PlayerInfo { get; set; }
        [JsonProperty(PropertyName = "run_type")]
        public string RunType { get; set; }
        [JsonProperty(PropertyName = "zone_info")]
        public RecordZoneInfo RecordZoneInfo { get; set; }
        [JsonProperty(PropertyName = "zone_run_info")]
        public DemoRecordModel RunInfo { get; set; }
    }
}
