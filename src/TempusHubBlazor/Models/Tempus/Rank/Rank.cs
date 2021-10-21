using Newtonsoft.Json;

namespace TempusHubBlazor.Models.Tempus.Rank;

public class Rank
{
    [JsonProperty(PropertyName = "player_info")]
    public PlayerInfo PlayerInfo { get; set; }

    [JsonProperty(PropertyName = "class_rank_info")]
    public ClassRankInfo ClassRankInfo { get; set; }

    [JsonProperty(PropertyName = "rank_info")]
    public RankInfo RankInfo { get; set; }
}