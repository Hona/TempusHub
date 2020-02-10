using Newtonsoft.Json;

namespace TempusHubBlazor.Models.Tempus
{
    public class ZoneCountsModel
    {
        [JsonProperty(PropertyName = "map")]
        public int Map { get; set; }

        [JsonProperty(PropertyName = "linear")]
        public int Linear { get; set; }

        [JsonProperty(PropertyName = "bonus")]
        public int Bonus { get; set; }

        [JsonProperty(PropertyName = "map_end")]
        public int MapEnd { get; set; }

        [JsonProperty(PropertyName = "checkpoint")]
        public int Checkpoint { get; set; }

        [JsonProperty(PropertyName = "bonus_end")]
        public int BonusEnd { get; set; }
    }
}