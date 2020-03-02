using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TempusHubBlazor.Models.Tempus.PlayerStats
{
    public class ZonesCount
    {
        [JsonProperty(PropertyName = "checkpoint")]
        public ZoneCount Checkpoint { get; set; }
        [JsonProperty(PropertyName = "course_end")]
        public ZoneCount CourseEnd { get; set; }
        [JsonProperty(PropertyName = "linear")]
        public ZoneCount Linear { get; set; }
        [JsonProperty(PropertyName = "map")]
        public ZoneCount Map { get; set; }
        [JsonProperty(PropertyName = "bonus")]
        public ZoneCount Bonus { get; set; }
        [JsonProperty(PropertyName = "misc")]
        public ZoneCount Misc { get; set; }
        [JsonProperty(PropertyName = "map_end")]
        public ZoneCount MapEnd { get; set; }
        [JsonProperty(PropertyName = "trick")]
        public ZoneCount Trick { get; set; }
        [JsonProperty(PropertyName = "course")]
        public ZoneCount Course { get; set; }
        [JsonProperty(PropertyName = "bonus_end")]
        public ZoneCount BonusEnd { get; set; }
        [JsonProperty(PropertyName = "special")]
        public ZoneCount Special { get; set; }
    }
}
