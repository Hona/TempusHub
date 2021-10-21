using Newtonsoft.Json;

namespace TempusHubBlazor.Models;

public class TempusRealName
{
    [JsonProperty(PropertyName = "id")]
    public int Id { get; set; }
    [JsonProperty(PropertyName = "nickname")]
    public string RealName { get; set; }
}