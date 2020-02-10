using Newtonsoft.Json;

namespace TempusHubBlazor.Models.Tempus.DetailedMapList
{
    public class ZoneCounts
    {
        [JsonProperty(PropertyName = "checkpoint")]
        public int Checkpoint { get; set; }

        [JsonProperty(PropertyName = "map")]
        public int Map { get; set; }

        [JsonProperty(PropertyName = "linear")]
        public int Linear { get; set; }

        [JsonProperty(PropertyName = "special")]
        public int Special { get; set; }

        [JsonProperty(PropertyName = "map_end")]
        public int MapEnd { get; set; }

        [JsonProperty(PropertyName = "course_end")]
        public int? CourseEnd { get; set; }

        [JsonProperty(PropertyName = "bonus")]
        public int? Bonus { get; set; }

        [JsonProperty(PropertyName = "misc")]
        public int? Misc { get; set; }

        [JsonProperty(PropertyName = "trick")]
        public int? Trick { get; set; }

        [JsonProperty(PropertyName = "course")]
        public int? Course { get; set; }

        [JsonProperty(PropertyName = "bonus_end")]
        public int? BonusEnd { get; set; }
    }
}