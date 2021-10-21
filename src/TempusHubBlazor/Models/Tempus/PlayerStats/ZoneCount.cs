using Newtonsoft.Json;

namespace TempusHubBlazor.Models.Tempus.PlayerStats
{
    public class ZoneCount
    {
        [JsonProperty(PropertyName = "count")]
        public long Count { get; set; }
    }
}
