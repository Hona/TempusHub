using Newtonsoft.Json;

namespace TempusHubBlazor.Models.Tempus
{
    public class TierInfoModel
    {
        [JsonProperty(PropertyName = "demoman")]
        public int Demoman { get; set; }

        [JsonProperty(PropertyName = "soldier")]
        public int Soldier { get; set; }
    }
}