using Newtonsoft.Json;

namespace TempusHubBlazor.Models.Tempus.DetailedMapList;

public class Videos
{
    [JsonProperty(PropertyName = "soldier")]
    public string Soldier { get; set; }

    [JsonProperty(PropertyName = "demoman")]
    public string Demoman { get; set; }
}