using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace TempusHubBlazor.Models.Tempus.Responses
{
    public class MapFullOverviewModel
    {
        [JsonProperty(PropertyName = "videos")]
        public VideosModel Videos { get; set; }

        [JsonProperty(PropertyName = "tier_info")]
        public TierInfoModel TierInfo { get; set; }

        [JsonProperty(PropertyName = "demoman_runs")]
        public List<RecordModel> DemomanRuns { get; set; }

        [JsonProperty(PropertyName = "authors")]
        public List<AuthorModel> Authors { get; set; }

        [JsonProperty(PropertyName = "map_info")]
        public MapInfoModel MapInfo { get; set; }

        [JsonProperty(PropertyName = "soldier_runs")]
        public List<RecordModel> SoldierRuns { get; set; }

        [JsonProperty(PropertyName = "zone_counts")]
        public ZoneCountsModel ZoneCounts { get; set; }
        public List<RecordModel> GetClassRuns(int id)
        {
            return id switch
            {
                3 => SoldierRuns,
                4 => DemomanRuns,
                _ => throw new ArgumentOutOfRangeException("No such class id exists"),
            };
        }
    }
}