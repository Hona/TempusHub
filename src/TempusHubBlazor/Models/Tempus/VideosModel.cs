using Newtonsoft.Json;

namespace TempusHubBlazor.Models.Tempus;

public class VideosModel
{
    [JsonProperty(PropertyName = "soldier")]
    public string Soldier { get; set; }

    [JsonProperty(PropertyName = "demoman")]
    public string Demoman { get; set; }
}