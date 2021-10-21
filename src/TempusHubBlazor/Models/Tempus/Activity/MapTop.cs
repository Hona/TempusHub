using Newtonsoft.Json;

namespace TempusHubBlazor.Models.Tempus.Activity;

public class MapTop : TempusRecordBase
{
    [JsonProperty(PropertyName = "rank")]
    public int Rank { get; set; }
}