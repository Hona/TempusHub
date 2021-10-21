using System.Collections.Generic;
using Newtonsoft.Json;

namespace TempusHubBlazor.Models.Tempus;

public class GameInfo
{
    [JsonProperty(PropertyName = "gameVersion")]
    public int GameVersion { get; set; }

    [JsonProperty(PropertyName = "hostname")]
    public string Hostname { get; set; }

    [JsonProperty(PropertyName = "users")]
    public List<ServerPlayerModel> Users { get; set; }

    [JsonProperty(PropertyName = "playerCount")]
    public int PlayerCount { get; set; }

    [JsonProperty(PropertyName = "tempusVersion")]
    public int TempusVersion { get; set; }

    [JsonProperty(PropertyName = "spVersion")]
    public int SpVersion { get; set; }

    [JsonProperty(PropertyName = "freeDisk")]
    public int? FreeDisk { get; set; }

    [JsonProperty(PropertyName = "maxPlayers")]
    public int MaxPlayers { get; set; }

    [JsonProperty(PropertyName = "appID")]
    public int AppId { get; set; }

    [JsonProperty(PropertyName = "nextMap")]
    public object NextMap { get; set; }

    [JsonProperty(PropertyName = "currentMap")]
    public string CurrentMap { get; set; }
}