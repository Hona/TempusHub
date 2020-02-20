using Newtonsoft.Json;

namespace TempusHubBlazor.Models.Tempus.Rank
{
    public class ClassRankInfo
    {
        [JsonProperty(PropertyName = "4")]
        public DemoRank DemoRank { get; set; }

        [JsonProperty(PropertyName = "3")]
        public SoldierRank SoldierRank { get; set; }
    }
}