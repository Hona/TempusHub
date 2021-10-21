using Newtonsoft.Json;

namespace TempusHubBlazor.Models.Tempus.Rank;

public class RankInfo
{
    [JsonProperty(PropertyName = "total_ranked")]
    public int TotalRanked { get; set; }

    [JsonProperty(PropertyName = "points")]
    public double Points { get; set; }

    [JsonProperty(PropertyName = "rank")]
    public int Rank { get; set; }
}