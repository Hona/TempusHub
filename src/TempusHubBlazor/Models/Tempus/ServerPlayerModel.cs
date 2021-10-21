using Newtonsoft.Json;

namespace TempusHubBlazor.Models.Tempus;

public class ServerPlayerModel
{
    [JsonProperty(PropertyName = "steamid")]
    public string SteamId { get; set; }

    [JsonProperty(PropertyName = "name")]
    public string Name { get; set; }

    [JsonProperty(PropertyName = "id")]
    public int? Id { get; set; }
}