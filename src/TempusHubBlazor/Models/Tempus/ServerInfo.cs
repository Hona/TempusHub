using Newtonsoft.Json;

namespace TempusHubBlazor.Models.Tempus;

public class ServerInfo
{
    [JsonProperty(PropertyName = "hidden")]
    public bool Hidden { get; set; }

    [JsonProperty(PropertyName = "addr")]
    public string Addr { get; set; }

    [JsonProperty(PropertyName = "country")]
    public string Country { get; set; }

    [JsonProperty(PropertyName = "id")]
    public int Id { get; set; }

    [JsonProperty(PropertyName = "shortname")]
    public string Shortname { get; set; }

    [JsonProperty(PropertyName = "port")]
    public int Port { get; set; }

    [JsonProperty(PropertyName = "name")]
    public string Name { get; set; }
}