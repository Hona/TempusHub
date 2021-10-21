using Newtonsoft.Json;

namespace TempusHubBlazor.Models.Tempus.DetailedMapList;

public class TierInfo
{
    [JsonProperty(PropertyName = "3")]
    public int Soldier { get; set; }

    [JsonProperty(PropertyName = "4")]
    public int Demoman { get; set; }
}