using Newtonsoft.Json;

namespace TempusHub.Core.Models.Demystify;

public class TempusRealName
{
    [JsonProperty(PropertyName = "id")]
    public int Id { get; set; }
    [JsonProperty(PropertyName = "nickname")]
    public string RealName { get; set; }
}