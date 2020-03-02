using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TempusHubBlazor.Models.Tempus.Rank;

namespace TempusHubBlazor.Models.Tempus.PlayerStats
{
    public class PlayerStatsModel
    {
        [JsonProperty(PropertyName = "player_info")]
        public PlayerInfo PlayerInfo { get; set; }
        [JsonProperty(PropertyName = "class_rank_info")]
        public ClassRankInfo ClassRankInfo { get; set; }
        [JsonProperty(PropertyName = "rank_info")]
        public RankInfo RankInfo { get; set; }
        [JsonProperty(PropertyName = "country_class_rank_info")]
        public ClassRankInfo CountryClassRankInfo { get; set; }
        [JsonProperty(PropertyName = "country_rank_info")]
        public BaseRank CountryOverallRankInfo { get; set; }
        [JsonProperty(PropertyName = "top_stats")]
        public ZonedStats TopTimeStats { get; set; }
        [JsonProperty(PropertyName = "pr_stats")]
        public ZonedStats PersonalRecordStats { get; set; }
        [JsonProperty(PropertyName = "wr_stats")]
        public ZonedStats WorldRecordStats { get; set; }
        [JsonProperty(PropertyName = "zone_count")]
        public ZonesCount ZonesCount { get; set; }
    }
}
